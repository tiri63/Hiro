﻿using Hiro.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Hiro
{
    /// <summary>
    /// Hiro_Schedule.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Schedule : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Schedule(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            var animation = !HSet.Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (HSet.Read_DCIni("Ani", "2").Equals("1"))
            {
                HAnimation.AddPowerAnimation(1, scbtn_1, sb, 50, null);
                HAnimation.AddPowerAnimation(1, scbtn_2, sb, 50, null);
                HAnimation.AddPowerAnimation(1, scbtn_3, sb, 50, null);
            }

            if (!animation)
                return;
            HAnimation.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
            dgs.ItemsSource = App.scheduleitems;
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            scbtn_1.Content = HText.Get_Translate("scnew");
            scbtn_2.Content = HText.Get_Translate("scdelete");
            scbtn_3.Content = HText.Get_Translate("scmodify");
            dgs.Columns[0].Header = HText.Get_Translate("sid");
            dgs.Columns[1].Header = HText.Get_Translate("sname");
            dgs.Columns[2].Header = HText.Get_Translate("stime");
            dgs.Columns[3].Header = HText.Get_Translate("scommand");
        }

        public void Load_Position()
        {
            HUI.Set_Control_Location(ExCellLabel, "scell", location: false);
            HUI.Set_Control_Location(scbtn_1, "scnew");
            HUI.Set_Control_Location(scbtn_2, "scdelete");
            HUI.Set_Control_Location(scbtn_3, "scmodify");
            HUI.Set_Control_Location(dgs, "sdata");
        }

        private void Dgs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Scbtn_3_Click(sender, e);
        }

        private void Scbtn_1_Click(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main == null) 
                return;
            Hiro_Main.hiro_newschedule ??= new(Hiro_Main);
            Hiro_Main.hiro_newschedule.scbtn_4.Visibility = Visibility.Hidden;
            Hiro_Main.hiro_newschedule.tb11.Text = "";
            Hiro_Main.hiro_newschedule.tb12.Text = "";
            Hiro_Main.hiro_newschedule.tb13.Text = "";
            Hiro_Main.hiro_newschedule.tb14.Text = "";
            Hiro_Main.newx.Content = HText.Get_Translate("new");
            Hiro_Main.current = Hiro_Main.hiro_newschedule;
            Hiro_Main.Set_Label(Hiro_Main.newx);

        }

        private void Scbtn_3_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count == 0 || dgs.SelectedIndex <= -1 ||
                dgs.SelectedIndex >= App.scheduleitems.Count || Hiro_Main == null) 
                return;
            Hiro_Main.hiro_newschedule ??= new(Hiro_Main);
            Hiro_Main.hiro_newschedule.index = dgs.SelectedIndex;
            Hiro_Main.hiro_newschedule.scbtn_4.Visibility = Visibility.Visible;
            Hiro_Main.hiro_newschedule.tb11.Text = App.scheduleitems[dgs.SelectedIndex].Name;
            Hiro_Main.hiro_newschedule.tb12.Text = App.scheduleitems[dgs.SelectedIndex].Time;
            Hiro_Main.hiro_newschedule.tb13.Text = App.scheduleitems[dgs.SelectedIndex].Command;
            Hiro_Main.hiro_newschedule.tb14.Text = "";
            switch (App.scheduleitems[dgs.SelectedIndex].re)
            {
                case -2.0:
                    Hiro_Main.hiro_newschedule.rbtn18.IsChecked = true;
                    break;
                case -1.0:
                    Hiro_Main.hiro_newschedule.rbtn19.IsChecked = true;
                    break;
                case 0.0:
                    Hiro_Main.hiro_newschedule.rbtn20.IsChecked = true;
                    break;
                default:
                    Hiro_Main.hiro_newschedule.rbtn21.IsChecked = true;
                    Hiro_Main.hiro_newschedule.tb14.Text = App.scheduleitems[dgs.SelectedIndex].re.ToString();
                    break;
            }
            Hiro_Main.newx.Content = HText.Get_Translate("mod");
            Hiro_Main.current = Hiro_Main.hiro_newschedule;
            Hiro_Main.Set_Label(Hiro_Main.newx);
        }

        private void Scbtn_2_Click(object sender, RoutedEventArgs e)
        {
            if (App.scheduleitems.Count != 0 && dgs.SelectedIndex > -1 && dgs.SelectedIndex < App.scheduleitems.Count)
            {
                Hiro_Utils.Delete_Alarm(dgs.SelectedIndex);

            }
        }
    }
}
