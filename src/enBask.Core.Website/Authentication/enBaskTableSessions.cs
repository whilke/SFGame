using enBask.ASF.Tablestorage.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using enBask.ASF.Tablestorage.Shared;
using Microsoft.Extensions.Caching.Distributed;

namespace enBask.Core.Website.Authentication
{
    public class enBaskTableSessionCache : IDistributedCache
    {
        const string TABLE_NAME = "enBask.Sessions";
        const string APP_NAME = "enBask.Core.App";
        TableClient _tableClient = null;

        public enBaskTableSessionCache()
        {
            _tableClient = new TableClient(APP_NAME,TABLE_NAME, 3);
            _tableClient.CreateIfNotExsistAsync().GetAwaiter().GetResult();
        }

        private async Task<enBaskSessionItem> GetItem(string key)
        {
            var p = enBaskSessionItem.GetPartition(key);
            var result = await _tableClient.GetAsync<enBaskSessionItem>(p, key);
            if (result.Response == StorageResponseCodes.Success)
            {
                return result.Context;
            }

            return null;
        }

        public byte[] Get(string key)
        {
            return GetAsync(key).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetAsync(string key)
        {
            var p = enBaskSessionItem.GetPartition(key);
            var context = await GetItem(key);
            return context != null ? context.Data : new byte[0];
        }

        public void Refresh(string key)
        {
            RefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task RefreshAsync(string key)
        {
            var p = enBaskSessionItem.GetPartition(key);
            var context = await GetItem(key);
            if (context != null)
            {
                context.Expire.AddMinutes(5);
                await _tableClient.UpdateAsync<enBaskSessionItem>(context);
            }
        }

        public void Remove(string key)
        {
            RemoveAsync(key).GetAwaiter().GetResult();
        }

        public async Task RemoveAsync(string key)
        {
            var p = enBaskSessionItem.GetPartition(key);
            var context = await GetItem(key);
            if (context != null)
            {
                await _tableClient.DeleteAsync<enBaskSessionItem>(context);
            }
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            enBaskSessionItem item = new enBaskSessionItem(key);
            item.Data = value;

            var dtExpire = GetAbsoluteExpiration(DateTimeOffset.UtcNow, options);
            item.Expire = dtExpire.HasValue ? dtExpire.Value.UtcDateTime : DateTime.UtcNow.AddYears(1);
            item.ETag = "*";

            var r = await _tableClient.UpdateAsync<enBaskSessionItem>(item, true);

        }

        private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                    options.AbsoluteExpiration.Value,
                    "The absolute expiration value must be in the future.");
            }
            var absoluteExpiration = options.AbsoluteExpiration;
            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                absoluteExpiration = creationTime + options.AbsoluteExpirationRelativeToNow;
            }
            if (options.SlidingExpiration.HasValue)
            {
                absoluteExpiration = absoluteExpiration + options.SlidingExpiration.Value;
            }

            return absoluteExpiration;
        }
    }
}
