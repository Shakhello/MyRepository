namespace CMS.Incoming
{
    public class FileInput
    {
        public int DocId { get; set; }

        public int FieldId { get; set; }

        public string Name { get; set; }

        public byte[] ContentData { get; set; }

        public string ContentType { get; set; }

        public string Comment { get; set; }
    }
}