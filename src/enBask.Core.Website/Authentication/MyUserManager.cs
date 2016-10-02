using enBask.ASF.Tablestorage.Client;
using enBask.Core.Shared.Clients;
using enBask.Core.Website.Models;
using Microsoft.AspNetCore.Http;
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
            var user = new UserEntity();
            user.CreateUser(username, password);

            user.SetAsUserNameRecord();
            await _tableClient.AddAsync<UserEntity>(user);

            user.SetAsUserIdRecord();
            await _tableClient.AddAsync<UserEntity>(user);

            GameManagerClient client = new GameManagerClient();
            var session = await client.Create("test");

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

        public async Task DeleteByName(string username)
        {
            var result = await _tableClient.GetAsync<UserEntity>(username, username);
            if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
            {
                await _tableClient.DeleteAsync<UserEntity>(result.Context);

                result = await _tableClient.GetAsync<UserEntity>(result.Context.UserId, result.Context.UserId);
                if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
                {
                    await _tableClient.DeleteAsync<UserEntity>(result.Context);
                }
            }
        }

        public async Task DeleteById(string userId)
        {
            var result = await _tableClient.GetAsync<UserEntity>(userId, userId);
            if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
            {
                await _tableClient.DeleteAsync<UserEntity>(result.Context);

                result = await _tableClient.GetAsync<UserEntity>(result.Context.Username, result.Context.Username);
                if (result.Response == ASF.Tablestorage.Shared.StorageResponseCodes.Success)
                {
                    await _tableClient.DeleteAsync<UserEntity>(result.Context);
                }
            }
        }

        #region helpers
        public bool IsSessionValid(IHttpContextAccessor HttpContextAccessor)
        {
            var context = HttpContextAccessor.HttpContext;
            return context.User.Identity.IsAuthenticated;
        }

        public string GetSessionUserName(IHttpContextAccessor HttpContextAccessor)
        {
            var context = HttpContextAccessor.HttpContext;
            var valid = context.User.Identity.IsAuthenticated;
            if (!valid) return string.Empty;

            return context.User.Identity.Name;
        }

        #endregion
    }
}
