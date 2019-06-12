using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Models;
using CMS.Proxies;
using CMS.UI;
using Common.Exceptions;

namespace CMS.Behaviours.Action
{
    internal class ServiceCallActionBehaviour : Behaviour, IActionBehaviour
    {
        public ServiceCallActionBehaviour(UserDefinition currentUser):base(currentUser)
        {

        }

        public ActionResult Execute(UI.Action action)
        {
            var serviceName = GetSingleString(action, UI.ParameterType.ServiceProxyName);
            if (serviceName == null)
            {
                throw new CustomValidationException($"Service definition not found.");
            }

            var serviceProxy = new ProxyServiceManager().GetService(serviceName);
            Task.WaitAll(serviceProxy.ExecuteRequest(action));

            return new ActionResult
            {
                Success = true,
            };
        }

        public UI.Action Make(ActionDefinition definition, DbSearchResponse ticketSet, UI.Event parentNode)
        {
            throw new NotImplementedException();
        }

        public int GetSingleInt(UI.Action action, UI.ParameterType parameter)
        {
            var baseValue = string.Empty;
            try
            {
                baseValue = action.Parameters.Single(x => x.ParameterType == UI.ParameterType.ServiceProxyName).DefaultValue;
            }
            catch (InvalidOperationException)
            {
                throw new CustomValidationException($"Single integer parameter of type '{parameter.ToString()}' expected.");
            }

            if (!int.TryParse(baseValue, out int result))
            {
                throw new CustomValidationException($"Couldn't find or read single integer parameter '{parameter.ToString()}'.");
            }

            return result;
        }

        public string GetSingleString(UI.Action action, UI.ParameterType parameter)
        {
            var baseValue = string.Empty;
            try
            {
                baseValue = action.Parameters.Single(x => x.ParameterType == UI.ParameterType.ServiceProxyName).DefaultValue;
            }
            catch (InvalidOperationException)
            {
                throw new CustomValidationException($"Single string parameter of type '{parameter.ToString()}' expected.");
            }

            return baseValue;
        }
    }
}
