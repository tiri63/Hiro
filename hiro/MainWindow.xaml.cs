﻿using System;
using System.Windows;

namespace hiro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal System.Windows.Controls.ContextMenu? cm = null;
        internal Windows.Networking.Connectivity.NetworkConnectivityLevel ncl = Windows.Networking.Connectivity.NetworkConnectivityLevel.None;
        internal string rec_nc = "";
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
        }

        public void InitializeInnerParameters()
        {
            Hiro_Tray.ToolTipText = App.AppTitle;
            Title = App.AppTitle;
            InitializeMethod();
            SourceInitialized += OnSourceInitialized;
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
                ncl = profile.GetNetworkConnectivityLevel();
            try
            {
                Windows.System.Power.PowerManager.RemainingChargePercentChanged += PowerManager_RemainingChargePercentChanged;
                Windows.System.Power.PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, "Hiro.Exception.Power");
            }
        }

        private void PowerManager_EnergySaverStatusChanged(object? sender, object e)
        {
            var p = Windows.System.Power.PowerManager.EnergySaverStatus;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Verbose", "0").Equals("1"))
            {
                switch (p)
                {
                    case Windows.System.Power.EnergySaverStatus.On:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("basaveron"), 2, Hiro_Utils.Get_Transalte("battery")));
                        break;
                    case Windows.System.Power.EnergySaverStatus.Disabled:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("basaveroff"), 2, Hiro_Utils.Get_Transalte("battery")));
                        break;
                    default:
                        break;
                }
            }
        }

        private void PowerManager_RemainingChargePercentChanged(object? sender, object e)
        {
            int p = Windows.System.Power.PowerManager.RemainingChargePercent;
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Verbose", "0").Equals("1"))
            {
                if (Windows.System.Power.PowerManager.BatteryStatus ==
                    Windows.System.Power.BatteryStatus.Charging) 
                    return;
                var low = Hiro_Utils.Read_Ini(App.LangFilePath, "local", "lowpower", "[0,1,2,3,4,6,8,10,20,30]").Replace("[", "[,").Replace("]", ",]").Trim();
                var notice = Hiro_Utils.Read_Ini(App.LangFilePath, "local", "tippower", "").Replace("[", "[,").Replace("]", ",]").Trim();
                if (low.IndexOf(p.ToString()) != -1)
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("powerlow").Replace("%p", p.ToString()), 2, Hiro_Utils.Get_Transalte("battery")));
                else if (notice.IndexOf(p.ToString()) != -1)
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("powertip").Replace("%p", p.ToString()), 2, Hiro_Utils.Get_Transalte("battery")));
            }
        }

        private void NetworkChange_NetworkAddressChanged(object? sender, EventArgs e)
        {
            if (App.dflag)
            {
                System.Net.NetworkInformation.NetworkInterface[] nfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                foreach (System.Net.NetworkInformation.NetworkInterface ni in nfaces)
                {
                    Hiro_Utils.LogtoFile(ni.Description + " - " + ni.NetworkInterfaceType.ToString());
                }
            }

            if (!Hiro_Utils.Read_Ini(App.dconfig, "Config", "Verbose", "0").Equals("1")) 
                return;
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            string ext = "";
            if (profile != null)
            {
                try
                {
                    if (profile.IsWlanConnectionProfile)
                    {
                        if (profile.WlanConnectionProfileDetails.GetConnectedSsid().Equals(string.Empty))
                            return;
                        ext = Hiro_Utils.Get_Transalte("netwlan").Replace("%s", profile.WlanConnectionProfileDetails.GetConnectedSsid());
                    }
                    else if (profile.IsWwanConnectionProfile)
                    {
                        if (profile.WwanConnectionProfileDetails.AccessPointName.Equals(string.Empty))
                            return;
                        ext = Hiro_Utils.Get_Transalte("netwwan").Replace("%s", profile.WwanConnectionProfileDetails.AccessPointName);
                    }
                    else
                    {
                        ext = Hiro_Utils.Get_Transalte("neteth");
                    }
                }
                catch(Exception ex)
                {
                    Hiro_Utils.LogError(ex, "Hiro.Exception.Wifi.Network");
                }
                if (ncl == profile.GetNetworkConnectivityLevel() && rec_nc.Equals(ext))
                    return;
                ncl = profile.GetNetworkConnectivityLevel();
                rec_nc = ext;
            }
            else
            {
                if (ncl == Windows.Networking.Connectivity.NetworkConnectivityLevel.None)
                    return;
                ncl = Windows.Networking.Connectivity.NetworkConnectivityLevel.None;
            }
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                switch (ncl)
                {
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("neton").Replace("%s", ext), 2, Hiro_Utils.Get_Transalte("net")));
                        break;
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.LocalAccess:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("netlan").Replace("%s", ext), 2, Hiro_Utils.Get_Transalte("net")));
                        break;
                    case Windows.Networking.Connectivity.NetworkConnectivityLevel.ConstrainedInternetAccess:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("netlimit").Replace("%s", ext), 2, Hiro_Utils.Get_Transalte("net")));
                        break;
                    default:
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("netoff"), 2, Hiro_Utils.Get_Transalte("net")));
                        break;
                };
            }));
        }


        private void OnSourceInitialized(object? sender, EventArgs e)
        {
            App.WND_Handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(App.WND_Handle);
            source?.AddHook(WndProc);
            Hiro_Utils.LogtoFile("[HIROWEGO]Main Window: AddHook WndProc");
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0320://系统颜色改变
                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "LockColor", "default").Equals("default"))
                        App.ColorCD = 3;
                    break;
                case 0x0083://prevent system from drawing outline
                    handled = true;
                    break;
                case 0x0218:
                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Verbose", "0").Equals("1"))
                    {
                        Hiro_Utils.GetSystemPowerStatusImpl(out Hiro_Utils.SYSTEM_POWER_STATUS p);
                        if (p.ACLineStatus == 1 && p.BatteryFlag == 8)
                            App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("bacharge"), 2, Hiro_Utils.Get_Transalte("battery")));
                        if (p.ACLineStatus == 1 && p.BatteryFlag != 8)
                            App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("baconnect"), 2, Hiro_Utils.Get_Transalte("battery")));
                        if (p.ACLineStatus == 0)
                            App.Notify(new Hiro_Notice(Hiro_Utils.Get_Transalte("baremove"), 2, Hiro_Utils.Get_Transalte("battery")));
                    }
                    break;
                case 0x0219:
                    if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Verbose", "0").Equals("1"))
                    {
                        var mms = Hiro_Utils.Get_Transalte("deinfo") + " - ";
                        switch (wParam.ToInt32())
                        {
                            case 0x0018:
                                mms += Hiro_Utils.Get_Transalte("deconfig");
                                break;
                            case 0x8000:
                                mms += Hiro_Utils.Get_Transalte("medconnect");
                                break;
                            case 0x8004:
                                mms += Hiro_Utils.Get_Transalte("medremove");
                                break;
                            default:
                                return IntPtr.Zero;
                        }
                        App.Notify(new Hiro_Notice(mms, 2, Hiro_Utils.Get_Transalte("device")));
                    }
                    break;
                case 0x0312:
                    new System.Threading.Thread(() =>
                    {
                        var indexid = wParam.ToInt32();
                        for (int vsi = 0; vsi < App.vs.Count - 1; vsi += 2)
                        {
                            if (App.vs[vsi] == indexid)
                            {
                                Dispatcher.Invoke(delegate
                                {
                                    Hiro_Utils.RunExe(App.cmditems[App.vs[vsi + 1]].Command);
                                });
                                break;
                            }
                        }
                    }).Start();
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;

        }

        public void Load_All_Colors()
        {
            Hiro_Utils.IntializeColorParameters();
            App.ed?.Load_Color();
            App.noti?.Load_Color();
            App.mn?.Load_Colors();
            foreach (Window win in Application.Current.Windows)
            {
                switch (win)
                {
                    case Hiro_Alarm a:
                        a.Load_Colors();
                        break;
                    case Hiro_Download b:
                        b.Load_Colors();
                        break;
                    case Hiro_LockScreen c:
                        c.Load_Colors();
                        break;
                    case Hiro_Msg f:
                        f.Load_Colors();
                        break;
                    case Hiro_Sequence d:
                        d.Load_Colors();
                        break;
                    case Hiro_Web e:
                        e.Load_Color();
                        break;
                    case Hiro_Finder g:
                        g.Load_Color();
                        break;
                    case Hiro_Player h:
                        h.Load_Color();
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void InitializeMethod()
        {
            Hiro_Tray.TrayMiddleMouseDown += delegate
            {
                var mc = Hiro_Utils.Read_Ini(App.dconfig, "Config", "MiddleClick", "2");
                switch (mc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()"); ;
                        break;
                    case "3":
                        var mce = Hiro_Utils.Read_Ini(App.dconfig, "Config", "MiddleAction", "");
                        Hiro_Utils.RunExe(mce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            Hiro_Utils.RunExe(App.mn.Visibility == Visibility.Visible ? "hide()" : "show()");
                        }
                        else
                        {
                            Hiro_Utils.RunExe("show()");
                        }
                        break;
                }
            };
            Hiro_Tray.TrayRightMouseDown += delegate
            {
                var rc = Hiro_Utils.Read_Ini(App.dconfig, "Config", "RightClick", "2");
                switch (rc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()");
                        break;
                    case "3":
                        var rce = Hiro_Utils.Read_Ini(App.dconfig, "Config", "RightAction", "");
                        Hiro_Utils.RunExe(rce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            Hiro_Utils.RunExe(App.mn.Visibility == Visibility.Visible ? "hide()" : "show()");
                        }
                        else
                        {
                            Hiro_Utils.RunExe("show()");
                        }
                        break;
                }
            };
            Hiro_Tray.TrayLeftMouseDown += delegate
            {
                var lc = Hiro_Utils.Read_Ini(App.dconfig, "Config", "LeftClick", "1");
                switch (lc)
                {
                    case "2":
                        Hiro_Utils.RunExe("menu()");
                        break;
                    case "3":
                        var lce = Hiro_Utils.Read_Ini(App.dconfig, "Config", "LeftAction", "");
                        Hiro_Utils.RunExe(lce);
                        break;
                    default:
                        if (App.mn != null)
                        {
                            Hiro_Utils.RunExe(App.mn.Visibility == Visibility.Visible ? "hide()" : "show()");
                        }
                        else
                        {
                            Hiro_Utils.RunExe("show()");
                        }
                        break;
                }
            };
        }

        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hiro_Tray.Dispose();
            Microsoft.Toolkit.Uwp.Notifications.ToastNotificationManagerCompat.Uninstall();
            Hiro_Utils.LogtoFile("[INFOMATION]Main UI: Closing " + e.GetType().ToString());
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            System.Windows.Controls.Canvas.SetTop(this, -233);
            System.Windows.Controls.Canvas.SetLeft(this, -233);
        }
    }
}
