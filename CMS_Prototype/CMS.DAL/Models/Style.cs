using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.DAL.Models
{
    public class Style : Entity
    {
        [MaxLength(100)]
        public string BorderWidth { get; set; }

        [MaxLength(100)]
        public string BorderColor { get; set; }

        [MaxLength(100)]
        public string BackgroundColor { get; set; }

        [MaxLength(100)]
        public string TextColor { get; set; }

        [MaxLength(100)]
        public string TextWeight { get; set; }

        public virtual ICollection<View> Views { get; set; }

        public virtual ICollection<Control> Controls { get; set; }
    }
}
