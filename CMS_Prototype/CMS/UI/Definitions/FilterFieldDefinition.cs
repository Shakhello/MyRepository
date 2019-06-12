using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class FilterFieldDefinition
    {
        public int FilterId { get; set; }

        public int FieldId { get; set; }

        public int Depth { get; set; }

        public int ChainId { get; set; }
    }
}
