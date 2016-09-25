using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using enBask.ASF.Tablestorage.Shared;

namespace enBask.Core.Website.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class UserEntity : BaseEntity
    {
        public string UserId
        { get; set; }

        public string Username
        {
            get;set;
        }

        public string HashCode
        { get; set; }

        public UserEntity() { }

        public void SetAsUserNameRecord()
        {
            Partition = Username;
            Id = Username;
            ETag = "";
        }

        public void SetAsUserIdRecord()
        {
            Partition = UserId;
            Id = UserId;
            ETag = "";
        }

        public void CreateUser(string username, string hash)
        {
            UserId = Guid.NewGuid().ToString().Replace("-", "");
            Username = username;
            HashCode = hash;
        }

        public ApplicationUser ValidateUser(string password)
        {
            string local_hash = password; //todo: hash password

            if (local_hash == HashCode)
            {
                return new ApplicationUser
                {
                    Id  = this.UserId,
                    Username = this.Username
                };
            }

            return null;
        }

        public ApplicationUser Get()
        {
            return new ApplicationUser
            {
                Id = this.UserId,
                Username = this.Username
            };
        }
    }
}
