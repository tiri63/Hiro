﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hiro.Helpers;
using Hiro.ModelViews;

namespace Hiro
{
    /// <summary>
    /// Hiro_Ticker.xaml 的交互逻辑
    /// </summary>
    public partial class Hiro_Ticker : Window
    {
        internal string id = string.Empty;
        internal string format = "%n";
        internal int target = int.MaxValue;
        internal int max = int.MaxValue;
        internal int min = int.MinValue;
        internal int current = 0;
        internal int bflag = 0;
        internal WindowAccentCompositor? compositor = null;
        private void VirtualTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }
        public Hiro_Ticker(string tid, string tformat = "%n", int tcurrent = 0, int tnum = int.MaxValue, int tmin = int.MinValue, int tmax = int.MaxValue)
        {
            InitializeComponent();
            Helpers.HUI.SetCustomWindowIcon(this);
            id = tid;
            format = tformat;
            current = tcurrent;
            max = tmax;
            min = tmin;
            target = tnum;
            SourceInitialized += OnSourceInitialized;
            Con.Content = format.Replace("%n", current.ToString());
            Load_Color();
            Load_Translate();
            Loaded += delegate
            {
                HiHiro();
                Hiro_Utils.SetShadow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            };
        }

        internal void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeReverse"] = App.AppForeColor == Colors.White ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppForeDimColor"] = Hiro_Utils.Color_Transparent(App.AppForeColor, 80);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 80));
            Resources["AppAccentDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, 20));
        }

        internal void Load_Translate()
        {
            Title = HText.Get_Translate("httitle").Replace("%h", App.appTitle);
            maxbtn.ToolTip = HText.Get_Translate("htup");
            resbtn.ToolTip = HText.Get_Translate("htdown");
            minbtn.ToolTip = HText.Get_Translate("min");
            closebtn.ToolTip = HText.Get_Translate("close");
        }

        public void HiHiro()
        {
            Loadbgi(Hiro_Utils.ConvertInt(HSet.Read_DCIni("Blur", "0")));
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                System.Windows.Media.Animation.Storyboard sb = new();
                HAnimation.AddPowerAnimation(2, Ctrl_Btns, sb, -50, null);
                sb.Begin();
            }
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
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
            HUI.Set_Bgimage(bgimage, this);
            bool animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            HAnimation.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        internal void OffsetNum(int num)
        {
            if (num > 0)
            {
                if (current < max)
                {
                    current += num;
                }
            }
            if (num < 0)
            {
                if (current > min)
                {
                    current+= num;
                }
            }
            RefreshContent();
        }

        internal void RefreshContent()
        {
            if (current > max)
                current = max;
            if (current < min)
                current = min;
            var target = format.Replace("%n", current.ToString());
            if (!Con.Content.Equals(target))
                Con.Content = target;
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

        private void Maxbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OffsetNum(1);
        }

        private void Resbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OffsetNum(-1);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Hiro_Utils.Move_Window((new System.Windows.Interop.WindowInteropHelper(this)).Handle);
        }

        private void Closebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void minbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }
    }
}
