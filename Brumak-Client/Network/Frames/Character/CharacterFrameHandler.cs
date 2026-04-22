using Brumak_Client.Forms;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Characters;

namespace Brumak_Client.Network.Frames.Character
{
    public class CharacterFrameHandler : IFrameHandler<CharacterFrame>
    {
        public async Task Handle(object context, CharacterFrame frame)
        {
            CharacterSelection.Instance?.RaiseCharacterFrameMessage(frame);
            CharacterCreation.Instance?.RaiseCharacterFrameMessage(frame);
        }
    }
}
