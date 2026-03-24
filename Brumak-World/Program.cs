using Brumak_ORM;
using Brumak_Shared.Metrics;

public class Program
{
    private static readonly Logger _logger = new("World", typeof(Program));

    public static void Main()
    {
        _logger.Log("Building DbContext...");
        Services.BuildServiceProvider(typeof(Brumak_ORM.Database.WorldDbContext));
        _logger.Log("Starting World server...");
    }
}