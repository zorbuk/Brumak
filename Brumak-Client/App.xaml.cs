using Brumak_ORM;
using Brumak_Shared.Metrics;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Brumak_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly bool ShowLogs = bool.Parse(Services.Configuration.GetConnectionString("ShowLogs")
        ?? throw Exceptions.New("'ShowLogs' is not correctly defined on ConnectionStrings."));

        public static readonly bool SaveLogs = bool.Parse(Services.Configuration.GetConnectionString("SaveLogs")
            ?? throw Exceptions.New("'SaveLogs' is not correctly defined on ConnectionStrings."));
    }
}
