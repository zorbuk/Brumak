using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Account.Model
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public DateTime PremiumExpirationDate { get; set; }
        public string? LastIp { get; set; }
        public DateTime CreatedAt { get; set; }

        public AccountDto() { }

        public AccountDto(Account account)
        {
            Id = account.Id;
            Username = account.Username;
            Nickname = account.Nickname;
            PremiumExpirationDate = account.PremiumExpirationDate;
            LastIp = account.LastIp;
            CreatedAt = account.CreatedAt;
        }
    }
}
