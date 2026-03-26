using Brumak_Client.Network;
using Brumak_ORM;
using Brumak_Shared.Metrics;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
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

        #region "Cursor Interop"
        [DllImport("user32.dll")]
        private static extern bool DestroyCursor(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [StructLayout(LayoutKind.Sequential)]
        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr handle);

        private class SafeCursorHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeCursorHandle(IntPtr handle) : base(true)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                return DestroyCursor(handle);
            }
        }

        private static class CursorInteropHelper
        {
            public static Cursor Create(System.Runtime.InteropServices.SafeHandle handle)
            {
                var cursorType = typeof(Cursor);
                var constructor = cursorType.GetConstructor(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new[] { typeof(System.Runtime.InteropServices.SafeHandle) },
                    null
                );

                return (Cursor)constructor.Invoke(new object[] { handle });
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            LoadCustomCursor();
            ClientFrameDispatcher.Initialize();

            _ = Task.Run(InitializeAuthNetworkAsync);
        }

        private async void InitializeAuthNetworkAsync()
        {
            while (true)
            {
                try
                {
                    NetworkManager.SetAuth(new TcpClientProvider());

                    using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                    NetworkManager.AuthClientManager.OnPingUpdated += ping =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (PingLabel != null && StatusEllipse != null)
                            {
                                PingLabel.Text = $"{ping} ms";
                                StatusEllipse.Fill = ping < 100
                                    ? System.Windows.Media.Brushes.LimeGreen
                                    : ping < 250 ? System.Windows.Media.Brushes.Orange : System.Windows.Media.Brushes.Red;
                            }
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
                catch (TimeoutException)
                {

                }
                catch (Exception)
                {
                    await Task.Delay(2000);
                }
            }
        }

        private void LoadCustomCursor()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Assets/Ui/Pointer.png");
                var streamInfo = Application.GetResourceStream(uri);

                if (streamInfo != null)
                {
                    using var stream = streamInfo.Stream;
                    using Bitmap original = new(stream);
                    Rectangle cropRect = new(0, 0, 19, 19);

                    using Bitmap croppedBitmap = original.Clone(cropRect, original.PixelFormat);
                    IntPtr hIcon = croppedBitmap.GetHicon();

                    IconInfo iconInfo = new();
                    GetIconInfo(hIcon, ref iconInfo);

                    iconInfo.xHotspot = 2;
                    iconInfo.yHotspot = 2;
                    iconInfo.fIcon = false;

                    IntPtr cursorHandle = CreateIconIndirect(ref iconInfo);

                    DestroyCursor(hIcon);
                    DeleteObject(iconInfo.hbmMask);
                    DeleteObject(iconInfo.hbmColor);

                    var safeCursorHandle = new SafeCursorHandle(cursorHandle);
                    var cursor = CursorInteropHelper.Create(safeCursorHandle);

                    this.Cursor = cursor;
                    Mouse.OverrideCursor = cursor;

                    _logger.Log("Cursor loaded.");
                }
                else
                {
                    _logger.Log("Cursor load failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error loading cursor: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        #region "Window Events"
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

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

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        #endregion
    }
}