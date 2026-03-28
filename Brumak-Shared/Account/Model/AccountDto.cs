using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Account.Model
{
    public class AccountDto(Account account)
    {
        public int Id { get; set; } = account.Id;

        public string Username { get; private set; } = account.Username;
        public string Nickname { get; private set; } = account.Nickname;

        public int PremiumExpirationDate { get; private set; } = account.PremiumExpirationDate;

        public string LastIp { get; private set; } = account.LastIp;

        public DateTime CreatedAt { get; private set; } = account.CreatedAt;
    }
}
