using Brumak_ORM;
using Brumak_Shared.Metrics;

public class Program
{
    private static readonly Logger _logger = new("Auth", typeof(Program));

    public static void Main()
    {
        _logger.Log("Building DbContext...");
        Services.BuildServiceProvider(typeof(Brumak_ORM.Database.AuthDbContext));
        _logger.Log("Starting Auth server...");
    }
}