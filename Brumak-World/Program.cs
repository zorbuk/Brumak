using Brumak_ORM;
using Brumak_ORM.Database;
using Brumak_ORM.Game.Character.Controller;
using Brumak_ORM.Game.Generic.Cache;
using Brumak_Shared.Metrics;
using Brumak_Shared.Server.Enum;
using Brumak_World.Network;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static readonly bool ShowLogs = bool.Parse(Services.Configuration.GetConnectionString("ShowLogs")
        ?? throw Exceptions.New("'ShowLogs' is not correctly defined on ConnectionStrings."));

    public static readonly bool SaveLogs = bool.Parse(Services.Configuration.GetConnectionString("SaveLogs")
        ?? throw Exceptions.New("'SaveLogs' is not correctly defined on ConnectionStrings."));

    private static readonly Logger _logger = new("World", typeof(Program), ShowLogs, SaveLogs);

    public static void Main()
    {
        Console.WriteLine(@"$$$$$$$\                                              $$\       
$$  __$$\                                             $$ |      
$$ |  $$ | $$$$$$\  $$\   $$\ $$$$$$\$$$$\   $$$$$$\  $$ |  $$\ 
$$$$$$$\ |$$  __$$\ $$ |  $$ |$$  _$$  _$$\  \____$$\ $$ | $$  |
$$  __$$\ $$ |  \__|$$ |  $$ |$$ / $$ / $$ | $$$$$$$ |$$$$$$  / 
$$ |  $$ |$$ |      $$ |  $$ |$$ | $$ | $$ |$$  __$$ |$$  _$$<  
$$$$$$$  |$$ |      \$$$$$$  |$$ | $$ | $$ |\$$$$$$$ |$$ | \$$\ 
\_______/ \__|       \______/ \__| \__| \__| \_______|\__|  \__|
                                                                
                                                                
                                                    WorldServer
                                                    github.com/zorbuk
                                                                     ");

        _logger.Log("Building DbContext...");
        Services.BuildServiceProvider(typeof(WorldDbContext));

        _logger.Log("Registering all Controllers (...)");
        Controllers.RegisterAllControllers(typeof(WorldDbContext));

        _logger.Log("Starting CacheFlushService (...)");
        var flushService = new CacheFlushService(interval: TimeSpan.FromSeconds(30));
        flushService.Register(Controllers.Get<CharacterController>());
        flushService.Register(Controllers.Get<CharacterExperiencesController>());
        flushService.Register(Controllers.Get<CharacterWorldPositionController>());
        flushService.Start();

        _logger.Log("Starting WorldServer (...)");
        WorldServer.Start();

        _logger.Log("Connecting to AuthServer (...)");
        AuthConnection.ConnectAsync().Wait();

        AuthConnection.SendStatus(ServerStatus.Online);

        _logger.Log("WorldServer is now Ready (...)");

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            AuthConnection.Disconnect();
            Environment.Exit(0);
        };

        AppDomain.CurrentDomain.ProcessExit += (_, _)
            => AuthConnection.Disconnect();

        Console.ReadLine();
    }
}