using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.DAL.Models
{
    public class View : Entity
    {
        public int? ParentViewId { get; set; }

        public int? TemplateId { get; set; }

        public int? SectionId { get; set; }

        public int? LinkedFieldId { get; set; }

        public int? StyleId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public int OrderIndex { get; set; }

        public ViewType ViewType { get; set; }

        public int GridWidth { get; set; }

        public virtual Section Section { get; set; }

        public virtual Style Style { get; set; }

        public virtual View ParentView { get; set; }

        public virtual Template Template { get; set; }

        public virtual Field LinkedField { get; set; }

        public virtual ICollection<View> ChildViews { get; set; }

        public virtual ICollection<Control> Controls { get; set; }

        [InverseProperty("View")]
        public virtual ICollection<Permission> Permissions { get; set; }

        [InverseProperty("View")]
        public virtual ICollection<Filter> Filters { get; set; }

        [NotMapped]
        public View RootView { get; set; }
    }
}
