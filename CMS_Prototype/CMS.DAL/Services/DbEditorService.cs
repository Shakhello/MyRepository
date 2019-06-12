using CMS.DAL.Behaviours;
using CMS.DAL.Common;
using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using CMS.DAL.Resources;
using Common.Exceptions;
using Common.ExtensionMethods;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace CMS.DAL.Services
{
    public class DbEditorService : DbUpdateService
    {

        #region Templates

        public Template CreateTemplate(Template template)
        {
            if (!IsValidName(template.Name))
                throw new CustomValidationException($"Template name can only contain Latin letters, digits and underscores.");

            ExecuteDbContextTransaction((db, transaction) =>
            {
                var templateTableName = Constants.DATA_TABLE_PREFIX + template.Name;
                var histTableName = Constants.HIST_TABLE_PREFIX + template.Name;

                var tableCheckCommand = @"select case when exists(select name from sysobjects 
                                        where name = @p0) then 1 else 0 end";

                var dataTableCreateCommand = GetDataTableScript(templateTableName, Constants.SYSTEM_FIELDS.Where(f => f.Name.ToUpper() != "ID"));

                var historyTableCreateCommand = GetHistoryTableScript(templateTableName, histTableName, Constants.SYSTEM_FIELDS.Where(f => f.Name.ToUpper() != "ID"));

                if (db.Templates.Any(t => t.Name.ToUpper() == template.Name.ToUpper()))
                    throw new CustomValidationException($"Template with the name {template.Name} already exists.");

                var dataTableCheckQuery = db.Database.SqlQuery<int>(tableCheckCommand, templateTableName);
                if (dataTableCheckQuery.FirstOrDefault() == 1)
                    throw new CustomValidationException($"Data table {templateTableName} already exists.");

                var histTableCheckQuery = db.Database.SqlQuery<int>(tableCheckCommand, histTableName);
                if (histTableCheckQuery.FirstOrDefault() == 1)
                    throw new CustomValidationException($"History table {histTableName} already exists.");

                db.Database.ExecuteSqlCommand(dataTableCreateCommand);
                db.Database.ExecuteSqlCommand(historyTableCreateCommand);

                template = db.Templates.Add(new Template()
                {
                    Name = template.Name,
                    DisplayName = template.DisplayName,
                    TemplateType = template.TemplateType
                });

                foreach (var sf in Constants.SYSTEM_FIELDS)
                {
                    sf.TemplateId = template.Id;
                    sf.Template = template;
                }

                db.Fields.AddRange(Constants.SYSTEM_FIELDS);

                db.SaveChanges();
            });

            return template;
        }

        public Template UpdateTemplate(Template template)
        {
            ExecuteDbContextTransaction((db, transaction) =>
            {
                var dbTemplate = db.Templates.Find(template.Id);

                if (dbTemplate == null)
                    throw new CustomValidationException($"No template with id = {template.Id}.");

                var dataTableRenameCommand = $@"exec sp_rename '{Constants.DATA_TABLE_PREFIX + dbTemplate.Name}', 
                    '{Constants.DATA_TABLE_PREFIX + template.Name}'";

                var histTableRenameCommand = $@"exec sp_rename '{Constants.HIST_TABLE_PREFIX + dbTemplate.Name}', 
                    '{Constants.HIST_TABLE_PREFIX + template.Name}'";

                db.Entry(dbTemplate).CurrentValues.SetValues(template);
                db.SaveChanges();

                db.Database.ExecuteSqlCommand(dataTableRenameCommand);
                db.Database.ExecuteSqlCommand(histTableRenameCommand);
            });

            return template;
        }

        public Template GetTemplate(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Templates
                    .Include(t => t.Fields)
                    .Include(t => t.Fields.Select(f => f.LinkedField))
                    .FirstOrDefault(t => t.Id == id);
            }
        }

        public List<Template> GetExistingTemplates()
        {
            using (var db = new CMSContext())
            {
                return db.Templates
                    .Include(t => t.Fields)
                    .Include(t => t.Fields.Select(f => f.LinkedField))
                    .ToList();
            }
        }

        public void DeleteTemplate(int id)
        {
            ExecuteDbContextTransaction((db, transaction) =>
            {
                var template = db.Templates
                                 .Include(t => t.Fields)
                                 .Include(t => t.Fields.Select(f => f.Parameters))
                                 .Include(t => t.Views)
                                 .FirstOrDefault(t => t.Id == id);

                if (template == null)
                    throw new CustomValidationException($"Template with id = {id} does not exist.");


                var dataTableName = Constants.DATA_TABLE_PREFIX + template.Name;
                var histTableName = Constants.HIST_TABLE_PREFIX + template.Name;

                var tableCheckCommand = @"select case when exists(select name from sysobjects 
                                        where name = @p0) then 1 else 0 end";

                var dataTableCheckQuery = db.Database.SqlQuery<int>(tableCheckCommand, dataTableName);
                if (dataTableCheckQuery.FirstOrDefault() == 0)
                    throw new CustomValidationException($"Data table {dataTableName} does not exist.");

                var histTableCheckQuery = db.Database.SqlQuery<int>(tableCheckCommand, histTableName);
                if (histTableCheckQuery.FirstOrDefault() == 1)
                    db.Database.ExecuteSqlCommand($"drop table {histTableName}", histTableName);

                db.Database.ExecuteSqlCommand($"drop table {dataTableName}", dataTableName);

                db.Templates.Remove(template);
                db.SaveChanges();
            });
        }

        #endregion

        #region Fields

        public Field GetField(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Fields
                    .Include(f => f.Template)
                    .FirstOrDefault(f => f.Id == id);
            }
        }

        public Field GetFieldByNameAndTemplateId(string fieldName, int templateId)
        {
            using (var db = new CMSContext())
            {
                return db.Fields
                    .Include(f => f.Template)
                    .FirstOrDefault(f => f.TemplateId == templateId && f.Name.ToUpper() == fieldName.ToUpper());
            }
        }

        public List<Field> GetFieldsByTemplateId(int templateId)
        {
            using (var db = new CMSContext())
            {
                return db.Fields
                    .Include(f => f.Template)
                    .Where(f => f.TemplateId == templateId)
                    .ToList();
            }
        }

        public Field UpdateField(Field field)
        {
            using (var db = new CMSContext())
            {
                var dbField = db.Fields.Find(field.Id);

                if (dbField == null)
                    throw new CustomValidationException($"No field with id = {field.Id}.");
                if (field.Name.ToLower() != dbField.Name.ToLower())
                    throw new CustomValidationException($"Not allowed to change field names, only display names.");
                if (field.FieldType != dbField.FieldType)
                    throw new CustomValidationException($"Not allowed to change field types.");

                db.Entry(dbField).CurrentValues.SetValues(field);
                db.SaveChanges();
            };

            return field;
        }

        public void DeleteField(int id)
        {
            ExecuteDbContextTransaction((db, transaction) =>
            {
                var field = db
                    .Fields
                    .Include(f => f.Template)
                    .Include(f => f.LinkedField)
                    .FirstOrDefault(f => f.Id == id);

                if (field == null)
                    throw new CustomValidationException($"Field with id = {id} does not exist.");

                var fieldBehaviour = BehaviourSelector.FieldBehaviours[field.FieldType]();
                fieldBehaviour.OnDelete(field, db, transaction);

                db.Fields.Remove(field);
                db.SaveChanges();
            });
        }

        public Field CreateField(Field field)
        {
            if (!IsValidName(field.Name))
                throw new CustomValidationException($"Field names can only contain Latin letters, digits and underscores.");

            if (field.FieldType == FieldType.Text && field.Length == null)
                throw new CustomValidationException($"String field can't have zero length.");

            Field newField = null;

            ExecuteDbContextTransaction((db, transaction) =>
            {
                var template = db.Templates.FirstOrDefault(t => t.Id == field.TemplateId);

                if (template == null)
                    throw new CustomValidationException($"Template not found for template id = {field.TemplateId}.");

                newField = db.Fields.Add(field);

                var fieldBehaviour = BehaviourSelector.FieldBehaviours[newField.FieldType]();
                fieldBehaviour.OnCreate(newField, db, transaction);

                db.SaveChanges();
            });

            return newField;
        }

        public Field CreateReferenceField(Field fieldA, Field fieldB)
        {
            if (!IsValidName(fieldA.Name) || !IsValidName(fieldB.Name))
                throw new CustomValidationException($"Field names can only contain Latin letters, digits and underscores.");

            Field newField = null;

            ExecuteDbContextTransaction((db, transaction) =>
            {
                var templateA = db.Templates.FirstOrDefault(t => t.Id == fieldA.TemplateId);
                var templateB = db.Templates.FirstOrDefault(t => t.Id == fieldB.TemplateId);

                fieldA.Template = templateA;
                fieldB.Template = templateB;

                var newFieldA = db.Fields.Add(fieldA);
                var newFieldB = db.Fields.Add(fieldB);

                db.SaveChanges();

                newFieldA.LinkedField = newFieldB;
                newFieldB.LinkedField = newFieldA;

                db.SaveChanges();

                newField = db.Fields.FirstOrDefault(f => f.Id == newFieldA.Id);
            });

            return newField;
        }

        public List<Field> GetFields(IEnumerable<int> ids)
        {
            using (var db = new CMSContext())
            {
                return db.Fields
                    .Include(f => f.Template)
                    .Where(f => ids.Contains(f.Id))
                    .ToList();
            }
        }

        #endregion

        #region Dictionaries

        public List<Dictionary> GetDictionaries()
        {
            using (var db = new CMSContext())
            {
                return db.Dictionaries.ToList();
            }
        }

        public Dictionary GetDictionary(int dictId)
        {
            using (var db = new CMSContext())
            {
                return db.Dictionaries.First(d => d.Id == dictId);
            }
        }

        public Dictionary CreateDictionary(Dictionary dict)
        {
            if (!IsValidName(dict.Name))
                throw new CustomValidationException($"Dictionary name can only contain letters, digits and underscores.");

            Dictionary newDict = null;

            ExecuteDbContextTransaction((db, transaction) =>
            {
                var dictTableName = Constants.DICT_TABLE_PREFIX + dict.Name;

                var tableCheckCommand = @"select case when exists(select name from sysobjects 
                                        where name = @p0) then 1 else 0 end";

                var dictTableCreateCommand = GetDictionaryTableScript(dictTableName, dict.DictionaryType);

                var dictTableCheckQuery = db.Database.SqlQuery<int>(tableCheckCommand, dictTableName);
                if (dictTableCheckQuery.FirstOrDefault() == 1)
                    throw new CustomValidationException($"Data table {dictTableName} already exists.");

                db.Database.ExecuteSqlCommand(dictTableCreateCommand);

                newDict = db.Dictionaries.Add(dict);

                db.SaveChanges();
            });

            return newDict;
        }

        public void DeleteDictionary(int dictId)
        {
            ExecuteDbContextTransaction((db, transaction) =>
            {
                var dict = db.Dictionaries.FirstOrDefault(t => t.Id == dictId);

                if (dict == null)
                    throw new CustomValidationException($"Dictionary with id = {dictId} does not exist.");

                var dictFields = db.Fields.Include(f => f.Template).Where(f => f.DictionaryId == dictId);

                if (dictFields.Count() > 0)
                    throw new CustomValidationException($@"Can't delete dictionary, because there are fields 
                        ({string.Join(", ", dictFields.Select(f => f.Template.Name + "." + f.Name))}) referencing it.");

                var dictTableName = Constants.DICT_TABLE_PREFIX + dict.Name;

                var tableCheckCommand = @"select case when exists(select name from sysobjects 
                                        where name = @p0) then 1 else 0 end";

                var dataTableCheckQuery = db.Database.SqlQuery<int>(tableCheckCommand, dictTableName);
                if (dataTableCheckQuery.FirstOrDefault() == 0)
                    throw new CustomValidationException($"Data table {dictTableName} does not exist.");

                db.Database.ExecuteSqlCommand($"drop table {dictTableName}", dictTableName);

                db.Dictionaries.Remove(dict);
                db.SaveChanges();
            });
        }

        #endregion

        #region Views

        public View GetViewShallow(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Views
                    .Include(v => v.Template)
                    .Include(v => v.Controls)
                    .Include(v => v.Controls.Select(c => c.ControlFields))
                    .Include(v => v.Controls.Select(c => c.ControlFields.Select(cf => cf.Field)))
                    .Include(v => v.LinkedField)
                    .Include(v => v.Filters)
                    .Include(v => v.Style)
                    .Include(v => v.Filters.Select(f => f.FilterFields))
                    .Include(v => v.Filters.Select(c => c.FilterFields.Select(cf => cf.Field)))
                    .FirstOrDefault(v => v.Id == id);
            }
        }

        public View GetViewDeep(int id, CMSContext db = null)
        {
            bool isFirstCall = (db == null) ? true : false;

            try
            {
                if (isFirstCall)
                    db = new CMSContext();

                var view = db.Views
                    .Include(v => v.Template)
                    .Include(v => v.ChildViews)
                    .Include(v => v.Filters)
                    .Include(v => v.Filters.Select(f => f.FilterFields))
                    .Include(v => v.Filters.Select(c => c.FilterFields.Select(cf => cf.Field)))
                    .Include(v => v.Filters.Select(c => c.FilterFields.Select(cf => cf.Field.Dictionary)))
                    .Include(v => v.LinkedField)
                    .Include(v => v.Style)
                    .Include(v => v.Controls)
                    .Include(v => v.Controls.Select(c => c.Style))
                    .Include(v => v.Controls.Select(c => c.ControlFields))
                    .Include(v => v.Controls.Select(c => c.ControlFields.Select(cf => cf.Field)))
                    .Include(v => v.Controls.Select(c => c.ControlFields.Select(cf => cf.Field.Dictionary)))
                    .Include(v => v.Controls.Select(c => c.ControlFields.Select(cf => cf.Field.LinkedField)))
                    .Include(v => v.Controls.Select(c => c.ControlFields.Select(cf => cf.Field.Template)))
                    .Include(v => v.Controls.Select(c => c.Events))
                    .Include(v => v.Controls.Select(c => c.Events.Select(e => e.Actions)))
                    .Include(v => v.Controls.Select(c => c.Events.Select(e => e.Actions.Select(a => a.Parameters))))
                    .FirstOrDefault(v => v.Id == id);

                if (view.ParentView == null)
                    view.RootView = view;
                else
                {
                    var curView = view;
                    while(curView.ParentView != null)
                    {
                        curView = curView.ParentView;
                    }

                    view.RootView = curView;
                }

                var childViews = view.ChildViews.ToList();

                for (int i = 0; i < childViews.Count; i++)
                    GetViewDeep(childViews[i].Id, db);

                return view;
            }
            finally
            {
                if (isFirstCall)
                    db.Dispose();
            }
        }

        public List<View> GetViews()
        {
            using (var db = new CMSContext())
            {
                return db.Views
                    .Include(v => v.ChildViews)
                    .Include(v => v.Filters)
                    .Include(v => v.Controls)
                    .ToList();
            }
        }

        public List<View> GetViewsBySectionId(int sectionId)
        {
            using (var db = new CMSContext())
            {
                return db.Views
                    .Include(v => v.Permissions)
                    .Include(v => v.Permissions.Select(p => p.Role))
                    .Where(v => v.SectionId == sectionId)
                    .ToList();
            }
        }

        public View CreateView(View view, string userLogin)
        {
            using (var db = new CMSContext())
            {
                var newView = db.Views.Add(view);

                if (newView.ParentView == null)
                {
                    var globalAdminsRole = db.Roles.FirstOrDefault(r => r.Name == Constants.ROLE_GLOBAL_ADMINS);

                    if (globalAdminsRole == null)
                        throw new CustomValidationException($"{Constants.ROLE_GLOBAL_ADMINS} role not found, can't create view.");

                    var currentUser = db.Users.FirstOrDefault(u => u.Login.ToUpper() == userLogin.ToUpper());

                    var viewAdminRole = db.Roles.Add(new Role()
                    {
                        Name = $"{newView.Name} {Constants.ADMIN_POSTFIX}",
                        DisplayName = $"{RC.ADMINS_PREFIX} {newView.DisplayName}",
                    });

                    var userRole = new UserRole() { Role = viewAdminRole, User = currentUser, UserCanChangeRole = true };

                    db.UserRoles.Add(userRole);

                    var roles = new List<Role> { globalAdminsRole, viewAdminRole };

                    foreach (var role in roles)
                    {
                        db.Permissions.Add(new Permission()
                        {
                            PermissionType = PermissionType.Write,
                            Role = role,
                            View = newView
                        });

                        db.Permissions.Add(new Permission()
                        {
                            PermissionType = PermissionType.ChangeSchema,
                            Role = role,
                            View = newView
                        });
                    }

                }

                db.SaveChanges();

                return db.Views.Include(v => v.ChildViews)
                    .Include(v => v.Filters)
                    .Include(v => v.Controls)
                    .FirstOrDefault(v => v.Id == newView.Id);
            };
        }

        public View UpdateView(View view)
        {
            using (var db = new CMSContext())
            {
                var dbView = db.Views.Find(view.Id);

                if (dbView == null)
                    throw new CustomValidationException($"No view with id = {view.Id}.");

                db.Entry(dbView).CurrentValues.SetValues(view);
                db.SaveChanges();
            };

            return view;
        }

        public void DeleteView(int id)
        {
            ExecuteDbContextTransaction((db, transaction) =>
            {
                var view = db.Views
                    .Include(v => v.ChildViews)
                    .Include(v => v.Controls)
                    .FirstOrDefault(v => v.Id == id);

                if (view == null)
                    throw new CustomValidationException($"View with id = {id} does not exist.");

                db.Views.Remove(view);
                db.SaveChanges();
            });
        }

        public List<View> FindViewsByName(string name)
        {
            using (var db = new CMSContext())
            {
                return db.Views.Where(v => v.DisplayName.ToUpper().Contains(name.ToUpper()) && v.ParentViewId == null)
                    .ToList();
            }
        }

        #endregion

        #region Sections

        public List<Section> GetSections(string userLogin)
        {
            using (var db = new CMSContext())
            {
                var sectionIdsUserHasPermissionsTo = db.Users
                    .Where(u => u.Login.ToUpper() == userLogin.ToUpper())
                    .SelectMany(u => u.UserRoles)
                    .Select(ur => ur.Role)
                    .SelectMany(r => r.Permissions)
                    .Select(p => p.View)
                    .Where(v => v.SectionId.HasValue)
                    .Select(v => v.SectionId)
                    .Distinct()
                    .ToList();

                return db.Sections.Where(s => sectionIdsUserHasPermissionsTo.Contains(s.Id))
                    .Include(s => s.Views)
                    .Include(s => s.Views.Select(v => v.Controls))
                    .Include(s => s.Views.Select(v => v.Controls.Select(c => c.ControlFields)))
                    .Include(s => s.Views.Select(v => v.Controls.Select(c => c.ControlFields.Select(cf => cf.Field))))
                    .Include(s => s.Views.Select(v => v.Filters))
                    .Include(s => s.Views.Select(v => v.Filters.Select(f => f.FilterFields)))
                    .Include(s => s.Views.Select(v => v.Filters.Select(f => f.FilterFields.Select(ff => ff.Field))))
                    .ToList();
            }
        }

        public Section GetSection(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Sections
                    .Include(s => s.Views)
                    .FirstOrDefault(s => s.Id == id);
            }
        }

        public Section CreateSection(Section section)
        {
            using (var db = new CMSContext())
            {
                var newSection = db.Sections.Add(section);
                db.SaveChanges();

                return db.Sections.Include(s => s.Views)
                    .FirstOrDefault(s => s.Id == newSection.Id);
            }
        }

        public void DeleteSection(int id)
        {
            using (var db = new CMSContext())
            {
                var section = db.Sections.FirstOrDefault(s => s.Id == id);
                if (section == null)
                    throw new CustomValidationException($"Section with id = {id} does not exist.");

                db.Sections.Remove(section);
                db.SaveChanges();
            }
        }

        #endregion

        #region Controls

        public Control GetControl(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Controls.FirstOrDefault(c => c.Id == id);
            }
        }

        public List<Control> GetControlsByViewId(int viewId)
        {
            using (var db = new CMSContext())
            {
                return db.Controls.Where(c => c.View.Id == viewId).ToList();
            }
        }

        public Control CreateControl(Control control)
        {
            using (var db = new CMSContext())
            {
                var view = db.Views.FirstOrDefault(v => v.Id == control.ViewId);
                control.View = view ?? throw new CustomValidationException($"View with id = {control.ViewId} does not exist.");
                var newControl = db.Controls.Add(control);

                db.SaveChanges();

                return db.Controls.FirstOrDefault(c => c.Id == newControl.Id);
            };
        }

        public Control UpdateControl(Control control)
        {
            using (var db = new CMSContext())
            {
                var dbControl = db.Controls.Find(control.Id);

                if (dbControl == null)
                    throw new CustomValidationException($"No control with id = {control.Id}.");

                db.Entry(dbControl).CurrentValues.SetValues(control);
                db.SaveChanges();
            };

            return control;
        }

        public void DeleteControl(int id)
        {
            using (var db = new CMSContext())
            {
                var control = db.Controls.FirstOrDefault(v => v.Id == id);
                if (control == null)
                    throw new CustomValidationException($"Control with id = {id} does not exist.");

                db.Controls.Remove(control);
                db.SaveChanges();
            };
        }

        public ControlField CreateControlField(ControlField controlField)
        {
            using (var db = new CMSContext())
            {
                var newControlField = db.ControlFields.Add(controlField);

                db.SaveChanges();

                return db.ControlFields.FirstOrDefault(f => f.ControlId == newControlField.ControlId &&
                    f.FieldId == newControlField.FieldId);
            };
        }

        public void DeleteControlField(ControlField controlField)
        {
            using (var db = new CMSContext())
            {
                var dbControlField = db.ControlFields.FirstOrDefault(c => c.ControlId == controlField.ControlId
                    && c.FieldId == controlField.FieldId);

                db.ControlFields.Remove(dbControlField);
                db.SaveChanges();
            };
        }

        #endregion

        #region Filters

        public Filter GetFilter(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Filters
                    .Include(f => f.FilterFields)
                    .Include(f => f.FilterFields.Select(ff => ff.Field))
                    .Include(f => f.FilterFields.Select(ff => ff.Field.Template))
                    .Include(f => f.FilterFields.Select(ff => ff.Field.Dictionary))
                    .Include(f => f.View)
                    .Include(f => f.View.Template)
                    .FirstOrDefault(f => f.Id == id);
            }
        }

        public List<Filter> GetFilters(IEnumerable<int> ids)
        {
            using (var db = new CMSContext())
            {
                return db.Filters
                    .Include(f => f.FilterFields)
                    .Include(f => f.FilterFields.Select(ff => ff.Field))
                    .Where(f => ids.Contains(f.Id))
                    .ToList();
            }
        }

        public List<Filter> GetFiltersByViewId(int viewId)
        {
            using (var db = new CMSContext())
            {
                return db.Filters
                    .Include(f => f.FilterFields)
                    .Include(f => f.FilterFields.Select(ff => ff.Field))
                    .Where(f => f.View.Id == viewId)
                    .ToList();
            }
        }

        public Filter CreateFilter(Filter filter)
        {
            using (var db = new CMSContext())
            {
                var view = db.Views.FirstOrDefault(v => v.Id == filter.ViewId);
                filter.View = view;

                var newFilter = db.Filters.Add(filter);

                db.SaveChanges();

                return db.Filters.FirstOrDefault(f => f.Id == newFilter.Id);
            };
        }

        public Filter UpdateFilter(Filter filter)
        {
            Filter dbFilter;

            using (var db = new CMSContext())
            {
                dbFilter = db.Filters
                    .FirstOrDefault(f => f.Id == filter.Id);

                if (dbFilter == null)
                    throw new CustomValidationException($"No filter with id = {filter.Id}.");

                db.Entry(dbFilter).CurrentValues.SetValues(filter);
                db.SaveChanges();
            };

            return dbFilter;
        }

        public void DeleteFilter(int id)
        {
            using (var db = new CMSContext())
            {
                var filter = db.Filters.FirstOrDefault(f => f.Id == id);
                if (filter == null)
                    throw new CustomValidationException($"Filter with id = {id} does not exist.");

                db.Filters.Remove(filter);
                db.SaveChanges();
            };
        }

        public FilterField CreateFilterField(FilterField filterField)
        {
            using (var db = new CMSContext())
            {
                var newFilterField = db.FilterFields.Add(filterField);

                db.SaveChanges();

                return db.FilterFields.FirstOrDefault(f => f.FilterId == newFilterField.FilterId &&
                    f.FieldId == newFilterField.FieldId && f.ChainId == newFilterField.ChainId);
            };
        }

        public void DeleteFilterField(FilterField filterField)
        {
            using (var db = new CMSContext())
            {
                var dbFilterField = db.FilterFields.FirstOrDefault(c => c.FilterId == filterField.FilterId
                    && c.FieldId == filterField.FieldId && c.ChainId == filterField.ChainId);

                db.FilterFields.Remove(dbFilterField);

                db.SaveChanges();
            };
        }

        #endregion

        #region Styles

        public List<Models.Style> GetStyles()
        {
            using (var db = new CMSContext())
            {
                return db.Styles.ToList();
            }
        }


        public Style GetStyle(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Styles.FirstOrDefault(s => s.Id == id);
            }
        }


        public Style UpdateStyle(Style style)
        {
            using (var db = new CMSContext())
            {
                var dbStyle = db.Styles.Find(style.Id);

                if (dbStyle == null)
                    throw new CustomValidationException($"No style with id = {style.Id}.");

                db.Entry(dbStyle).CurrentValues.SetValues(style);
                db.SaveChanges();
            };

            return style;
        }

        public Style CreateStyle(Style style)
        {
            using (var db = new CMSContext())
            {
                var newStyle = db.Styles.Add(style);

                
                db.SaveChanges();

                return newStyle;
            }
        }

        public void DeleteStyle(int id)
        {
            using (var db = new CMSContext())
            {
                var style = db.Styles.FirstOrDefault(s => s.Id == id);

                db.Styles.Remove(style);

                db.SaveChanges();

            }
        }

        #endregion

        #region Events

        public Event GetEvent(int id)
        {
            using (var db = new CMSContext())
            {
                return db.Events
                    .Include(e => e.Actions)
                    .Include(e => e.Actions.Select(a => a.Parameters))
                    .FirstOrDefault(e => e.Id == id);
            }
        }

        public Event CreateEvent(Event evt)
        {
            using (var db = new CMSContext())
            {
                evt.Control = db.Controls.Single(c => c.Id == evt.ControlId);

                var newEvent = db.Events.Add(evt);
                db.SaveChanges();

                return newEvent;
            }
        }

        public void DeleteEvent(int id)
        {
            using (var db = new CMSContext())
            {
                var evt = db.Events.FirstOrDefault(e => e.Id == id);
                db.Events.Remove(evt);

                db.SaveChanges();
            }
        }

        #endregion

        #region Actions

        public Action CreateAction(Action action)
        {
            using (var db = new CMSContext())
            {
                var newAction = db.Actions.Add(action);
                db.SaveChanges();

                return newAction;
            };

        }

        public Action UpdateAction(Action action)
        {
            using (var db = new CMSContext())
            {
                var dbAction = db.Actions
                    .Include(f => f.Parameters)
                    .FirstOrDefault(f => f.Id == action.Id);

                var pIds = action.Parameters.Select(p => p.Id).ToList();
                var existingParams = db.Parameters.Where(f => pIds.Contains(f.Id));

                var existingParamIds = existingParams.Select(p => p.Id);

                foreach (var existParam in existingParams)
                    db.Entry(existParam).CurrentValues.SetValues(action.Parameters.First(p => p.Id == existParam.Id));

                var newParams = action.Parameters.Where(p => !existingParamIds.Contains(p.Id)).ToList();

                db.Parameters.AddRange(newParams);
                db.SaveChanges();

                var parameters = db.Parameters.Where(p => p.ActionId == action.Id).ToList();

                action.Parameters.Clear();

                foreach (var p in parameters)
                    dbAction.Parameters.Add(p);


                db.Entry(dbAction).CurrentValues.SetValues(action);
                db.SaveChanges();

                return dbAction;
            };
        }

        public void DeleteAction(int id)
        {
            using (var db = new CMSContext())
            {
                var action = db.Actions.FirstOrDefault(a => a.Id == id);
                db.Actions.Remove(action);

                db.SaveChanges();
            }
        }

        #endregion

        #region Parameters

        public List<Parameter> CreateParameters(IEnumerable<Parameter> parameters)
        {
            using (var db = new CMSContext())
            {
                foreach (var p in parameters)
                {
                    var action = db.Actions.FirstOrDefault(a => a.Id == p.ActionId);
                    p.Action = action;
                }


                var newParameters = db.Parameters.AddRange(parameters);
                db.SaveChanges();

                return newParameters.ToList();
            };
        }

        public void DeleteParameters(IEnumerable<int> ids)
        {
            using (var db = new CMSContext())
            {
                var parameters = db.Parameters.Where(p => ids.Contains(p.Id));

                db.Parameters.RemoveRange(parameters);

                db.SaveChanges();
            };
        }

        #endregion

        #region Users, Roles, Permissions


        public User GetUserByLogin(string login)
        {
            using (var db = new CMSContext())
            {
                return db.Users
                    .Include(u => u.UserRoles)
                    .Include(u => u.UserRoles.Select(ur => ur.Role))
                    .FirstOrDefault(u => u.Login.ToUpper() == login.ToUpper());
            }
        }

        public List<User> FindUsersByLogin(string login)
        {
            using (var db = new CMSContext())
            {
                return db.Users.Where(u => u.Login.ToUpper().Contains(login.ToUpper())).ToList();
            }
        }

        public User CreateUser(User user)
        {
            using (var db = new CMSContext())
            {
                var newUser = db.Users.Add(user);
                db.SaveChanges();

                return newUser;
            }
        }

        public List<Role> GetRolesBySectionId(int sectionId)
        {
            using (var db = new CMSContext())
            {
                var roles = db
                    .Views.Where(v => v.SectionId == sectionId)
                    .SelectMany(v => v.Permissions)
                    .Select(p => p.Role)
                    .Distinct()
                    .Include(r => r.Permissions)
                    .Include(r => r.Permissions.Select(p => p.View))
                    .Include(r => r.UserRoles)
                    .Include(r => r.UserRoles.Select(ur => ur.User))
                    .Include(r => r.UserRoles.Select(ur => ur.Role))
                    .ToList();

                return roles;
            }
        }

        public Role CreateRoleWithSectionId(Role role, int sectionId)
        {
            using (var db = new CMSContext())
            {
                var newRole = db.Roles.Add(role);


                var views = db
                    .Sections
                    .Where(s => s.Id == sectionId)
                    .Include(s => s.Views)
                    .SelectMany(s => s.Views)
                    .Select(v => v);

                foreach (var view in views)
                {
                    var permission = new Permission()
                    {
                        PermissionType = PermissionType.Read,
                        Role = newRole,
                        View = view
                    };

                    db.Permissions.Add(permission);
                }

                db.SaveChanges();

                return newRole;
            }
        }

        public UserRole GetUserRole(int roleId, int userId)
        {
            using (var db = new CMSContext())
            {
                return db.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);
            }
        }

        public UserRole AddUserToRole(UserRole userRole, string userLogin)
        {
            using (var db = new CMSContext())
            {
                var currentUser = db.Users.FirstOrDefault(u => u.Login.ToUpper() == userLogin.ToUpper());

                if (!db.UserRoles.Any(ur => ur.RoleId == userRole.RoleId && ur.UserId == currentUser.Id && ur.UserCanChangeRole))
                    throw new CustomValidationException("Current user doesn't have rights to add users to the role.");

                var newUserRole = db.UserRoles.Add(userRole);
                db.SaveChanges();

                return newUserRole;
            }
        }

        public void UpdateUserRole(UserRole userRole, string userLogin)
        {
            using (var db = new CMSContext())
            {
                var dbUserRole = db.UserRoles.Find(userRole.RoleId, userRole.UserId);

                var currentUser = db.Users.FirstOrDefault(u => u.Login.ToUpper() == userLogin.ToUpper());

                if (!db.UserRoles.Any(ur => ur.RoleId == userRole.RoleId && ur.UserId == currentUser.Id && ur.UserCanChangeRole))
                    throw new CustomValidationException("Current user doesn't have rights to update this user-role relation.");

                db.Entry(dbUserRole).CurrentValues.SetValues(userRole);

                db.SaveChanges();
            }
        }

        public void RemoveUserFromRole(UserRole userRole, string userLogin)
        {
            using (var db = new CMSContext())
            {
                var currentUser = db.Users.FirstOrDefault(u => u.Login.ToUpper() == userLogin.ToUpper());

                if (!db.UserRoles.Any(ur => ur.RoleId == userRole.RoleId && ur.UserId == currentUser.Id && ur.UserCanChangeRole))
                    throw new CustomValidationException("Current user doesn't have rights to delete permissions from this role.");

                var dbUserRole = db.UserRoles.FirstOrDefault(ur => ur.RoleId == userRole.RoleId && ur.UserId == userRole.UserId);

                db.UserRoles.Remove(dbUserRole);
                db.SaveChanges();
            }
        }

        public Permission AddPermissionToRole(Permission permission, string userLogin)
        {
            using (var db = new CMSContext())
            {
                var currentUser = db.Users.FirstOrDefault(u => u.Login.ToUpper() == userLogin.ToUpper());

                if (!db.UserRoles.Any(ur => ur.RoleId == permission.RoleId && ur.UserId == currentUser.Id && ur.UserCanChangeRole))
                    throw new CustomValidationException("Current user doesn't have rights to add permissions to this role.");

                var np = db.Permissions.Add(permission);
                db.SaveChanges();

                var newPermission = db.Permissions.Include(p => p.View).FirstOrDefault(p => p.Id == np.Id);

                return newPermission;
            }
        }

        public void RemovePermissionFromRole(Permission permission, string userLogin)
        {
            using (var db = new CMSContext())
            {
                var currentUser = db.Users.FirstOrDefault(u => u.Login.ToUpper() == userLogin.ToUpper());

                if (!db.UserRoles.Any(ur => ur.RoleId == permission.RoleId && ur.UserId == currentUser.Id && ur.UserCanChangeRole))
                    throw new CustomValidationException("Current user doesn't have rights to delete permissions from this role.");

                var dbPermission = db.Permissions.FirstOrDefault(p => p.Id == permission.Id);

                db.Permissions.Remove(dbPermission);

                db.SaveChanges();
            }
        }

        public void UpdatePermission(Permission permission)
        {
            using (var db = new CMSContext())
            {
                var dbPermission = db.Permissions.Find(permission.Id);

                db.Entry(dbPermission).CurrentValues.SetValues(permission);

                db.SaveChanges();
            }
        }

        #endregion

        #region Private

        private bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$");
        }

        private string GetDataTableScript(string dataTableName, IEnumerable<Field> fields)
        {
            var query = string.Empty;

            var fieldList = new List<string>();

            fieldList.Add("Id int identity(1, 1) primary key");

            if (fields != null)
                fieldList.AddRange(GenerateColumns(fields));

            query = string.Format("create table {0} ({1});", dataTableName, string.Join(", ", fieldList));

            return query;
        }

        private string GetDictionaryTableScript(string dictTableName, DictionaryType dictType)
        {
            var fieldList = new List<string>();

            if (dictType == DictionaryType.Int)
                fieldList.Add("Id int primary key");
            else
                fieldList.Add("Id nvarchar(20) primary key");

            fieldList.Add("Description nvarchar(500) null");

            return $"create table {dictTableName} ({string.Join(", ", fieldList)})";
        }

        private string GetHistoryTableScript(string dataTableName, string historyTableName, IEnumerable<Field> fields)
        {
            var query = string.Empty;

            var fieldList = new List<string>();

            fieldList.Add("Id int identity(1, 1) primary key");
            fieldList.Add("DocId int");
            fieldList.Add("VersionId int not null");
            fieldList.Add($"foreign key (DocId) references {dataTableName}(Id)");

            if (fields != null)
                fieldList.AddRange(GenerateColumns(fields));

            query = string.Format("create table {0} ({1});", historyTableName, string.Join(", ", fieldList));

            return query;
        }

        //public string GetSQLType()
        //{
        //    return Constants.CMS_TO_SQL[FieldType];
        //}

        //public FieldType GetFieldType()
        //{
        //    return FieldType;
        //}

        //public int? GetLength()
        //{
        //    return Length;
        //}

        //public DictionaryType? GetDictionaryType()
        //{
        //    return Dictionary?.DictionaryType;
        //}

        private List<string> GenerateColumns(IEnumerable<Field> fields)
        {
            return fields.Select(f =>
                string.Format("[{0}] {1} {2}", f.Name,
                Constants.CMS_TO_SQL[f.FieldType].ToLower() != "nvarchar" ? Constants.CMS_TO_SQL[f.FieldType] 
                : Constants.CMS_TO_SQL[f.FieldType] + "(" + f.Length + ")", "null")).ToList();
        }

        #endregion
    }
}
