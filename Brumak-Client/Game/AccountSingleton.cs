using Brumak_Client.Network;
using Brumak_Shared.Account.Model;
using Brumak_Shared.Network.Frames.Account;
using System.ComponentModel;

public class AccountSingleton : INotifyPropertyChanged
{
    private static AccountSingleton _instance = new();
    public static AccountSingleton Instance => _instance;

    private AccountDto _account;

    public string Nickname => _account?.Nickname;
    public string PremiumStatus
    {
        get
        {
            if (_account == null)
                return "";

            var now = DateTime.UtcNow;
            var expiration = _account.PremiumExpirationDate;

            if (expiration > now)
            {
                var remaining = expiration - now;

                if (remaining.TotalDays >= 1)
                    return $"Premium activo ({remaining.Days} día{(remaining.Days > 1 ? "s" : "")})";

                if (remaining.TotalHours >= 1)
                    return $"Premium activo ({(int)remaining.TotalHours} horas)";

                return "Premium activo (menos de 1 hora)";
            }

            var expiredDays = (now - expiration).Days;

            if (expiredDays <= 0)
                return "Premium finalizado recientemente";

            return $"Premium finalizado hace {expiredDays} día{(expiredDays > 1 ? "s" : "")}";
        }
    }

    public void SetAccount(AccountDto account)
    {
        _account = account;
        OnPropertyChanged(nameof(Nickname));
        OnPropertyChanged(nameof(PremiumStatus));
    }

    public void Clear()
    {
        NetworkManager.AuthClientManager.Send(new LogoutFrame { Username = _account.Username });
        _account = null!;
        OnPropertyChanged(nameof(Nickname));
        OnPropertyChanged(nameof(PremiumStatus));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}