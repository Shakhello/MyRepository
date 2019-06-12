using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CMS.DAL.Services;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours
{
    internal class FileControlBehaviour : Behaviour, IControlBehaviour
    {
        public FileControlBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public Control Make(ControlDefinition definition, DAL.Models.DbSearchResponse ticketSet, Instance parentNode, Action<Control> controlAction)
        {
            Control control = new Control
            {
                ControlType = Mapper.Map<ControlType>(definition.ControlType),
                ParentNode = parentNode
            };

            var ticket = ticketSet.Tickets.FirstOrDefault();

            if (ticket == null)
            {
                throw new CustomValidationException("Ticket is null");
            }

            DAL.Models.Control dalControl = (DAL.Models.Control)definition.Entity;

            if(!dalControl.FieldId.HasValue || dalControl.FieldId <= 0)
                throw new CustomValidationException("Control.FieldId must have value");


            control.DocId = Convert.ToInt32(ticket[dalControl.Field.TemplateId, "Id"]);

            if (!control.DocId.HasValue || control.DocId <= 0)
                throw new CustomValidationException("Control.DocId must has value");

            control.Props.Add("DisplayName", definition.DisplayName);
            control.Props.Add("FieldId", dalControl.Field.Id);
            control.Props.Add("Order", definition.OrderIndex);
            control.Props.Add("Width", definition.Width);
            control.Props.Add("Style", Mapper.Map<StyleDefinition>(((DAL.Models.Control)definition.Entity).Style)?.GetPropertiesAsArray());

            DbFileService fileService = new DbFileService();

            List<DAL.Models.File> files = fileService.GetFiles(dalControl.FieldId.Value, control.DocId.Value);

            control.Props.Add("Files", files);

            Event eventUpload = new Event()
            {
                Virtual = true,
                EventType = EventType.Click,
                ParentNode = control
            };

            var actionUpload = new UploadFileActionBehaviour(CurrentUser).Make(null, null, eventUpload);

            eventUpload.Actions = new List<UI.Action>() { actionUpload };

            control.Events = new List<Event>() { eventUpload };

            return control;
        }

    }
}
