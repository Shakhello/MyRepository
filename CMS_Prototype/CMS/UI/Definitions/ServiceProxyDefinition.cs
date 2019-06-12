using System.Collections.Generic;

namespace CMS.UI
{
    public class ServiceProxyDefinition
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<ServiceProxyParameterDefinition> Parameters { get; set; }
    }
}
