using CMS.UI;
using System;
using System.Collections.Generic;

namespace CMS.Behaviours
{
    public static class BehaviourSelector
    {
        public static readonly Dictionary<EventType, Func<UserDefinition, IEventBehaviour>> EventBehaviours =
            new Dictionary<EventType, Func<UserDefinition, IEventBehaviour>>()
            {
                { EventType.Click, (user) => new DefaultEventBehaviour(user) },
                { EventType.Change, (user) => new DefaultEventBehaviour(user) }
            };

        public static readonly Dictionary<ActionType, Func<UserDefinition, IActionBehaviour>> ActionBehaviours =
            new Dictionary<ActionType, Func<UserDefinition, IActionBehaviour>>()
            {
                { ActionType.Goto, (user) => new GotoActionBehaviour(user) },
                { ActionType.GoBack, (user) => new GoBackActionBehaviour(user) },
                { ActionType.OpenSection, (user) => new OpenSectionActionBehaviour(user) },
                { ActionType.CreateDocument, (user) => new CreateDocumentActionBehaviour(user) },
                { ActionType.UpdateDocument, (user) => new UpdateDocumentActionBehaviour(user) },
                { ActionType.DeleteDocument, (user) => new DeleteDocumentActionBehaviour(user) },
                { ActionType.Search, (user) => new SearchActionBehaviour(user) },
                { ActionType.GetSectionSettings, (user) => new GetSectionSettingsBehaviour(user) },
                { ActionType.SearchTicketsWithAutoComplete, (user) => new SearchTicketsWithAutoCompleteBehaviour(user) },
                { ActionType.UploadFile, (user) => new UploadFileActionBehaviour(user) },
                { ActionType.DownloadFile, (user) => new DownloadFileActionBehaviour(user) },
                { ActionType.DeleteFile, (user) => new DeleteFileActionBehaviour(user) }
            };

        public static readonly Dictionary<ParameterType, Func<UserDefinition, IParameterBehaviour>> ParameterBehaviours =
            new Dictionary<ParameterType, Func<UserDefinition, IParameterBehaviour>>()
            {
                { ParameterType.FilterId, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.FilterValue, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.TemplateId, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.ViewId, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.AutoCompleteSearchFieldId, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.AutoCompleteDisplayFieldId, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.CopyValueFrom, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.CopyValueTo, (user) => new DefaultParameterBehaviour(user) },
                { ParameterType.ViewAnchor, (user) => new DefaultParameterBehaviour(user) }
            };

        public static readonly Dictionary<ViewType, Func<UserDefinition, IViewBehaviour>> ViewBehaviours =
            new Dictionary<ViewType, Func<UserDefinition, IViewBehaviour>>()
            {
                { ViewType.Table, (user) => new TableContainerViewBehaviour(user) },
                { ViewType.TableHeader, (user) => new TableHeaderViewBehaviour(user) },
                { ViewType.TableFooter, (user) => new TableFooterViewBehaviour(user) },
                { ViewType.TableBody, (user) => new TableBodyViewBehaviour(user) },
                { ViewType.TableRecord, (user) => new TableRecordViewBehaviour(user) },
                { ViewType.TableCell, (user) => new TableCellViewBehaviour(user) },
                { ViewType.Block, (user) => new DefaultViewBehaviour(user) },
                { ViewType.Tab, (user) => new DefaultViewBehaviour(user) },
                { ViewType.TabContainer, (user) => new DefaultViewBehaviour(user) },
                { ViewType.SectionsSettings, (user) => new SectionSettingsViewBehaviour(user) },
            };

        public static readonly Dictionary<FieldType, Func<UserDefinition, IFieldBehaviour>> FieldBehaviours =
            new Dictionary<FieldType, Func<UserDefinition, IFieldBehaviour>>()
            {
                { FieldType.Text, (user) => new DefaultFieldBehaviour(user) },
                { FieldType.Dictionary, (user) => new DictionaryFieldBehaviour(user) },
                { FieldType.MultiDictionary, (user) => new DictionaryFieldBehaviour(user) },
                { FieldType.Integer, (user) => new DefaultFieldBehaviour(user) },
                { FieldType.DateTime, (user) => new DefaultFieldBehaviour(user) },
                { FieldType.Flag, (user) => new DefaultFieldBehaviour(user) },
                { FieldType.File, (user) => new DefaultFieldBehaviour(user) }
            };

        public static readonly Dictionary<ControlType, Func<UserDefinition, IControlBehaviour>> ControlBehaviours =
            new Dictionary<ControlType, Func<UserDefinition, IControlBehaviour>>()
            {
                { ControlType.Label, (user) => new LabelControlBehaviour(user) },
                { ControlType.Link, (user) => new DefaultControlBehaviour(user) },
                { ControlType.Hidden, (user) => new DefaultControlBehaviour(user) },
                { ControlType.Switch, (user) => new DefaultControlBehaviour(user) },
                { ControlType.TextArea, (user) => new DefaultControlBehaviour(user) },
                { ControlType.TextInput, (user) => new DefaultControlBehaviour(user) },
                { ControlType.MultiSelect, (user) => new MultiSelectControlBehaviour(user) },
                { ControlType.Select, (user) => new MultiSelectControlBehaviour(user) },
                { ControlType.DatePicker, (user) => new DefaultControlBehaviour(user) },
                { ControlType.Button, (user) => new ButtonControlBehaviour(user) },
                { ControlType.TableHeaderControl, (user) => new TableHeaderControlBehaviour(user) },
                { ControlType.ButtonPageNumberControl, (user) => new ButtonPageNumberControlBehaviour(user) },
                { ControlType.ButtonSearch, (user) => new ButtonSearchControlBehaviour(user) },
                { ControlType.File, (user) => new FileControlBehaviour(user) },
                { ControlType.AutoComplete, (user) => new AutoCompleteControlBehaviour(user) }
            };

        public static readonly Dictionary<FilterType, Func<UserDefinition, IFilterBehaviour>> FilterBehaviours =
            new Dictionary<FilterType, Func<UserDefinition, IFilterBehaviour>>()
            {
                { FilterType.Text, (user) => new DefaultFilterBehaviour(user) },
                { FilterType.Hidden, (user) => new DefaultFilterBehaviour(user) },
                { FilterType.DateRange, (user) => new DefaultFilterBehaviour(user) },
                { FilterType.MultiSelect, (user) => new MultiSelectFilterBehaviour(user) },
                { FilterType.Select, (user) => new MultiSelectFilterBehaviour(user) },
                { FilterType.Switch, (user) => new DefaultFilterBehaviour(user) },
                { FilterType.Switch3, (user) => new DefaultFilterBehaviour(user) }
            };

        public static readonly Dictionary<SectionType, Func<UserDefinition, ISectionBehaviour>> SectionBehaviours =
            new Dictionary<SectionType, Func<UserDefinition, ISectionBehaviour>>()
            {
                { SectionType.Default, (user) => new DefaultSectionBehaviour(user) },
            };

        public static readonly Dictionary<ViewLinkType, Func<UserDefinition, IViewLinkBehaviour>> ViewLinkBehaviours =
            new Dictionary<ViewLinkType, Func<UserDefinition, IViewLinkBehaviour>>()
            {
                { ViewLinkType.Default, (user) => new DefaultViewLinkBehaviour(user) },
            };
    }
}
