﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Hiro.Helpers;
using Hiro.ModelViews;

namespace Hiro
{
    /// <summary>
    /// message.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Msg : Window
    {
        internal Hiro_Background? bg = null;
        internal string? toolstr = null;
        internal int aflag = -1;
        internal int bflag = 0;
        public event EventHandler<EventArgs>? OKButtonPressed;
        public event EventHandler<EventArgs>? RejectButtonPressed;
        public event EventHandler<EventArgs>? CancelButtonPressed;
        internal WindowAccentCompositor? compositor = null;
        public Hiro_Msg(string? config = null)
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
            toolstr = config;
            if (toolstr != null)
            {
                string sndPath = HText.Path_PPX(HSet.Read_Ini(toolstr, "Message", "Music", ""));
                if (System.IO.File.Exists(sndPath))
                    try
                    {
                        System.Media.SoundPlayer sndPlayer = new(sndPath);
                        //循环播放
                        // sndPlayer.PlayLooping();
                        //播放一次
                        sndPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Message.Sound");
                    }
                OKButtonPressed += delegate
                {
                    Hiro_Utils.RunExe(HSet.Read_Ini(toolstr, "Action", "accept", "nop"));
                };
                RejectButtonPressed += delegate
                {
                    Hiro_Utils.RunExe(HSet.Read_Ini(toolstr, "Action", "reject", "nop"));
                };
                CancelButtonPressed += delegate
                {
                    Hiro_Utils.RunExe(HSet.Read_Ini(toolstr, "Action", "cancel", "nop"));
                };
            }
            SourceInitialized += OnSourceInitialized;
            if (!HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                acceptbtn.Visibility = Visibility.Visible;
                rejectbtn.Visibility = Visibility.Visible;
                cancelbtn.Visibility = Visibility.Visible;
                backtitle.Visibility = Visibility.Visible;
                backcontent.Visibility = Visibility.Visible;
            };
            Loaded += delegate
            {
                Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
                if (!App.dflag) 
                    return;
                HLogger.LogtoFile("[MESSAGE]Title: " + backtitle.Content);
                HLogger.LogtoFile("[MESSAGE]Content: " + backcontent.Content);
            };
            ContentRendered += delegate
            {
                Hiro_Utils.Delay(1);
                HiHiro();
            };
            Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }

        public void HiHiro()
        {
            if (!HSet.Read_DCIni("Ani", "2").Equals("1")) 
                return;
            acceptbtn.Visibility = Visibility.Visible;
            rejectbtn.Visibility = Visibility.Visible;
            cancelbtn.Visibility = Visibility.Visible;
            backtitle.Visibility = Visibility.Visible;
            backcontent.Visibility = Visibility.Visible;
            Storyboard sb = new();
            HAnimation.AddPowerAnimation(3, acceptbtn, sb, -50, null);
            HAnimation.AddPowerAnimation(3, rejectbtn, sb, -50, null);
            HAnimation.AddPowerAnimation(3, cancelbtn, sb, -50, null);
            HAnimation.AddPowerAnimation(0, backtitle, sb, 50, null);
            HAnimation.AddPowerAnimation(0, backcontent, sb, 50, null);
            sb.Begin();
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);
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

        public void Load_Position()
        {
            bg?.Show();
            Width = (toolstr != null && System.IO.File.Exists(toolstr) && !HSet.Read_Ini(toolstr, "location", "width", "-1").Equals("-1")) ? double.Parse(HSet.Read_Ini(toolstr, "location", "width", "-1")) : SystemParameters.PrimaryScreenWidth;
            Height = (toolstr != null && System.IO.File.Exists(toolstr) && !HSet.Read_Ini(toolstr, "location", "width", "-1").Equals("-1")) ? double.Parse(HSet.Read_Ini(toolstr, "location", "height", "-1")) : 200;
            if (toolstr != null && System.IO.File.Exists(toolstr))
            {
                HUI.Set_Control_Location(backtitle, "title", extra: true, path: toolstr);
                HUI.Set_Control_Location(cancelbtn, "cancel", extra: true, bottom: true, right: true, path: toolstr);
                HUI.Set_Control_Location(rejectbtn, "reject", extra: true, bottom: true, right: true, path: toolstr);
                HUI.Set_Control_Location(acceptbtn, "accept", extra: true, bottom: true, right: true, path: toolstr);
                backcontent.Height = Height - backtitle.Height - backtitle.Margin.Top * 2 - acceptbtn.Margin.Bottom * 2 - acceptbtn.Height;
                var le = backcontent.Width;
                HUI.Set_Control_Location(backcontent, "content", extra: true, path: toolstr);
                if (Math.Abs(le - backcontent.Width) < 0.001)
                    backcontent.Width = Width - backcontent.Margin.Left * 2;
                sv.FontFamily = backcontent.FontFamily;
                sv.FontSize = backcontent.FontSize;
                sv.Height = backcontent.Height - SystemParameters.HorizontalScrollBarHeight;
                sv.Width = backcontent.Width - SystemParameters.VerticalScrollBarWidth;
            }
            Canvas.SetLeft(this, SystemParameters.PrimaryScreenWidth / 2 - Width / 2);
            Canvas.SetTop(this, SystemParameters.PrimaryScreenHeight / 2 - Height / 2);
            Load_Colors();
        }

        public void Load_Colors()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }
        public void Loadbgi(int direction)
        {
            if (HSet.Read_DCIni("Background", "1").Equals("3"))
            {
                compositor ??= new(this);
                HUI.Set_Acrylic(bgimage, this, windowChrome, compositor);
                return;
            }
            if (compositor != null)
            {
                compositor.IsEnabled = false;
            }
            if (bflag == 1)
                return;
            bflag = 1;
            if (toolstr != null && System.IO.File.Exists(HText.Path_PPX(HSet.Read_Ini(toolstr, "Message", "Background", ""))))
                HUI.Set_Bgimage(bgimage, this, HText.Path_PPX(HSet.Read_Ini(toolstr, "Message", "Background", "")));
            else
                HUI.Set_Bgimage(bgimage, this);
            var animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            HAnimation.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                HLogger.LogtoFile("[MESSAGE]Accept Button Clicked");
            OnOKButtonPressed(new());
            bg?.Fade_Out();
            Close();
        }

        private void Rejectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                HLogger.LogtoFile("[MESSAGE]Reject Button Clicked");
            OnRejectButtonPressed(new());
            bg?.Fade_Out();
            Close();
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                HLogger.LogtoFile("[MESSAGE]Cancel Button Clicked");
            OnCancelButtonPressed(new());
            bg?.Fade_Out();
            Close();
        }

        private void Msg_Closing(object sender, CancelEventArgs e)
        {
            bg?.Fade_Out();
            if (toolstr != null && HSet.Read_Ini(toolstr, "Action", "Delete", "false").ToLower().Equals("true"))
            {
                System.IO.File.Delete(toolstr);
            }
        }
        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (bg == null)
            {
                Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            }
        }

        private void Msg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bg == null)
            {
                Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            }
        }

        protected virtual void OnOKButtonPressed(EventArgs e)
        {
            OKButtonPressed?.Invoke(this, e);
        }

        protected virtual void OnRejectButtonPressed(EventArgs e)
        {
            RejectButtonPressed?.Invoke(this, e);
        }
        
        protected virtual void OnCancelButtonPressed(EventArgs e)
        {
            CancelButtonPressed?.Invoke(this, e);
        }

    }
}
