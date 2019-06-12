using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class DictionaryLink
    {
        public int Id { get; set; }

        public int FieldId { get; set; }

        public int DocId { get; set; }

        public int? DictionaryKeyInt { get; set; }

        public string DictionaryKeyString { get; set; }
    }
}
