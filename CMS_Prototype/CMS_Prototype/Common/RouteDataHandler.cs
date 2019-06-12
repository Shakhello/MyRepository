using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Unity.Common
{
    public class RouteDataHandler : DelegatingHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestId = Guid.NewGuid();
            var curRequest = HttpContext.Current.Request;
            var requestURL = curRequest.Url.AbsoluteUri;
            var ip = curRequest.UserHostAddress;

            curRequest.RequestContext.RouteData.Values.Add("RequestId", requestId);
            curRequest.RequestContext.RouteData.Values.Add("RequestIP", ip);
            curRequest.RequestContext.RouteData.Values.Add("RequestDateTime", DateTime.Now);
            curRequest.RequestContext.RouteData.Values.Add("RequestURL", requestURL);


            return await base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    return task.Result;
                },
                cancellationToken);
        }
    }
}