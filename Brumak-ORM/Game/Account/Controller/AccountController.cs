using Brumak_ORM.Database;
using Brumak_ORM.Game.Generic;
using Brumak_Shared.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brumak_ORM.Game.Account.Controller
{
    public class AccountController(AuthDbContext context, IServiceProvider serviceProvider)
        : GenericController<Brumak_Shared.Account.Model.Account, AuthDbContext>(context, serviceProvider), IGenericController
    {
        #region "Generic Controller Functions"
        public override bool Create(Brumak_Shared.Account.Model.Account account)
        {
            if (string.IsNullOrWhiteSpace(account.Username))
                return false;

            account.PasswordHash = PasswordHasher.Hash(account.PasswordHash);

            return base.Create(account);
        }

        public override Brumak_Shared.Account.Model.Account? GetById(int id)
        {
            return base.GetById(id);
        }

        public override bool Update(Brumak_Shared.Account.Model.Account entity)
        {
            return base.Update(entity);
        }

        public override bool Delete(int id)
        {
            return base.Delete(id);
        }

        public override void Save()
        {
            base.Save();
        }
        #endregion

        #region "Custom Controller Functions"
        public Brumak_Shared.Account.Model.Account? GetByUsername(string username)
        {
            return base.GetAll().FirstOrDefault(x => x.Username.Equals(username));
        }

        public Brumak_Shared.Account.Model.Account? GetByNickname(string nickname)
        {
            return base.GetAll().FirstOrDefault(x => x.Nickname.Equals(nickname));
        }

        public bool ValidatePassword(string username, string password)
        {
            var account = GetByUsername(username);
            if (account is null) return false;
            return PasswordHasher.Verify(password, account.PasswordHash);
        }

        public bool UpdateLastIp(int id, string ip)
        {
            var account = base.GetById(id);
            if (account is null) return false;

            account.LastIp = ip;
            return base.Update(account);
        }
        #endregion
    }
}
