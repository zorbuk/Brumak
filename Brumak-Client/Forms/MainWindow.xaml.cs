using Brumak_Client.Audio;
using Brumak_Client.Network;
using Brumak_ORM;
using Brumak_Shared.Metrics;
using Brumak_Shared.Network.Frames.Account;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Brumak_Client.Forms
{
    public partial class MainWindow : Window
    {
        private static readonly Logger _logger = new("Client", typeof(MainWindow), App.ShowLogs, App.SaveLogs);
        public static MainWindow Instance { get; private set; } = null!;

        private readonly string AuthIp = Services.Configuration.GetConnectionString("AuthServerIp")
            ?? throw Exceptions.New("'AuthServerIp' is not correctly defined on ConnectionStrings.");

        private readonly int AuthPort = int.Parse(Services.Configuration.GetConnectionString("AuthServerPort")
            ?? throw Exceptions.New("'AuthServerPort' is not correctly defined on ConnectionStrings."));

        public event Action<AccountFrame>? OnAccountFrameMessage;

        #region "Constructor"
        public MainWindow()
        {
            InitializeComponent();

            #region "Ui Sounds"
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler((sender, _) =>
            {
                if (sender is FrameworkElement el && el.Tag is "silent") return;
                AudioManager.Instance.UI.Play(@"/Assets/Audio/Ui/Click.ogg");
            }));

            AudioManager.Instance.BGM.Play(@"/Assets/Audio/Bgm/A Sailor's Dream.ogg", true);
            #endregion

            Instance = this;
            LoadCustomCursor();

            OnAccountFrameMessage += accountFrame =>
            {
                Dispatcher.Invoke(() =>
                {
                    switch (accountFrame)
                    {
                        case AccountErrorFrame accountErrorFrame:
                            CAAccountCreationResultTb.Visibility = Visibility.Visible;
                            CAAccountCreationResultTb.Text = accountErrorFrame.Message;
                            LAAccountLoginResult.Visibility = Visibility.Visible;
                            LAAccountLoginResult.Text = accountErrorFrame.Message;
                            break;

                        case LoginSuccessFrame loginSuccessFrame:
                            AccountSingleton.Instance.SetAccount(loginSuccessFrame.Account);
                            Application.Current.MainWindow = ServerSelection.Instance;
                            if(ServerSelection.Instance == null)
                                ServerSelection.Instance ??= new();
                            ServerSelection.Instance.AskServersFrame();
                            ServerSelection.Instance.Show();
                            Hide();
                            break;

                        case RegisterSuccessFrame registerSuccessFrame:
                            CAAccountCreationResultTb.Visibility = Visibility.Visible;
                            CAAccountCreationResultTb.Text = registerSuccessFrame.Message;
                            break;
                    }
                });
            };

            if (NetworkManager.AuthClientManager == null)
            {
                ClientFrameDispatcher.Initialize();
                _ = Task.Run(InitializeAuthNetworkAsync);
            }

            ServerSelection.Instance ??= new();
        }
        #endregion
        #region "Network"
        public async void InitializeAuthNetworkAsync()
        {
            while (true)
            {
                try
                {
                    NetworkManager.SetAuth(new TcpClientProvider());

                    NetworkManager.AuthClientManager.OnPingUpdated += ping =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (PingLabel == null || StatusEllipse == null) return;

                            PingLabel.Text = $"{ping} ms";
                            StatusEllipse.Fill = ping < 100
                                ? System.Windows.Media.Brushes.LimeGreen
                                : ping < 250
                                    ? System.Windows.Media.Brushes.Orange
                                    : System.Windows.Media.Brushes.Red;
                        });
                    };

                    NetworkManager.AuthClientManager.OnDisconnected += reconnect =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (PingLabel != null && StatusEllipse != null)
                            {
                                PingLabel.Text = "En estos momentos el servidor está sin conexión, reconectando (...)";
                                StatusEllipse.Fill = System.Windows.Media.Brushes.Red;
                            }

                            if (reconnect)
                                InitializeAuthNetworkAsync();
                        });
                    };

                    await NetworkManager.AuthClientManager.ConnectAsync(AuthIp, AuthPort);
                    break;
                }
                catch (TimeoutException) { }
                catch (Exception)
                {
                    await Task.Delay(2000);
                }
            }
        }
        #endregion
        #region "Cursor"
        #region "Cursor Interop"
        [DllImport("user32.dll")] private static extern bool DestroyCursor(IntPtr handle);
        [DllImport("user32.dll")] private static extern IntPtr CreateIconIndirect(ref IconInfo icon);
        [DllImport("user32.dll")] private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        private class SafeCursorHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeCursorHandle(IntPtr handle) : base(true) => SetHandle(handle);
            protected override bool ReleaseHandle() => DestroyCursor(handle);
        }

        private static class CursorInteropHelper
        {
            public static Cursor Create(System.Runtime.InteropServices.SafeHandle handle)
            {
                var constructor = typeof(Cursor).GetConstructor(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    [typeof(System.Runtime.InteropServices.SafeHandle)],
                    null
                );
                return (Cursor)constructor!.Invoke([handle]);
            }
        }
        #endregion
        private void LoadCustomCursor()
        {
            try
            {
                var streamInfo = Application.GetResourceStream(
                    new Uri("pack://application:,,,/Assets/Ui/Pointer.png"));

                if (streamInfo == null)
                {
                    _logger.Log("Cursor load failed.");
                    return;
                }

                using var stream = streamInfo.Stream;
                using Bitmap original = new(stream);
                using Bitmap cropped = original.Clone(new Rectangle(0, 0, 19, 19), original.PixelFormat);

                IntPtr hIcon = cropped.GetHicon();
                IconInfo info = new();
                GetIconInfo(hIcon, ref info);

                info.xHotspot = 2;
                info.yHotspot = 2;
                info.fIcon = false;

                IntPtr cursorHandle = CreateIconIndirect(ref info);
                DestroyCursor(hIcon);
                DeleteObject(info.hbmMask);
                DeleteObject(info.hbmColor);

                Mouse.OverrideCursor = this.Cursor = CursorInteropHelper.Create(new SafeCursorHandle(cursorHandle));

                _logger.Log("Cursor loaded.");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error loading cursor: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        #endregion
        #region "Helpers"
        private void CloseTermsAndConditionsIfOpened()
        {
            if (TermsUserControl.Visibility == Visibility.Visible)
                TermsUserControl.Visibility = Visibility.Hidden;
        }
        #endregion
        #region "Validations"
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool ValidateLoginAccount()
        {
            LAAccountLoginResult.Visibility = Visibility.Hidden;
            LAAccountLoginResult.Text = "";

            List<string> errors = [];

            if (string.IsNullOrWhiteSpace(LAUsername.Text?.Trim()))
                errors.Add("El usuario o email es obligatorio.");
            else
                LAUsername.ClearValue(BorderBrushProperty);

            if (string.IsNullOrWhiteSpace(LAPassword.Password))
                errors.Add("La contraseña es obligatoria.");
            else
                LAPassword.ClearValue(BorderBrushProperty);

            if (errors.Count > 0)
            {
                LAAccountLoginResult.Text = string.Join("\n", errors);
                LAAccountLoginResult.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        private bool ValidateCreateAccount()
        {
            CAAccountCreationResultTb.Visibility = Visibility.Hidden;
            CAAccountCreationResultTb.Text = "";

            List<string> errors = [];

            if (string.IsNullOrWhiteSpace(CAEmail.Text))
                errors.Add("El email es obligatorio.");
            else if (CAEmail.Text.Length < 4 || !IsValidEmail(CAEmail.Text))
                errors.Add("El email no es válido.");

            if (string.IsNullOrWhiteSpace(CAUsername.Text))
                errors.Add("El usuario es obligatorio.");
            else if (CAUsername.Text.Length < 4)
                errors.Add("El usuario debe tener al menos 4 caracteres.");

            if (string.IsNullOrWhiteSpace(CAPassword.Password))
                errors.Add("La contraseña es obligatoria.");
            else if (CAPassword.Password.Length < 4)
                errors.Add("La contraseña debe tener al menos 4 caracteres.");

            if (string.IsNullOrWhiteSpace(CANickname.Text))
                errors.Add("El apodo es obligatorio.");
            else if (CANickname.Text.Length < 4)
                errors.Add("El apodo debe tener al menos 4 caracteres.");

            if (CATermsAndConditionsCb.IsChecked != true)
                errors.Add("Debes aceptar los términos y condiciones.");

            if (errors.Count > 0)
            {
                CAAccountCreationResultTb.Text = string.Join("\n", errors);
                CAAccountCreationResultTb.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }
        #endregion
        #region "Window Events"
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void ShowLogin(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Visible;
            RegisterPanel.Visibility = Visibility.Collapsed;
            BtnLogin.Style = (Style)FindResource("TabActive");
            BtnRegister.Style = (Style)FindResource("TabInactive");
        }

        private void ShowRegister(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
            BtnLogin.Style = (Style)FindResource("TabInactive");
            BtnRegister.Style = (Style)FindResource("TabActive");
        }

        private void ShowTermsAndConditions(object sender, RoutedEventArgs e)
            => TermsUserControl.Visibility = Visibility.Visible;

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var ip = Ip.Get() ?? throw Exceptions.New("Fatal error Ip should exist.");
            if (!ValidateCreateAccount()) return;

            CloseTermsAndConditionsIfOpened();

            NetworkManager.AuthClientManager.Send(new RegisterFrame
            {
                Username = CAUsername.Text,
                Password = CAPassword.Password,
                Nickname = CANickname.Text,
                Email = CAEmail.Text,
                Ip = ip
            });
        }

        private void LoginAccount_Click(object sender, RoutedEventArgs e)
        {
            var ip = Ip.Get() ?? throw Exceptions.New("Fatal error Ip should exist.");
            if (!ValidateLoginAccount()) return;

            CloseTermsAndConditionsIfOpened();

            NetworkManager.AuthClientManager.Send(new LoginFrame
            {
                Username = LAUsername.Text,
                Password = LAPassword.Password,
                Ip = ip
            });
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e) => AudioMixerPanel.Toggle();

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
        #endregion
        #region "Raise Events"
        public void RaiseAccountFrameMessage(AccountFrame frame)
            => OnAccountFrameMessage?.Invoke(frame);
        #endregion
    }
}