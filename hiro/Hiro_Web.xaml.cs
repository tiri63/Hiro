﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace hiro
{
    /// <summary>
    /// Web.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Web : Window
    {
        internal bool self = false;
        internal string? fixed_title = null;
        private ResizeMode rm = ResizeMode.NoResize;
        private WindowState ws = WindowState.Normal;
        private WindowStyle wt = WindowStyle.None;
        private string prefix = "";
        private bool secure = false;
        internal int bflag = 0;
        private string FlowTitle = "";
        private string startUri = "<hiuser>";
        private string currentUrl = "hiro";
        private string iconUrl = "/hiro_circle.ico";
        private ImageSource? savedicon = null;
        private ImageSource? savedWebIcon = null;
        public Hiro_Web(string? uri = null, string? title = null, string startUri = "<hiuser>")
        {
            InitializeComponent();
            if (!startUri.Equals("<hiuser>"))
            {
                uribtn.Visibility = Visibility.Visible;
                uribtn.Content = startUri;
            }
            this.startUri = startUri;
            Title = App.AppTitle;
            Load_Color();
            Load_Translate();
            Refreash_Layout();
            if (uri != null && uri.ToLower().Equals("hiro://crash"))
            {
                CrashedGrid.Visibility = Visibility.Visible;
                Show();
            }
            else
            {
                var env = Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(userDataFolder: Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare($"<hiapp>\\web\\{startUri}\\")));
                wv2.EnsureCoreWebView2Async(env.Result);
                CrashedGrid.Visibility = Visibility.Visible;
                try
                {
                    string edgever = Microsoft.Web.WebView2.Core.CoreWebView2Environment.GetAvailableBrowserVersionString();
                    if (string.IsNullOrEmpty(edgever))
                    {
                        Hiro_Utils.RunExe($"notify({Hiro_Utils.Get_Translate("webnotinstall")},2)", Hiro_Utils.Get_Translate("web"));
                        Close();
                    }
                    else
                    {
                        if (uri != null && uri.ToLower().Equals("hiro://clear"))
                        {
                            Width = Height = 1;
                            ShowInTaskbar = false;
                            Margin = new(-1, -1, 0, 0);
                            WindowStyle = WindowStyle.None;
                            WindowStartupLocation = WindowStartupLocation.Manual;
                            Show();
                            Hide();
                            wv2.CoreWebView2InitializationCompleted += ClearCache;
                        }
                        else
                        {
                            if (uri != null)
                            {
                                if (uri.Trim().Equals(string.Empty))
                                    uri = startUri.Equals("<hiuser>") ? "https://rexio.cn/" : Hiro_Utils.Read_Ini(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare($"<hiapp>\\web\\{startUri}\\web.config")), "Web", "Home", "https://rexio.cn/");
                                try
                                {
                                    wv2.Source = new Uri(uri);
                                }
                                catch
                                {
                                    wv2.Source = new Uri($"http://{uri}");
                                }
                            }
                            Show();
                            fixed_title = title;
                            wv2.CoreWebView2InitializationCompleted += Wv2_CoreWebView2InitializationCompleted;
                            Loaded += delegate
                            {
                                HiHiro();
                            };
                        }
                    }
                    Hiro_Utils.LogtoFile($"Edge Webview2 Version: {edgever}");
                }
                catch (Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Web.EdgeWebviewNotInstalled");
                    Close();
                }
            }
        }

        public void HiHiro()
        {
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, TitleGrid, sb, -50, null);
                sb.Begin();
            }
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            UpdateTitleLabel();
        }

        private void ClearCache(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            wv2.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
            Hiro_Utils.RunExe("Delete(<hiapp>\\images\\web\\)", Hiro_Utils.Get_Translate("web"));
            App.Notify(new(Hiro_Utils.Get_Translate("webclear"), 2, Hiro_Utils.Get_Translate("Web")));
            Close();
        }
        public void Loadbgi(int direction, bool animation)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage, this);
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Wv2_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            savedicon = Icon;
            wv2.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            wv2.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            wv2.CoreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;
            wv2.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            wv2.CoreWebView2.FrameNavigationStarting += CoreWebView2_FrameNavigationStarting;
            wv2.CoreWebView2.FrameNavigationCompleted += CoreWebView2_FrameNavigationCompleted;
            wv2.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            wv2.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            wv2.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            wv2.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
            wv2.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsDocumentPlayingAudioChanged;
            wv2.CoreWebView2.IsDefaultDownloadDialogOpenChanged += CoreWebView2_IsDefaultDownloadDialogOpenChanged;
            wv2.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            wv2.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            //wv2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            wv2.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            //wv2.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            wv2.CoreWebView2.Settings.IsStatusBarEnabled = false;
            if (fixed_title == null)
                URLGrid.Visibility = Visibility.Visible;
            if (fixed_title != null && !Topmost)
                topbtn.Visibility = Visibility.Collapsed;
        }

        private void CoreWebView2_ContextMenuRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            for (var index = 0; index < e.MenuItems.Count; index++)
            {
                if (!e.MenuItems[index].Name.Equals("other") &&
                    !e.MenuItems[index].Name.Equals("createQrCode"))
                    continue;
                e.MenuItems.RemoveAt(index);
                index--;
            }
        }

        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
            UpdateIcon();
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            var visual = PreBtn.Visibility;
            var visual2 = NextBtn.Visibility;
            PreBtn.Visibility = wv2.CoreWebView2.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            NextBtn.Visibility = wv2.CoreWebView2.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            if (PreBtn.Visibility == Visibility.Visible && visual != PreBtn.Visibility && animation)
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(0, PreBtn, sb, -50, null);
                sb.Begin();
            }
            if (NextBtn.Visibility == Visibility.Visible && visual2 != NextBtn.Visibility && animation)
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(0, NextBtn, sb, -50, null);
                sb.Begin();
            }
        }

        public void Load_Color()
        {
            Background = new SolidColorBrush(App.AppAccentColor);
            CrashedButton.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Hiro_Utils.Set_Bgimage(bgimage, this);
        }

        private void CoreWebView2_IsDefaultDownloadDialogOpenChanged(object? sender, object e)
        {
            if (wv2.CoreWebView2.IsDefaultDownloadDialogOpen == true)
            {
                wv2.CoreWebView2.CloseDefaultDownloadDialog();
            }
        }

        private void CoreWebView2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            var visual = soundbtn.Visibility;
            soundbtn.Visibility = wv2.CoreWebView2.IsDocumentPlayingAudio ? Visibility.Visible : Visibility.Collapsed;
            if (soundbtn.Visibility == Visibility.Visible && visual != soundbtn.Visibility && animation)
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(2, soundbtn, sb, -50, null);
                sb.Begin();
            }
            UpdateTitleLabel(wv2.CoreWebView2.IsDocumentPlayingAudio);
        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            Web_FullScreen(wv2.CoreWebView2.ContainsFullScreenElement);
        }

        private void Web_FullScreen(bool isFullScreen)
        {
            if (isFullScreen)
            {
                TitleGrid.Visibility = Visibility.Collapsed;
                wt = WindowStyle;
                ws = WindowState;
                rm = ResizeMode;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                TitleGrid.Visibility = Visibility.Visible;
                WindowStyle = wt;
                WindowState = ws;
                ResizeMode = rm;
                Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
                Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
                Refreash_Layout();
            }
        }

        private void SetIcon(BitmapImage bi)
        {
            Dispatcher.Invoke(() =>
            {
                ImageBrush ib = new()
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = bi
                };
                uicon.Source = bi;
                Icon = bi;
                savedWebIcon = bi;
            });
        }

        private void SetSavedPrimitiveIcon()
        {
            Dispatcher.Invoke(() =>
            {
                if (savedicon != null)
                {
                    uicon.Source = savedicon;
                    Icon = savedicon;
                }
            });
        }

        private void UpdateIcon()
        {
            if (WebGrid.Visibility != Visibility.Visible)
            {
                SetSavedPrimitiveIcon();
            }
            else
            {
                var iconUri = wv2.CoreWebView2.FaviconUri;
                if (iconUri != null && !iconUri.Trim().Equals(string.Empty))
                {
                    if (!iconUri.Equals(iconUrl))
                    {
                        new System.Threading.Thread(() =>
                        {
                            var output = Hiro_Utils.Path_Prepare("<hiapp>\\images\\web\\") + Guid.NewGuid() + ".hwico";
                            Hiro_Utils.CreateFolder(output);
                            var result = Hiro_Utils.GetWebContent(iconUri, true, output);
                            if (!result.Equals("error"))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        BitmapImage? bi = Hiro_Utils.GetBitmapImage(output);
                                        if (bi != null)
                                        {
                                            SetIcon(bi);
                                            iconUrl = iconUri;
                                        }
                                        else
                                        {
                                            SetSavedPrimitiveIcon();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Hiro_Utils.LogError(ex, "Hiro.Web.Favicon");
                                    }

                                });
                                try
                                {
                                    System.IO.File.Delete(output);
                                }
                                catch { }
                            }
                            else
                            {
                                SetSavedPrimitiveIcon();
                            }
                        }).Start();
                    }         
                    else
                    {
                        if (savedWebIcon != null)
                        {
                            uicon.Source = savedWebIcon;
                            Icon = savedWebIcon;
                        }
                    }
                }
                else
                {
                    SetSavedPrimitiveIcon();
                }
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            UpdateIcon();
            UpdateTitleLabel();
            URLBtn.Content = secure ? Hiro_Utils.Get_Translate("websecure") : Hiro_Utils.Get_Translate("webinsecure");
            URLSign.Content = secure ? "\uF61A" : "\uF618";
            URLBtn.ToolTip = secure ? Hiro_Utils.Get_Translate("websecuretip") : Hiro_Utils.Get_Translate("webinsecuretip");
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1") && URLGrid.Visibility == Visibility.Visible)
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, URLGrid, sb, -50, null);
                sb.Begin();
            }
            Loading(false);
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            UpdateIcon();
            currentUrl = wv2.CoreWebView2.Source;
            secure = true;
            Loading(true);
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var msg = e.TryGetWebMessageAsString();
                App.Notify(new(msg, 2, Hiro_Utils.Get_Translate("web")));
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Webview");
            }
        }

        private void CoreWebView2_FrameNavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Loading(false);
        }

        private void CoreWebView2_FrameNavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.ToLower().StartsWith("http://"))
                secure = false;
            Loading(true);
            if (URLBox.Visibility == Visibility.Visible)
            {
                URLBox.Visibility = Visibility.Collapsed;
                TitleLabel.Visibility = Visibility.Visible;
            }
        }

        private void Loading(bool state)
        {
            if (state)
            {
                wvpb.Visibility = Visibility.Visible;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Indeterminate, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
            else
            {
                wvpb.Visibility = Visibility.Collapsed;
                Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, new System.Windows.Interop.WindowInteropHelper(this).Handle);
            }
            UpdateTitleLabel(state);
        }
        private void CoreWebView2_DownloadStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            Hiro_Utils.RunExe($"Download({e.DownloadOperation.Uri})", Hiro_Utils.Get_Translate("web"));
            if (wv2.CoreWebView2.DocumentTitle.Trim().Equals(string.Empty) || wv2.CoreWebView2.Source.ToLower().StartsWith("about"))
                Close();
            e.Cancel = true;
            e.Handled = true;
        }

        private void CoreWebView2_WindowCloseRequested(object? sender, object e)
        {
            Close();
        }

        private void CoreWebView2_NewWindowRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            if (!e.Uri.ToLower().StartsWith("about"))
            {
                if (self)
                    wv2.Source = new(e.Uri);
                else
                {
                    Hiro_Web web = new(e.Uri, fixed_title, startUri);
                    web.WindowState = WindowState;
                    web.Show();
                    web.Refreash_Layout();
                }
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wv2.Stop();
            wv2.DataContext = null;
            wv2.Dispose();
            e.Cancel = false;
        }

        private void Minbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Closebtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void Maxbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
            e.Handled = true;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Refreash_Layout();
        }

        public void Refreash_Layout()
        {

            TitleGrid.Height = WindowState == WindowState.Maximized ? 26 : 32;
            WebGrid.Margin = TitleGrid.Visibility == Visibility.Collapsed ? new(0) : WindowState == WindowState.Maximized ? new(0, 26, 0, 0) : new(0, 32, 0, 0);
            maxbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
            resbtn.Visibility = ResizeMode == ResizeMode.NoResize || ResizeMode == ResizeMode.CanMinimize ? Visibility.Collapsed : WindowState == WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
            closebtn.Margin = WindowState == WindowState.Maximized ? new(0, -5, 0, 0) : new(0, -2, 0, 0);
            closebtn.Height = WindowState == WindowState.Maximized ? 30 : 32;
            BaseGrid.Margin = WindowState == WindowState.Maximized ? new(6) : new(0);
            soundbtn.Margin = WindowState == WindowState.Maximized ? new(0, 1, soundbtn.Margin.Right, 0) : new(0, 0, soundbtn.Margin.Right, 0);
            uribtn.Margin = WindowState == WindowState.Maximized ? new(0, 1, uribtn.Margin.Right, 0) : new(0, 0, uribtn.Margin.Right, 0);
            topbtn.Margin = WindowState == WindowState.Maximized ? new(0, 1, topbtn.Margin.Right, 0) : new(0, 0, topbtn.Margin.Right, 0);
            soundbtn.Height = WindowState == WindowState.Maximized ? 29 : 30;
            uribtn.Height = WindowState == WindowState.Maximized ? 29 : 30;
            Hiro_Utils.Set_Control_Location(URLBtn, "websecure", location: false);
            Hiro_Utils.Set_Control_Location(URLBox, "weburl", location: false);
            Hiro_Utils.Set_Control_Location(PreBtn, "webpre", location: false);
            Hiro_Utils.Set_Control_Location(NextBtn, "webnext", location: false);
            Hiro_Utils.Set_Control_Location(uribtn, "webspace", location: false);
            Hiro_Utils.Set_Control_Location(CrashedLabel, "webcrashtip");
            Hiro_Utils.Set_Control_Location(CrashedButton, "webcrashbtn");
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")), false);
        }

        public void Load_Translate()
        {
            minbtn.ToolTip = Hiro_Utils.Get_Translate("Min");
            closebtn.ToolTip = Hiro_Utils.Get_Translate("close");
            maxbtn.ToolTip = Hiro_Utils.Get_Translate("max");
            resbtn.ToolTip = Hiro_Utils.Get_Translate("restore");
            PreBtn.ToolTip = Hiro_Utils.Get_Translate("webpre");
            PreBtn.Content = Hiro_Utils.Get_Translate("webprec").Replace("%s", "◀");
            NextBtn.ToolTip = Hiro_Utils.Get_Translate("webnext");
            NextBtn.Content = Hiro_Utils.Get_Translate("webnextc").Replace("%s", "▶");
            URLBtn.Content = Hiro_Utils.Get_Translate("webinsecure");
            CrashedLabel.Content = Hiro_Utils.Get_Translate("webcrashtip");
            CrashedButton.Content = Hiro_Utils.Get_Translate("webcrashbtn");
            URLSign.Content = "\uF618";
            URLBtn.ToolTip = Hiro_Utils.Get_Translate("webinsecuretip");
            topbtn.ToolTip = Topmost ? Hiro_Utils.Get_Translate("webbottom") : Hiro_Utils.Get_Translate("webtop");
            soundbtn.ToolTip = Hiro_Utils.Get_Translate("webmute");
            uribtn.ToolTip = Hiro_Utils.Get_Translate("webspace").Replace("%s", startUri);
        }

        private void Resbtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void TitleGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            e.Handled = true;
        }

        private void PreBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.GoBack();
            e.Handled = true;
        }

        private void NextBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.GoForward();
            e.Handled = true;
        }

        private void URLBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TitleLabel.Visibility = URLBox.Visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            URLBox.Visibility = URLBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            URLBox.Text = wv2.CoreWebView2.Source;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                if (TitleLabel.Visibility == Visibility.Visible)
                    Hiro_Utils.AddPowerAnimation(1, TitleLabel, sb, -50, null);
                else
                    Hiro_Utils.AddPowerAnimation(1, URLBox, sb, -50, null);
                sb.Begin();
            }
            e.Handled = true;
        }

        private void URLBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            Hiro_Web web = new(URLBox.Text, fixed_title, startUri);
                            web.WindowState = WindowState;
                            web.Show();
                            web.Refreash_Layout();
                            e.Handled = true;
                            return;
                        }
                        switch (URLBox.Text.ToLower())
                        {
                            case "topmost:on":
                            case "topmost:true":
                                Topmost = true;
                                break;
                            case "topmost:off":
                            case "topmost:false":
                                Topmost = false;
                                break;
                            case "mute:on":
                            case "mute:true":
                                wv2.CoreWebView2.IsMuted = true;
                                break;
                            case "mute:off":
                            case "mute:false":
                            case "fullscreen":
                                wv2.CoreWebView2.IsMuted = false;
                                break;
                            default:
                                if (fixed_title == null)
                                    try
                                    {
                                        wv2.CoreWebView2.Navigate(URLBox.Text);
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            wv2.CoreWebView2.Navigate($"http://{URLBox.Text}");
                                        }
                                        catch
                                        {
                                            Hiro_Utils.LogError(ex, "Hiro.Exception.Webview.URL");
                                        }
                                    }
                                break;
                        }


                        URLBox.Visibility = Visibility.Collapsed;
                        TitleLabel.Visibility = Visibility.Visible;
                        e.Handled = true;
                        break;
                    }
                case Key.Escape:
                    URLBox.Visibility = Visibility.Collapsed;
                    TitleLabel.Visibility = Visibility.Visible;
                    e.Handled = true;
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refreash_Layout();
        }

        private void Soundbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wv2.CoreWebView2.IsMuted = !wv2.CoreWebView2.IsMuted;
            soundbtn.Content = wv2.CoreWebView2.IsMuted ? "\uE198" : "\uE995";
            soundbtn.ToolTip = wv2.CoreWebView2.IsMuted ? Hiro_Utils.Get_Translate("websound") : Hiro_Utils.Get_Translate("webmute");
            e.Handled = true;
        }

        private void UpdateTitleLabel(bool loading = false)
        {
            URLBox.Visibility = Visibility.Collapsed;
            TitleLabel.Visibility = Visibility.Visible;
            if (WebGrid.Visibility != Visibility.Visible)
            {
                TitleLabel.Text = App.AppTitle;
            }
            else
            {
                prefix = wv2.CoreWebView2.IsDocumentPlayingAudio ? Hiro_Utils.Get_Translate("webmusic") : "";
                string ti = fixed_title ?? Hiro_Utils.Get_Translate("webtitle");
                TitleLabel.Text = loading? ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", Hiro_Utils.Get_Translate("loading")).Replace("%p", prefix).Replace("%h", App.AppTitle) : ti.Replace("%t", wv2.CoreWebView2.DocumentTitle).Replace("%i", "").Replace("%p", prefix).Replace("%h", App.AppTitle);
                if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1") || FlowTitle.Equals(Title))
                {
                    Title = TitleLabel.Text;
                    return;
                }
                    
                FlowTitle = Title;
                System.Windows.Media.Animation.Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(1, TitleLabel, sb, -50, null);
                sb.Begin();
            }
            Title = TitleLabel.Text;
        }

        private void Topbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Topmost = !Topmost;
            topbtn.Content = Topmost ? "\uE77A" : "\uE718";
            topbtn.ToolTip = Topmost ? Hiro_Utils.Get_Translate("webbottom") : Hiro_Utils.Get_Translate("webtop");
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyStates == Keyboard.GetKeyStates(Key.W) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Tab) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (WebGrid.Visibility != Visibility.Visible)
                {
                    WebGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    WebGrid.Visibility = Visibility.Hidden;
                }
                UpdateTitleLabel();
                UpdateIcon();
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.F11))
            {
                Web_FullScreen(TitleGrid.Visibility == Visibility.Visible);
                e.Handled = true;
            }
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
            {
                if (TitleGrid.Visibility != Visibility.Visible)
                {
                    Web_FullScreen(false);
                    e.Handled = true;
                }
            }
        }

        private void Uribtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.RunExe(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare($"<hiapp>\\web\\{startUri}\\EBWebView\\")), Hiro_Utils.Get_Translate("web"));
        }

        private void CrashedButton_Click(object sender, RoutedEventArgs e)
        {
            Hiro_Web web = new(currentUrl, fixed_title, startUri);
            web.WindowState = WindowState;
            web.Show();
            web.Refreash_Layout();
            Close();
        }
    }
}
