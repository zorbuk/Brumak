using Brumak_Shared.Server.Enum;

namespace Brumak_Shared.Server.Model
{
    public class ServerInfo
    {
        public int Id { get; set;  } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Community { get; set; } = "";
        public string Flag { get; set; } = "";
        public string SplashArt { get; set; } = "";
        public ServerStatus Status { get; set; } = ServerStatus.Offline;
        public string Population { get; set; } = "";
        public bool IsMonoaccount { get; set; }
        public string Ping { get; set; } = "— ms";
        public string Ip { get; set; } = "";
        public int Port { get; set; }
    }

    public class Server
    {
        public Server() { }

        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Community { get; set; }
        public required string Flag { get; set; }
        public required string SplashArt { get; set; }
        public required ServerStatus Status { get; set; } = ServerStatus.Offline;
        public required string Population { get; set; }
        public required bool IsMonoaccount { get; set; } = false;
        public required string Ping { get; set; }

        public required string Ip { get; set; }
        public required int Port { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
