namespace CMS.Incoming
{
    public class DocumentWithLink
    {
        /// <summary>
        /// ID поля, которое ссылается на дочерний шаблон
        /// </summary>
        public int FieldId { get; set; }
        /// <summary>
        /// ID родительского документа
        /// </summary>
        public int ParentDocId { get; set; }
        /// <summary>
        /// Тело дочернего документа
        /// </summary>
        public Document Document { get; set; }
    }
}