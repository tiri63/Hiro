﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Hiro.Helpers;
using Hiro.ModelViews;
using Hiro.Resources;
using static Hiro.Helpers.HSet;
using static Hiro.Helpers.HLogger;
using System.Text;

namespace Hiro
{
    /// <summary>
    /// mainui.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_MainUI : Window
    {
        internal Page? current = null;
        private Label? selected = null;
        private int bflag = 0;
        internal Hiro_Home? hiro_home = null;
        internal Hiro_Items? hiro_items = null;
        internal Hiro_Schedule? hiro_schedule = null;
        internal Hiro_Config? hiro_config = null;
        internal Hiro_Profile? hiro_profile = null;
        internal Hiro_About? hiro_about = null;
        internal Hiro_NewItem? hiro_newitem = null;
        internal Hiro_NewSchedule? hiro_newschedule = null;
        internal Hiro_Time? hiro_time = null;
        internal Hiro_Color? hiro_color = null;
        internal Hiro_Acrylic? hiro_acrylic = null;
        internal Hiro_Proxy? hiro_proxy = null;
        internal Hiro_Chat? hiro_chat = null;
        internal Hiro_Login? hiro_login = null;
        internal string vlcPath = "";
        internal string currentBack = "";
        internal WindowAccentCompositor? compositor = null;
        internal bool _dragFlag = false;
        private int _msgCount = 0;

        public Hiro_MainUI()
        {
            InitializeComponent();
            HUI.SetCustomWindowIcon(this);
            LogtoFile("[HIROWEGO]Main UI: Initializing");
            SourceInitialized += OnSourceInitialized;
            MainUI_Initialize();
            MainUI_FirstInitialize();
            Loaded += delegate
            {
                HiHiro();
                Load_Video();
                System.Threading.Tasks.Task.Delay(1000).ContinueWith(t =>
                {
                    HMediaInfoManager.Initialize();
                });
            };
        }

        internal void Load_Video()
        {
            if (Read_DCIni("Background", "1").Equals("4"))
            {
                var video = Read_PPDCIni("BackVideo", "");
                var b = BackVideo.Visibility == Visibility.Visible && (BackVideo.IsPlaying || BackVideo.IsPaused) && BackVideo.MediaInfo.MediaSource.Equals(video.Trim());
                if ((System.IO.File.Exists(video) || video.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || video.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase) || video.StartsWith("rstp:", StringComparison.CurrentCultureIgnoreCase)) && !b)
                {
                    new System.Threading.Thread(async () =>
                    {
                        await Dispatcher.Invoke(async () =>
                        {
                            try
                            {
                                await BackVideo.Open(new Uri(video));
                                if (Visibility != Visibility.Visible)
                                    await BackVideo.Pause();
                                BackVideo.IsMuted = Read_DCIni("BackVideoMute", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase);
                                BackVideo.Visibility = Visibility.Visible;
                            }
                            catch (Exception e)
                            {
                                LogError(e, "Hiro.Main.Initialize.BackVideo");
                                BackVideo.Visibility = Visibility.Collapsed;
                            }
                        });
                    }).Start();
                }
            }
        }

        public void MainUI_Initialize()
        {
            InitializeUIWindow();
            LogtoFile("[HIROWEGO]Main UI: Load Transaltation");
            Load_Translate();
            LogtoFile("[HIROWEGO]Main UI: Load Data");
            Load_Data();
            LogtoFile("[HIROWEGO]Main UI: Load Colors");
            Load_Colors();
            LogtoFile("[HIROWEGO]Main UI: Load Position");
            OpacityBgi();
            LogtoFile("[HIROWEGO]Main UI: Load OpacityMask");
            Load_Position();
        }

        public void MainUI_FirstInitialize()
        {
            LogtoFile("[HIROWEGO]Main UI: Set Home");
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            LogtoFile("[HIROWEGO]Main UI: Intitalized");
            var bg = Read_DCIni("Background", "1");
            if (!bg.Equals("3") && !bg.Equals("4"))
                new System.Threading.Thread(() =>
            {
                var strFileName = Read_DCIni("BackImage", string.Empty);
                if (System.IO.File.Exists(@strFileName))
                {
                    try
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(@strFileName);
                        double w = img.Width;
                        double h = img.Height;
                        double ww = 550 * 2;
                        double hh = 450 * 2;
                        Dispatcher.Invoke(() =>
                        {
                            ww = Width * 2;
                            hh = Height * 2;
                        });
                        bool m = false;
                        if (Read_DCIni("Compression", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (ww < w && hh < h)
                            {
                                var r = 0;
                                while (ww < w && hh < h)
                                {
                                    w /= 2;
                                    h /= 2;
                                    r++;
                                }
                                w *= 2;
                                h *= 2;
                                if (r > 1)
                                {
                                    img = HMedia.ZoomImage(img, Convert.ToInt32(h), Convert.ToInt32(w));
                                    m = true;
                                }

                            }
                            long len = 0;
                            using (var stream = new System.IO.MemoryStream())
                            {
                                img.Save(stream, HMedia.GetImageFormat(img));
                                len = stream.Length;
                            }
                            if (len > 2048 * 1024)
                            {
                                img = HMedia.ZipImage(img, HMedia.GetImageFormat(img), 2048);
                                m = true;
                            }
                        }
                        if (m)
                        {
                            System.Drawing.Bitmap b = new(img);
                            img.Dispose();
                            strFileName = @"<hiapp>\images\background\" + strFileName.Substring(strFileName.LastIndexOf("\\"));
                            strFileName = HText.Path_PPX(strFileName);
                            HFile.CreateFolder(strFileName);
                            if (System.IO.File.Exists(strFileName))
                                System.IO.File.Delete(strFileName);
                            b.Save(strFileName);
                            b.Dispose();
                            strFileName = HText.Anti_Path_Prepare(strFileName);
                            Write_Ini(App.dConfig, "Config", "BackImage", strFileName);
                        }
                        else
                            img.Dispose();
                        Dispatcher.Invoke(async () =>
                        {
                            HUI.Set_Bgimage(bgimage, this);
                            currentBack = Read_PPDCIni("BackImage", "");
                        });
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, "Hiro.Exception.Background.Image.Select");
                    }
                }
            }).Start();
            LogtoFile("[HIROWEGO]Main UI: Loaded");
        }

        public void HiHiro()
        {
            if (Read_DCIni("Background", "1").Equals("2"))
                Blurbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
            if (!Read_DCIni("Ani", "2").Equals("0"))
            {
                Set_Label(selected ?? homex);
            }
            titlelabel.Visibility = Visibility.Visible;
            versionlabel.Visibility = Visibility.Visible;
            minbtn.Visibility = Visibility.Visible;
            closebtn.Visibility = Visibility.Visible;
            stack.Visibility = Visibility.Visible;
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                HAnimation.AddPowerAnimation(0, titlelabel, sb, -50, null);
                HAnimation.AddPowerAnimation(0, versionlabel, sb, -50, null);
                HAnimation.AddPowerAnimation(2, minbtn, sb, -50, null);
                HAnimation.AddPowerAnimation(2, closebtn, sb, -50, null);
                HAnimation.AddPowerAnimation(0, stack, sb, -50, null);
                if (infoPoly.Visibility == Visibility.Visible)
                    HAnimation.AddPowerAnimation(0, infoPoly, sb, -50, null, 150, 100);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
            LogtoFile("[HIROWEGO]Main UI: Hi!Hiro!");
        }

        public void AddToInfoCenter(string text)
        {
            _msgCount++;
            Dispatcher.Invoke(() =>
            {
                infotext.AppendText(text);
                var fv = infoPoly.Visibility;
                infoPoly.Visibility = Visibility.Visible;
                if (fv != Visibility.Visible)
                {
                    if (Read_DCIni("Ani", "2").Equals("1"))
                    {
                        Storyboard sb = new();
                        HAnimation.AddPowerAnimation(0, infoPoly, sb, -50, null);
                        sb.Begin();
                    }
                    HUI.Set_FrameworkElement_Location(TitleGrid, "tigridp", Read_DCIni("Ani", "2").Equals("1"));
                }
                else
                {
                    if (Read_DCIni("Ani", "2").Equals("1"))
                    {
                        var s = HUI.Get_FrameworkElement_Location("tgridt");
                        var s2 = HUI.Get_FrameworkElement_Location("tgridct");
                        var sb = HAnimation.AddColorAnimaton(App.AppAccentColor, s2.Left, infoPoly, "(Shape.Stroke).(SolidColorBrush.Color)", null);
                        sb.Completed += delegate
                        {
                            infoPoly.Stroke = new SolidColorBrush(App.AppAccentColor);
                            new System.Threading.Thread(() =>
                            {
                                System.Threading.Thread.Sleep((int)s2.Top);
                                Dispatcher.Invoke(() =>
                                {
                                    var sb2 = HAnimation.AddColorAnimaton(App.AppForeColor, s2.Right, infoPoly, "(Shape.Stroke).(SolidColorBrush.Color)", null);
                                    sb2.Completed += delegate
                                    {
                                        infoPoly.Stroke = new SolidColorBrush(App.AppForeColor);
                                    };
                                    sb2.Begin();
                                });
                            }).Start();
                        };
                        sb.Begin();
                        HUI.Set_FrameworkElement_Location(TitleGrid, "tigridpp", true, s.Left, 0, 0.6);
                        new System.Threading.Thread(() =>
                        {
                            System.Threading.Thread.Sleep((int)s.Top);
                            Dispatcher.Invoke(() =>
                            {
                                HUI.Set_FrameworkElement_Location(TitleGrid, infoPoly.Visibility == Visibility.Visible ? "tigridp" : "tigrid", true, s.Right, 0.9);
                            });
                        }).Start();
                    }
                }
                if (App.tb != null)
                {
                    App.tb.MsgLabel.Content = "📧 " + _msgCount.ToString();
                    App.tb.LoadGrid(App.tb.MsgGrid);
                }
            });
        }

        public void InitializeUIWindow()
        {
            if (App.Locked)
                versionlabel.Content = Hiro_Resources.ApplicationVersion + " 🔒";
            else
                versionlabel.Content = Hiro_Resources.ApplicationVersion;
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
            LogtoFile("[HIROWEGO]Main UI: AddHook WndProc");
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                default:
                    //Console.WriteLine("Msg: " + m.Msg + ";LParam: " + m.LParam + ";WParam: " + m.WParam + ";Result: " + m.Result);
                    break;
            }
            return IntPtr.Zero;

        }

        internal void HideAll()
        {
            if (MainGrid.Visibility == Visibility.Visible)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    var sb = HAnimation.AddPowerOutAnimation(0, MainGrid, null, null, 100);
                    sb.Completed += delegate
                    {
                        MainGrid.Visibility = Visibility.Collapsed;
                    };
                    sb.Begin();
                }
                else
                {
                    MainGrid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MainGrid.Visibility = Visibility.Visible;
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    var sb = HAnimation.AddPowerAnimation(0, MainGrid, null, 100);
                    sb.Begin();
                }
            }
        }

        #region UI相关
        public void Load_Position()
        {
            HUI.Set_FrameworkElement_Location(TitleGrid, infoPoly.Visibility == Visibility.Visible ? "tigridp" : "tigrid");
            HUI.Set_FrameworkElement_Location(infoPoly, "infopoly");
            HUI.Set_FrameworkElement_Location(DropInfo, "dropg");
            HUI.Set_Control_Location(titlelabel, "title");
            HUI.Set_Control_Location(versionlabel, "version");
            HUI.Set_Control_Location(infotitle, "infotitle");
            HUI.Set_Control_Location(infotext, "infotext");
            HUI.Set_Control_Location(homex, "home", location: false);
            HUI.Set_Control_Location(itemx, "item", location: false);
            HUI.Set_Control_Location(schedulex, "schedule", location: false);
            HUI.Set_Control_Location(configx, "config", location: false);
            HUI.Set_Control_Location(profilex, "profile", location: false);
            HUI.Set_Control_Location(aboutx, "about", location: false);
            HUI.Set_Control_Location(newx, "new", location: false);
            HUI.Set_Control_Location(colorx, "color", location: false);
            HUI.Set_Control_Location(timex, "time", location: false);
            HUI.Set_Control_Location(proxyx, "proxy", location: false);
            HUI.Set_Control_Location(chatx, "chat", location: false);
            HUI.Set_Control_Location(loginx, "login", location: false);
            HUI.Set_Control_Location(dropIci, "dropInfo", location: false);
        }

        internal void Set_AcrylicStyle(bool preview = false, int type = 0)
        {
            if (preview)
            {
                SetAcrylicStyle(type);
            }
            else
            {
                switch (Read_DCIni("AcrylicMain", "0"))
                {
                    case "1":
                        {
                            SetAcrylicStyle(1);
                            break;
                        }
                    case "2":
                        {
                            SetAcrylicStyle(2);
                            break;
                        }
                    default:
                        {
                            SetAcrylicStyle(0);
                            break;
                        }
                }
            }

        }

        private void SetAcrylicStyle(int type)
        {
            switch (type)
            {

                case 1:
                    {
                        switch (Read_DCIni("Ani", "2"))
                        {
                            case "2":
                                {
                                    var sb = HAnimation.AddDoubleAnimaton(1, 250, bgimage, "Opacity", null, 0);
                                    sb.Completed += (e, args) =>
                                    {
                                        bgimage.Opacity = 1;
                                    };
                                    if (bgimage.Visibility != Visibility.Collapsed)
                                    {
                                        var sb2 = HAnimation.AddDoubleAnimaton(0, 250, bgimage, "Opacity", null);
                                        sb2.Completed += (e, args) =>
                                        {
                                            bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                                            bgimage.Opacity = 0;
                                            bgimage.Margin = new Thickness(130, 0, 0, 0);
                                            sb.Begin();
                                        };
                                        sb2.Begin();
                                    }
                                    else
                                    {
                                        bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                                        bgimage.Margin = new Thickness(130, 0, 0, 0);
                                        bgimage.Opacity = 0;
                                        bgimage.Visibility = Visibility.Visible;
                                        sb.Begin();
                                    }
                                    break;
                                }
                            case "1":
                                {
                                    var sb = HAnimation.AddDoubleAnimaton(1, 250, bgimage, "Opacity", null, 0);
                                    sb = HAnimation.AddThicknessAnimaton(new Thickness(130, 0, 0, 0), 250, bgimage, "Margin", sb);
                                    sb.Completed += (e, args) =>
                                    {
                                        bgimage.Opacity = 1;
                                        bgimage.Margin = new Thickness(130, 0, 0, 0);
                                    };
                                    if (bgimage.Visibility != Visibility.Collapsed)
                                    {
                                        var sb2 = HAnimation.AddDoubleAnimaton(0, 250, bgimage, "Opacity", null);
                                        sb2 = HAnimation.AddThicknessAnimaton(new Thickness(240, 0, 0, 0), 250, bgimage, "Margin", sb2);
                                        sb2.Completed += (e, args) =>
                                        {
                                            bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                                            bgimage.Opacity = 0;
                                            bgimage.Margin = new Thickness(360, 0, 0, 0);
                                            sb.Begin();
                                        };
                                        sb2.Begin();
                                    }
                                    else
                                    {
                                        bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                                        bgimage.Opacity = 0;
                                        bgimage.Visibility = Visibility.Visible;
                                        sb.Begin();
                                    }
                                    break;
                                }
                            default:
                                {
                                    bgimage.Visibility = Visibility.Visible;
                                    bgimage.Margin = new Thickness(130, 0, 0, 0);
                                    bgimage.Background = new SolidColorBrush(App.AppAccentColor);
                                    break;
                                }
                        }
                        break;
                    }
                case 2:
                    {
                        switch (Read_DCIni("Ani", "2"))
                        {
                            case "2":
                                {
                                    var sb = HAnimation.AddDoubleAnimaton(1, 250, bgimage, "Opacity", null, 0);
                                    sb.Completed += (e, args) =>
                                    {
                                        bgimage.Opacity = 1;
                                    };
                                    if (bgimage.Visibility != Visibility.Collapsed)
                                    {
                                        var sb2 = HAnimation.AddDoubleAnimaton(0, 250, bgimage, "Opacity", null);
                                        sb2.Completed += (e, args) =>
                                        {
                                            bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this);
                                            bgimage.Opacity = 0;
                                            bgimage.Margin = new Thickness(130, 0, 0, 0);
                                            sb.Begin();
                                        };
                                        sb2.Begin();
                                    }
                                    else
                                    {
                                        bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this);
                                        bgimage.Margin = new Thickness(130, 0, 0, 0);
                                        bgimage.Opacity = 0;
                                        bgimage.Visibility = Visibility.Visible;
                                        sb.Begin();
                                    }
                                    break;
                                }
                            case "1":
                                {
                                    var sb = HAnimation.AddDoubleAnimaton(1, 250, bgimage, "Opacity", null, 0);
                                    sb = HAnimation.AddThicknessAnimaton(new Thickness(130, 0, 0, 0), 250, bgimage, "Margin", sb);
                                    sb.Completed += (e, args) =>
                                    {
                                        bgimage.Opacity = 1;
                                        bgimage.Margin = new Thickness(130, 0, 0, 0);
                                    };
                                    if (bgimage.Visibility != Visibility.Collapsed)
                                    {
                                        var sb2 = HAnimation.AddDoubleAnimaton(0, 250, bgimage, "Opacity", null);
                                        sb2 = HAnimation.AddThicknessAnimaton(new Thickness(240, 0, 0, 0), 250, bgimage, "Margin", sb2);
                                        sb2.Completed += (e, args) =>
                                        {
                                            bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this);
                                            bgimage.Opacity = 0;
                                            bgimage.Margin = new Thickness(360, 0, 0, 0);
                                            sb.Begin();
                                        };
                                        sb2.Begin();
                                    }
                                    else
                                    {
                                        bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this);
                                        bgimage.Opacity = 0;
                                        bgimage.Visibility = Visibility.Visible;
                                        sb.Begin();
                                    }
                                    break;
                                }
                            default:
                                {
                                    bgimage.Visibility = Visibility.Visible;
                                    bgimage.Margin = new Thickness(130, 0, 0, 0);
                                    bgimage.Background = HUI.Set_Bgimage(bgimage.Background, this);
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        switch (Read_DCIni("Ani", "2"))
                        {
                            case "2":
                                {
                                    if (bgimage.Visibility != Visibility.Collapsed)
                                    {
                                        var sb2 = HAnimation.AddDoubleAnimaton(0, 250, bgimage, "Opacity", null);
                                        sb2.Completed += (e, args) =>
                                        {
                                            bgimage.Opacity = 1;
                                            bgimage.Visibility = Visibility.Collapsed;
                                            bgimage.Margin = new Thickness(0, 0, 0, 0);
                                        };
                                        sb2.Begin();
                                    }
                                    else
                                        bgimage.Margin = new Thickness(0, 0, 0, 0);
                                    break;
                                }
                            case "1":
                                {
                                    if (bgimage.Visibility != Visibility.Collapsed)
                                    {
                                        var sb2 = HAnimation.AddDoubleAnimaton(0, 250, bgimage, "Opacity", null);
                                        sb2 = HAnimation.AddThicknessAnimaton(new Thickness(240, 0, 0, 0), 250, bgimage, "Margin", sb2);
                                        sb2.Completed += (e, args) =>
                                        {
                                            bgimage.Opacity = 1;
                                            bgimage.Visibility = Visibility.Collapsed;
                                            bgimage.Margin = new Thickness(0, 0, 0, 0);
                                        };
                                        sb2.Begin();
                                    }
                                    else
                                        bgimage.Margin = new Thickness(0, 0, 0, 0);
                                    break;
                                }
                            default:
                                {
                                    bgimage.Opacity = 1;
                                    bgimage.Visibility = Visibility.Collapsed;
                                    bgimage.Margin = new Thickness(0, 0, 0, 0);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        public void Load_Colors()
        {
            switch (Read_DCIni("Background", "1"))
            {
                case "1":
                    Blurbgi(0);
                    break;
                case "2":
                    Blurbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")));
                    break;
                case "3":
                    {
                        compositor ??= new(this);
                        HUI.Set_Acrylic(bgimage, this, windowChrome, compositor);
                        Set_AcrylicStyle();
                        break;
                    }
            };
            Foreground = new SolidColorBrush(App.AppForeColor);
            #region 颜色
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppForeColor"] = App.AppForeColor;
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
            Resources["InfoAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 200));
            infoPoly.Stroke = new SolidColorBrush(App.AppForeColor);
            #endregion
            minbtn.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 0));
            if (App.wnd != null)
            {
                App.wnd.trayBorder.BorderBrush = new SolidColorBrush(App.AppAccentColor);
                App.wnd.trayBorder.Background = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 150));
                App.wnd.trayText.Foreground = new SolidColorBrush(App.AppForeColor);
            }
            Load_Labels(false);
            hiro_home?.Load_Color();
            hiro_items?.Load_Color();
            hiro_schedule?.Load_Color();
            hiro_config?.Load_Color();
            hiro_profile?.Load_Color();
            hiro_about?.Load_Color();
            hiro_newitem?.Load_Color();
            hiro_newschedule?.Load_Color();
            hiro_time?.Load_Color();
            hiro_color?.Load_Color();
            hiro_chat?.Load_Color();
            hiro_login?.Load_Color();
            hiro_acrylic?.Load_Color();
        }

        public static void Load_Data()
        {
            App.cmditems.Clear();
            var i = 1;
            var p = 1;
            var inipath = App.dConfig;
            var ti = Read_Ini(inipath, i.ToString(), "Title", "");
            var co = Read_Ini(inipath, i.ToString(), "Command", "");
            bool reged = HHotKeys.hotkeys.Count > 0;
            while (!ti.Trim().Equals("") && co.StartsWith("(") && co.EndsWith(")"))
            {
                var key = Read_Ini(App.dConfig, i.ToString(), "HotKey", "").Trim();
                try
                {
                    if (!reged && key.IndexOf(",") != -1)
                    {
                        var mo = uint.Parse(key[..key.IndexOf(",")]);
                        var vkey = uint.Parse(key.Substring(key.IndexOf(",") + 1, key.Length - key.IndexOf(",") - 1));
                        try
                        {
                            if (mo != 0 && vkey != 0)
                            {
                                HHotKeys.RegisterKey(mo, (Key)vkey, i - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError(ex, $"Hiro.Exception.Hotkey.Register{Environment.NewLine}Key: {mo} + {key}");
                        }

                    }

                }
                catch (Exception ex)
                {
                    LogError(ex, "Hiro.Exception.Hotkey.Register.Format");
                }
                co = co[1..^1];
                App.cmditems.Add(new HClass.Cmditem(p, i, ti, co, key));
                i++;
                p = (i % 10 == 0) ? i / 10 : i / 10 + 1;
                ti = Read_Ini(inipath, i.ToString(), "Title", "");
                co = Read_Ini(inipath, i.ToString(), "Command", "");
            }

            App.scheduleitems.Clear();
            i = 1;
            inipath = App.sConfig;
            ti = Read_Ini(inipath, i.ToString(), "Time", "");
            co = Read_Ini(inipath, i.ToString(), "Command", "");
            var na = Read_Ini(inipath, i.ToString(), "Name", "");
            var re = Read_Ini(inipath, i.ToString(), "Repeat", "-2.0");
            if (co.Length >= 2)
                co = co[1..^1];
            while (!ti.Equals("") && !co.Equals("") && !na.Equals("") && !re.Equals(""))
            {
                System.Globalization.DateTimeFormatInfo dtFormat = new()
                {
                    ShortDatePattern = "yyyy/MM/dd HH:mm:ss"
                };
                switch (double.Parse(re))
                {
                    case -1.0:
                        {
                            DateTime dt = Convert.ToDateTime(ti, dtFormat);
                            DateTime now = DateTime.Now;
                            TimeSpan ts = dt - now;
                            while (ts.TotalMinutes < 0)
                            {
                                dt = dt.AddDays(1.0);
                                ts = dt - now;
                            }
                            ti = dt.ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(inipath, i.ToString(), "Time", ti);
                            break;
                        }
                    case 0.0:
                        {
                            DateTime dt = Convert.ToDateTime(ti, dtFormat);
                            DateTime now = DateTime.Now;
                            TimeSpan ts = dt - now;
                            while (ts.TotalMinutes < 0)
                            {
                                dt = dt.AddDays(7.0);
                                ts = dt - now;
                            }
                            ti = dt.ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(inipath, i.ToString(), "Time", ti);
                            break;
                        }
                    case -2.0:
                        break;
                    default:
                        {
                            DateTime dt = Convert.ToDateTime(ti, dtFormat);
                            DateTime now = DateTime.Now;
                            TimeSpan ts = dt - now;
                            while (ts.TotalMinutes < 0)
                            {
                                dt = dt.AddDays(double.Parse(re));
                                ts = dt - now;
                            }
                            ti = dt.ToString("yyyy/MM/dd HH:mm:ss");
                            Write_Ini(inipath, i.ToString(), "Time", ti);
                            break;
                        }
                }
                App.scheduleitems.Add(new HClass.Scheduleitem(i, na, ti, co, double.Parse(re)));
                i++;
                ti = Read_Ini(inipath, i.ToString(), "Time", "");
                co = Read_Ini(inipath, i.ToString(), "Command", "");
                na = Read_Ini(inipath, i.ToString(), "Name", "");
                re = Read_Ini(inipath, i.ToString(), "Repeat", "-2.0");
                if (co.Length >= 2)
                    co = co[1..^1];
            }
            App.Load_Menu();
        }

        public void Load_Translate()
        {
            Title = HText.Get_Translate("mainTitle").Replace("%a", App.appTitle).Replace("%v", HText.Get_Translate("version").Replace("%c", Hiro_Resources.ApplicationVersion));
            titlelabel.Content = App.appTitle;
            infotitle.Content = HText.Get_Translate("infotitle");
            minbtn.ToolTip = HText.Get_Translate("min");
            closebtn.ToolTip = HText.Get_Translate("close");
            infoPoly.ToolTip = HText.Get_Translate("info");
            homex.Content = HText.Get_Translate("home");
            itemx.Content = HText.Get_Translate("item");
            schedulex.Content = HText.Get_Translate("schedule");
            configx.Content = HText.Get_Translate("config");
            profilex.Content = HText.Get_Translate("profile");
            aboutx.Content = HText.Get_Translate("about");
            newx.Content = HText.Get_Translate("new");
            colorx.Content = HText.Get_Translate("color");
            timex.Content = HText.Get_Translate("time");
            proxyx.Content = HText.Get_Translate("proxy");
            chatx.Content = HText.Get_Translate("chat");
            loginx.Content = HText.Get_Translate("login");
            acrylicx.Content = HText.Get_Translate("acrylic");
            dropIci.Content = HText.Get_Translate("dropInfo");
            hiro_home?.Update_Labels();
            hiro_home?.Load_Position();
            hiro_items?.Load_Translate();
            hiro_items?.Load_Position();
            hiro_schedule?.Load_Translate();
            hiro_schedule?.Load_Position();
            hiro_config?.Load_Translate();
            hiro_config?.Load_Position();
            hiro_profile?.Load_Translate();
            hiro_profile?.Load_Position();
            hiro_about?.Load_Translate();
            hiro_about?.Load_Position();
            hiro_newitem?.Load_Translate();
            hiro_newitem?.Load_Position();
            hiro_newschedule?.Load_Translate();
            hiro_newschedule?.Load_Position();
            hiro_time?.Load_Translate();
            hiro_time?.Load_Position();
            hiro_color?.Load_Translate();
            hiro_color?.Load_Position();
            hiro_proxy?.Load_Translate();
            hiro_proxy?.Load_Position();
            hiro_chat?.Load_Translate();
            hiro_chat?.Load_Position();
            hiro_login?.Load_Translate();
            hiro_login?.Load_Position();
            hiro_acrylic?.Load_Translate();
            hiro_acrylic?.Load_Position();
        }

        public void Load_Labels(bool reload = true)
        {
            if (reload)
            {
                homex.Background = new SolidColorBrush(Colors.Transparent);
                itemx.Background = new SolidColorBrush(Colors.Transparent);
                schedulex.Background = new SolidColorBrush(Colors.Transparent);
                configx.Background = new SolidColorBrush(Colors.Transparent);
                profilex.Background = new SolidColorBrush(Colors.Transparent);
                aboutx.Background = new SolidColorBrush(Colors.Transparent);
                newx.Background = new SolidColorBrush(Colors.Transparent);
                colorx.Background = new SolidColorBrush(Colors.Transparent);
                timex.Background = new SolidColorBrush(Colors.Transparent);
                proxyx.Background = new SolidColorBrush(Colors.Transparent);
                chatx.Background = new SolidColorBrush(Colors.Transparent);
                loginx.Background = new SolidColorBrush(Colors.Transparent);
                acrylicx.Background = new SolidColorBrush(Colors.Transparent);
                homex.IsEnabled = true;
                itemx.IsEnabled = true;
                schedulex.IsEnabled = true;
                configx.IsEnabled = true;
                profilex.IsEnabled = true;
                aboutx.IsEnabled = true;
                newx.IsEnabled = true;
                colorx.IsEnabled = true;
                timex.IsEnabled = true;
                proxyx.IsEnabled = true;
                chatx.IsEnabled = true;
                loginx.IsEnabled = true;
                acrylicx.IsEnabled = true;

            }
            homex.Foreground = Foreground;
            itemx.Foreground = Foreground;
            schedulex.Foreground = Foreground;
            configx.Foreground = Foreground;
            profilex.Foreground = Foreground;
            aboutx.Foreground = Foreground;
            newx.Foreground = Foreground;
            colorx.Foreground = Foreground;
            timex.Foreground = Foreground;
            proxyx.Foreground = Foreground;
            chatx.Foreground = Foreground;
            loginx.Foreground = Foreground;
            acrylicx.Foreground = Foreground;
        }
        #endregion

        private void Titlelabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Bglabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Closebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (infocenter.Visibility == Visibility.Visible)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    var sb = HAnimation.AddPowerOutAnimation(0, infocenter, null, null, -100, 300, 200);
                    sb.Completed += delegate
                    {
                        if (BackVideo.Visibility == Visibility.Visible && BackVideo.Visibility == Visibility.Visible)
                            BackVideo.Play();
                        infocenter.Visibility = Visibility.Hidden;
                        if (Read_DCIni("Ani", "2").Equals("1"))
                        {
                            infocenter.IsEnabled = false;
                            Storyboard? sbe = new();
                            HAnimation.AddPowerOutAnimation(0, infoPoly, sbe, null, -50, 150, 100);
                            sbe.Completed += delegate
                            {
                                infoPoly.Visibility = Visibility.Collapsed;
                            };
                            sbe.Begin();
                        }
                        else
                        {
                            infoPoly.Visibility = Visibility.Hidden;
                        }
                        HUI.Set_FrameworkElement_Location(TitleGrid, "tigrid", Read_DCIni("Ani", "2").Equals("1"));
                        sb = null;
                    };
                    sb.Begin();
                }
                else
                {
                    infocenter.Visibility = Visibility.Collapsed;
                    infoPoly.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                Hiro_Utils.RunExe(Read_DCIni("Min", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase)
                    ? "hide()"
                    : "exit()");
            }
        }

        private void Homex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Set_Label(homex);
        }

        public void Set_Home_Labels(string val)
        {
            val = (App.CustomUsernameFlag == 0) ? HText.Get_Translate(val).Replace("%u", App.eUserName) : HText.Get_Translate(val + "cus").Replace("%u", App.username);
            if (current is not Hiro_Home hh)
                return;
            if (!hh.Hello.Text.Equals(val))
            {
                hh.Hello.Text = val;
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    var sb = new Storyboard();
                    HAnimation.AddPowerAnimation(0, hh.Hello, sb);
                    sb.Begin();
                }
            }
            val = HText.Path_PPX(HText.Get_Translate("copyright"));
            if (!hh.Copyright.Text.Equals(val))
                hh.Copyright.Text = val;
        }

        public void Set_Label(Label label)
        {
            var animation = !Read_DCIni("Ani", "2").Equals("0");
            if (current == hiro_acrylic)
                Blurbgi(0);
            Load_Labels();
            if (App.Logined != true && (label == profilex || label == chatx))
                label = loginx;
            label.IsEnabled = false;
            if (label != newx && label != timex)
            {
                newx.Visibility = Visibility.Hidden;
            }
            if (label != timex)
            {
                timex.Visibility = Visibility.Hidden;
            }
            if (label != colorx)
            {
                colorx.Visibility = Visibility.Hidden;
            }
            if (label != proxyx)
            {
                proxyx.Visibility = Visibility.Hidden;
            }
            if (label != loginx)
            {
                loginx.Visibility = Visibility.Hidden;
            }
            if (label != acrylicx)
            {
                acrylicx.Visibility = Visibility.Hidden;
            }
            var duration = Math.Abs(label.Margin.Top - bgx.Margin.Top);
            if (!Read_DCIni("Ani", "2").Equals("0"))
            {
                duration = duration > label.Height * 2 ? 2 * duration : 6 * duration;
                Storyboard? sb = new();
                sb = HAnimation.AddThicknessAnimaton(label.Margin, duration, bgx, "Margin", sb);
                sb.Completed += delegate
                {
                    bgx.Margin = new Thickness(label.Margin.Left, label.Margin.Top, 0, 0);
                };
                sb.Begin();
            }
            else
            {
                bgx.Margin = new Thickness(label.Margin.Left, label.Margin.Top, 0, 0);
            }
            if (selected == label)
            {
                if (animation && current != null)
                {
                    Storyboard sb = new();
                    HAnimation.AddPowerAnimation(0, current, sb, 50, null);
                    switch (current)
                    {
                        case Hiro_Items his:
                            his.HiHiro();
                            break;
                        case Hiro_Schedule hs:
                            hs.HiHiro();
                            break;
                        case Hiro_Config hc:
                            hc.HiHiro();
                            break;
                        case Hiro_Profile hp:
                            hp.HiHiro();
                            break;
                        case Hiro_NewItem hni:
                            hni.HiHiro();
                            break;
                        case Hiro_NewSchedule hns:
                            hns.HiHiro();
                            break;
                        case Hiro_Time ht:
                            ht.HiHiro();
                            break;
                        case Hiro_Color hcr:
                            hcr.HiHiro();
                            break;
                        case Hiro_Proxy hpy:
                            hpy.HiHiro();
                            break;
                        case Hiro_Chat hct:
                            hct.HiHiro();
                            break;
                        case Hiro_Login hlo:
                            hlo.HiHiro();
                            break;
                        case Hiro_Acrylic hac:
                            hac.HiHiro();
                            break;
                    }

                    sb.Begin();
                }
                label.IsEnabled = true;
                return;
            }
            else
            {
                var backg = HText.Path_PPX(Read_Ini(App.dConfig, "Background", label.Name, ""));
                if (!System.IO.File.Exists(backg))
                {
                    backg = Read_PPDCIni("BackImage", "");
                }
                if (backg != currentBack && !Read_DCIni("Background", "1").Equals("3"))
                {
                    HUI.Set_Bgimage(bgimage, this, backg);
                    currentBack = backg;
                    if (Read_DCIni("Background", "1").Equals("2"))
                        Blurbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
                }
            }
            if (label == homex)
            {
                hiro_home ??= new();
                current = hiro_home;
                frame.Content = current;
            }
            if (label == itemx || label == schedulex || label == profilex || label == configx || label == chatx)
            {
                if (App.Locked)
                {
                    Action sc = new(() =>
                    {
                        App.Locked = false;
                        if (App.mn != null)
                            App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion;
                        App.mn?.Set_Label(label);
                    });
                    Action fa = new(() =>
                    {
                        if (App.mn == null)
                            return;
                        App.mn.versionlabel.Content = Hiro_Resources.ApplicationVersion + (App.Locked ? " 🔒" : "");
                        App.mn.Set_Label(selected ?? homex);
                    });
                    Hiro_Utils.Register(sc, fa, fa);
                    return;
                }
                if (label == itemx)
                {
                    hiro_items ??= new(this);
                    current = hiro_items;
                    frame.Content = current;
                }
                if (label == schedulex)
                {
                    hiro_schedule ??= new(this);
                    current = hiro_schedule;
                    frame.Content = current;
                }
                if (label == profilex)
                {
                    hiro_profile ??= new(this);
                    current = hiro_profile;
                    frame.Content = current;
                }
                if (label == configx)
                {
                    hiro_config ??= new(this);
                    current = hiro_config;
                    frame.Content = current;
                }
                if (label == chatx)
                {
                    hiro_chat ??= new(this);
                    current = hiro_chat;
                    frame.Content = hiro_chat;
                }
            }
            if (label == aboutx)
            {
                hiro_about ??= new(this);
                current = hiro_about;
                frame.Content = current;
            }
            if (label == newx)
            {
                newx.Visibility = Visibility.Visible;
                if (current is Hiro_Time ht)
                {
                    ht.GoBack();
                }
                else if (current != null)
                {
                    frame.Content = current;
                }
            }
            if (label == timex)
            {
                newx.Visibility = Visibility.Visible;
                timex.Visibility = Visibility.Visible;
                if (hiro_time != null)
                {
                    current = hiro_time;
                    frame.Content = current;
                }
            }
            if (label == colorx)
            {
                colorx.Visibility = Visibility.Visible;
                hiro_color ??= new(this);
                //hiro_color.color_picker.Color = App.AppAccentColor;
                hiro_color.Unify_Color(true);
                current = hiro_color;
                frame.Content = hiro_color;
            }
            if (label == proxyx)
            {
                proxyx.Visibility = Visibility.Visible;
                hiro_proxy ??= new(this);
                current = hiro_proxy;
                frame.Content = hiro_proxy;
            }
            if (label == loginx)
            {
                hiro_login ??= new(this);
                current = hiro_login;
                frame.Content = hiro_login;
                loginx.Visibility = Visibility.Visible;
            }
            if (label == acrylicx)
            {
                hiro_acrylic ??= new(this);
                current = hiro_acrylic;
                frame.Content = hiro_acrylic;
                acrylicx.Visibility = Visibility.Visible;
            }
            selected = label;
            label.IsEnabled = true;
        }

        private void Timex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(timex);
        }

        private void Itemx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(itemx);
        }

        private void Configx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(configx);
        }

        private void Profilex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(profilex);
        }

        private void Aboutx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(aboutx);
        }

        private void Newx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(newx);
        }

        private void Colorx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(colorx);
        }

        private void Minbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Ui_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Read_DCIni("Min", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                Visibility = Visibility.Hidden;
                if (BackVideo.Visibility == Visibility.Visible)
                    BackVideo.Pause();
            }
            else
            {
                Hiro_Utils.RunExe("exit()");
                LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
            }
            e.Cancel = true;
        }

        private void Titlelabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var thickness = versionlabel.Margin;
            thickness.Left = titlelabel.Margin.Left + titlelabel.ActualWidth + 2;
            versionlabel.Margin = thickness;
        }

        private void Schedulex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(schedulex);
        }

        internal void Blurbgi(int direction, bool all = true)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            if (all)
            {
                foreach (Window win in Application.Current.Windows)
                {
                    switch (win)
                    {
                        case Hiro_Alarm a:
                            a.Loadbgi(direction);
                            break;
                        case Hiro_Msg e:
                            e.Loadbgi(direction);
                            break;
                        case Hiro_Sequence c:
                            c.Loadbgi(direction);
                            break;
                        case Hiro_Download d:
                            d.Loadbgi(direction);
                            break;
                        case Hiro_Web f:
                            f.Loadbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
                            break;
                        case Hiro_Finder g:
                            g.Loadbgi(direction);
                            break;
                        case Hiro_Player h:
                            h.Loadbgi(direction);
                            break;
                        case Hiro_Ticker i:
                            i.Loadbgi(direction);
                            break;
                        case Hiro_Splash j:
                            j.Loadbgi(direction);
                            break;
                        case Hiro_Encrypter k:
                            k.Loadbgi(direction);
                            break;
                        case Hiro_ImageViewer l:
                            l.Loadbgi(direction, false);
                            break;
                        case Hiro_TextEditor m:
                            m.Loadbgi(direction, false);
                            break;
                    }

                    System.Windows.Forms.Application.DoEvents();
                }
            }
            bool animation = !Read_DCIni("Ani", "2").Equals("0");
            System.ComponentModel.BackgroundWorker bw = new();
            bw.RunWorkerCompleted += delegate
            {
                SetHCStatus();
            };
            if (Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                HUI.Set_Acrylic(bgimage, this, windowChrome, compositor);
                Set_AcrylicStyle();
                SetHCStatus();
                StopVideo();
            }
            else
            {
                bgimage.Margin = new Thickness(0);
                bgimage.Visibility = Visibility.Visible;
                HUI.Set_Bgimage(bgimage, this);
                currentBack = Read_PPDCIni("BackImage", "");
                if (compositor != null)
                {
                    compositor.IsEnabled = false;
                }
                if (Read_DCIni("Background", "1").Equals("2"))
                {

                    HAnimation.Blur_Animation(direction, animation, bgimage, this, bw);
                    StopVideo();
                }
                else if (Read_DCIni("Background", "1").Equals("4"))
                    Load_Video();
                else
                {
                    StopVideo();
                    SetHCStatus();
                }
            }
            bflag = 0;
        }

        private void StopVideo()
        {
            if (BackVideo.Visibility == Visibility.Visible)
            {
                BackVideo.Visibility = Visibility.Collapsed;
                if (BackVideo.IsPlaying || BackVideo.IsPaused)
                {

                    BackVideo.Stop();
                    BackVideo.Close();
                }
            }
        }

        private void SetHCStatus()
        {
            if (current is not Hiro_Config hc)
                return;
            hc.blureff.IsEnabled = false;
            hc.rbtn14.IsEnabled = true;
            hc.rbtn15.IsEnabled = true;
            hc.btn10.IsEnabled = true;
            hc.acrylic_btn.IsEnabled = true;
            hc.video_btn.IsEnabled = true;
            if (hc.rbtn15.IsChecked == true)
                hc.blureff.IsEnabled = true;
        }

        internal void OpacityBgi()
        {
            if (Read_DCIni("Background", "1").Equals("3"))
            {
                Background = null;
                return;
            }
            HUI.Set_Opacity(bgimage, this);
            foreach (Window win in Application.Current.Windows)
            {
                switch (win)
                {
                    case Hiro_Alarm a:
                        HUI.Set_Opacity(a.bgimage, a);
                        break;
                    case Hiro_Msg e:
                        HUI.Set_Opacity(e.bgimage, e);
                        break;
                    case Hiro_Sequence c:
                        HUI.Set_Opacity(c.bgimage, c);
                        break;
                    case Hiro_Download d:
                        HUI.Set_Opacity(d.bgimage, d);
                        break;
                    case Hiro_Web f:
                        HUI.Set_Opacity(f.bgimage, f);
                        break;
                    case Hiro_Finder g:
                        HUI.Set_Opacity(g.bgimage, g);
                        break;
                    case Hiro_Player h:
                        HUI.Set_Opacity(h.bgimage, h);
                        break;
                    case Hiro_Ticker i:
                        HUI.Set_Opacity(i.bgimage, i);
                        break;
                    case Hiro_Encrypter k:
                        HUI.Set_Opacity(k.bgimage, k);
                        break;
                    case Hiro_ImageViewer l:
                        HUI.Set_Opacity(l.bgimage, l);
                        break;
                    case Hiro_TextEditor m:
                        HUI.Set_Opacity(m.bgimage, m);
                        break;
                }
                if (hiro_profile != null)
                    HUI.Set_Opacity(hiro_profile.Profile_Background, hiro_profile.BackControl);
                if (hiro_chat != null)
                    HUI.Set_Opacity(hiro_chat.Profile_Background, hiro_chat.BackControl);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void Schedulex_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!App.dflag)
                return;
            DateTime dt = DateTime.Now.AddSeconds(5);
            for (int i = 0; i < 1; i++)
            {
                dt = dt.AddSeconds(2);
                App.scheduleitems.Add(new HClass.Scheduleitem(App.scheduleitems.Count + 1, "Test" + i.ToString(), dt.ToString("yyyy/MM/dd HH:mm:ss"), "alarm", -1.0));

            }

        }

        internal void Hiro_We_Info()
        {
            _msgCount = 0;
            Dispatcher.Invoke(() =>
            {
                if (App.tb != null && App.tb.MusicControlGrid.Visibility != Visibility.Visible)
                {
                    App.tb.MsgLabel.Content = "📧 0";
                    App.tb.RemoveGrid(App.tb.MsgGrid);
                }
            });
            infocenter.Visibility = Visibility.Visible;
            if (BackVideo.Visibility == Visibility.Visible)
                BackVideo.Pause();
            infocenter.IsEnabled = true;
            if (!Read_DCIni("Ani", "2").Equals("0"))
            {
                Storyboard? sb = HAnimation.AddPowerAnimation(0, infocenter, null, -100, null, 300, 200);
                sb.Completed += delegate
                {
                    sb = null;
                };
                sb.Begin();
            }
        }
        private void Versionlabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.dflag)
            {
                Load_Translate();
                Load_Position();
            }
            else
            {
                Hiro_Utils.RunExe(App.Locked ? "auth()" : "lock()");
            }
        }

        private void Ui_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else if (WindowState != WindowState.Minimized)
            {
                if (Read_DCIni("Background", "1").Equals("2"))
                    Blurbgi(Hiro_Utils.ConvertInt(Read_DCIni("Blur", "0")), false);
                if (BackVideo.IsPaused && BackVideo.Visibility == Visibility.Visible)
                    BackVideo.Play();
            }
            else
            {
                if (BackVideo.Visibility == Visibility.Visible)
                    BackVideo.Pause();
            }
        }

        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!frame.CanGoBack)
                return;
            var rb = frame.RemoveBackEntry();
            while (rb != null)
            {
                rb = frame.RemoveBackEntry();
            }
        }

        private void Proxyx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(proxyx);
        }

        private void Chatx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(chatx);
        }

        private void Infotitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Versionlabel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Loginx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(loginx);
        }

        private void Acrylicx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Set_Label(acrylicx);
        }

        private void BackVideo_MediaOpened(object sender, Unosquare.FFME.Common.MediaOpenedEventArgs e)
        {

            if (BackVideo.MediaInfo.Streams.Count > 0 && Read_DCIni("BackVideoCrop", "true").Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                var w = BackVideo.MediaInfo.Streams[0].PixelWidth;
                var h = BackVideo.MediaInfo.Streams[0].PixelHeight;
                var ww = Width;
                var hh = Height;
                var wi = ww / w;
                var hi = hh / h;
                var www = hh * w / h;
                var hhh = ww * h / w;
                if (wi < hi)
                {
                    BackVideo.Width = www;
                    BackVideo.Height = hh;
                }
                else
                {
                    BackVideo.Width = ww;
                    BackVideo.Height = hhh;
                }
            }
        }

        private void ui_Activated(object sender, EventArgs e)
        {
            if (BackVideo.IsPaused && BackVideo.Visibility == Visibility.Visible)
                BackVideo.Play();
        }

        private void ui_Deactivated(object sender, EventArgs e)
        {
            if (BackVideo.Visibility == Visibility.Visible)
                BackVideo.Pause();
        }

        private void InfoPolyFake_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_We_Info();
        }

        private void BaseGrid_Drop(object sender, DragEventArgs e)
        {
            HideDropInfo();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths.Length > 0)
                {
                    Hiro_Utils.OpenInNewHiro($"\"base({Convert.ToBase64String(Encoding.Default.GetBytes(filePaths[0]))})\"", false);
                    e.Handled = true;
                }
            }
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var f = (string)e.Data.GetData(DataFormats.Text);
                Hiro_Utils.OpenInNewHiro($"\"base({Convert.ToBase64String(Encoding.Default.GetBytes(f))})\"", false);
                e.Handled = true;
            }
        }

        private void BaseGrid_PreviewDragEnter(object sender, DragEventArgs e)
        {
            _dragFlag = true;
            if (DropInfo.Visibility != Visibility.Visible)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    DropInfo.Visibility = Visibility.Visible;
                    var s = HAnimation.AddPowerAnimation(0, DropInfo, null, -50, null, 250, 150);
                    s.Begin();
                }
                else
                {
                    DropInfo.Visibility = Visibility.Visible;
                }
            }
            e.Handled = true;
        }

        internal void HideDropInfo()
        {
            if (DropInfo.Visibility == Visibility.Visible)
            {
                if (!Read_DCIni("Ani", "2").Equals("0"))
                {
                    var s = HAnimation.AddPowerOutAnimation(0, DropInfo, null, null, -50, 250, 150);
                    s.Completed += delegate
                    {
                        DropInfo.Visibility = Visibility.Collapsed;
                    };
                    s.Begin();
                }
                else
                {
                    dropIci.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
