﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// message.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Msg : Window
    {
        internal Hiro_Background? bg = null;
        internal String? toolstr = null;
        internal int aflag = -1;
        internal int bflag = 0;
        public Hiro_Msg(string? config = null)
        {
            InitializeComponent();
            toolstr = config;
            if (toolstr != null)
            {
                string sndPath = Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(toolstr, "Message", "Music", "")));
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
                    Hiro_Utils.LogtoFile("[ERROR]" + ex.Message);
                }
            }
            SourceInitialized += OnSourceInitialized;
            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                acceptbtn.Visibility = Visibility.Visible;
                rejectbtn.Visibility = Visibility.Visible;
                cancelbtn.Visibility = Visibility.Visible;
                backtitle.Visibility = Visibility.Visible;
                backcontent.Visibility = Visibility.Visible;
            };
            Loaded += delegate
            {
                Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
                if (App.dflag)
                {
                    Hiro_Utils.LogtoFile("[MESSAGE]Title: " + backtitle.Content);
                    Hiro_Utils.LogtoFile("[MESSAGE]Content: " + backcontent.Content);
                }
                Mouse.Capture(acceptbtn);
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
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                acceptbtn.Visibility = Visibility.Visible;
                rejectbtn.Visibility = Visibility.Visible;
                cancelbtn.Visibility = Visibility.Visible;
                backtitle.Visibility = Visibility.Visible;
                backcontent.Visibility = Visibility.Visible;
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(3, acceptbtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, rejectbtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, cancelbtn, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(0, backtitle, sb, 50, null);
                Hiro_Utils.AddPowerAnimation(0, backcontent, sb, 50, null);
                sb.Begin();
            }
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
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
            if (bg != null)
            {
                bg.Show();
            }
            Width = (toolstr != null && System.IO.File.Exists(toolstr) && !Hiro_Utils.Read_Ini(toolstr, "location", "width", "-1").Equals("-1")) ? double.Parse(Hiro_Utils.Read_Ini(toolstr, "location", "width", "-1")) : SystemParameters.PrimaryScreenWidth;
            Height = (toolstr != null && System.IO.File.Exists(toolstr) && !Hiro_Utils.Read_Ini(toolstr, "location", "width", "-1").Equals("-1")) ? double.Parse(Hiro_Utils.Read_Ini(toolstr, "location", "height", "-1")) : 200;
            if (toolstr != null && System.IO.File.Exists(toolstr))
            {
                Hiro_Utils.Set_Control_Location(backtitle, "title", extra: true, path: toolstr);
                Hiro_Utils.Set_Control_Location(cancelbtn, "cancel", extra: true, bottom: true, right: true, path: toolstr);
                Hiro_Utils.Set_Control_Location(rejectbtn, "reject", extra: true, bottom: true, right: true, path: toolstr);
                Hiro_Utils.Set_Control_Location(acceptbtn, "accept", extra: true, bottom: true, right: true, path: toolstr);
                backcontent.Height = Height - backtitle.Height - backtitle.Margin.Top * 2 - acceptbtn.Margin.Bottom * 2 - acceptbtn.Height;
                var le = backcontent.Width;
                Hiro_Utils.Set_Control_Location(backcontent, "content", extra: true, path: toolstr);
                if (le == backcontent.Width)
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
            if (bflag == 1)
                return;
            bflag = 1;
            if (toolstr != null && System.IO.File.Exists(Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(toolstr, "Message", "Background", "")))))
                Hiro_Utils.Set_Bgimage(bgimage, Hiro_Utils.Path_Prepare_EX(Hiro_Utils.Path_Prepare(Hiro_Utils.Read_Ini(toolstr, "Message", "Background", ""))));
            else
                Hiro_Utils.Set_Bgimage(bgimage);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                Hiro_Utils.LogtoFile("[MESSAGE]Accept Button Clicked");
            if (toolstr != null)
                Hiro_Utils.RunExe(Hiro_Utils.Read_Ini(toolstr, "Action", "accept", "nop"));
            if (bg != null)
                bg.Fade_Out();
            Close();
        }

        private void Rejectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                Hiro_Utils.LogtoFile("[MESSAGE]Reject Button Clicked");
            if (toolstr != null)
                Hiro_Utils.RunExe(Hiro_Utils.Read_Ini(toolstr, "Action", "reject", "nop"));
            if (bg != null)
                bg.Fade_Out();
            Close();
        }

        private void Cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.dflag)
                Hiro_Utils.LogtoFile("[MESSAGE]Cancel Button Clicked");
            if (toolstr != null)
                Hiro_Utils.RunExe(Hiro_Utils.Read_Ini(toolstr, "Action", "cancel", "nop"));
            if (bg != null)
                bg.Fade_Out();
            Close();
        }

        private void Msg_Closing(object sender, CancelEventArgs e)
        {
            if (bg != null)
                bg.Fade_Out();
            if (toolstr != null && Hiro_Utils.Read_Ini(toolstr, "Action", "Delete", "false").ToLower().Equals("true"))
            {
                System.IO.File.Delete(toolstr);
            }
        }

        private void Msg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bg == null)
            {
                Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
            }
        }

    }
}
