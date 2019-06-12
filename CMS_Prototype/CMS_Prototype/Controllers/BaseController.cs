using Common;
using Common.Context;
using Common.Exceptions;
using Common.Logging;
using Unity.Common;
using Unity.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Unity.Controllers
{
    /// <summary>
    /// 
    /// </summary>

    [AuthenticateUnityUser]
    //[EnableCors(origins: "http://localhost:63113,http://localhost", headers: "*", methods: "*")]
    public abstract class BaseController : ApiController, ILoggable
    {
        public IContextProvider ContextProvider;


        public ILogger Logger { get; set; }

        protected Response GetResponse(Action action)
        {
            return GetResponse<Response, Action, object>(action);
        }

        protected Response<T> GetResponse<T>(Func<T> action)
        {
            return GetResponse<Response<T>, Func<T>, T>(action);
        }

        protected Response<T> GetResponse<T>(Func<IEnumerable<T>> action)
        {
            return GetResponse<Response<T>, Func<IEnumerable<T>>, T>(action);
        }

        protected void ValidatePostModel<T>(T model) where T : class
        {
            if (model == null)
                throw new CustomValidationException(RC.POST_MODEL_NOT_MAPPED);
        }

        #region Private
        private TResponse GetResponse<TResponse, TAction, TEntity>(TAction action) where TResponse : class
        {

#if !DEBUG
            try
            {
#endif
                if (action is Action)
                {
                    (action as Action)();
                    return Response.Ok() as TResponse;
                }
                else if (action is Func<TEntity>)
                {
                    var result = (action as Func<TEntity>)();
                    return Response<TEntity>.Ok(result) as TResponse;
                }
                else if (action is Func<IEnumerable<TEntity>>)
                {
                    var result = (action as Func<IEnumerable<TEntity>>)();
                    return Response<TEntity>.Ok(result) as TResponse;
                }
                else
                    throw new CustomValidationException("Unknown type of action passed to GetResponse method.");
#if !DEBUG
            }

            catch (Exception e)
            {
                Logger.Error(e);

                if (action is Action)
                    return Response.Error($"RequestId: {ContextProvider.RequestId} {e.Message}") as TResponse;
                else
                    return Response<TEntity>.Error($"RequestId: {ContextProvider.RequestId} {e.Message}") as TResponse;
            }
#endif
        }


        private void ValidateForLoops<TEntity>()
        {
            if (!typeof(TEntity).IsValueType)
            {
                //var objects = new Dictionary<TEntity>
            }
        }

        #endregion

    }
}