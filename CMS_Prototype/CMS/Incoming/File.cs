namespace CMS.Incoming
{
    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] ContentData { get; set; }

        public string ContentType { get; set; }

        public string Comment { get; set; }
    }
}