using CMS.DAL.Common;
using CMS.DAL.Interfaces;
using CMS.DAL.Services;
using Common.Exceptions;
using Common.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.DAL.Models
{
    public class DbSearchRequest
    {
        private static readonly DbEditorService dbEditorService = new DbEditorService();

        public List<IFilterWithValue> Filters { get; set; }

        public DbSearchRequestParams SortingAndPagingParams { get; set; }

        public List<KeyValuePair<string, Field>> DisplayFields { get; set; }

        public View View { get; set; }

        public string Query { get; set; }

        public DbSearchRequest(IEnumerable<Field> displayFields, IEnumerable<Field> searchFields, OperationType operationType, object value)
        {
            var nl = Environment.NewLine;

            var template = displayFields.FirstOrDefault()?.Template;

            var dispFields = displayFields.ToList();

            if (template == null)
                throw new CustomValidationException("Unable to determine template from display fields.");

            if (!displayFields.Any(f => f.Name.ToUpper() == "ID"))
                dispFields.Add(dbEditorService.GetFieldByNameAndTemplateId("ID", template.Id));

            var query = $"select {string.Join(", ", dispFields.Select(f => f.Name))}, _totalCount = count(*) over(){nl}" +
                        $"from {Constants.DATA_TABLE_PREFIX + template.Name}{nl}" +
                        $"{GetWhereConditions(searchFields, operationType, value)}";

            Query = query;
        }

        public DbSearchRequest(View view, List<IFilterWithValue> filterValues, DbSearchRequestParams searchingParams)
        {
            Filters = filterValues;
            SortingAndPagingParams = searchingParams;
            View = view;

            var nl = Environment.NewLine;

            var filters = filterValues
                .Select(f => new KeyValuePair<IFilter, object>(f.GetFilter(), f.GetValue()))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var tree = GetTableTree(view, filters);

            var nodes = GetFlatListOfNodes(tree);

            var sortFieldTable = nodes
                .Where(node => node is DataTableNode)
                .FirstOrDefault(n => ((DataTableNode)n).SortFields.Contains(searchingParams.SortField));

            var sortFieldTableAlias = (sortFieldTable != null) ? sortFieldTable.Alias : string.Empty;

            (var queryDisplayFields, var displayFields, var groupByDisplayFields) = GetDisplayFields(nodes);

            DisplayFields = displayFields;

            Constants.JoinCase joinCase = searchingParams.SearchWithInnerJoin ? Constants.JoinCase.INNER : Constants.JoinCase.LEFT;

            var query = $"select {string.Join(", ", queryDisplayFields.Select(kv => kv.Key))}, _totalCount = count(*) over(){nl}" +
                        $"from {((DataTableNode)tree).TableName} {tree.Alias}{nl}" +
                        $"{GetJoinStatements(tree, joinCase)}" +
                        $"{GetWhereConditions(nodes, filters)}" +
                        $"{GetGroupByStatement(nodes, groupByDisplayFields)}" +
                        $"{searchingParams.ConvertToString(sortFieldTableAlias)}";

            Query = query;
        }

        public override string ToString()
        {
            return Query;
        }

        // Строит дерево таблиц для структуры join'ов.
        private TableNode GetTableTree(View view, Dictionary<IFilter, object> filtersWithValues)
        {
            var controls = GetControls(view);

            if (controls.Count == 0)
                throw new CustomValidationException($"View (id = {view.Id}) has no field bound controls to display.");

            var mainTemplate = controls.First().ControlFields.OrderBy(cf => cf.Depth).First().Field.Template;

            DataTableNode head = new DataTableNode() { TemplateName = mainTemplate.Name, TemplateId = mainTemplate.Id };

            head.IsHead = true;
            head.Alias = GetAlias(head.TableName);
            head.DisplayFields = controls
                .Select(c => c.ControlFields.OrderBy(cf => cf.Depth))
                .Select(cf => cf.First().Field)
                .Where(f => !f.FieldType.In(FieldType.Reference, FieldType.Dictionary, FieldType.File))
                .ToList();

            Dictionary<TableNode, TableNode> tables = new Dictionary<TableNode, TableNode>() { { head, head } };

            var maxDepthControls = controls.Max(c => c.ControlFields.Count);

            var maxDepthFilters = 0;
            foreach (var filter in filtersWithValues.Keys)
            {
                if (filter.GetFilterFields().Count > 0)
                {
                    var localMax = filter.GetFilterFields().GroupBy(ff => ff.ChainId).Max(g => g.Count());
                    if (maxDepthFilters < localMax)
                        maxDepthFilters = localMax;
                }
            }

            var maxDepth = (maxDepthControls > maxDepthFilters) ? maxDepthControls : maxDepthFilters;

            List<Tuple<object, int>> controlChains = controls.Select(c => new Tuple<object, int>(c, 0)).ToList();

            List<Tuple<object, int>> filterChains = filtersWithValues.Keys.ToList()
                .SelectMany(f => f.GetFilterFields().GroupBy(ff => ff.ChainId).Select(g => new Tuple<object, int>(f, g.Key)))
                .ToList();

            var filtersAndControls = controlChains.Union(filterChains).ToList();

            var currentNodesForControls = filtersAndControls
                .Select(c => new KeyValuePair<Tuple<object, int>, TableNode>(c, head))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            for (var i = 0; i < maxDepth; i++)
            {
                foreach (var filterOrControl in filtersAndControls)
                {
                    List<Tuple<object, Field>> links = null;

                    if (filterOrControl.Item1 is Control control)
                    {
                        links = control
                            .ControlFields
                            .OrderBy(cf => cf.Depth)
                            .Select(cf => new Tuple<object, Field>(cf.Control, cf.Field)).ToList();
                    }
                    else if (filterOrControl.Item1 is IFilter filter)
                    {
                        links = filter
                            .GetFilterFields().Where(ff => ff.ChainId == filterOrControl.Item2)
                            .OrderBy(ff => ff.Depth)
                            .Select(cf => new Tuple<object, Field>(cf.GetFilter(), cf.GetField())).ToList();
                    }

                    if (links.Count > i)
                    {
                        TableNode node;

                        var fieldType = links[i].Item2.FieldType;

                        if (fieldType == FieldType.Reference)
                        {
                            node = new LinkTableNode() { FieldId = links[i].Item2.Id };
                            node.Alias = GetAlias(node.TableName) + links[i].Item2.Id;
                            node.Parent = currentNodesForControls[filterOrControl];
                        }
                        else if (fieldType == FieldType.Dictionary)
                        {
                            node = new LinkDictionaryNode() { FieldId = links[i].Item2.Id };
                            node.Alias = GetAlias(node.TableName) + links[i].Item2.Id;
                            node.Parent = (currentNodesForControls[filterOrControl] is DataTableNode dataTableNode) ? dataTableNode : null;
                        }
                        else
                        {
                            node = new DataTableNode()
                            {
                                TemplateName = links[i].Item2.Template.Name,
                                TemplateId = links[i].Item2.Template.Id
                            };

                            var parent = (currentNodesForControls[filterOrControl] is LinkTableNode linkTableNode) ? linkTableNode : null;

                            node.Alias = GetAlias(node.TableName) + parent?.FieldId;
                            node.Parent = parent;
                        }

                        if (!tables.ContainsKey(node))
                        {
                            tables.Add(node, node);
                            currentNodesForControls[filterOrControl].Children.Add(node);
                            currentNodesForControls[filterOrControl] = node;
                        }
                        else
                            currentNodesForControls[filterOrControl] = tables[node];

                        node = currentNodesForControls[filterOrControl];

                        if (node is DataTableNode dataNode && !fieldType.In(FieldType.Reference, FieldType.Dictionary, FieldType.File))
                        {
                            if (filterOrControl.Item1 is Control ctrl)
                            {
                                var alreadyHasDisplayField = dataNode.DisplayFields.Select(f => f.Id).Contains(links[i].Item2.Id);
                                var alreadyHasField = dataNode.SortFields.Select(f => f.Id).Contains(links[i].Item2.Id);

                                if (!alreadyHasDisplayField)
                                    dataNode.DisplayFields.Add(links[i].Item2);

                                if (!alreadyHasField)
                                    dataNode.SortFields.Add(links[i].Item2);
                            }
                            else if (filterOrControl.Item1 is IFilter flt)
                            {
                                if (!dataNode.Conditions.ContainsKey(flt))
                                    dataNode.Conditions.Add(flt, new List<Field>() { links[i].Item2 });
                                else
                                    dataNode.Conditions[flt].Add(links[i].Item2);
                            }
                        }
                        else if (node is LinkDictionaryNode dictTableNode && fieldType == FieldType.Dictionary)
                        {
                            if (filterOrControl.Item1 is Control ctrl)
                            {

                            }
                            else if (filterOrControl.Item1 is Filter flt)
                            {
                                if (!dictTableNode.Conditions.ContainsKey(flt))
                                    dictTableNode.Conditions.Add(flt, new List<Field>() { links[i].Item2 });
                                else
                                    dictTableNode.Conditions[flt].Add(links[i].Item2);
                            }
                        }
                    }
                }
            }

            return head;
        }

        private string GetJoinStatements(TableNode node, Constants.JoinCase joinCase = Constants.JoinCase.LEFT)
        {
            string nl = Environment.NewLine;
            string subQuery = string.Empty;
            string joinCaseQuery = Constants.GetJoinCase(joinCase);

            foreach (var child in node.Children)
            {
                if (node is DataTableNode dtn && child is LinkTableNode ltn)
                {
                    subQuery += $"{joinCaseQuery} join {ltn.TableName} {ltn.Alias} on {ltn.Alias}.FieldId = {ltn.FieldId} and {ltn.Alias}.DocId1 = {dtn.Alias}.Id{nl}";
                }
                else if (node is LinkTableNode ltn1 && child is LinkTableNode ltn2)
                {
                    subQuery += $"{joinCaseQuery} join {ltn2.TableName} {ltn2.Alias} on {ltn2.Alias}.FieldId = {ltn2.FieldId} and {ltn2.Alias}.DocId1 = {ltn1.Alias}.DocId2{nl}";
                }
                else if (node is LinkTableNode lTn && child is DataTableNode dTn)
                {
                    subQuery += $"{joinCaseQuery} join {dTn.TableName} {dTn.Alias} on {dTn.Alias}.Id = {lTn.Alias}.DocId2{nl}";
                }
                else if (node is DataTableNode dtN && child is LinkDictionaryNode ldn)
                {
                    subQuery += $"left join {ldn.TableName} {ldn.Alias} on {ldn.Alias}.FieldId = {ldn.FieldId} and {dtN.Alias}.Id = {ldn.Alias}.DocId{nl}";
                }

                subQuery += GetJoinStatements(child, joinCase);
            }

            return !string.IsNullOrEmpty(subQuery) ? $"{subQuery}{nl}" : subQuery;
        }

        private List<Control> GetControls(View view)
        {
            var list = view.Controls.Where(c => c.ControlFields.Count > 0).ToList();

            foreach (var childView in view.ChildViews.Where(v => v.ViewType != ViewType.Table))
                list.AddRange(GetControls(childView));

            return list;
        }

        private string GetGroupByStatement(List<TableNode> nodes, List<KeyValuePair<string, Field>> dispFields)
        {
            string nl = Environment.NewLine;

            var dictNodes = nodes.Where(n => n is LinkDictionaryNode);

            if (dictNodes.Count() == 0)
                return string.Empty;

            return $"group by {string.Join(", ", dispFields.Select(kv => kv.Key))}{nl}";
        }


        private string GetWhereConditions(List<TableNode> nodes, Dictionary<IFilter, object> filterValues, bool excludeDeleted = true)
        {
            string nl = Environment.NewLine;

            var andConditions = new List<List<string>>();

            var dataNodes = nodes.Where(node => node is DataTableNode);
            var dictNodes = nodes.Where(node => node is LinkDictionaryNode);

            foreach (var node in dataNodes.Union(dictNodes))
            {
                foreach (var condition in node.Conditions)
                {
                    var orConditions = new List<string>();

                    foreach (var filterValue in filterValues)
                    {
                        if (condition.Key == filterValue.Key && filterValue.Value != null)
                        {
                            foreach (var field in condition.Value)
                            {
                                if (node is DataTableNode)
                                {
                                    var s = $"{node.Alias}.{field.Name} {GetOperation(filterValue.Key)} {GetStringValue(filterValue.Key.Operation.Value, filterValue.Value)}";
                                    orConditions.Add(s);
                                }
                                else if (node is LinkDictionaryNode)
                                {
                                    var columnName = field.Dictionary.DictionaryType == DictionaryType.String ? "DictionaryKeyString" : "DictionaryKeyInt";

                                    var s = $"{node.Alias}.{columnName} {GetOperation(filterValue.Key)} {GetStringValue(filterValue.Key.Operation.Value, filterValue.Value)} " +
                                        $"and {node.Alias}.FieldId = {field.Id}";
                                    orConditions.Add(s);
                                }
                            }
                        }
                    }

                    if (orConditions.Count > 0)
                        andConditions.Add(orConditions);
                }
                
                // Добавляем параметре IsDeleted = false
                if (excludeDeleted && node is DataTableNode)
                {
                    var isDeletedCondition = new List<string>() { $"{node.Alias}.{Constants.FIELD_IS_DELETED} IS NULL OR {node.Alias}.{Constants.FIELD_IS_DELETED} = 0" };
                    andConditions.Add(isDeletedCondition);
                }
            }

            var result = string.Join(" and ", andConditions.Select(ac => "(" + string.Join(" or ", ac.Select(oc => oc)) + ")"));

            return !string.IsNullOrEmpty(result) ? $"where {result}{nl}" : result;
        }


        private string GetWhereConditions(IEnumerable<Field> searchFields, OperationType operation, object value)
        {
            string nl = Environment.NewLine;

            var orConditions = new List<string>();

            foreach (var field in searchFields)
            {
                var s = $"{field.Name} {GetOperation(operation)} {GetStringValue(operation, value)}";
                orConditions.Add(s);
            }

            var result = string.Join(" or ", orConditions);

            return !string.IsNullOrEmpty(result) ? $"where {result}{nl}" : result;
        }


        private (List<KeyValuePair<string, Field>>, List<KeyValuePair<string, Field>>, List<KeyValuePair<string, Field>>) GetDisplayFields(List<TableNode> nodes)
        {
            var dataNodes = nodes.Where(node => node is DataTableNode).Select(node => (DataTableNode)node).ToList();

            foreach (var node in dataNodes)
            {
                var dataNode = node;
                if (!dataNode.DisplayFields.Select(df => df.Name.ToUpper()).Contains("ID"))
                    dataNode.DisplayFields.Insert(0, dbEditorService.GetFieldByNameAndTemplateId("ID", dataNode.TemplateId));
            }

            var kvList1 = dataNodes
                .SelectMany(node => node
                    .DisplayFields
                    .Select(df => new KeyValuePair<string, Field>($"{node.Alias}.{df.Name} as [{node.TemplateName}.{df.Name}]", df))).ToList();

            var kvList2 = dataNodes
                .SelectMany(node => node
                    .DisplayFields
                    .Select(df => new KeyValuePair<string, Field>($"{node.TemplateName}.{df.Name}", df))).ToList();

            var kvList3 = dataNodes
                .SelectMany(node => node
                    .DisplayFields
                    .Select(df => new KeyValuePair<string, Field>($"{node.Alias}.{df.Name}", df))).ToList();

            return (kvList1, kvList2, kvList3);
        }

        private List<TableNode> GetFlatListOfNodes(TableNode node)
        {
            var list = new List<TableNode>() { node };

            foreach (var child in node.Children)
                list.AddRange(GetFlatListOfNodes(child));

            return list;
        }

        private string GetAlias(string tableName)
        {
            return (tableName[0] + string.Join("", tableName.Skip(1).Where(c => c >= 'A' && c <= 'Z'))).ToLower();
        }

        public string GetOperation(IFilter filterDef)
        {
            return GetOperation(filterDef.Operation.Value);
        }

        public string GetOperation(OperationType operationType)
        {
            if (operationType == OperationType.EqualTo)
                return "=";
            else if (operationType == OperationType.Like)
                return "like";
            else if (operationType == OperationType.StartsWith)
                return "like";
            else if (operationType == OperationType.In)
                return "in";
            else if (operationType == OperationType.BetweenInclusive)
                return "between";
            else if (operationType == OperationType.GreaterThan)
                return ">";
            else if (operationType == OperationType.GreaterOrEqualTo)
                return ">=";
            else if (operationType == OperationType.LessThan)
                return "<";
            else if (operationType == OperationType.LessOrEqualTo)
                return "<=";

            return "=";
        }

        private string GetStringValue(OperationType operation, object value)
        {
            if (value == null)
                return string.Empty;

            if (value is string)
            {
                if (operation == OperationType.Like)
                    return $"N'%{value.ToString()}%'";
                if (operation == OperationType.StartsWith)
                    return $"N'{value.ToString()}%'";

                return $"N'{value.ToString()}'";
            }
            else if (value is IEnumerable<int> ints && operation == OperationType.In)
            {
                if (ints.Count() == 0)
                    return string.Empty;

                return $"({string.Join(", ", ints)})";
            }
            else if (value is IEnumerable<int> inTs && operation == OperationType.EqualTo)
            {
                if (inTs.Count() == 0)
                    return string.Empty;

                return $"{inTs.ElementAt(0).ToString()}";
            }
            else if (value is IEnumerable<string> strings && operation == OperationType.In)
            {
                if (strings.Count() == 0)
                    return string.Empty;

                return $"({string.Join(", ", strings.Select(v => "'" + v + "'"))})";
            }
            else if (value is IEnumerable<DateTime> dates)
            {
                if (operation != OperationType.BetweenInclusive)
                    throw new CustomValidationException("Date range filter must have a proper operation assigned in constructor (between).");

                if (dates.Count() < 0 || dates.Count() > 2)
                    return string.Empty;

                if (dates.Count() == 1)
                    return "'" + dates.First().ToString("yyyy/MM/dd") + "'";
                else
                    return $"'{dates.First().ToString("yyyy/MM/dd")}' and '{dates.Last().ToString("yyyy/MM/dd")}'";

            }
            else if (value is bool boolValue)
            {
                return boolValue ? "1" : "0";
            }
            else
                return value.ToString();

        }



    }
}
