using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Account.Model
{
    public class Account
    {
        public int Id { get; set; }

        public required string Username { get; set; }
        public required string Nickname { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public required int PremiumExpirationDate { get; set; }

        public required string RegisteredIp { get; set; }
        public required string LastIp { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
