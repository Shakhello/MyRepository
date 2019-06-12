using System;

namespace CMS.DAL.Models
{
    public class File
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatedBy { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string Comment { get; set; }

        public byte[] ContentData { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeleteDate { get; set; }

        public string DeletedBy { get; set; }
    }
}
