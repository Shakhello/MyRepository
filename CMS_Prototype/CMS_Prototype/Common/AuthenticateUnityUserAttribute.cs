using CMS.Services;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Unity.Common
{
    public class AuthenticateUnityUserAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var a = actionContext.ControllerContext.Request?.Headers?.Contains("Origin");

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var userName = HttpContext.Current.User.Identity.Name;

                var nameParts = userName.Split('\\');

                new EditorService(HttpContext.Current.User).GetOrCreateUser(nameParts.Last());
            }
            else
                throw new CustomValidationException("Windows authentication must be enabled for this application.");
        }

        
    }
}