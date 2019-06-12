using AutoMapper;

using CMS.DAL.Services;
using CMS.UI;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace CMS.Services
{
    public class EditorService : Service
    {
        public EditorService(IPrincipal currentPrincipal) : base(currentPrincipal) { }

        #region Templates

        public UI.TemplateDefinition GetTemplate(int id)
        {
            var dbTemplate = DbEditorService.GetTemplate(id);
            var template = Mapper.Map<UI.TemplateDefinition>(dbTemplate);

            return template;
        }

        public List<UI.TemplateDefinition> GetTemplates()
        {
            var dbTemplates = DbEditorService.GetExistingTemplates();
            return dbTemplates.Select(t => Mapper.Map<UI.TemplateDefinition>(t)).ToList();
        }

        public UI.TemplateDefinition CreateTemplate(UI.TemplateDefinition template)
        {
            var dalTemplate = DbEditorService.CreateTemplate(Mapper.Map<DAL.Models.Template>(template));
            return Mapper.Map<UI.TemplateDefinition>(dalTemplate);
        }

        public void DeleteTemplate(int id)
        {
            DbEditorService.DeleteTemplate(id);
        }

        public UI.TemplateDefinition UpdateTemplate(UI.TemplateDefinition template)
        {
            var dbTemplate = Mapper.Map<DAL.Models.Template>(template);
            dbTemplate = DbEditorService.UpdateTemplate(dbTemplate);

            return Mapper.Map<UI.TemplateDefinition>(dbTemplate);
        }

        #endregion

        #region Fields

        public UI.FieldDefinition UpdateField(UI.FieldDefinition field)
        {
            var dbField = Mapper.Map<DAL.Models.Field>(field);
            dbField = DbEditorService.UpdateField(dbField);

            return Mapper.Map<UI.FieldDefinition>(dbField);
        }

        public UI.FieldDefinition CreateField(UI.FieldDefinition field)
        {
            var dbField = Mapper.Map<DAL.Models.Field>(field);
            dbField = DbEditorService.CreateField(dbField);

            return Mapper.Map<UI.FieldDefinition>(dbField);
        }

        public UI.FieldDefinition CreateReferenceField(UI.FieldDefinition fieldA, UI.FieldDefinition fieldB)
        {
            var dbField1 = Mapper.Map<DAL.Models.Field>(fieldA);
            var dbField2 = Mapper.Map<DAL.Models.Field>(fieldB);

            var newField = DbEditorService.CreateReferenceField(dbField1, dbField2);

            return Mapper.Map<UI.FieldDefinition>(newField);
        }

        public void DeleteField(int id)
        {
            DbEditorService.DeleteField(id);
        }

        #endregion

        #region Dictionaries

        public List<UI.DictionaryDefinition> GetDictionaries()
        {
            var dbDicts = DbEditorService.GetDictionaries();

            return dbDicts.Select(d => Mapper.Map<UI.DictionaryDefinition>(d)).ToList();
        }

        public UI.DictionaryDefinition GetDictionary(int id)
        {
            var dbDict = DbEditorService.GetDictionary(id);

            return Mapper.Map<UI.DictionaryDefinition>(dbDict);
        }

        public UI.DictionaryDefinition CreateDictionary(UI.DictionaryDefinition dict)
        {
            var dalDict = DbEditorService.CreateDictionary(Mapper.Map<DAL.Models.Dictionary>(dict));

            return Mapper.Map<UI.DictionaryDefinition>(dalDict);
        }

        public void DeleteDictionary(int id)
        {
            DbEditorService.DeleteDictionary(id);
        }

        #endregion

        #region Views

        public UI.ViewDefinition GetView(int id)
        {
            var dalView = DbEditorService.GetViewDeep(id);

            return Mapper.Map<UI.ViewDefinition>(dalView);
        }

        public List<UI.ViewDefinition> GetViews()
        {
            var views = DbEditorService.GetViews();

            return views.Select(v => Mapper.Map<UI.ViewDefinition>(v)).ToList();
        }

        public List<UI.ViewDefinition> FindViewsByName(string name)
        {
            var views = DbEditorService.FindViewsByName(name);

            return views.Select(v => Mapper.Map<UI.ViewDefinition>(v)).ToList();
        }

        public UI.ViewDefinition CreateView(UI.ViewDefinition view)
        {
            var dalView = DbEditorService.CreateView(Mapper.Map<DAL.Models.View>(view), CurrentUser.Login);

            if (view.Controls != null)
            {
                foreach (var control in view.Controls)
                {
                    var dalControl = DbEditorService.CreateControl(Mapper.Map<DAL.Models.Control>(control));
                    dalView.Controls.Add(dalControl);
                }
            }

            var newView = Mapper.Map<UI.ViewDefinition>(dalView);

            if (view.ChildViews != null)
            {
                for (var i = 0; i < view.ChildViews.Count; i++)
                {
                    var subView = CreateView(view.ChildViews[i]);
                    newView.ChildViews.Add(subView);
                }
            }

            return newView;
        }

        public UI.ViewDefinition UpdateView(UI.ViewDefinition view)
        {
            var dbView = Mapper.Map<DAL.Models.View>(view);
            dbView = DbEditorService.UpdateView(dbView);

            return Mapper.Map<UI.ViewDefinition>(dbView);
        }

        public void DeleteView(int id)
        {
            DbEditorService.DeleteView(id);
        }

        #endregion

        #region Sections

        public List<UI.SectionDefinition> GetSections()
        {
            var sections = DbEditorService.GetSections(CurrentUser.Login);

            return sections.Select(s => Mapper.Map<UI.SectionDefinition>(s)).ToList();
        }

        public UI.SectionDefinition CreateSection(UI.SectionDefinition section)
        {
            var dalSection = DbEditorService.CreateSection(Mapper.Map<DAL.Models.Section>(section));

            return Mapper.Map<UI.SectionDefinition>(dalSection);
        }

        public void DeleteSection(int id)
        {
            DbEditorService.DeleteSection(id);
        }

        #endregion

        #region Controls

        public UI.ControlDefinition GetControl(int id)
        {
            var control = DbEditorService.GetControl(id);

            return Mapper.Map<UI.ControlDefinition>(control);
        }

        public List<UI.ControlDefinition> GetControlsByViewId(int viewId)
        {
            var controls = DbEditorService.GetControlsByViewId(viewId);

            return controls.Select(c => Mapper.Map<UI.ControlDefinition>(c)).ToList();
        }

        public UI.ControlDefinition CreateControl(UI.ControlDefinition control)
        {
            var dalControl = DbEditorService.CreateControl(Mapper.Map<DAL.Models.Control>(control));

            return Mapper.Map<UI.ControlDefinition>(dalControl);
        }

        public UI.ControlDefinition UpdateControl(UI.ControlDefinition control)
        {
            var dbControl = Mapper.Map<DAL.Models.Control>(control);
            dbControl = DbEditorService.UpdateControl(dbControl);

            return Mapper.Map<UI.ControlDefinition>(dbControl);
        }

        public void DeleteControl(int id)
        {
            DbEditorService.DeleteControl(id);
        }

        public UI.ControlFieldDefinition CreateControlField(UI.ControlFieldDefinition controlField)
        {
            var dalControlField = DbEditorService.CreateControlField(Mapper.Map<DAL.Models.ControlField>(controlField));

            return Mapper.Map<UI.ControlFieldDefinition>(dalControlField);
        }

        public void DeleteControlField(UI.ControlFieldDefinition controlField)
        {
            DbEditorService.DeleteControlField(Mapper.Map<DAL.Models.ControlField>(controlField));
        }

        #endregion

        #region Filters

        public UI.FilterDefinition GetFilter(int id)
        {
            var filter = DbEditorService.GetFilter(id);

            return Mapper.Map<UI.FilterDefinition>(filter);
        }


        public List<UI.FilterDefinition> GetFiltersByViewId(int viewId)
        {
            var filters = DbEditorService.GetFiltersByViewId(viewId);

            return filters.Select(c => Mapper.Map<UI.FilterDefinition>(c)).ToList();
        }

        public UI.FilterDefinition CreateFilter(UI.FilterDefinition filter)
        {
            var dalFilter = DbEditorService.CreateFilter(Mapper.Map<DAL.Models.Filter>(filter));

            return Mapper.Map<UI.FilterDefinition>(dalFilter);
        }

        public UI.FilterDefinition UpdateFilter(UI.FilterDefinition filter)
        {
            var dbFilter = Mapper.Map<DAL.Models.Filter>(filter);
            dbFilter = DbEditorService.UpdateFilter(dbFilter);

            return Mapper.Map<UI.FilterDefinition>(dbFilter);
        }

        public void DeleteFilter(int id)
        {
            DbEditorService.DeleteFilter(id);
        }

        public UI.FilterFieldDefinition CreateFilterField(UI.FilterFieldDefinition filterField)
        {
            var dalFilterField = DbEditorService.CreateFilterField(Mapper.Map<DAL.Models.FilterField>(filterField));

            return Mapper.Map<UI.FilterFieldDefinition>(dalFilterField);
        }

        public void DeleteFilterField(UI.FilterFieldDefinition filterField)
        {
            DbEditorService.DeleteFilterField(Mapper.Map<DAL.Models.FilterField>(filterField));
        }

        #endregion

        #region Styles

        public List<UI.StyleDefinition> GetStyles()
        {
            var dbTemplates = DbEditorService.GetStyles();

            return dbTemplates.Select(t => Mapper.Map<UI.StyleDefinition>(t)).ToList();
        }

        public StyleDefinition GetStyle(int id)
        {
            var dbStyle = DbEditorService.GetStyle(id);

            return Mapper.Map<StyleDefinition>(dbStyle);
        }

        public StyleDefinition CreateStyle(StyleDefinition style)
        {
            var dbStyle = Mapper.Map<DAL.Models.Style>(style);
            dbStyle = DbEditorService.CreateStyle(dbStyle);

            return Mapper.Map<UI.StyleDefinition>(dbStyle);
        }

        public StyleDefinition UpdateStyle(StyleDefinition style)
        {
            var dbStyle = Mapper.Map<DAL.Models.Style>(style);
            dbStyle = DbEditorService.UpdateStyle(dbStyle);

            return Mapper.Map<UI.StyleDefinition>(dbStyle);
        }

        public void DeleteStyle(int id)
        {
            DbEditorService.DeleteStyle(id);
        }

        #endregion

        #region Events

        public UI.EventDefinition GetEvent(int id)
        {
            var dalEvent = DbEditorService.GetEvent(id);

            return Mapper.Map<UI.EventDefinition>(dalEvent);
        }

        public UI.EventDefinition CreateEvent(UI.EventDefinition evt)
        {
            var dalEvent = DbEditorService.CreateEvent(Mapper.Map<DAL.Models.Event>(evt));

            return Mapper.Map<UI.EventDefinition>(dalEvent);
        }

        public void DeleteEvent(int id)
        {
            DbEditorService.DeleteEvent(id);
        }

        #endregion

        #region Actions

        public UI.ActionDefinition CreateAction(UI.ActionDefinition action)
        {
            var dalAction = DbEditorService.CreateAction(Mapper.Map<DAL.Models.Action>(action));

            return Mapper.Map<UI.ActionDefinition>(dalAction);
        }

        public UI.ActionDefinition UpdateAction(UI.ActionDefinition action)
        {
            var dalAction = DbEditorService.UpdateAction(Mapper.Map<DAL.Models.Action>(action));

            return Mapper.Map<UI.ActionDefinition>(dalAction);
        }

        public void DeleteAction(int id)
        {
            DbEditorService.DeleteAction(id);
        }

        #endregion

        #region Parameters

        public List<UI.ParameterDefinition> CreateParameters(IEnumerable<UI.ParameterDefinition> parameters)
        {
            var dalParameters = DbEditorService.CreateParameters(parameters.Select(p => Mapper.Map<DAL.Models.Parameter>(p)).ToList());

            return dalParameters.Select(p => Mapper.Map<UI.ParameterDefinition>(p)).ToList();
        }

        public void DeleteParameters(IEnumerable<int> ids)
        {
            DbEditorService.DeleteParameters(ids);
        }

        #endregion

        #region Users, Roles, Permissions

        public List<UI.UserDefinition> FindUsersByLogin(string login)
        {
            var dbUsers = DbEditorService.FindUsersByLogin(login);

            var users =  dbUsers.Select(dbUser => Mapper.Map<UI.UserDefinition>(dbUser)).ToList();

            return users;
        }

        public UI.UserDefinition GetOrCreateUser(string login)
        {
            var existingUser = DbEditorService.GetUserByLogin(login);

            if (existingUser == null)
            {
                var newDbUser = DbEditorService.CreateUser(new DAL.Models.User() { Login = login });
                return Mapper.Map<UI.UserDefinition>(newDbUser);
            }
            else
                return Mapper.Map<UI.UserDefinition>(existingUser);
        }

        public RoleDefinition CreateRoleWithSectionId(RoleDefinition role, int sectionId)
        {
            var dalRole = Mapper.Map<DAL.Models.Role>(role);

            var newRole = DbEditorService.CreateRoleWithSectionId(dalRole, sectionId);

            return Mapper.Map<UI.RoleDefinition>(newRole);
        }

        public UserRoleDefinition GetUserRole(int roleId, int userId)
        {
            var dbUserRole = DbEditorService.GetUserRole(roleId, userId);

            return Mapper.Map<UI.UserRoleDefinition>(dbUserRole);
        }

        public UserRoleDefinition AddUserToRole(UserRoleDefinition userRole)
        {
            var dalUserRole = Mapper.Map<DAL.Models.UserRole>(userRole);

            var newUserRole = DbEditorService.AddUserToRole(dalUserRole, CurrentUser.Login);

            return Mapper.Map<UI.UserRoleDefinition>(newUserRole);
        }

        public void UpdateUserRole(UserRoleDefinition userRole)
        {
            var dalUserRole = Mapper.Map<DAL.Models.UserRole>(userRole);

            DbEditorService.UpdateUserRole(dalUserRole, CurrentUser.Login);
        }

        public void RemoveUserFromRole(UserRoleDefinition userRole)
        {
            var dalUserRole = Mapper.Map<DAL.Models.UserRole>(userRole);

            DbEditorService.RemoveUserFromRole(dalUserRole, CurrentUser.Login);
        }

        public PermissionDefinition AddPermissionToRole(PermissionDefinition permission)
        {
            var dalPermission = Mapper.Map<DAL.Models.Permission>(permission);

            var newPermission = DbEditorService.AddPermissionToRole(dalPermission, CurrentUser.Login);

            return Mapper.Map<UI.PermissionDefinition>(newPermission);
        }

        public void RemovePermissionFromRole(PermissionDefinition permission)
        {
            var dalPermission = Mapper.Map<DAL.Models.Permission>(permission);

            DbEditorService.RemovePermissionFromRole(dalPermission, CurrentUser.Login);
        }

        public void UpdatePermission(PermissionDefinition permission)
        {
            var dalPermission = Mapper.Map<DAL.Models.Permission>(permission);

            DbEditorService.UpdatePermission(dalPermission);
        }

        #endregion

    }
}
