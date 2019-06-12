using CMS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace CMS.DAL.Services
{
    public class DbDictionaryCache : DbCacheService
    {
        public static void ClearForFields(IEnumerable<int> fieldIds)
        {
            foreach (var fieldId in fieldIds)
            {
                var key = $"DictionaryValuesForField_{fieldId}";
                Cache.Remove(key);
            }
        }

        public Dictionary<object, string> GetDictionaryRecords(Models.Dictionary dict)
        {
            var dictName = $"dict_{dict.Name}_records";

            return GetObjectFromCache(dictName, 600, () => new DbDictionaryService().GetDictionaryRecords(dict));
        }

        public void AddDictionaryRecord<T>(Models.Dictionary dict, T key, string value)
        {
            var dictName = $"dict_{dict.Name}_records";
            Cache.Remove(dictName);

            new DbDictionaryService().AddDictionaryRecord(dict, key, value);
        }

        public void UpdateDictionaryRecord<T>(Models.Dictionary dict, T key, string value)
        {
            var dictName = $"dict_{dict.Name}_records";
            Cache.Remove(dictName);

            new DbDictionaryService().UpdateDictionaryRecord(dict, key, value);
        }

        public void DeleteDictionaryRecord<T>(Models.Dictionary dict, T key)
        {
            var dictName = $"dict_{dict.Name}_records";
            Cache.Remove(dictName);

            new DbDictionaryService().DeleteDictionaryRecord(dict, key);
        }

        public List<object> GetValues(Models.Dictionary dict, int fieldId, int docId)
        {
            var key = $"DictionaryValuesForField_{fieldId}";

            var links = GetObjectFromCache(key, 10, () => new DbDictionaryService().GetValuesForField(dict, fieldId));

            if (dict.DictionaryType == DictionaryType.Int)
            {
                var ints = links.Where(l => l.DocId == docId).Select(dl => dl.DictionaryKeyInt).ToList();
                return ints.Select(i => (object)i).ToList();
            }
            else
            {
                var strings = links.Where(l => l.DocId == docId).Select(dl => dl.DictionaryKeyString).ToList();
                return strings.Select(i => (object)i).ToList();
            }
        }

        public void SetTicketDictionaryValues<T>(int fieldId, int docId, IEnumerable<T> values)
        {
            var key = $"DictionaryValuesForField_{fieldId}";
            Cache.Remove(key);

            new DbDictionaryService().SetTicketDictionaryValues(fieldId, docId, values);
        }

    }
}
