using Brumak_Client.Audio;
using Brumak_Client.Network;
using Brumak_Shared.Network.Frames.Servers;
using Brumak_Shared.Server.Enum;
using Brumak_Shared.Server.Model;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Brumak_Client.Forms
{
    public partial class ServerSelection : Window
    {
        public static ServerSelection Instance { get; set; } = null!;

        private ServerInfo[] _servers = [];
        private int _selectedIndex = 0;
        private readonly List<Border> _serverBorders = [];
        private bool _isGoingBack = false;

        public event Action<ServersFrame>? OnServersFrameMessage;

        #region "Constructor"
        public ServerSelection()
        {
            InitializeComponent();

            #region "Ui Sounds"
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler((sender, _) =>
            {
                if (sender is FrameworkElement el && el.Tag is "silent") return;
                AudioManager.Instance.UI.Play(@"/Assets/Audio/Ui/Click.ogg");
            }));
            #endregion

            Instance = this;
            DataContext = AccountSingleton.Instance;

            Closed += (_, _) => Instance = null!;

            OnServersFrameMessage += serversFrame =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (serversFrame.Servers == null || serversFrame.Servers.Count == 0)
                        return;

                    string? previousSelection = _servers.Length > 0
                        ? _servers[_selectedIndex].Name
                        : null;

                    _servers = [.. serversFrame.Servers.Select(s => new ServerInfo
                    {
                        Id            = s.Id,
                        Name          = s.Name,
                        Description   = s.Description,
                        Community     = s.Community,
                        Flag          = s.Flag,
                        SplashArt     = s.SplashArt,
                        Status        = (ServerStatus)(int)s.Status,
                        Population    = s.Population,
                        IsMonoaccount = s.IsMonoaccount,
                        Ping          = "— ms",
                        Ip            = s.Ip,
                        Port          = s.Port
                    })];

                    ServerListPanel.Children.Clear();
                    _serverBorders.Clear();
                    BuildServerList();

                    int restoredIndex = previousSelection != null
                        ? Array.FindIndex(_servers, s => s.Name == previousSelection)
                        : -1;

                    if (restoredIndex < 0)
                        restoredIndex = Array.FindIndex(_servers, s => s.Status != ServerStatus.Offline);

                    SelectServer(restoredIndex >= 0 ? restoredIndex : 0);
                });

                _ = MeasurePingsAsync();
            };
        }
        #endregion
        #region "Ask Frames"
        public void AskServersFrame()
        {
            NetworkManager.AuthClientManager.Send(new ServersFrame());
        }
        #endregion
        #region "Server List"
        private void BuildServerList()
        {
            for (int i = 0; i < _servers.Length; i++)
            {
                var server = _servers[i];
                int index = i;

                var pingColor = server.Status switch
                {
                    ServerStatus.Online => new SolidColorBrush(Color.FromRgb(0x44, 0xFF, 0x88)),
                    ServerStatus.Busy => new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x00)),
                    ServerStatus.Offline => new SolidColorBrush(Color.FromRgb(0xFF, 0x33, 0x44)),
                    _ => Brushes.Gray
                };
                var pingBg = server.Status switch
                {
                    ServerStatus.Online => new SolidColorBrush(Color.FromArgb(0x22, 0x44, 0xFF, 0x88)),
                    ServerStatus.Busy => new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xCC, 0x00)),
                    ServerStatus.Offline => new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0x33, 0x44)),
                    _ => Brushes.Transparent
                };

                var statusDot = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    VerticalAlignment = VerticalAlignment.Center,
                    Fill = pingColor
                };

                var statusText = new TextBlock
                {
                    Text = server.Status switch
                    {
                        ServerStatus.Online => server.Population,
                        ServerStatus.Busy => server.Population,
                        ServerStatus.Offline => "Sin conexión",
                        _ => ""
                    },
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromArgb(0x88, 0xFF, 0xFF, 0xFF)),
                    Margin = new Thickness(6, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var statusRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
                statusRow.Children.Add(statusDot);
                statusRow.Children.Add(statusText);

                if (server.Status != ServerStatus.Offline)
                {
                    statusRow.Children.Add(new TextBlock
                    {
                        Text = "·",
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Color.FromArgb(0x44, 0xFF, 0xFF, 0xFF)),
                        Margin = new Thickness(6, 0, 6, 0)
                    });
                    statusRow.Children.Add(new TextBlock
                    {
                        Text = server.Population,
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Color.FromArgb(0x88, 0xFF, 0xFF, 0xFF)),
                        VerticalAlignment = VerticalAlignment.Center
                    });
                }

                var nameRow = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                nameRow.Children.Add(new TextBlock
                {
                    Text = server.Name,
                    Style = (Style)FindResource("ServerNameText")
                });
                nameRow.Children.Add(new Border
                {
                    Style = (Style)FindResource("TagBadge"),
                    Child = new TextBlock
                    {
                        Text = $"{server.Flag} {server.Community}",
                        FontSize = 10,
                        Foreground = new SolidColorBrush(Color.FromArgb(0xCC, 0xFF, 0xFF, 0xFF))
                    }
                });

                if (server.IsMonoaccount)
                {
                    nameRow.Children.Add(new Border
                    {
                        Background = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xAA, 0x44)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xAA, 0x44)),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(2),
                        Padding = new Thickness(5, 2, 5, 2),
                        Margin = new Thickness(6, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        Child = new TextBlock
                        {
                            Text = "MONOCUENTA",
                            FontSize = 9,
                            FontWeight = FontWeights.Bold,
                            Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xAA, 0x44))
                        }
                    });
                }

                var infoStack = new StackPanel();
                infoStack.Children.Add(nameRow);
                infoStack.Children.Add(statusRow);

                var pingBadge = new Border
                {
                    Background = pingBg,
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(7, 3, 7, 3),
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = new TextBlock
                    {
                        Text = server.Ping,
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = pingColor
                    }
                };

                var rowGrid = new Grid();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                Grid.SetColumn(infoStack, 0);
                Grid.SetColumn(pingBadge, 1);
                rowGrid.Children.Add(infoStack);
                rowGrid.Children.Add(pingBadge);

                var border = new Border
                {
                    Style = (Style)FindResource("ServerItem"),
                    Child = rowGrid,
                    Opacity = server.Status == ServerStatus.Offline ? 0.5 : 1.0
                };

                border.MouseLeftButtonDown += (_, _) => SelectServer(index);

                _serverBorders.Add(border);
                ServerListPanel.Children.Add(border);
            }

            SelectedServer.Visibility = Visibility.Visible;
        }

        private void SelectServer(int index)
        {
            _selectedIndex = index;
            var server = _servers[index];

            ServerSplashArt.Source = new System.Windows.Media.Imaging.BitmapImage(
                new System.Uri(server.SplashArt, System.UriKind.Relative));

            SelectedServerName.Text = server.Name;
            SelectedServerDesc.Text = server.Description;
            SelectedCommunityFlag.Text = server.Flag;
            SelectedCommunityText.Text = server.Community;

            (SelectedStatusDot.Fill, SelectedStatusText.Text) = server.Status switch
            {
                ServerStatus.Online => (new SolidColorBrush(Color.FromRgb(0x44, 0xFF, 0x88)), "En línea"),
                ServerStatus.Busy => (new SolidColorBrush(Color.FromRgb(0xFF, 0xCC, 0x00)), "Alta carga"),
                ServerStatus.Offline => (new SolidColorBrush(Color.FromRgb(0xFF, 0x33, 0x44)), "Sin conexión"),
                _ => (Brushes.Gray, "Desconocido")
            };

            ConnectBtn.IsEnabled = server.Status != ServerStatus.Offline;
            ConnectBtn.Opacity = server.Status != ServerStatus.Offline ? 1.0 : 0.4;

            for (int i = 0; i < _serverBorders.Count; i++)
                _serverBorders[i].Style = (Style)FindResource(i == index ? "ServerItemSelected" : "ServerItem");
        }
        #endregion
        #region "Ping"
        private async Task MeasurePingsAsync()
        {
            var snapshot = _servers.ToArray();

            await Task.WhenAll(snapshot.Select(async server =>
            {
                string ping;
                try
                {
                    using var client = new TcpClient();
                    var sw = Stopwatch.StartNew();
                    await client.ConnectAsync(server.Ip, server.Port)
                                .WaitAsync(TimeSpan.FromSeconds(2));
                    sw.Stop();
                    ping = $"{sw.ElapsedMilliseconds} ms";
                }
                catch
                {
                    ping = "— ms";
                }

                await Dispatcher.InvokeAsync(() => UpdateServerPing(server, ping));
            }));
        }

        private void UpdateServerPing(ServerInfo server, string ping)
        {
            server.Ping = ping;

            int index = Array.IndexOf(_servers, server);
            if (index < 0 || index >= _serverBorders.Count) return;

            if (_serverBorders[index].Child is Grid grid &&
                grid.Children[1] is Border pingBadge &&
                pingBadge.Child is TextBlock pingText)
            {
                pingText.Text = ping;
            }
        }
        #endregion
        #region "Window Events"
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _isGoingBack = true;
            AccountSingleton.Instance.Clear();
            Application.Current.MainWindow = MainWindow.Instance;
            MainWindow.Instance.Show();
            Close();
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (_servers == null || _servers.Length <= 0)
                return;
            var server = _servers[_selectedIndex];
            if (server.Status == ServerStatus.Offline) return;

            await NetworkManager.TransitionToWorld(server.Ip, server.Port);

            Application.Current.MainWindow = CharacterSelection.Instance;
            if (CharacterSelection.Instance == null)
                CharacterSelection.Instance ??= new(server);
            CharacterSelection.Instance.AskCharactersFrame();
            CharacterSelection.Instance.Show();
            Hide();
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e) => AudioMixerPanel.Toggle();

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!_isGoingBack)
                Application.Current.Shutdown();
        }
        #endregion
        #region "Raise Events"
        public void RaiseServersFrameMessage(ServersFrame frame)
        {
            OnServersFrameMessage?.Invoke(frame);
        }
        #endregion
    }
}