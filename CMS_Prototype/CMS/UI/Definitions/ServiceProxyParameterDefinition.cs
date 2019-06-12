namespace CMS.UI
{
    public class ServiceProxyParameterDefinition
    {
        public string ServiceName { get; set; }
        public string Name { get; set; }
        public ServiceCallParameterInOut InOut { get; set; }
        public FieldType Type { get; set; }
        public bool Required { get; set; }

        public ServiceProxyParameterDefinition(
            string serviceName, 
            string parameterName, 
            ServiceCallParameterInOut inOut, 
            bool required, 
            FieldType type)
        {
            this.ServiceName = serviceName;
            this.Name = parameterName;
            this.InOut = inOut;
            this.Required = required;
            this.Type = type;
        }

        public ServiceProxyParameterDefinition()
            :this(string.Empty, string.Empty, ServiceCallParameterInOut.In, false, FieldType.Text)
        {

        }
    }
}
