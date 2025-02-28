﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Hiro.Helpers.HText;
using static Hiro.Helpers.HSet;
using Hiro.Helpers;

namespace Hiro
{
    /// <summary>
    /// Hiro_Items.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_Items : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        public Hiro_Items(Hiro_MainUI? Parent)
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
            bool animation = !Read_DCIni("Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Read_DCIni("Ani", "2").Equals("1"))
            {
                HAnimation.AddPowerAnimation(1, btn1, sb, 50, null);
                HAnimation.AddPowerAnimation(1, btn2, sb, 50, null);
                HAnimation.AddPowerAnimation(1, btn3, sb, 50, null);
                HAnimation.AddPowerAnimation(1, btn4, sb, 50, null);
                HAnimation.AddPowerAnimation(1, btn5, sb, 50, null);
                HAnimation.AddPowerAnimation(1, btn6, sb, 50, null);
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
            dgi.ItemsSource = App.cmditems;
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppForeDim"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppForeColor, 80));
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            btn1.Content = Get_Translate("inew");
            btn2.Content = Get_Translate("iup");
            btn3.Content = Get_Translate("idown");
            btn4.Content = Get_Translate("ilaunch");
            btn5.Content = Get_Translate("idelete");
            btn6.Content = Get_Translate("imodify");
            dgi.Columns[0].Header = Get_Translate("page");
            dgi.Columns[1].Header = Get_Translate("id");
            dgi.Columns[2].Header = Get_Translate("Name");
            dgi.Columns[3].Header = Get_Translate("Command");
        }

        public void Load_Position()
        {
            HUI.Set_Control_Location(ExCellLabel, "icell", location: false);
            HUI.Set_Control_Location(btn1, "inew");
            HUI.Set_Control_Location(btn2, "iup");
            HUI.Set_Control_Location(btn3, "idown");
            HUI.Set_Control_Location(btn4, "ilaunch");
            HUI.Set_Control_Location(btn5, "idelete");
            HUI.Set_Control_Location(btn6, "imodify");
            HUI.Set_Control_Location(dgi, "data");
        }

        private void Dgi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Btn6_Click(sender, e);
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            if (Hiro_Main == null) 
                return;
            Hiro_Main.hiro_newitem ??= new(Hiro_Main);
            Hiro_Main.hiro_newitem.Load_ComboBox();
            Hiro_Main.hiro_newitem.ntn9.Visibility = Visibility.Hidden;
            Hiro_Main.hiro_newitem.tb7.Text = "";
            Hiro_Main.hiro_newitem.tb8.Text = "";
            Hiro_Main.hiro_newitem.keybox.SelectedIndex = 0;
            Hiro_Main.hiro_newitem.modibox.SelectedIndex = 0;
            Hiro_Main.newx.Content = Get_Translate("new");
            Hiro_Main.current = Hiro_Main.hiro_newitem;
            Hiro_Main.Set_Label(Hiro_Main.newx);

        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            btn2.IsEnabled = false;
            Hiro_Utils.Delay(200);
            btn2.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > 0 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                HClass.Cmditem nec = new(App.cmditems[i - 1].Page, App.cmditems[i - 1].Id, App.cmditems[i].Name, App.cmditems[i].Command, App.cmditems[i].HotKey);
                App.cmditems[i] = new(App.cmditems[i].Page, App.cmditems[i].Id, App.cmditems[i - 1].Name, App.cmditems[i - 1].Command, App.cmditems[i - 1].HotKey);
                App.cmditems[i - 1] = nec;
                var inipath = App.dConfig;
                Write_Ini(inipath, i.ToString(), "Title", nec.Name);
                Write_Ini(inipath, i.ToString(), "Command", "(" + nec.Command + ")");
                Write_Ini(inipath, i.ToString(), "HotKey", nec.HotKey);
                Write_Ini(inipath, (i + 1).ToString(), "Title", App.cmditems[i].Name);
                Write_Ini(inipath, (i + 1).ToString(), "Command", "(" + App.cmditems[i].Command + ")");
                Write_Ini(inipath, (i + 1).ToString(), "HotKey", App.cmditems[i].HotKey);
                dgi.SelectedIndex = i - 1;
                App.Load_Menu();
                HHotKeys.SwitchIfExist(i, i - 1);
            }
            GC.Collect();
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            btn3.IsEnabled = false;
            Hiro_Utils.Delay(200);
            btn3.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count - 1)
            {
                var i = dgi.SelectedIndex;
                Helpers.HClass.Cmditem nec = new(App.cmditems[i + 1].Page, App.cmditems[i + 1].Id, App.cmditems[i].Name, App.cmditems[i].Command, App.cmditems[i].HotKey);
                App.cmditems[i] = new(App.cmditems[i].Page, App.cmditems[i].Id, App.cmditems[i + 1].Name, App.cmditems[i + 1].Command, App.cmditems[i + 1].HotKey);
                App.cmditems[i + 1] = nec;
                var inipath = App.dConfig;
                Write_Ini(inipath, (i + 1).ToString(), "Title", App.cmditems[i].Name);
                Write_Ini(inipath, (i + 1).ToString(), "Command", "(" + App.cmditems[i].Command + ")");
                Write_Ini(inipath, (i + 1).ToString(), "HotKey", App.cmditems[i].HotKey);
                Write_Ini(inipath, (i + 2).ToString(), "Title", App.cmditems[i + 1].Name);
                Write_Ini(inipath, (i + 2).ToString(), "Command", "(" + App.cmditems[i + 1].Command + ")");
                Write_Ini(inipath, (i + 2).ToString(), "HotKey", App.cmditems[i + 1].HotKey);
                dgi.SelectedIndex = i + 1;
                App.Load_Menu();
                HHotKeys.SwitchIfExist(i, i + 1);
            }
            GC.Collect();
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            btn5.IsEnabled = false;
            Hiro_Utils.Delay(200);
            HHotKeys.UnregisterIfExist(dgi.SelectedIndex);
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                var i = dgi.SelectedIndex;
                var inipath = App.dConfig;
                while (i < App.cmditems.Count - 1)
                {
                    App.cmditems[i].Name = App.cmditems[i + 1].Name;
                    App.cmditems[i].Command = App.cmditems[i + 1].Command;
                    App.cmditems[i].HotKey = App.cmditems[i + 1].HotKey;
                    Write_Ini(inipath, (i + 1).ToString(), "Title", Read_Ini(inipath, (i + 2).ToString(), "Title", " "));
                    Write_Ini(inipath, (i + 1).ToString(), "Command", Read_Ini(inipath, (i + 2).ToString(), "Command", " "));
                    Write_Ini(inipath, (i + 1).ToString(), "HotKey", Read_Ini(inipath, (i + 2).ToString(), "HotKey", " "));
                    i++;
                    System.Windows.Forms.Application.DoEvents();
                }
                Write_Ini(inipath, (i + 1).ToString(), "Title", " ");
                Write_Ini(inipath, (i + 1).ToString(), "Command", " ");
                Write_Ini(inipath, (i + 1).ToString(), "HotKey", " ");
                App.cmditems.RemoveAt(i);
                var total = (App.cmditems.Count % 10 == 0) ? App.cmditems.Count / 10 : App.cmditems.Count / 10 + 1;
                if (App.page > total - 1 && App.page > 0)
                    App.page--;
                App.Load_Menu();
            }
            btn5.IsEnabled = true;
            GC.Collect();
        }

        private void Btn6_Click(object sender, RoutedEventArgs e)
        {
            if (App.cmditems.Count == 0 || dgi.SelectedIndex <= -1 || dgi.SelectedIndex >= App.cmditems.Count ||
                Hiro_Main == null) 
                return;
            Hiro_Main.hiro_newitem ??= new(Hiro_Main);
            Hiro_Main.hiro_newitem.index = dgi.SelectedIndex;
            Hiro_Main.hiro_newitem.Load_ComboBox();
            Hiro_Main.hiro_newitem.ntn9.Visibility = Visibility.Visible;
            Hiro_Main.hiro_newitem.tb7.Text = App.cmditems[dgi.SelectedIndex].Name;
            Hiro_Main.hiro_newitem.tb8.Text = App.cmditems[dgi.SelectedIndex].Command;
            var key = App.cmditems[dgi.SelectedIndex].HotKey;
            try
            {
                if (key.IndexOf(",") != -1)
                {
                    var mo = int.Parse(key[..key.IndexOf(",")]);
                    var vkey = int.Parse(key.Substring(key.IndexOf(",") + 1, key.Length - key.IndexOf(",") - 1));
                    Hiro_Main.hiro_newitem.modibox.SelectedIndex = HHotKeys.Index_Modifier(false, mo);
                    Hiro_Main.hiro_newitem.keybox.SelectedIndex = HHotKeys.Index_vKey(false, vkey);
                }
                else
                {
                    Hiro_Main.hiro_newitem.modibox.SelectedIndex = 0;
                    Hiro_Main.hiro_newitem.keybox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Items.Bind");
                Hiro_Main.hiro_newitem.modibox.SelectedIndex = 0;
                Hiro_Main.hiro_newitem.keybox.SelectedIndex = 0;
            }
            Hiro_Main.newx.Content = Get_Translate("mod");
            Hiro_Main.current = Hiro_Main.hiro_newitem;
            Hiro_Main.Set_Label(Hiro_Main.newx);
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            btn4.IsEnabled = false;
            Hiro_Utils.Delay(200);
            btn4.IsEnabled = true;
            if (App.cmditems.Count != 0 && dgi.SelectedIndex > -1 && dgi.SelectedIndex < App.cmditems.Count)
            {
                Hiro_Utils.RunExe(App.cmditems[dgi.SelectedIndex].Command, App.cmditems[dgi.SelectedIndex].Name);
            }
        }
    }
}
