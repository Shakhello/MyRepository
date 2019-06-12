using CMS.UI;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Behaviours
{
    internal class UploadFileActionBehaviour : Behaviour, IActionBehaviour
    {
        public UploadFileActionBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public ActionResult Execute(UI.Action action)
        {
            if (!action.Props.ContainsKey("FieldId"))
            {
                throw new CustomValidationException("There is no FieldId in Action.Props");
            }

            int fieldId = Convert.ToInt32(action.Props["FieldId"]);

            if (fieldId <= 0)
            {
                throw new CustomValidationException("fieldId <= 0");
            }

            int docId = action.DocId.Value;

            IEnumerable<DAL.Models.File> files = (action.Value as List<Incoming.File>).Select(f => new DAL.Models.File()
            {
                Name = f.Name,
                ContentType = f.ContentType,
                ContentData = f.ContentData,
                Comment = f.Comment,
                CreateDate = DateTime.Now,
                CreatedBy = CurrentUser.Login
            });

            DAL.Services.DbFileService fileService = new DAL.Services.DbFileService();

            return new ActionResult()
            {
                Success = true,
                ActionType = action.ActionType,
                Data = fileService.AddFiles(fieldId, docId, files).Select(f=>new Incoming.File()
                {
                    Id = f.Id,
                    Name = f.Name,
                    Comment = f.Comment
                })
            };
        }

        public UI.Action Make(ActionDefinition definition, DAL.Models.DbSearchResponse ticketSet, Event parentNode)
        {
            if (parentNode == null)
            {
                throw new CustomValidationException("Event argument in UploadFileActionBehaviour is null");
            }

            Control control = parentNode.ParentNode as Control;

            if(!control.Props.ContainsKey("FieldId"))
            {
                throw new CustomValidationException("There is no FieldId Prop in FileUploadControl");
            }

            int fieldId = Convert.ToInt32(control.Props["FieldId"]);

            if (fieldId <= 0)
            {
                throw new CustomValidationException("FieldId <= 0");
            }

            var action = new UI.Action()
            {
                Virtual = true,
                ParentNode = parentNode,
                ActionType = ActionType.UploadFile,
                DocId = control.DocId,
                Props = new Dictionary<string, object>() {
                    { "FieldId", fieldId }
                }
            };

            return action;
        }
    }
}
