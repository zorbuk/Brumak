using Brumak_Shared.Server.Enum;
using Brumak_Shared.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brumak_ORM.Game.Server.Controller
{
    public class ServerConfiguration : IEntityTypeConfiguration<Brumak_Shared.Server.Model.Server>
    {
        public void Configure(EntityTypeBuilder<Brumak_Shared.Server.Model.Server> server)
        {
            server.HasKey(x => x.Id);
            server.ToTable("Servers");

            server.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("varchar(32)");

            server.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2048)
                .HasColumnType("text");

            server.Property(x => x.Community)
                .IsRequired()
                .HasMaxLength(16)
                .HasColumnType("varchar(16)");

            server.Property(x => x.Flag)
                .IsRequired()
                .HasMaxLength(4)
                .HasColumnType("varchar(4)");

            server.Property(x => x.SplashArt)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("varchar(256)");

            server.Property(x => x.Status)
                .IsRequired()
                .HasColumnType("int");

            server.Property(x => x.Population)
                .IsRequired()
                .HasMaxLength(16)
                .HasColumnType("varchar(16)");

            server.Property(x => x.IsMonoaccount)
                .IsRequired()
                .HasColumnType("tinyint(1)");

            server.Property(x => x.Ping)
                .IsRequired()
                .HasMaxLength(16)
                .HasColumnType("varchar(16)");

            server.Property(x => x.Ip)
                .IsRequired()
                .HasMaxLength(45)
                .HasColumnType("varchar(45)");

            server.Property(x => x.Port)
                .IsRequired()
                .HasColumnType("int");

            server.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            server.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            server.HasIndex(x => x.Name)
                .IsUnique();

            server.HasIndex(x => x.Port)
                .IsUnique();

            server.HasData([
                    new Brumak_Shared.Server.Model.Server() {
                        Id = 1,
                        Name = "Aether",
                        Description = "\"Antes de que este universo tuviera nombre... yo ya había abandonado el anterior.\"\r\n\nAether no nació en ningún mundo. Surgió entre ellos.\r\n\nUna entidad de origen desconocido, tejida de materia dimensional y voluntad pura, que atraviesa el tejido de la realidad en busca de algo que pocos comprenden y ninguno puede detenerle de alcanzar: el poder absoluto.\r\n\nNo conquista mundos. Los consume. No sigue caminos. Los crea.\r\n\nBienvenido al plano de Aether. Aquí las reglas de tu universo no aplican.",
                        Community = "INT",
                        Flag = "🌏︎",
                        SplashArt = "/Assets/Servers/Aether.jpg",
                        Status = ServerStatus.Offline,
                        Population = "Low",
                        IsMonoaccount = false,
                        Ping = "— ms",
                        Ip = "127.0.0.1",
                        Port = 890,
                    },
                    new Brumak_Shared.Server.Model.Server() {
                        Id = 2,
                        Name = "Kronos",
                        Description = "\"Los mortales temen al tiempo. Yo lo recuerdo todo.\"\r\n\nKronos existe desde antes del primer segundo. Observó el nacimiento de las estrellas, la caída de los imperios y el silencio que precede a cada nueva era.\r\n\nNo busca dominar. Busca comprender. Cada evento, cada decisión, cada instante que ha existido o existirá... es una página en el libro que aún no termina de escribir.\r\n\nEste servidor es su dominio. Un lugar donde el tiempo no avanza — se estudia.\r\n\nEntra con la mente abierta. Kronos tiene mucho que enseñarte.",
                        Community = "ESP",
                        Flag = "🇪🇸",
                        SplashArt = "/Assets/Servers/Kronos.jpg",
                        Status = ServerStatus.Offline,
                        Population = "Low",
                        IsMonoaccount = false,
                        Ping = "— ms",
                        Ip = "127.0.0.1",
                        Port = 891,
                    },
                    new Brumak_Shared.Server.Model.Server() {
                        Id = 3,
                        Name = "Nexus",
                        Description = "\"No verás mi rostro. Solo verás a dónde puedes ir.\"\r\n\nNadie sabe cómo luce Nexus. Nadie necesita saberlo.\r\n\nEs la presencia que sientes cuando cruzas un umbral sin saber qué hay al otro lado. El hilo invisible que conecta reinos que jamás deberían haberse tocado. Cada portal que se abre en cualquier rincón de la existencia... lleva su firma.\r\n\nNo gobierna mundos. Los une. No habla. Conecta.\r\n\nEste servidor es uno de sus nodos. Un punto donde caminos de distintos universos convergen en un solo lugar. Lo que encuentres aquí depende de a cuántas puertas te atrevas a llamar.\r\n\nEl dios sin rostro ya te estaba esperando.",
                        Community = "FR",
                        Flag = "🇫🇷",
                        SplashArt = "/Assets/Servers/Nexus.jpg",
                        Status = ServerStatus.Offline,
                        Population = "Low",
                        IsMonoaccount = false,
                        Ping = "— ms",
                        Ip = "127.0.0.1",
                        Port = 892,
                    },
                    new Brumak_Shared.Server.Model.Server() {
                        Id = 4,
                        Name = "Vortex",
                        Description = "\"La debilidad no es un estado. Es una elección.\"\r\n\nVortex no nació dragón. Se convirtió en uno a base de consumir todo lo que se interponía en su camino.\r\n\nFuego antiguo. Escamas forjadas en el núcleo de mundos extintos. Una voluntad que no dobla, no negocia y no olvida una afrenta. Para Vortex, la existencia tiene una sola ley que vale la pena respetar: el poder es la única verdad.\r\n\nNo admira a los fuertes. Los desafía. No tolera a los débiles. Los forja... o los devora.\r\n\nEste servidor es su guarida. Aquí el fuego no quema por destruir — quema para demostrar quién queda en pie.\r\n\n¿Tienes lo que hace falta para estar aquí?",
                        Community = "BR",
                        Flag = "🇧🇷",
                        SplashArt = "/Assets/Servers/Vortex.jpg",
                        Status = ServerStatus.Offline,
                        Population = "Low",
                        IsMonoaccount = false,
                        Ping = "— ms",
                        Ip = "127.0.0.1",
                        Port = 893,
                    },
                ]);
        }
    }
}
