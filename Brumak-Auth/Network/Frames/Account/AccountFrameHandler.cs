using Brumak_ORM;
using Brumak_Shared.Account.Model;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Account;

namespace Brumak_Auth.Network.Frames.Account
{
    public class AccountFrameHandler : IFrameHandler<AccountFrame>
    {
        public async void Handle(object context, AccountFrame frame)
        {
            var session = (AuthClientSession)context;
            var accountController = Controllers.GetAccountController 
                ?? throw Exceptions.New("Fatal error Controllers doesn't have AccountController.");

            switch (frame)
            {
                case LoginFrame login:
                    if (accountController.ValidatePassword(login.Username, login.Password))
                    {
                        var account = accountController.GetByUsername(login.Username) ?? throw Exceptions.New("Fatal error Account should exist.");
                        accountController.UpdateLastIp(account.Id, login.Ip);
                        session.Send(new LoginSuccessFrame() { Account = new AccountDto(account) });
                    }
                    else
                        session.Send(new AccountErrorFrame() { Message = "No se ha podido conectar."});
                    break;

                case RegisterFrame register:
                    if (accountController.Create(new Brumak_Shared.Account.Model.Account()
                    {
                        Email = register.Email,
                        Nickname = register.Nickname,
                        PasswordHash = register.Password,
                        RegisteredIp = register.Ip,
                        Username = register.Username,
                        PremiumExpirationDate = DateTime.Now,
                        LastIp = register.Ip
                    }))
                        session.Send(new RegisterSuccessFrame() { Message = "¡Registrado correctamente!" });
                    else
                        session.Send(new AccountErrorFrame() { Message = "No se ha podido registrar." });
                    break;

                case LogoutFrame logout:

                    break;
            }
        }
    }
}
