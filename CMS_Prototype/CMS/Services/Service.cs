using AutoMapper;
using CMS.DAL.Services;
using CMS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Services
{
    public abstract class Service
    {
        protected static readonly DbDocumentService DbDocService;
        protected static readonly DbEditorService DbEditorService;
        protected static readonly DbDictionaryCache DbDictionaryCache;

        protected readonly UserDefinition CurrentUser;

        static Service()
        {
            DbDocService = new DbDocumentService();
            DbEditorService = new DbEditorService();
            DbDictionaryCache = new DbDictionaryCache();
        }

        public Service(IPrincipal currentPrincipal)
        {
            var userName = currentPrincipal.Identity.Name;
            var nameParts = userName.Split('\\');
            var dbUser = DbEditorService.GetUserByLogin(nameParts.Last());

            CurrentUser = Mapper.Map<UI.UserDefinition>(dbUser);
        }
    }
}
