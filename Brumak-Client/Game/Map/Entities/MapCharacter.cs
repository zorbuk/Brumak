using Brumak_Shared.Character.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Client.Game.Map.Characters
{
    public class MapCharacter : CharacterDto
    {
        MapCharacter() : base() { }

        public int DungeonInstanceId { get; set; } = -1;
        public bool IsSpectator { get; set; }
        public bool IsFighting { get; set; }

        public CharacterDto GetCharacterDto => new()
        {
            Id = this.Id,
            Name = this.Name,
            Skin = this.Skin,
            Class = this.Class,
            SkinHexColors = this.SkinHexColors,
            CreatedAt = this.CreatedAt
        };
    }
}
