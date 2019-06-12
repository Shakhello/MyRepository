using CMS.UI;
using Common.Exceptions;
using System;

namespace CMS.Behaviours
{
    internal class DeleteFileActionBehaviour : Behaviour, IActionBehaviour
    {
        public DeleteFileActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            if (!action.Props.ContainsKey("FileId"))
            {
                throw new CustomValidationException("There is not Prop FileId");
            }

            int fileId = Convert.ToInt32(action.Props["FileId"]);

            DAL.Services.DbFileService fileService = new DAL.Services.DbFileService();

            DAL.Models.File file = fileService.GetFile(fileId);

            file.Deleted = true;
            file.DeleteDate = DateTime.Now;
            file.DeletedBy = CurrentUser.Login;

            fileService.UpdateFile(file);

            return new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType
            };
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            var action = new UI.Action()
            {
                Virtual = true,
                ParentNode = parentNode,
                ActionType = ActionType.DeleteFile,
                DocId = parentNode.DocId
            };

            return action;
        }
    }
}
