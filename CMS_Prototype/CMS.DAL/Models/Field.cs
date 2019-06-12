using CMS.DAL.Common;
using CMS.DAL.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.DAL.Models
{
    public class Field : Entity //, IField
    {
        public int TemplateId { get; set; }

        public int? LinkedFieldId { get; set; }

        public int? DictionaryId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public int? Length { get; set; }

        public FieldType FieldType { get; set; }

        public bool IsSystem { get; set; }

        public virtual Field LinkedField { get; set; }

        public virtual Dictionary Dictionary { get; set; }

        [Required]
        public virtual Template Template { get; set; }

        public virtual ICollection<Parameter> Parameters { get; set; }

        public virtual ICollection<ControlField> FieldControls { get; set; }

        public virtual ICollection<FilterField> FieldFilters { get; set; }

        [InverseProperty("LinkedField")]
        public virtual ICollection<View> Views { get; set; }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Field))
                return false;

            return Id == ((Field)other).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();

        }

        //#region IField

        //public int GetId()
        //{
        //    return Id;
        //}

        //public string GetName()
        //{
        //    return Name;
        //}

        //public string GetSQLType()
        //{
        //    return Constants.CMS_TO_SQL[FieldType];
        //}

        //public FieldType GetFieldType()
        //{
        //    return FieldType;
        //}

        //public int? GetLength()
        //{
        //    return Length;
        //}

        //public DictionaryType? GetDictionaryType()
        //{
        //    return Dictionary?.DictionaryType;
        //}

        //#endregion



    }
}
