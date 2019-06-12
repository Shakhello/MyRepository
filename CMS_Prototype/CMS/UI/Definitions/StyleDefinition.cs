using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;

namespace CMS.UI
{
    public class StyleDefinition : Definition
    {

        public string BorderWidth { get; set; }

        public string BorderColor { get; set; }

        public string BackgroundColor { get; set; }

        public string TextColor { get; set; }

        public string TextWeight { get; set; }

        internal string[] GetPropertiesAsArray()
        {
            var props = new List<string>
            {
                BorderWidth,
                BorderColor,
                BackgroundColor,
                TextColor,
                TextWeight
            };

            return props.Where(p => !string.IsNullOrEmpty(p)).ToArray();
        }
    }
}
