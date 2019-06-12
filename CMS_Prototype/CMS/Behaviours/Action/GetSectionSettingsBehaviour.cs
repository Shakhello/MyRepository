using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMS.UI;
using Common.ExtensionMethods;

namespace CMS.Behaviours
{
    internal class GetSectionSettingsBehaviour : Behaviour, IActionBehaviour
    {
        public GetSectionSettingsBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            var sectionIdStr = action.Parameters.FirstOrDefault(p => p.ParameterType == ParameterType.SectionId).DefaultValue;

            var sectionId = Convert.ToInt32(sectionIdStr);

            var dbRoles = DbEditorService.GetRolesBySectionId(sectionId);

            for (var i = 0; i < dbRoles.Count; i++)
            {
                var irrelevantPermissions = dbRoles[i].Permissions.Where(p => p.View.ParentView != null).ToList();

                for (var ii = 0; ii < irrelevantPermissions.Count(); ii++)
                    dbRoles[i].Permissions.Remove(irrelevantPermissions[ii]);
            }

            var roles = dbRoles.Select(r => 
            {
                var ur = DbEditorService.GetUserRole(r.Id, CurrentUser.Id);
                var roleDef = Mapper.Map<UI.RoleDefinition>(r);

                roleDef.CanBeEditedByCurrentUser = ur?.UserCanChangeRole ?? false;
                return roleDef;

            }).ToList();

            var view = new SectionSettingsViewBehaviour(CurrentUser).Make(null, null, null, (v) => 
            {
                v.Props.Add("Roles", roles.ToList());
                v.Props.Add("SectionId", sectionId);
            });

            return new ActionResult()
            {
                Success = true,
                ActionType = ActionType.GetSectionSettings,
                Data = view
            };
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, UI.Event parentNode)
        {
            var action = new UI.Action()
            {
                Virtual = true,
                ParentNode = parentNode,
                ActionType = ActionType.GetSectionSettings
            };

            action.Parameters = new List<Parameter>()
            {
                new UI.Parameter()
                {
                    ParameterType = ParameterType.SectionId,
                    ParentNode = action,
                    Virtual = true,
                    DefaultValue = ((SectionDefinition)parentNode.ParentNode.ParentNode.Definition).Id.ToString()
                }
            };

            return action;
        }
    }
}
