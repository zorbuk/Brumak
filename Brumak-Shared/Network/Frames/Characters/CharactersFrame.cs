using Brumak_Shared.Character.Model;
using System.Text.Json.Serialization;

namespace Brumak_Shared.Network.Frames.Characters
{
    public enum CharacterAction
    {
        GetCharacters = 0,
        CreateCharacter = 1,
        DeleteCharacter = 2,
        Error = 3,
        GetCharactersSuccess = 4,
        CreateCharacterSuccess = 5,
        DeleteCharacterSuccess = 6,
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
    [JsonDerivedType(typeof(GetCharactersFrame), (int)CharacterAction.GetCharacters)]
    [JsonDerivedType(typeof(CreateCharacterFrame), (int)CharacterAction.CreateCharacter)]
    [JsonDerivedType(typeof(DeleteCharacterFrame), (int)CharacterAction.DeleteCharacter)]
    [JsonDerivedType(typeof(CharacterErrorFrame), (int)CharacterAction.Error)]
    [JsonDerivedType(typeof(GetCharactersSuccessFrame), (int)CharacterAction.GetCharactersSuccess)]
    [JsonDerivedType(typeof(CreateCharacterSuccessFrame), (int)CharacterAction.CreateCharacterSuccess)]
    [JsonDerivedType(typeof(DeleteCharacterSuccessFrame), (int)CharacterAction.DeleteCharacterSuccess)]

    public abstract class CharacterFrame : BaseFrame
    {
        public override string Type => FrameType.Characters;
        public abstract CharacterAction Action { get; }
    }

    public class GetCharactersFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.GetCharacters;

        public int AccountId { get; set; }
        public int ServerId { get; set; }
    }

    public class CreateCharacterFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.CreateCharacter;

        public int AccountId { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; } = null!;
        public SkinEnum Skin { get; set; }
        public SkinEnum Class { get; set; }
        public List<string> SkinHexColors { get; set; } = [];
    }

    public class DeleteCharacterFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.DeleteCharacter;

        public int CharacterId { get; set; }
    }

    public class CharacterErrorFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.Error;

        public string Message { get; set; } = null!;
    }

    public class GetCharactersSuccessFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.GetCharactersSuccess;

        public List<CharacterDto> Characters { get; set; } = [];
    }

    public class CreateCharacterSuccessFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.CreateCharacterSuccess;

        public CharacterDto Character { get; set; } = null!;
    }

    public class DeleteCharacterSuccessFrame : CharacterFrame
    {
        public override CharacterAction Action => CharacterAction.DeleteCharacterSuccess;

        public int CharacterId { get; set; }
    }
}