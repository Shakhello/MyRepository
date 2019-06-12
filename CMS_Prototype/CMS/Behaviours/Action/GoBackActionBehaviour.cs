using System;
using AutoMapper;
using CMS.DAL.Models;
using CMS.UI;

namespace CMS.Behaviours
{
    internal class GoBackActionBehaviour : Behaviour, IActionBehaviour
    {
        public GoBackActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            var result = new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType,
                Data = null
            };

            return result;
        }

        public UI.Action Make(ActionDefinition definition, DbSearchResponse ticketSet, UI.Event parentNode)
        {
            var action = new UI.Action()
            {
                ActionType = Mapper.Map<UI.ActionType>(definition.ActionType),
                ParentNode = parentNode
            };

            return action;
        }
    }
}
