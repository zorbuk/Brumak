using Brumak_Shared.Drawings.Skin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Brumak_Shared.Character.Model
{
    public enum GenderEnum
    {
        Undefined,
        Male,
        Female,
    }

    // todo: mover a otro lado ...
    public enum SkinEnum
    {
        // Classes
        Class_Warrior,
        Class_Archer,
        Class_Summoner,
        Class_Enchanter,
        Class_Mage,
        Class_Priest,

        // Monsters, etc...
        Training_Scarecrow,
    }

    public class ClassInfo
    {
        public ClassInfo() { }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Icon { get; set; }
        public SkinEnum Skin { get; set; }
        public SkinEnum Class { get; set; }
    }

    public class Character : IAbstractEntity
    {
        public Character() { }

        public int Id { get; set; }
        public required int ServerId { get; set; }
        public required int AccountId { get; set; }

        public required string Name { get; set; }
        public required SkinEnum Skin { get; set; }
        public required SkinEnum Class {  get; set; }
        public required List<string> SkinHexColors { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CharacterDto
    {
        public CharacterDto() { }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public SkinEnum Skin { get; set; }
        public SkinEnum Class { get; set; }
        public List<string> SkinHexColors { get; set; } = [];
        public CharacterExperiences CharacterExperiences { get; set; }
        public DateTime CreatedAt { get; set; }

        public CharacterDto(Character character, CharacterExperiences experience)
        {
            Id = character.Id;
            Name = character.Name;
            Skin = character.Skin;
            Class = character.Class;
            SkinHexColors = character.SkinHexColors;
            CreatedAt = character.CreatedAt;
            CharacterExperiences = experience;
        }
    }

    public class CharacterExperiences
    {
        public CharacterExperiences() { }

        public int CharacterId { get; set; }

        public long Experience { get; set; }
    }

    public class CharacterWorldPosition
    {
        public CharacterWorldPosition() { }

        public int CharacterId { get; set; }

        public required int MapId { get; set; }
        public required int PositionX { get; set; }
        public required int PositionY { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
