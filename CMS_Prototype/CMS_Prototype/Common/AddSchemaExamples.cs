using CMS.Incoming;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unity.Common
{
    public class AddSchemaExamples : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            if (type == typeof(Document))
            {
                schema.example = new Document
                {
                    TemplateId = 123,
                    Values = new Dictionary<string, object>()
                    {
                        {"Name", "Vasya" },
                        {"Age", 30 }
                    }
                };
            }
        }
    }
}