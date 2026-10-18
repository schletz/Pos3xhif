using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstDemo.Application.Model
{
    [Table("User")]
    public class User
    {
        public User(string username, string salt, string passwordHash, Store store)
        {
            Username = username;
            Salt = salt;
            PasswordHash = passwordHash;
            Store = store;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        [MaxLength(255)]
        public string Username { get; set; }
        [MaxLength(44)]  // 256 bit Hash as base64
        public string Salt { get; set; }
        [MaxLength(88)]  // 512 bit SHA512 Hash as base64
        public string PasswordHash { get; set; }
        public Store Store { get; set; }
    }
}
