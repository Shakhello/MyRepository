using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Services
{
    public abstract class DbCacheService
    {
        protected static readonly MemoryCache Cache = MemoryCache.Default;

        protected T GetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction)
        {
            var cachedObject = (T)Cache[cacheItemName];

            if (cachedObject == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes)
                };

                cachedObject = objectSettingFunction();
                Cache.Set(cacheItemName, cachedObject, policy);
            }

            return cachedObject;
        }
    }
}
