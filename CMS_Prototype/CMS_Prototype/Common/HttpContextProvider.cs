using Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Unity.Common
{
    public class HttpContextProvider : IContextProvider
    {
        RouteData _routeData;

        /// <summary>
        /// 
        /// </summary>
        public HttpContextProvider()
        {
            HttpRequest request;

            try
            {
                request = HttpContext.Current.Request;
            }
            catch
            {
                request = null;
            }

            _routeData = request?.RequestContext.RouteData;
            
        }

        /// <summary>
        /// Ид реквеста
        /// </summary>
        public Guid RequestId
        {
            get
            {
                return (Guid)_routeData?.Values["RequestId"];
            }
        }

        /// <summary>
        /// IP реквеста
        /// </summary>
        public string RequestIP
        {
            get
            {
                return (string)_routeData?.Values["RequestIP"];
            }
        }

        /// <summary>
        /// URL реквеста
        /// </summary>
        public string RequestURL
        {
            get
            {
                return (string)_routeData?.Values["RequestURL"];
            }
        }

        /// <summary>
        /// Дата и время реквеста
        /// </summary>
        public DateTime RequestDateTime
        {
            get
            {
                return _routeData != null ? (DateTime)_routeData.Values["RequestDateTime"] : DateTime.Now;
            }
        }
    }
}