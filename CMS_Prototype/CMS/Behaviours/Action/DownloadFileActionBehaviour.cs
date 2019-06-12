using CMS.UI;
using Common.Exceptions;
using System;

namespace CMS.Behaviours
{
    internal class DownloadFileActionBehaviour : Behaviour, IActionBehaviour
    {
        public DownloadFileActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            if (!action.Props.ContainsKey("FileId"))
            {
                throw new CustomValidationException("There is not Prop FileId");
            }

            var fileId = Convert.ToInt32(action.Props["FileId"]);

            var fileService = new DAL.Services.DbFileService();

            var file = fileService.GetFile(fileId);

            return new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType,
                Data = new Incoming.File()
                {
                    Name = file.Name,
                    ContentType = file.ContentType,
                    ContentData = file.ContentData
                }
            };
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            var action = new UI.Action()
            {
                Virtual = true,
                ParentNode = parentNode,
                ActionType = ActionType.DownloadFile,
                DocId = parentNode.DocId
            };

            return action;
        }
    }
}
