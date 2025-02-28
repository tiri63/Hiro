﻿using Hiro.Helpers;
using System.Windows;

namespace Hiro
{
    /// <summary>
    /// background.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Background : Window
    {
        public Hiro_Background()
        {
            InitializeComponent();
            Title = App.appTitle;
            Helpers.HUI.SetCustomWindowIcon(this);
            Hiro_Utils.SetWindowToForegroundWithAttachThreadInput(this);
        }

        private void Back_Loaded(object sender, RoutedEventArgs e)
        {
            if (!HSet.Read_DCIni("Ani", "2").Equals("0"))
            {
                System.Windows.Media.Animation.Storyboard? sb = new();
                sb = HAnimation.AddDoubleAnimaton(0.7, 300, this, "Opacity", sb, 0);
                sb.Completed += delegate
                {
                    Opacity = 0.7;
                };
                sb.Begin();
            }
            else
                Opacity = 0.7;
        }

        internal void Fade_Out()
        {
            if (!HSet.Read_DCIni("Ani", "2").Equals("0"))
            {
                System.Windows.Media.Animation.Storyboard? sb = new();
                sb = HAnimation.AddDoubleAnimaton(0, 300, this, "Opacity", sb);
                sb.Completed += delegate
                {
                    Close();
                };
                sb.Begin();
            }
            else
            {
                Close();
            }
        }
    }
}
