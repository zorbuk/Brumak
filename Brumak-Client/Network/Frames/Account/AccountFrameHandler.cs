using Brumak_Client.Forms;
using Brumak_Shared.Network.Frames;
using Brumak_Shared.Network.Frames.Account;

namespace Brumak_Client.Network.Frames.Account
{
    public class AccountFrameHandler : IFrameHandler<AccountFrame>
    {
        public void Handle(object context, AccountFrame frame)
        {
            MainWindow.Instance.RaiseAccountFrameMessage(frame);
        }
    }
}
