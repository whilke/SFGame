using enBask.ASF.Tablestorage.Client;
using enBask.Core.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace enBask.Core.Website.Authentication
{
    public class MyUserManager
    {
        const string TABLE_NAME = "enBask.Accounts";
        const string APP_NAME = "enBask.Core.App";
        TableClient _tableClient = null;

        public MyUserManager()
        {
            _tableClient = new TableClient(APP_NAME, TABLE_NAME, 3);
            _tableClient.CreateIfNotExsistAsync().GetAwaiter().GetResult();
        }


        public async Task<ApplicationUser> CreateUser(string username, string password)
        {
            var hash = password; //todo: hash password;
            var user = new UserEntity();
            user.CreateUser(username, hash);

            user.SetAsUserNameRecord();
            await _tableClient.AddAsync<UserEntity>(user);

            user.SetAsUserIdRecord();
            await _tableClient.AddAsync<UserEntity>(user);

            return user.ValidateUser(password);
        }

        public async Task<ApplicationUser> Login(string username, string password)
        {
            var result = await _tableClient.GetAsync<UserEntity>(username, username);
            if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
            {
                var user = result.Context;
                return user.ValidateUser(password);
            }

            return null;
        }

        public async Task<ApplicationUser> LookupByName(string username)
        {
            var result = await _tableClient.GetAsync<UserEntity>(username, username);
            if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
            {
                var user = result.Context;
                return user.Get();
            }
            return null;
        }

        public async Task<ApplicationUser> LookupById(string userId)
        {
            var result = await _tableClient.GetAsync<UserEntity>(userId, userId);
            if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
            {
                var user = result.Context;
                return user.Get();
            }
            return null;
        }
    }
}
