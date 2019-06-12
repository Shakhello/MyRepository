using AutoMapper;
using CMS.Behaviours;
using CMS.DAL.Services;
using CMS.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace CMS.Services
{
    public class TicketService : Service
    {
        public TicketService(IPrincipal currentPrincipal) : base(currentPrincipal) { }

        public List<Section> GetSections()
        {
            var defs = DbEditorService
                .GetSections(CurrentUser.Login)
                .Select(s => Mapper.Map<SectionDefinition>(s)).ToList();

            var sections = defs.Select(def => BehaviourSelector.SectionBehaviours[SectionType.Default](CurrentUser).Make(def)).ToList();

            return sections;
        }

        public EventResult ExecuteEvent(Event evt)
        {
            return BehaviourSelector.EventBehaviours[evt.EventType](CurrentUser).Execute(evt);
        }

        #region Dictionaries

        public List<DictionaryRecord> GetDictionaryRecords(int dictId)
        {
            var dictDef = DbEditorService.GetDictionary(dictId);

            var list = new List<DictionaryRecord>();

            var records = DbDictionaryCache.GetDictionaryRecords(dictDef);
            foreach (var rec in records.Select(r => r))
                list.Add(new DictionaryRecord() { DictionaryId = dictDef.Id, Key = rec.Key.ToString(), Description = rec.Value });

            return list;
        }

        public void AddDictionaryRecord(DictionaryRecord record)
        {
            var dictDef = DbEditorService.GetDictionary(record.DictionaryId);

            if (dictDef.DictionaryType == DAL.Models.DictionaryType.Int)
                DbDictionaryCache.AddDictionaryRecord(dictDef, Convert.ToInt32(record.Key), record.Description);
            else
                DbDictionaryCache.AddDictionaryRecord(dictDef, record.Key, record.Description);
        }

        public void UpdateDictionaryRecord(DictionaryRecord record)
        {
            var dictDef = DbEditorService.GetDictionary(record.DictionaryId);

            if (dictDef.DictionaryType == DAL.Models.DictionaryType.Int)
                DbDictionaryCache.UpdateDictionaryRecord(dictDef, Convert.ToInt32(record.Key), record.Description);
            else
                DbDictionaryCache.UpdateDictionaryRecord(dictDef, record.Key, record.Description);
        }

        public void DeleteDictionaryRecord(int dictId, string key)
        {
            var dictDef = DbEditorService.GetDictionary(dictId);

            if (dictDef.DictionaryType == DAL.Models.DictionaryType.Int)
                DbDictionaryCache.DeleteDictionaryRecord(dictDef, Convert.ToInt32(key));
            else
                DbDictionaryCache.DeleteDictionaryRecord(dictDef, key);
        }

        #endregion

    }
}
