﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_Finder.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Finder : Window
    {
        private int bflag = 0;
        private int cflag = 0;
        public Hiro_Finder()
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth / 5 * 4;
            Height = SystemParameters.PrimaryScreenHeight / 10;
            Title = App.AppTitle;
            Hiro_Initialize();
            SourceInitialized += OnSourceInitialized;
            ContentRendered += delegate
             {
                 Size msize = new();
                 Hiro_Utils.Get_Text_Visual_Width(PlaceHolder, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                 while (msize.Height <= PlaceHolder.ActualHeight)
                 {
                     Hiro_Utils.Get_Text_Visual_Width(PlaceHolder, VisualTreeHelper.GetDpi(this).PixelsPerDip, out msize);
                     PlaceHolder.FontSize++;
                     if (PlaceHolder.FontSize > 36)
                         break;
                 }
                 PlaceHolder.FontSize--;
                 HiHiro();
                 Focus();
                 Keyboard.Focus(Hiro_Text);
                 Mouse.Capture(Hiro_Text);
                 Hiro_Text.Focus();
             };
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            source.AddHook(WndProc);
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0083:
                    handled = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            Loadbgi(Hiro_Utils.ConvertInt(Hiro_Utils.Read_Ini(App.dconfig, "Config", "Blur", "0")));
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
        }

        public void Load_Translate()
        {
            PlaceHolder.Content = Hiro_Utils.Get_Transalte("hirogo");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(PlaceHolder, "hirogo", location: false);
            Hiro_Utils.Set_Control_Location(Hiro_Text, "hirogotb", location: false);
        }

        public void HiHiro()
        {
            if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(String.Empty))
                PlaceHolder.Visibility = Visibility.Visible;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Storyboard sb = new();
                Hiro_Utils.AddPowerAnimation(0, PlaceHolder, sb, 50, null);
                sb.Begin();
            }
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            TryClose();
        }

        public void Loadbgi(int direction)
        {
            if (bflag == 1)
                return;
            bflag = 1;
            Hiro_Utils.Set_Bgimage(bgimage);
            bool animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Hiro_Utils.Blur_Animation(direction, animation, bgimage, this);
            bflag = 0;
        }

        private void Hiro_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Hiro_Text.Text.Equals("") || Hiro_Text.Text.Equals(String.Empty))
            {
                PlaceHolder.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceHolder.Visibility = Visibility.Hidden;
            }
        }

        private void Hiro_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                TryClose();
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                Hiro_Utils.RunExe(Hiro_Text.Text);
                TryClose();
                e.Handled = true;
            }
        }
        
        private void TryClose()
        {
            if (cflag == 0)
            {
                cflag = 1;
                Close();
            }
        }

        private void Hiro_Text_LostFocus(object sender, RoutedEventArgs e)
        {
            TryClose();
        }

        private void Hiro_Text_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TryClose();
        }
    }
}
