using CMS.Incoming;
using CMS.Services;
using CMS.UI;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Unity.Controllers
{
    public class DataController : BaseController
    {
        #region Non-GUI API

        /// <summary>
        /// Создает новый документ в какой-либо из таблиц данных
        /// </summary>
        /// <param name="document">Документ</param>
        /// <returns>Ид созданного документа</returns>
        [HttpPost]
        [Route("api/ticket/CreateDocument")]
        public Response<int> CreateDocument([FromBody] Document document)
        {
            return GetResponse<int>(() =>
            {
                return new DataService(User).CreateDocument(document);
            });
        }

        /// <summary>
        /// Получает документ по ид темплейта и ид документа
        /// </summary>
        /// <param name="templateId">Ид темплейта</param>
        /// <param name="docId">Ид документа</param>
        /// <returns>Документ</returns>
        [HttpGet]
        [Route("api/ticket/GetDocument")]
        public Response<Document> GetDocument(int templateId, int docId)
        {
            return GetResponse<Document>(() =>
            {
                return new DataService(User).GetDocument(templateId, docId);
            });
        }

        /// <summary>
        /// Находит документы по произвольному параметру
        /// </summary>
        /// <param name="templateId">ID шаблона</param>
        /// <param name="paramName">Имя поля</param>
        /// <param name="paramValue">Значение поля</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/GetDocumentsByParameter")]
        public Response<Document> GetDocumentsByParameter(int templateId, string paramName, string paramValue)
        {
            return GetResponse<Document>(() =>
            {
                try
                {
                    return new DataService(User).GetDocumentsByParameter(templateId, paramName, paramValue);
                }
                catch (Exception e)
                {
                    return null;
                }
            });
        }

        [HttpPost]
        [Route("api/ticket/AddFileToDocument")]
        public Response<int> AddFileToDocument([FromBody] FileInput fileInput)
        {
            return GetResponse<int>(() =>
            {
                return new DataService(User).AddFile(fileInput);
            });
        }

        /// <summary>
        /// Удаляет документ по ид темплейта и ид документа
        /// </summary>
        /// <param name="templateId">Ид темплейта</param>
        /// <param name="docId">Ид документа</param>
        /// <returns>Успешность выполнения</returns>
        [HttpGet]
        [Route("api/ticket/DeleteDocument")]
        public Response DeleteDocument(int templateId, int docId)
        {
            return GetResponse(() =>
            {
                new DataService(User).DeleteDocument(templateId, docId);
            });
        }

        /// <summary>
        /// Создает тикет и связывает его с родительским тикетом
        /// </summary>
        /// <param name="docementWithLink">Объект типа TicketLink</param>
        /// <returns>Успешность выполнения</returns>
        //[HttpPost]
        //[Route("api/ticket/CreateDocumentWithLink")]
        //public Response<int> CreateDocumentWithLink([FromBody] DocumentWithLink docementWithLink)
        //{
        //    return GetResponse<int>(() =>
        //    {
        //        return new DataService(User).CreateDocumentWithLink(docementWithLink);
        //    });
        //}


        #endregion
    }
}