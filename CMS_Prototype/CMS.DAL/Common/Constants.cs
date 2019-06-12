using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using System.Collections.Generic;

namespace CMS.DAL.Common
{
    public static class Constants
    {
        public static readonly string DATA_TABLE_PREFIX = "data_";

        public static readonly string DICT_TABLE_PREFIX = "dict_";

        public static readonly string HIST_TABLE_PREFIX = "hist_";

        public static readonly string LINK_TABLE_PREFIX = "link_";

        public static readonly string FIELD_ID = "Id";

        public static readonly string FIELD_CREATE_DATE = "CreateDate";

        public static readonly string FIELD_CREATED_BY = "CreatedBy";

        public static readonly string FIELD_CHANGE_DATE = "ChangeDate";

        public static readonly string FIELD_CHANGED_BY = "ChangedBy";

        public static readonly string FIELD_IS_DRAFT = "IsDraft";

        public static readonly string FIELD_IS_ARCHIVED = "IsArchived";

        public static readonly string FIELD_IS_DELETED = "IsDeleted";

        public static readonly int TABLE_PAGE_SIZE = 20;

        public static readonly string ROLE_GLOBAL_ADMINS = "Global Administrators";

        public static readonly string ADMIN_POSTFIX = "Administrators";

        public static readonly string DICT_LINK_TABLE_NAME = "DictionaryLink";

        public enum JoinCase
        {
            LEFT,
            INNER
        }

        public static string GetJoinCase(JoinCase joinCase)
        {
            switch (joinCase)
            {
                case JoinCase.LEFT: return "left";
                case JoinCase.INNER: return "inner";
                default: return "left";
            }
        }

        public static readonly Dictionary<FieldType, string> CMS_TO_SQL = new Dictionary<FieldType, string>()
            {
                { FieldType.DateTime, "datetime" },
                { FieldType.Reference, "int" },
                { FieldType.Dictionary, "int" },
                { FieldType.Integer, "int" },
                { FieldType.Decimal, "decimal" },
                { FieldType.Double, "float" },
                { FieldType.Flag, "bit" },
                { FieldType.Guid, "uniqueidentifier" },
                { FieldType.Text, "nvarchar" }
            };

        //new Field()
        //{
        //    Name = sf.GetName(),
        //                FieldType = sf.GetFieldType(),
        //                Length = sf.GetLength(),
        //                IsSystem = true
        //            })


        public static readonly List<Field> SYSTEM_FIELDS = new List<Field>()
        {
            new Field(){ Name = FIELD_ID, FieldType = FieldType.Integer, IsSystem = true },
            new Field(){ Name = FIELD_CREATE_DATE, FieldType = FieldType.DateTime, IsSystem = true },
            new Field(){ Name = FIELD_CREATED_BY, FieldType = FieldType.Text, Length = 255, IsSystem = true },
            new Field(){ Name = FIELD_CHANGE_DATE, FieldType = FieldType.DateTime, IsSystem = true },
            new Field(){ Name = FIELD_CHANGED_BY, FieldType = FieldType.Text, Length = 255, IsSystem = true },
            new Field(){ Name = FIELD_IS_DRAFT, FieldType = FieldType.Flag, IsSystem = true },
            new Field(){ Name = FIELD_IS_ARCHIVED, FieldType = FieldType.Flag, IsSystem = true },
            new Field(){ Name = FIELD_IS_DELETED, FieldType = FieldType.Flag, IsSystem = true }
        };

        //public static readonly List<ControlType> DICTIONARY_CONTROLS = new List<ControlType>()
        //{
        //    ControlType.MultiSelect, ControlType.Select
        //};

        //public static readonly List<ControlType> REF_CONTROLS = new List<ControlType>()
        //{
        //    ControlType.Link, ControlType.Table
        //};

        //public static readonly List<ControlType> DATA_CONTROLS = new List<ControlType>()
        //{
        //    ControlType.DatePicker,
        //    ControlType.Hidden,
        //    ControlType.Label,
        //    ControlType.Switch,
        //    ControlType.TextArea,
        //    ControlType.TextInput
        //};

        //public static readonly List<ControlType> FUNC_CONTROLS = new List<ControlType>()
        //{
        //    ControlType.Button
        //};
    }
}
