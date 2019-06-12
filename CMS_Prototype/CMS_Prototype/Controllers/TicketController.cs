using CMS.DAL.Services;
using CMS.Services;
using CMS.UI;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Unity.Controllers
{
    /// <summary>
    /// Контроллер для работы с данными тикетов
    /// </summary>
    public class TicketController : BaseController
    {
        /// <summary>
        /// Получает все представления для текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/GetSections")]
        public Response<Section> GetSections()
        {
            return GetResponse<Section>(() =>
            {
                return new TicketService(User).GetSections();
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/ExecuteEvent")]
        public Response<EventResult> ExecuteEvent([FromBody] Event evt)
        {
            return GetResponse<EventResult>(() =>
            {
                return new TicketService(User).ExecuteEvent(evt);
            });
        }

        /// <summary>
        /// Загружает файлы на сервер
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ticket/UploadFiles")]
        public async System.Threading.Tasks.Task<Response<EventResult>> UploadFilesAsync()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Response<EventResult>.Error("Wrong request format");
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var eventContent = provider.Contents.FirstOrDefault(i => i.Headers.ContentDisposition.Name == "\"data\"");
            var eventJson = eventContent.ReadAsStringAsync().Result;
            Event evnt = JsonConvert.DeserializeObject<Event>(eventJson);

            var actionUpload = evnt.Actions.FirstOrDefault(i => i.ActionType == ActionType.UploadFile);

            actionUpload.Value = (actionUpload.Value as JArray).Select(t =>
            {
                CMS.Incoming.File file = t.ToObject<CMS.Incoming.File>();

                var fileContent = provider.Contents.FirstOrDefault(i => i.Headers.ContentDisposition.FileName.IndexOf(file.Name) > 0);

                var fileData = fileContent.ReadAsByteArrayAsync().Result;

                file.ContentData = fileData;

                return file;
            }).ToList();

            return GetResponse<EventResult>(() =>
            {
                return new TicketService(User).ExecuteEvent(evnt);
            });
        }

        /// <summary>
        /// Скачать файл с сервера
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ticket/DownloadFile")]
        public HttpResponseMessage DownloadFile(string jsonEvent)
        {
            Event evnt = JsonConvert.DeserializeObject<Event>(jsonEvent);

            var eventResult = new TicketService(User).ExecuteEvent(evnt);

            var downloadActionResult = eventResult.ActionResults.FirstOrDefault(ar => ar.ActionType == ActionType.DownloadFile);

            var file = downloadActionResult.Data as CMS.Incoming.File;

            HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(new MemoryStream(file.ContentData));
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("application");
            result.Content.Headers.ContentDisposition.FileName = file.Name;

            return result;
        }
    }
}