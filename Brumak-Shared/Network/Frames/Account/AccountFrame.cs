using Brumak_Shared.Account.Model;
using System.Text.Json.Serialization;

namespace Brumak_Shared.Network.Frames.Account
{
    public enum AccountAction
    {
        Login = 0,
        Register = 1,
        Logout = 2,
        Error = 3,
        LoginSuccess = 4,
        RegisterSuccess = 5,
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
    [JsonDerivedType(typeof(LoginFrame), (int)AccountAction.Login)]
    [JsonDerivedType(typeof(RegisterFrame), (int)AccountAction.Register)]
    [JsonDerivedType(typeof(LogoutFrame), (int)AccountAction.Logout)]
    [JsonDerivedType(typeof(AccountErrorFrame), (int)AccountAction.Error)]
    [JsonDerivedType(typeof(LoginSuccessFrame), (int)AccountAction.LoginSuccess)]
    [JsonDerivedType(typeof(RegisterSuccessFrame), (int)AccountAction.RegisterSuccess)]

    public abstract class AccountFrame : BaseFrame
    {
        public override string Type => FrameType.Account;
        public abstract AccountAction Action { get; }
    }

    public class LoginFrame : AccountFrame
    {
        public override AccountAction Action => AccountAction.Login;

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Ip { get; set; } = null!;
    }

    public class RegisterFrame : AccountFrame
    {
        public override AccountAction Action => AccountAction.Register;

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Ip {  get; set; } = null!;
    }

    public class LogoutFrame : AccountFrame
    {
        public override AccountAction Action => AccountAction.Logout;

        public string Username { get; set; } = null!;
    }

    public class AccountErrorFrame : AccountFrame
    {
        public override AccountAction Action => AccountAction.Error;

        public string Message { get; set; } = null!;
    }

    public class LoginSuccessFrame : AccountFrame
    {
        public override AccountAction Action => AccountAction.LoginSuccess;

        public AccountDto Account { get; set; } = null!;
    }

    public class RegisterSuccessFrame : AccountFrame
    {
        public override AccountAction Action => AccountAction.RegisterSuccess;

        public string Message { get; set; } = null!;
    }
}