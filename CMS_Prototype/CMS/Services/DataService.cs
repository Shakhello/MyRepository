using CMS.DAL.Services;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CMS.Services
{
    public class DataService : Service
    {
        public DataService(IPrincipal currentPrincipal) : base(currentPrincipal) { }

        #region Non-GUI API

        public int CreateDocument(Incoming.Document document)
        {
            var template = DbEditorService.GetTemplate(document.TemplateId);

            if (template == null)
                throw new CustomValidationException($"Template not found for id {document.TemplateId}");

            var values = template.Fields
                .Select(f => new KeyValuePair<DAL.Models.Field, object>(f, document.Values[f.Name]))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            return DbDocService.CreateDocument(values);
        }

        //public int CreateDocumentWithLink(Incoming.DocumentWithLink documentWithLink)
        //{
        //    var document = documentWithLink.Document;

        //    var template = DbEditorService.GetTemplate(document.TemplateId);

        //    if (template == null)
        //        throw new CustomValidationException($"Template not found for id {document.TemplateId}");

        //    var docId = DbDocService.CreateDocument(template, document.Values);

        //    DbDocService.CreateLink(documentWithLink.FieldId, documentWithLink.ParentDocId, docId);

        //    return docId;
        //}

        public Incoming.Document GetDocument(int templateId, int docId)
        {
            var template = DbEditorService.GetTemplate(templateId);

            if (template == null)
                throw new CustomValidationException($"Template not found for id {templateId}");

            var doc = new Incoming.Document()
            {
                TemplateId = templateId,
                Values = DbDocService.GetDocument(template.Name, docId)
            };

            return doc;
        }

        public List<Incoming.Document> GetDocumentsByParameter(int templateId, string paramName, object paramValue)
        {
            var template = DbEditorService.GetTemplate(templateId);

            if (template == null)
            {
                throw new CustomValidationException($"Template not found for id:{templateId}");
            }

            var searchResult = DbDocService.GetDocumentsByParameter(template.Name, paramName, paramValue);

            if (searchResult == null)
            {
                throw new CustomValidationException($"Result is null for id:{templateId}, pararmName:{paramName}, paramValue:{paramValue}");
            }

            var result = searchResult.Select(values =>
                new Incoming.Document()
                {
                    TemplateId = templateId,
                    Values = values
                }).ToList();

            return result;
        }

        public void DeleteDocument(int templateId, int docId)
        {
            var template = DbEditorService.GetTemplate(templateId);

            if (template == null)
                throw new CustomValidationException($"Template not found for id {templateId}");

            DbDocService.DeleteDocument(template, docId);
        }

        public List<int> AddFile(Incoming.FileInput fileInput)
        {
            DbFileService fileService = new DbFileService();
            DAL.Models.File file = new DAL.Models.File()
            {
                Name = fileInput.Name,
                ContentType = fileInput.ContentType,
                CreateDate = DateTime.Now,
                ContentData = fileInput.ContentData
            };
            return fileService.AddFiles(fileInput.FieldId, fileInput.DocId, new[] { file }).Select(f => f.Id).ToList();
        }

        #endregion
    }
}
