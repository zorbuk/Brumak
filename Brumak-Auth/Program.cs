using Brumak_Auth.Network;
using Brumak_ORM;
using Brumak_Shared.Metrics;

public class Program
{
    private static readonly Logger _logger = new("Auth", typeof(Program));

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
                                                                
                                                                
                                                    AuthServer
                                                    github.com/zorbuk
                                                                     ");
        _logger.Log("Building DbContext (...)");
        Services.BuildServiceProvider(typeof(Brumak_ORM.Database.AuthDbContext));
        _logger.Log("Registering all Controllers (...)");
        Controllers.RegisterAllControllers();
        _logger.Log("Starting AuthServer (...)");
        AuthServer.Start();
        Console.ReadLine();
    }
}