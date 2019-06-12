using AutoMapper;
using CMS.DAL.Services;
using CMS.UI;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Behaviours
{
    internal abstract class Behaviour
    {
        protected static readonly DbEditorService DbEditorService = new DbEditorService();

        protected static readonly DbDocumentService DbDocumentService = new DbDocumentService();

        protected static readonly DbDictionaryCache DbDictionaryCache = new DbDictionaryCache();

        protected readonly UserDefinition CurrentUser;

        public Behaviour(UserDefinition currentUser)
        {
            CurrentUser = currentUser;
        }

        protected static TEnum GetEnumValue<TEnum>(string value) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }


        protected List<View> GetFlatListOfViews(View parentView)
        {
            var views = new List<View>() { parentView };

            foreach (var childView in parentView.ChildViews)
                views.AddRange(GetFlatListOfViews(childView));

            return views;
        }
    }
}
