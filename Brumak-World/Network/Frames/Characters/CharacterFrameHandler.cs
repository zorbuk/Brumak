using Brumak_ORM;
using Brumak_Shared.Character.Model;
using Brumak_Shared.Experience.Static;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Characters;

namespace Brumak_World.Network.Frames.Characters
{
    public class CharacterFrameHandler : IFrameHandler<CharacterFrame>
    {
        public async Task Handle(object context, CharacterFrame frame)
        {
            var session = (WorldClientSession)context;
            var characterController = Controllers.GetCharacterController
                ?? throw Exceptions.New("Fatal error Controllers doesn't have CharacterController.");

            switch (frame)
            {
                case GetCharactersFrame getCharacters:
                    var characters = characterController.GetCharacters(getCharacters.AccountId, getCharacters.ServerId);

                    session.Send(new GetCharactersSuccessFrame
                    {
                        Characters = [.. characters.Select(x => new CharacterDto(x, 
                        Controllers.GetCharacterExperiencesController?.GetByCharacter(x.Id) 
                        ?? throw Exceptions.New("Experience for character " + x.Name + " not found")))]
                    });
                    break;

                case CreateCharacterFrame createCharacter:
                    var existingCharacters = characterController.GetCharacters(createCharacter.AccountId, createCharacter.ServerId);

                    if (existingCharacters.Count >= 5)
                    {
                        session.Send(new CharacterErrorFrame { Message = "Has alcanzado el límite de 5 personajes por servidor." });
                        break;
                    }

                    if (characterController.Create(new Character
                    {
                        AccountId = createCharacter.AccountId,
                        ServerId = createCharacter.ServerId,
                        Name = createCharacter.Name,
                        Skin = createCharacter.Skin,
                        Class = createCharacter.Class,
                        SkinHexColors = createCharacter.SkinHexColors
                    }))
                    {
                        var created = characterController.GetCharacters(createCharacter.AccountId, createCharacter.ServerId)
                            .OrderByDescending(x => x.CreatedAt)
                            .First();

                        session.Send(new CreateCharacterSuccessFrame { Character = new CharacterDto(created, 
                            Controllers.GetCharacterExperiencesController?.GetByCharacter(created.Id) 
                            ?? throw Exceptions.New("Experience for character " + created.Name + " not found")) });
                    }
                    else
                        session.Send(new CharacterErrorFrame { Message = "No se ha podido crear el personaje." });
                    break;

                case DeleteCharacterFrame deleteCharacter:
                    if (characterController.Delete(deleteCharacter.CharacterId))
                        session.Send(new DeleteCharacterSuccessFrame { CharacterId = deleteCharacter.CharacterId });
                    else
                        session.Send(new CharacterErrorFrame { Message = "No se ha podido eliminar el personaje." });
                    break;
            }
        }
    }
}