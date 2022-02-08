﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Credentials.UI;
using Windows.Storage.Streams;

namespace hiro
{

    public class cmditem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int p;
        private int i;
        private string na;
        private string co;

        public int page {
            get { return this.p; }
            set
            {
                this.p = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(page)));
                }
            }
        }
        public int id {
            get { return this.i; }
            set
            {
                this.i = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(id)));
                }
            }
        }
        public string name { 
            get { return this.na; } 
            set 
            { 
                this.na = value;
            if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(name)));
                } 
            }
}
        public string command
        {
            get { return this.co; }
            set
            {
                this.co = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(command)));
                }
            }
        }

        public cmditem()
        {
            this.page = -1;
            this.id = -1;
            this.name = string.Empty;
            this.command = string.Empty;
            this.p = -1;
            this.id = -1;
            this.na = string.Empty;
            this.co = string.Empty;
        }
        public cmditem(int a, int b, string c, string d)
        {
            this.page = a;
            this.id = b;
            this.name = c;
            this.command = d;
            this.p = a;
            this.id = b;
            this.na = c;
            this.co = d;
        }
        public cmditem(cmditem c)
        {
            if (c == null)
            {
                this.page = -1;
                this.id = -1;
                this.name = string.Empty;
                this.command = string.Empty;
                this.p = -1;
                this.id = -1;
                this.na = string.Empty;
                this.co = string.Empty;
            }
            else
            {
                this.page = c.page;
                this.id = c.id;
                this.name = c.name;
                this.command = c.command;
                this.p = c.page;
                this.id = c.id;
                this.na = c.name;
                this.co = c.command;
            }
        }

    }
    public class scheduleitem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int i;
        private string na;
        private string ti;
        private string co;
        public double re;
        public string time
        {
            get { return ti; }
            set
            {
                ti = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(time)));
                }
            }
        }
        public int id
        {
            get { return i; }
            set
            {
                i = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(id)));
                }
            }
        }
        public string name
        {
            get { return na; }
            set
            {
                na = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(name)));
                }
            }
        }
        public string command
        {
            get { return co; }
            set
            {
                co = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(command)));
                }
            }
        }

        public scheduleitem()
        {
            time = "19000101000000";
            id = -1;
            name = string.Empty;
            command = string.Empty;
            re = -2.0;
            ti = "19000101000000";
            i = -1;
            na = string.Empty;
            co = string.Empty;
        }
        public scheduleitem(int b, string a, string c, string d, double e)
        {
            name = a;
            id = b;
            time = c;
            command = d;
            re = e;
            na = a;
            i = b;
            ti = c;
            co = d;
        }
        public scheduleitem(scheduleitem c)
        {
            if (c == null)
            {
                time = "19000101000000";
                id = -1;
                name = string.Empty;
                command = string.Empty;
                re = -2.0;
                ti = "19000101000000";
                i = -1;
                na = string.Empty;
                co = string.Empty;
            }
            else
            {
                time = c.time;
                id = c.id;
                name = c.name;
                command = c.command;
                re = c.re;
                ti = c.time;
                i = c.id;
                na = c.name;
                co = c.command;
            }
        }

    }
    public class alarmwin
    {
        public Alarm win;
        public int id;
        public alarmwin(Alarm a, int b)
        {
            win = a;
            id = b;
        }
        public alarmwin(int a, Alarm b)
        {
            win = b;
            id = a;
        }
    }
    public class noticeitem
    {
        public string msg;
        public int time;

        public noticeitem(string ms = "NULL", int ti = 1)
        {
            msg = ms;
            time = ti;
        }
    }
    public class language : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string name
        {
            get { return na; }
            set
            {
                na = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(name)));
                }
            }
        }
        public string langname
        {
            get { return la; }
            set
            {
                la = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(langname)));
                }
            }
        }

        private string na;
        private string la;

        public language()
        {
            name = "null";
            langname = "null";
            na = "null";
            la = "null";
        }

        public language(string Name, string LangName)
        {
            name = Name;
            langname = LangName;
            na = Name;
            la = LangName;
        }
        
    }


    public partial class utils : Component
    {
        public utils()
        {
            InitializeComponent();
        }

        public utils(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region 配置相关
        #region API函数声明
        [System.Runtime.InteropServices.DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        #endregion
        #region 读Ini文件
        public static string Read_Ini(string iniFilePath, string Section, string Key, string defaultText)
        {
            if (System.IO.File.Exists(iniFilePath))
            {
            byte[] buffer = new byte[1024];
            int ret = GetPrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(defaultText), buffer, 1024, iniFilePath);
                return Encoding.GetEncoding("utf-8").GetString(buffer, 0, ret).Trim();
            }
            else
            {
                return defaultText;
            }
        }
        #endregion


        #region 写Ini文件
        public static bool Write_Ini(string iniFilePath, string Section, string Key, string Value)
        {
            try
            {
                if (!System.IO.File.Exists(iniFilePath))
                    System.IO.File.Create(iniFilePath).Close();
                long OpStation = WritePrivateProfileString(Encoding.GetEncoding("utf-8").GetBytes(Section), Encoding.GetEncoding("utf-8").GetBytes(Key), Encoding.GetEncoding("utf-8").GetBytes(Value), iniFilePath);
                if (OpStation == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
                return false;
            }
                
        }
    #endregion
        #region 写日志相关
        public static void LogtoFile(string val)
        {
            try
            {
                if (!System.IO.File.Exists(App.LogFilePath))
                    System.IO.File.Create(App.LogFilePath).Close();
                System.IO.FileStream fs = new(App.LogFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                System.IO.StreamReader sr = new(fs);
                var str = sr.ReadToEnd();
                System.IO.StreamWriter sw = new(fs);
                sw.Write(DateTime.Now.ToString("[HH:mm:ss]") + val + Environment.NewLine);
                sw.Flush();
                sw.Close();
                sr.Close();
                sr.Dispose();
                fs.Close();

            }
            catch (Exception ex)
            {
                LogtoFile("[ERROR]" + ex.Message);

            }
        }
        #endregion
        #region 翻译文件
        public static string Get_Transalte(string val)
        {
            return Read_Ini(App.CurrentDirectory + "\\system\\lang\\" + App.lang + ".hlp", "translate", val, "<???>").Replace("\\n",Environment.NewLine);
        }
        #endregion
        #region UI 相关
        public static void Set_Bgimage(System.Windows.Controls.Control sender)
        {
            var strFileName = Read_Ini(App.dconfig, "Configuration", "backimage", "");
            if (Read_Ini(App.dconfig, "Configuration", "background", "1").Equals("1") || !System.IO.File.Exists(strFileName))
            {
                sender.Background = new System.Windows.Media.SolidColorBrush(App.AppAccentColor);
            }
            else
            {
                System.Windows.Media.ImageBrush ib = new()
                    {
                        Stretch = System.Windows.Media.Stretch.UniformToFill,
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(strFileName))
                    };
                    sender.Background = ib;
            }
        }
        public static void Set_Control_Location(System.Windows.Controls.Control sender,string val,bool extra = false,string? path = "", bool right = false, bool bottom = false)
        {
            if (extra == false || path == null || !System.IO.File.Exists(path))
                path = App.LangFilePath;
            try
            {
                if (sender != null)
                {
                    var loc = Read_Ini(path, "location", val, string.Empty);
                    loc = loc.Replace(" ", "").Replace("%b", " ");
                    loc = loc[(loc.IndexOf("{") + 1)..];
                    loc = loc[..loc.LastIndexOf("}")];
                    var fontname = loc[..loc.IndexOf(",")];
                    loc = loc[(fontname.Length + 1)..];
                    var fontsize = loc[..loc.IndexOf(",")];
                    loc = loc[(fontsize.Length + 1)..];
                    var fontweight = loc[..loc.IndexOf(",")];
                    loc = loc[(fontweight.Length + 1)..];
                    var fontstyle = loc[..loc.IndexOf(",")];
                    loc = loc[(fontstyle.Length + 1)..];
                    var left = loc.Substring(0, loc.IndexOf(","));
                    loc = loc[(left.Length + 1)..];
                    var top = loc[..loc.IndexOf(",")];
                    loc = loc[(top.Length + 1)..];
                    var width = loc.Substring(0, loc.IndexOf(","));
                    loc = loc[(width.Length + 1)..];
                    var height = loc;
                    if (fontname != "-1")
                        sender.FontFamily = new System.Windows.Media.FontFamily(fontname);
                    if (fontsize != "-1")
                        sender.FontSize = double.Parse(fontsize);
                    sender.FontWeight = fontweight switch
                    {
                        "0" => System.Windows.FontWeights.Light,
                        "1" => System.Windows.FontWeights.Bold,
                        _ => System.Windows.FontWeights.Normal,
                    };
                    sender.FontStyle = fontstyle switch
                    {
                        "0" => System.Windows.FontStyles.Italic,
                        "1" => System.Windows.FontStyles.Oblique,
                        _ => System.Windows.FontStyles.Normal,
                    };
                    if (width != "-1") 
                        sender.Width = double.Parse(width);
                    if (height != "-1") 
                        sender.Height = double.Parse(height);
                    System.Windows.Thickness thickness = sender.Margin;
                    if (left != "-1")
                    {
                        if (right == false)
                            thickness.Left = double.Parse(left);
                        else
                            thickness.Right = double.Parse(left);
                    }
                    if (top != "-1")
                    {
                        if (bottom == false)
                            thickness.Top = double.Parse(top);
                        else
                            thickness.Bottom = double.Parse(top);

                    }
                    if(right == true)
                        sender.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    if (bottom == true)
                        sender.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    sender.Margin = thickness;                    
                }
            }
            catch (Exception ex)
            {
                LogtoFile("[ERROR]" + "sender " + val + "|" +  ex.Message);
            }
            
        }

        public static void Set_Grid_Location(System.Windows.Controls.Grid sender,string val)
        {
            try
            {
                if (sender != null)
                {
                    var loc = Read_Ini(App.LangFilePath, "location", val, string.Empty);
                    loc = loc.Replace(" ", "").Replace("%b", " ");
                    loc = loc.Substring(loc.IndexOf("{") + 1);
                    loc = loc.Substring(0, loc.LastIndexOf("}"));
                    var left = loc.Substring(0, loc.IndexOf(","));
                    loc = loc.Substring(left.Length + 1);
                    var top = loc.Substring(0, loc.IndexOf(","));
                    loc = loc.Substring(top.Length + 1);
                    var width = loc.Substring(0, loc.IndexOf(","));
                    loc = loc.Substring(width.Length + 1);
                    var height = loc;
                    if (width != "-1")
                        sender.Width = double.Parse(width);
                    if (height != "-1")
                        sender.Height = double.Parse(height);
                    System.Windows.Thickness thickness = sender.Margin;
                    if (left != "-1")
                        thickness.Left = double.Parse(left);
                    if (top != "-1")
                        thickness.Top = double.Parse(top);
                    sender.Margin = thickness;

                }
            }
            catch (System.Exception ex)
            {
                LogtoFile("[ERROR]" + "sender " + val + "|" + ex.Message);
            }
            
        }
        #endregion
        #endregion

        #region 字符串处理
        public static String Path_Replace(String path, String toReplace, String replaced)
        {
            var resu = (replaced.EndsWith("\\")) ? replaced.Substring(0, replaced.Length - 1) : replaced;
            resu = Microsoft.VisualBasic.Strings.Replace(path, toReplace, resu, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
            if (resu != null)
                return resu;
            else
                return "";
        }

        private static String Anti_Path_Replace(String path, String replaced, String toReplace)
        {
            var resu = (toReplace.EndsWith("\\")) ? toReplace.Substring(0, toReplace.Length - 1) : toReplace;
            resu = Microsoft.VisualBasic.Strings.Replace(path, resu, replaced, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
            if (resu != null)
                return resu;
            else
                return "";
        }

        public static String Anti_Path_Prepare(String vpath)
        {
            var path = vpath;
            path = Anti_Path_Replace(path, "<current>", AppDomain.CurrentDomain.BaseDirectory);
            path = Anti_Path_Replace(path, "<system>", Environment.SystemDirectory);
            path = Anti_Path_Replace(path, "<systemx86>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.SystemX86.Path);
            path = Anti_Path_Replace(path, "<idesktop>", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            path = Anti_Path_Replace(path, "<ideskdir>", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            path = Anti_Path_Replace(path, "<cdeskdir>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            path = Anti_Path_Replace(path, "<idocument>", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            path = Anti_Path_Replace(path, "<cdocument>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            path = Anti_Path_Replace(path, "<iappdata>", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            path = Anti_Path_Replace(path, "<cappdata>", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            path = Anti_Path_Replace(path, "<imusic>", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            path = Anti_Path_Replace(path, "<cmusic>", Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            path = Anti_Path_Replace(path, "<ipicture>", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            path = Anti_Path_Replace(path, "<cpicture>", Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));
            path = Anti_Path_Replace(path, "<istart>", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            path = Anti_Path_Replace(path, "<cstart>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
            path = Anti_Path_Replace(path, "<istartup>", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            path = Anti_Path_Replace(path, "<cstartup>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            path = Anti_Path_Replace(path, "<ivideo>", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            path = Anti_Path_Replace(path, "<cvideo>", Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos));
            path = Anti_Path_Replace(path, "<istartup>", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            path = Anti_Path_Replace(path, "<cstartup>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            path = Anti_Path_Replace(path, "<iprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            path = Anti_Path_Replace(path, "<cprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));
            path = Anti_Path_Replace(path, "<iprog>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            path = Anti_Path_Replace(path, "<cprog>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
            path = Anti_Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads.Path);
            path = Anti_Path_Replace(path, "<cdownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.PublicDownloads.Path);
            path = Anti_Path_Replace(path, "<win>", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            path = Anti_Path_Replace(path, "<recent>", Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            path = Anti_Path_Replace(path, "<profile>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Anti_Path_Replace(path, "<sendto>", Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
            path = Anti_Path_Replace(path, "<huser>", App.EnvironmentUsername);
            return path;
        }


        public static String Path_Prepare(String vpath)
        {
            var path = vpath;
            path = Path_Replace(path, "<current>", AppDomain.CurrentDomain.BaseDirectory);
            path = Path_Replace(path, "<system>", Environment.SystemDirectory);
            path = Path_Replace(path, "<systemx86>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.SystemX86.Path);
            path = Path_Replace(path, "<idesktop>", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            path = Path_Replace(path, "<ideskdir>", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            path = Path_Replace(path, "<cdeskdir>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
            path = Path_Replace(path, "<idocument>", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            path = Path_Replace(path, "<cdocument>", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            path = Path_Replace(path, "<iappdata>", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            path = Path_Replace(path, "<cappdata>", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            path = Path_Replace(path, "<imusic>", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            path = Path_Replace(path, "<cmusic>", Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic));
            path = Path_Replace(path, "<ipicture>", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            path = Path_Replace(path, "<cpicture>", Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));
            path = Path_Replace(path, "<istart>", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
            path = Path_Replace(path, "<cstart>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
            path = Path_Replace(path, "<istartup>", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            path = Path_Replace(path, "<cstartup>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            path = Path_Replace(path, "<ivideo>", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            path = Path_Replace(path, "<cvideo>", Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos));
            path = Path_Replace(path, "<istartup>", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            path = Path_Replace(path, "<cstartup>", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));
            path = Path_Replace(path, "<iprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            path = Path_Replace(path, "<cprogx86>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86));
            path = Path_Replace(path, "<iprog>", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            path = Path_Replace(path, "<cprog>", Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
            path = Path_Replace(path, "<idownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.Downloads.Path);
            path = Path_Replace(path, "<cdownload>", Microsoft.WindowsAPICodePack.Shell.KnownFolders.PublicDownloads.Path);
            path = Path_Replace(path, "<win>", Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            path = Path_Replace(path, "<recent>", Environment.GetFolderPath(Environment.SpecialFolder.Recent));
            path = Path_Replace(path, "<profile>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Path_Replace(path, "<sendto>", Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
            path = Path_Replace(path, "<huser>", App.EnvironmentUsername);
            return path;
        }
        #endregion

        #region 设置壁纸

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public enum Style : int
        {
            Fill,
            Fit,
            Span,
            Stretch,
            Tile,
            Center
        }
        #endregion

        #region 延时
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                System.Windows.Forms.Application.DoEvents();//可执行某无聊的操作
            }
        }
        #endregion


        #region 运行文件
        public static void RunExe(String path)
        {
            path = Path_Prepare(path);
            if(path.ToLower().StartsWith("hiroad("))
            {
                try
                {
                    String titile, mes, toolstr;
                    toolstr = path.Substring(7, path.LastIndexOf(")") - 7);
                    titile = toolstr.Substring(0, toolstr.IndexOf(","));
                    toolstr = toolstr.Substring(titile.Length + 1);
                    mes = toolstr.Substring(0, toolstr.IndexOf(","));
                    toolstr = toolstr.Substring(mes.Length + 1);
                    if (toolstr.Equals("<product>"))
                        toolstr = utils.Get_Transalte("dlproduct");
                    Download dl = new(1,toolstr);
                    dl.textBoxHttpUrl.Text = titile;
                    dl.SavePath.Text = mes;
                    dl.autorun.IsChecked = true;
                    dl.autorun.IsEnabled = false;
                    dl.Show();
                    dl.StartDownload();
                    return;
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    return;
                }
            }
            if(path.ToLower().StartsWith("download("))
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(9, path.LastIndexOf(")") - 9);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                            Download dl = new(0, "");
                            dl.textBoxHttpUrl.Text = titile;
                            dl.SavePath.Text = mes;
                            dl.Show();
                            return;
                        }
                        else
                        {
                            Download dl = new(0, "");
                            dl.textBoxHttpUrl.Text = titile;
                            dl.Show();
                            return;
                        }

                    }
                    else
                    {
                        Download dl = new(0, "");
                        dl.textBoxHttpUrl.Text = toolstr;
                        dl.Show();
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                    return;
                }
            }
            if (path.Length > 22 && path.Substring(0, 5).ToLower() == "save(")
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(5, path.LastIndexOf(")") - 5);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                        }
                        else
                        {
                            App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                            return;
                        }

                    }

                    else
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                    return;
                }
                BackgroundWorker bw = new();
                mes = Path_Prepare(mes);
                bw.DoWork += delegate
                {
                    toolstr = GetWebContent(titile, true, mes);
                };
                bw.RunWorkerCompleted += delegate
                {
                    if (toolstr.ToLower().Equals("error"))
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("error"), 2));
                    }
                    else
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("success"), 2));
                    }
                };
                bw.RunWorkerAsync();
                return;
            }
            if (path.ToLower().Equals("memory()"))
            {
                GC.Collect();
                return;
            }
            if (path.ToLower().Equals("debug()"))
            {
                App.dflag = !App.dflag;
                if(App.dflag)
                    path = "notify(" + Get_Transalte("debugon") + ",2)";
                else
                    path = "notify(" + Get_Transalte("debugoff") + ",2)";
            }
            if(path.ToLower() == "weather(0)")
            {
                path = "alarm(" + Get_Transalte("weather") + ",https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang=" + App.lang + ")";
            }
            if (path.ToLower() == "weather(1)")
            {
                path = "notify(https://api.rexio.cn/v1/rex.php?r=weather&k=6725dccca57b2998e8fc47cee2a8f72f&lang=" + App.lang + ",2)";
            }
            if (path.Length > 7 && path.Substring(0, 6).ToLower() == "debug(")
            {
                path = path.Substring(6);
                path = path.Substring(0, path.Length - 1);
                path = "notify(" + path + ")";
            }
            if (path.Length > 7 && path.Substring(0, 6).ToLower() == "alarm(")
            {
                path = path[6..];
                path = path[0..^1];
                if (path.IndexOf(",") != -1)
                {
                    var pa = path.Substring(0, path.IndexOf(","));
                    path = path.Substring(pa.Length + 1);
                    if (pa.ToLower().StartsWith("http://") || pa.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            pa = utils.GetWebContent(pa);
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            utils.RunExe("alarm(" + pa + "," + path + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (path.ToLower().StartsWith("http://") || path.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            path = utils.GetWebContent(path);
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            utils.RunExe("alarm(" + pa + "," + path.Replace("<br>", "\\n") + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (Read_Ini(App.dconfig, "Configuration", "toast", "0").Equals("1"))
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .AddArgument("Launch", App.AppTitle)
                            .AddText(pa)
                            .AddText(path.Replace("\\n", Environment.NewLine))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(utils.Get_Transalte("alarmone")))
                            .Show();
                    }
                    else
                    {
                        Alarm ala = new(-1, CustomedTitle: pa, CustomTitle: true, CustomedContnet: path.Replace("\\n", Environment.NewLine), CustomContent: true, OneButtonOnly: true);
                        ala.Show();
                    }
                }
                else
                {
                    if (path.ToLower().StartsWith("http://") || path.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            path = utils.GetWebContent(path);
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            utils.RunExe("alarm(" + path.Replace("<br>", "\\n") + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                    if (Read_Ini(App.dconfig, "Configuration", "toast", "0").Equals("1"))
                    {
                        new Microsoft.Toolkit.Uwp.Notifications.ToastContentBuilder()
                            .AddText(utils.Get_Transalte("alarmtitle"))
                            .AddText(path.Replace("\\n", Environment.NewLine))
                            .AddButton(new Microsoft.Toolkit.Uwp.Notifications.ToastButton()
                                        .SetContent(utils.Get_Transalte("alarmone")))
                            .Show();
                    }
                    else
                    {
                        Alarm ala = new(-1, CustomedContnet: path.Replace("\\n", Environment.NewLine), CustomContent: true, OneButtonOnly: true);
                        ala.Show();
                    }
                        
                }
                return;
            }
            if (path.Length > 4 && path.ToLower() == "hello" || path.ToLower() == "hello()")
            {
                var hr = DateTime.Now.Hour;
                var morning = utils.Read_Ini(App.LangFilePath, "local", "morning", "[6,7,8,9,10]");
                var noon = utils.Read_Ini(App.LangFilePath, "local", "noon", "[11,12,13]");
                var afternoon = utils.Read_Ini(App.LangFilePath, "local", "afternoon", "[14,15,16,17,18]");
                var evening = utils.Read_Ini(App.LangFilePath, "local", "evening", "[19,20,21,22]");
                var night = utils.Read_Ini(App.LangFilePath, "local", "night", "[23,0,1,2,3,4,5]");
                morning = "," + morning.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                noon = "," + noon.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                afternoon = "," + afternoon.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                evening = "," + evening.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                night = "," + night.Replace("[", "").Replace("]", "").Replace(" ", "") + ",";
                if (morning.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("morning").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(utils.Get_Transalte("morningcus").Replace("%u", App.Username), 2));

                }
                else if (noon.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("noon").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(utils.Get_Transalte("nooncus").Replace("%u", App.Username), 2));

                }
                else if (afternoon.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("afternoon").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(utils.Get_Transalte("afternooncus").Replace("%u", App.Username), 2));

                }
                else if (evening.IndexOf("," + hr + ",") != -1)
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("evening").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(utils.Get_Transalte("eveningcus").Replace("%u", App.Username), 2));
                }
                else
                {
                    if (App.CustomUsernameFlag == 0)
                        App.Notify(new noticeitem(utils.Get_Transalte("night").Replace("%u", App.EnvironmentUsername), 2));
                    else
                        App.Notify(new noticeitem(utils.Get_Transalte("nightcus").Replace("%u", App.Username), 2));
                }
                return;
            }
            if (path.Length > 2 && path.ToLower() == "nop" || path.ToLower() == "nop()")
            {
                return;
            }
            if (path.Length > 14 && path.Substring(0, 10).ToLower() == "wallpaper(")
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(10, path.LastIndexOf(")") - 10);
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr.Substring(0, toolstr.LastIndexOf(","));
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr.Substring(titile.Length + 1);
                        }
                        else
                        {
                            mes = "3";
                        }

                    }

                    else
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                        return;
                    }
                }
                else
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                    return;
                }
                using (Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    int[] para = new int[] { 10, 6, 22, 2, 0, 0 };
                    int[] par = new int[] { 0, 0, 0, 0, 1, 0 };
                    var v = Convert.ToInt32(mes);
                    if (v < 0)
                        v = 0;
                    if (v > 5)
                        v = 5;
                    if (key != null)
                    {
                        key.SetValue(@"WallpaperStyle", para[v].ToString());
                        key.SetValue(@"TileWallpaper", par[v].ToString());
                    }
                }
                SystemParametersInfo(20, 0, titile, 0x01 | 0x02);
                App.Notify(new noticeitem(utils.Get_Transalte("wpchanged"), 2));
                return;
            }
            if (path.Length > 11 && path.Substring(0, 6).ToLower() == "bingw(")
            {
                String toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(6, path.LastIndexOf(")") - 6);
                    if (toolstr.StartsWith("\""))
                        toolstr = toolstr.Substring(1);
                    if (toolstr.EndsWith("\""))
                        toolstr = toolstr.Substring(0, toolstr.Length - 1);
                    try
                    {
                        if (!System.IO.File.Exists(toolstr))
                        {
                            System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, "https://api.rexio.cn/v1/rex.php?r=wallpaper");
                            request.Headers.Add("UserAgent", "Rex/2.1.0 (Hiro Inside)");
                            request.Content = new System.Net.Http.StringContent("");
                            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                            System.ComponentModel.BackgroundWorker bw = new();
                            bw.DoWork += delegate {
                                try
                                {
                                    System.Net.Http.HttpResponseMessage response = App.hc.Send(request);

                                    if (response.Content != null)
                                    {
                                        System.IO.Stream stream = response.Content.ReadAsStream();
                                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                                        image.Save(toolstr);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show(ex.ToString(), utils.Get_Transalte("error") + " - " + App.AppTitle);
                                    utils.LogtoFile("[ERROR]" + ex.Message);
                                }
                            };
                            bw.RunWorkerCompleted += delegate
                            {
                                if (!System.IO.File.Exists(toolstr))
                                    App.Notify(new noticeitem(utils.Get_Transalte("unknown"), 2));
                                else
                                    App.Notify(new noticeitem(utils.Get_Transalte("wpsaved"), 2));
                            };
                            bw.RunWorkerAsync();
                        }
                        else
                            App.Notify(new noticeitem(utils.Get_Transalte("wpexist"), 2));
                    }
                    catch (Exception ex)
                    {
                        App.Notify(new noticeitem(utils.Get_Transalte("unknown"), 2));
                        System.Windows.MessageBox.Show(ex.ToString(), utils.Get_Transalte("error") + " - " + App.AppTitle);
                        utils.LogtoFile("[ERROR]" + ex.Message);

                    }
                }
                else
                {
                    App.Notify(new noticeitem(utils.Get_Transalte("syntax"), 2));
                    return;
                }
                return;
            }
            if (path.Length > 5 && path.Substring(0, 6).ToLower() == "exit()")
            {
                try
                {
                    System.Environment.Exit(System.Environment.ExitCode);
                    System.Windows.Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                    throw;
                }

            }
            if (path.Length > 5 && path.Substring(0, 6).ToLower() == "show()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    if (!Read_Ini(App.dconfig, "Configuration", "ani", "1").Equals("0"))
                    {
                        if (Read_Ini(App.dconfig, "Configuration", "blur", "0").Equals("1"))
                            Blur_Animation(true, true, App.mn.bgimage, App.mn);
                        else
                            Blur_Animation(false, true, App.mn.bgimage, App.mn);
                    }

                }
                else
                {
                    App.mn = new();
                    App.mn.Show();
                }  
                return;
            }
            if (path.Length > 5 && path.Substring(0, 6).ToLower() == "lock()")
            {
                if(App.mn != null)
                {
                    App.Locked = true;
                    if(0 < App.mn.tc.SelectedIndex && App.mn.tc.SelectedIndex < 4)
                        App.mn.Set_Label(App.mn.homex);
                    App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                }
                return;
            }
            if (path.Length > 5 && path.Substring(0, 6).ToLower() == "auth()")
            {
                BackgroundWorker sc = new();
                BackgroundWorker fa = new();
                sc.RunWorkerCompleted += delegate
                 {
                     if(App.mn != null)
                     {
                         App.mn.versionlabel.Content = App.AppVersion;
                         App.Locked = false;
                     }
                 };
                fa.RunWorkerCompleted += delegate
                {
                    if (App.mn != null)
                    {
                        App.mn.versionlabel.Content = App.AppVersion + " 🔒";
                    }
                };
                Register(sc, fa, fa);
                return;
            }
            if (path.Length > 5 && path[..6].ToLower() == "home()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.homex);
                }
                return;
            }
            if (path.Length > 5 && path[..6].ToLower() == "item()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.itemx);
                }
                return;
            }
            if (path.Length > 9 && path[..10].ToLower() == "schedule()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.schedulex);
                }
                return;
            }
            if (path.Length > 7 && path[..8].ToLower() == "config()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.helpx);
                }
                return;
            }
            if (path.Length > 5 && path[..6].ToLower() == "help()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.helpx);
                }
                return;
            }
            if (path.Length > 6 && path[..7].ToLower() == "about()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Visible;
                    App.mn.Set_Label(App.mn.aboutx);
                }
                return;
            }
            if (path.Length > 5 && path[..6].ToLower() == "hide()")
            {
                if (App.mn != null)
                {
                    App.mn.Visibility = System.Windows.Visibility.Hidden;
                }
                return;
            }
            if (path.Length > 8 && path[..9].ToLower() == "restart()")
            {
                if (App.mn != null)
                    App.mn.Close();
                App.mn = null;
                System.Threading.Thread.Sleep(100);
                App.mn = new();
                App.mn.Show();
                return;
            }
            if (path.Length > 5 && path.Substring(0, 6).ToLower() == "menu()")
            {
                if(App.cm != null)
                    App.cm.IsOpen = true;
                return;
            }
            if (path.Length > 7 && path.Substring(0, 8).ToLower() == "editor()")
            {
                if (App.ed == null)
                    App.ed = new Editor();
                App.ed.Show();
                return;
            }
            if (path.Length > 8 && path.Substring(0, 9).ToLower() == "lockscr()")
            {
                if (App.ls == null)
                    App.ls = new Lockscr();
                App.ls.Show();
                return;
            }
            if (path.Length > 8 && path.Substring(0, 8).ToLower() == "message(")
            {
                String toolstr;
                BackgroundWorker accept, reject, cancel;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path.Substring(8, path.LastIndexOf(")") - 8);
                    toolstr = Path_Prepare(toolstr);
                }
                else
                {
                    toolstr = "syntax error";
                }
                accept = new();
                reject = new();
                cancel = new();
                accept.RunWorkerCompleted += delegate
                {
                    utils.RunExe(utils.Read_Ini(toolstr, "Action", "accept", "nop"));
                };
                reject.RunWorkerCompleted += delegate
                {
                    utils.RunExe(utils.Read_Ini(toolstr, "Action", "reject", "nop"));
                };
                cancel.RunWorkerCompleted += delegate
                {
                    utils.RunExe(utils.Read_Ini(toolstr, "Action", "cancel", "nop"));
                };
                Background? bg = null;
                if (Read_Ini(toolstr, "Action", "Background", "true").ToLower().Equals("true"))
                    bg = new();
                message msg = new();
                msg.accept = accept;
                msg.reject = reject;
                msg.cancel = cancel;
                msg.bg = bg;
                msg.Title = utils.Read_Ini(toolstr, "Message", "title", utils.Get_Transalte("syntax")) + " - " + App.AppTitle;
                msg.backtitle.Content = utils.Read_Ini(toolstr, "Message", "title", utils.Get_Transalte("syntax"));
                msg.sv.Content = utils.Read_Ini(toolstr, "Message", "content", utils.Get_Transalte("syntax")).Replace("\\n", Environment.NewLine);
                msg.acceptbtn.Content = utils.Read_Ini(toolstr, "Message", "accept", utils.Get_Transalte("msgaccept"));
                msg.rejectbtn.Content = utils.Read_Ini(toolstr, "Message", "reject", utils.Get_Transalte("msgreject"));
                msg.cancelbtn.Content = utils.Read_Ini(toolstr, "Message", "cancel", utils.Get_Transalte("msgcancel"));
                msg.toolstr = toolstr;
                msg.Load_Position();
                msg.Show();
                return;
            }
            if (path.Length > 7 && path[..7].ToLower() == "notify(")
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path[7..path.LastIndexOf(")")];
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr[..toolstr.LastIndexOf(",")];
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr[(titile.Length + 1)..];
                        }
                        else
                        {
                            mes = "2";
                        }

                    }

                    else
                    {
                        titile = toolstr;
                        mes = "2";
                    }
                }
                else
                {
                    titile = utils.Get_Transalte("syntax");
                    mes = "2";
                }
                try
                {
                    int duration = Convert.ToInt32(mes);
                    if (titile.ToLower().StartsWith("http://") || titile.ToLower().StartsWith("https://"))
                    {
                        BackgroundWorker bw = new();
                        bw.DoWork += delegate
                        {
                            titile = utils.GetWebContent(titile).Replace("<br>", "\\n");
                        };
                        bw.RunWorkerCompleted += delegate
                        {
                            utils.RunExe("notify(" + titile + "," + duration.ToString() + ")");
                        };
                        bw.RunWorkerAsync();
                        return;
                    }
                        App.Notify(new(titile, duration));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString(), utils.Get_Transalte("error") + " - " + App.AppTitle);
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
                return;
            }

            if (path.Length > 4 && path.Substring(0, 4).ToLower() == "seq(")
            {
                String toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path[4..path.LastIndexOf(")")];
                    if (toolstr.StartsWith("\""))
                        toolstr = toolstr[1..];
                    if (toolstr.EndsWith("\""))
                        toolstr = toolstr[0..^1];
                    if (!System.IO.File.Exists(toolstr))
                    {
                        App.Notify(new(Get_Transalte("syntax"), 2));
                        return;
                    }
                    Sequence sq = new();
                    sq.Show();
                    sq.SeqExe(toolstr);
                    
                    
                }
                else
                {
                    App.Notify(new(Get_Transalte("syntax"), 2));
                    return;
                }
                return;
            }

            if (path.Length > 4 && path[..4].ToLower() == "run(")
            {
                String titile, mes, toolstr;
                if (path.LastIndexOf(")") != -1)
                {
                    toolstr = path[4..path.LastIndexOf(")")];
                    if (toolstr.LastIndexOf(",") != -1)
                    {
                        titile = toolstr[..toolstr.LastIndexOf(",")];
                        if (titile.Length < toolstr.Length - 1)
                        {
                            mes = toolstr[(titile.Length + 1)..];
                        }
                        else
                        {
                            App.Notify(new noticeitem(Get_Transalte("syntax"), 2));
                            return;
                        }

                    }

                    else
                    {
                        App.Notify(new(Get_Transalte("syntax"), 2));
                        return;
                    }
                }
                else
                {
                    App.Notify(new(Get_Transalte("syntax"), 2));
                    return;
                }
                try
                {
                    ProcessStartInfo pinfo = new();
                    pinfo.UseShellExecute = true;
                    List<string> blank = new();
                    var a = 0;
                    while (titile.IndexOf("\"") != -1)
                    {
                        a++;
                        var lef = titile[..titile.IndexOf("\"")];
                        titile = titile[(lef.Length + 1)..];
                        if (titile.IndexOf("\"") == -1)
                            break;
                        var inside = titile[..titile.IndexOf("\"")];
                        titile = titile[(inside.Length + 1)..];
                        blank.Add(inside);
                        titile = lef + "[" + a.ToString() + "]" + titile;
                    }
                    if (titile.IndexOf(" ") == -1)
                    {
                        pinfo.FileName = titile;
                    }
                    else
                    {
                        pinfo.FileName = titile.Substring(0, titile.IndexOf(" "));
                        pinfo.Arguments = titile.Substring(titile.IndexOf(" ") + 1);
                    }
                    a = 1;
                    while (blank.Count > 0 && pinfo.FileName.IndexOf("[" + a.ToString() + "]") != -1)
                    {
                        pinfo.FileName = pinfo.FileName.Replace("[" + a.ToString() + "]", blank[0]);
                        blank.RemoveAt(0);
                        a++;
                    }
                    while (blank.Count > 0 && pinfo.Arguments.IndexOf("[" + a.ToString() + "]") != -1)
                    {
                        pinfo.Arguments = pinfo.Arguments.Replace("[" + a.ToString() + "]", "\"" + blank[0]) + "\"";
                        blank.RemoveAt(0);
                        a++;
                    }
                    if (mes.StartsWith("\""))
                        mes = mes.Substring(1);
                    if (mes.EndsWith("\""))
                        mes = mes.Substring(0, mes.Length - 1);
                    if (titile.StartsWith("\""))
                        titile = titile.Substring(1);
                    if (mes.EndsWith("\""))
                        titile = titile.Substring(0, titile.Length - 1);
                    mes = mes.ToLower();
                    if (mes.IndexOf("a") != -1)
                        pinfo.Verb = "runas";
                    if (mes.IndexOf("h") != -1)
                        pinfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    if (mes.IndexOf("i") != -1)
                        pinfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                    if (mes.IndexOf("x") != -1)
                        pinfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                    if (mes.IndexOf("n") != -1)
                        pinfo.CreateNoWindow = true;
                    //启动进程
                    Process? p = Process.Start(pinfo);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString(), utils.Get_Transalte("error") + " - " + App.AppTitle);
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
                return;
            }

            Console.WriteLine(path);
            try
            {
                ProcessStartInfo pinfo = new();
                pinfo.UseShellExecute = true;
                List<string> blank = new();
                var a = 0;
                while (path.IndexOf("\"") != -1)
                {
                    a++;
                    var lef = path[..path.IndexOf("\"")];
                    path = path[(lef.Length + 1)..];
                    if (path.IndexOf("\"") == -1)
                        break;
                    var inside = path[..path.IndexOf("\"")];
                    path = path[(inside.Length + 1)..];
                    blank.Add(inside);
                    path = lef + "[" + a.ToString() + "]" + path;
                }
                if (path.IndexOf(" ") == -1)
                {
                    pinfo.FileName = path;
                }
                else
                {
                    pinfo.FileName = path[..path.IndexOf(" ")];
                    pinfo.Arguments = path[(path.IndexOf(" ") + 1)..];
                }
                a = 1;
                while (blank.Count > 0 && pinfo.FileName.IndexOf("[" + a.ToString() + "]") != -1)
                {
                    pinfo.FileName = pinfo.FileName.Replace("[" + a.ToString() + "]", blank[0]);
                    blank.RemoveAt(0);
                    a++;
                }
                while (blank.Count > 0 && pinfo.Arguments.IndexOf("[" + a.ToString() + "]") != -1)
                {
                    pinfo.Arguments = pinfo.Arguments.Replace("[" + a.ToString() + "]", "\"" + blank[0]) + "\"";
                    blank.RemoveAt(0);
                    a++;
                }
                //启动进程
                Process? p = Process.Start(pinfo);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), utils.Get_Transalte("error") + " - " + App.AppTitle);
                utils.LogtoFile("[ERROR]" + ex.Message);
            }

        }
        #endregion

        #region Windows Hello
        private async static void NewUser(String AccountId, BackgroundWorker success, BackgroundWorker falied, BackgroundWorker cancel)
        {
            var keyCreationResult = await Windows.Security.Credentials.KeyCredentialManager.RequestCreateAsync(AccountId, Windows.Security.Credentials.KeyCredentialCreationOption.FailIfExists);
            if (keyCreationResult.Status == KeyCredentialStatus.CredentialAlreadyExists)
            {
                //label5.Text = "User Already Created.";
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    success.RunWorkerAsync();
                }
                else
                {
                    falied.RunWorkerAsync();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.Success)
            {
                
                var userKey = keyCreationResult.Credential;

                var keyAttestationResult = await userKey.GetAttestationAsync();
                /*KeyCredentialAttestationStatus keyAttestationRetryType = 0;*/

                if (keyAttestationResult.Status == KeyCredentialAttestationStatus.Success)
                {
                    success.RunWorkerAsync();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.TemporaryFailure)
                {
                    falied.RunWorkerAsync();
                }
                else if (keyAttestationResult.Status == KeyCredentialAttestationStatus.NotSupported)
                {
                    success.RunWorkerAsync();
                }
            }
            else if (keyCreationResult.Status == KeyCredentialStatus.UserCanceled ||
                keyCreationResult.Status == KeyCredentialStatus.UserPrefersPassword)
            {
                cancel.RunWorkerAsync();
            }
        }
        private async void DeleteUser(String AccountId)
        {
            var openKeyResult = await KeyCredentialManager.OpenAsync(AccountId);
            if (openKeyResult.Status == KeyCredentialStatus.Success)
            {
                UserConsentVerificationResult consentResult = await UserConsentVerifier.RequestVerificationAsync(AccountId);
                if (consentResult.Equals(UserConsentVerificationResult.Verified))
                {
                    // continue
                    //label5.Text = "User Verified.\nUser Deleted.";
                    await KeyCredentialManager.DeleteAsync(AccountId);
                }
                else
                {
                    //label5.Text = "User Verifying falied.";
                }
                return;
            }
            if (openKeyResult.Status == KeyCredentialStatus.NotFound)
            {
                //label5.Text = AccountId + " doesn't exist!\n";
                return;
            }
            //label5.Text = "Error Occurred.";
            return;
        }
        public async static void Register(BackgroundWorker success, BackgroundWorker falied, BackgroundWorker cancel)
        {
            var keyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();
            if (!keyCredentialAvailable)
            {
                success.RunWorkerAsync();
                return;
            }
            NewUser("N+@" + App.EnvironmentUsername, success, falied, cancel);
            //Auth(null, "aki-helper@" + textBox1.Text);
        }
        #endregion
        #region 设置窗口阴影
        private const int CS_DropSHADOW = 0x20000;
        private const int GCL_STYLE = (-26);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        public static void SetShadow(IntPtr hwnd)
        {
            SetClassLong(hwnd, GCL_STYLE, GetClassLong(hwnd, GCL_STYLE) | CS_DropSHADOW);
        }

        #endregion

        #region 获取系统电量

        public struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);
        #endregion

        #region 获取壁纸

        

        #endregion

        #region 模糊动画
        public static void Blur_Animation(bool direction,bool animation, System.Windows.Controls.Label label, System.Windows.Window win)
        {
            double rd = App.blurradius;
            if (!animation)
            {
                if (direction)
                {
                    Set_Animation_Label(rd, label, win);
                }
                else
                {
                    rd = 0.0;
                    Set_Animation_Label(rd, label, win);
                    label.Effect = null;
                }
            }
            else
            {
                double step = rd / App.blursec;
                if (direction)
                {
                    var rds = 0.0;
                    while (rds < rd)
                    {
                        rds += step;
                        if (rds > rd)
                            rds = rd;
                        Set_Animation_Label(rds, label, win);
                        Delay(App.blurdelay);
                    }
                }
                else
                {
                    while(rd > 0.0)
                    {
                        rd -= step;
                        if (rd < 0.0)
                            rd = 0.0;
                        Set_Animation_Label(rd, label, win);
                        Delay(App.blurdelay);
                    }
                    label.Effect = null;
                }
            }
        }
        private static void Set_Animation_Label(double rd, System.Windows.Controls.Label label, System.Windows.Window win)
        {
            label.Effect = new System.Windows.Media.Effects.BlurEffect()
            {
                Radius = rd,
                RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance
            };
            System.Windows.Thickness tn = label.Margin;
            if (win.Width > win.Height)
            {
                tn.Top = -rd;
                tn.Left = -rd * win.Width / win.Height;
            }
            else
            {
                tn.Left = -rd;
                tn.Top = -rd * win.Height / win.Width;
            }
            label.Margin = tn;
            label.Width = win.Width - tn.Left * 2;
            label.Height = win.Height - tn.Top * 2;
        }
        #endregion

        #region 获取命令翻译
        public static string Get_CMD_Translation(string cmd)
        {
            string a = "";
            int b = 1;
            string c = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_cmd", " ");
            string d = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_content", " ").Replace("\\n", Environment.NewLine);
            while (!c.Equals(" ") && !d.Equals(" "))
            {
                if(cmd.ToLower().StartsWith(c.ToLower()))
                {
                    a = d;
                    break;
                }
                b++;
                c = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_cmd", " ");
                d = Read_Ini(App.LangFilePath, "Command", b.ToString() + "_content", " ").Replace("\\n", Environment.NewLine);
            }
            return a;
        }
        #endregion

        #region 检查更新
        public static string GetWebContent(String url, bool save = false,String? savepath = null)
        {
            System.Net.Http.HttpRequestMessage request = new(System.Net.Http.HttpMethod.Get, url);
            request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
            request.Content = new System.Net.Http.StringContent("");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Options.TryAdd("AllowAutoRedirect", true);
            request.Options.TryAdd("KeppAlive", true);
            request.Options.TryAdd("ProtocolVersion", System.Net.HttpVersion.Version11);
            //这里设置协议
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
            System.Net.ServicePointManager.CheckCertificateRevocationList = true;
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Net.ServicePointManager.Expect100Continue = false;

            try
            {
                System.Net.Http.HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    System.IO.Stream stream = response.Content.ReadAsStream();
                    string result = string.Empty;
                    if (save == true && savepath != null)
                    {
                        try
                        {
                            using (var fileStream = System.IO.File.Create(savepath))
                            {
                                stream.CopyTo(fileStream);
                            }
                            return "saved";
                        }
                        catch (Exception ex)
                        {
                            utils.LogtoFile("[ERROR]" + ex.Message);
                            return "error";
                        }
                    }
                    else
                    {
                        System.IO.StreamReader sr = new(stream);
                        result = sr.ReadToEnd();
                        return result;
                    }
                }
                else
                {
                    utils.LogtoFile("[ERROR]Response is null");
                    return Get_Transalte("error");
                }
            }
            catch (Exception ex)
            {
                utils.LogtoFile("[ERROR]" + ex.Message);
                return Get_Transalte("error");
            }


        }
        private static bool CheckValidationResult()
        {
            return true;
        }
        #endregion

        [System.Runtime.InteropServices.DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        public static void Move_Window(IntPtr handle)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        #region 获取用户头像
        [System.Runtime.InteropServices.DllImport("shell32.dll", EntryPoint = "#261",
           CharSet = System.Runtime.InteropServices.CharSet.Unicode, PreserveSig = false)]
        public static extern void GetUserTilePath(
          string username,
          UInt32 whatever, // 0x80000000
          StringBuilder picpath, int maxLength);

        public static string GetUserTilePath(string username)
        {   // username: use null for current user
            var sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }
        #endregion

        #region 闹钟功能

        public static void OK_Alarm(int id)
        {
            if(App.dflag)
                utils.LogtoFile("[DEBUG]Alarm ID " + id.ToString());
            if (id > -1)
            {
                System.Globalization.DateTimeFormatInfo dtFormat = new();
                dtFormat.ShortDatePattern = "yyyy/MM/dd HH:mm:ss";
                try
                {
                    DateTime dt = Convert.ToDateTime(App.scheduleitems[id].time, dtFormat);
                    switch (App.scheduleitems[id].re)
                    {
                        case -2.0:
                            break;
                        case -1.0:
                            App.scheduleitems[id].time = dt.AddDays(1.0).ToString("yyyy/MM/dd HH:mm:ss");
                            utils.Write_Ini(App.sconfig, (id + 1).ToString(), "time", App.scheduleitems[id].time);
                            break;
                        case 0.0:
                            App.scheduleitems[id].time = dt.AddDays(7.0).ToString("yyyy/MM/dd HH:mm:ss");
                            utils.Write_Ini(App.sconfig, (id + 1).ToString(), "time", App.scheduleitems[id].time);
                            break;
                        default:
                            App.scheduleitems[id].time = dt.AddDays(Math.Abs(App.scheduleitems[id].re)).ToString("yyyy/MM/dd HH:mm:ss");
                            utils.Write_Ini(App.sconfig, (id + 1).ToString(), "time", App.scheduleitems[id].time);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    utils.LogtoFile("[ERROR]" + ex.Message);
                }
                
            }
        }

        public static void Delete_Alarm(int id)
        {
            var inipath = App.sconfig;
            if (id > -1)
            { 
                while (id < App.scheduleitems.Count - 1)
                {
                    App.scheduleitems[id].name = App.scheduleitems[id + 1].name;
                    App.scheduleitems[id].command = App.scheduleitems[id + 1].command;
                    App.scheduleitems[id].time = App.scheduleitems[id + 1].time;
                    utils.Write_Ini(inipath, (id + 1).ToString(), "name", utils.Read_Ini(inipath, (id + 2).ToString(), "name", " "));
                    utils.Write_Ini(inipath, (id + 1).ToString(), "command", utils.Read_Ini(inipath, (id + 2).ToString(), "command", " "));
                    utils.Write_Ini(inipath, (id + 1).ToString(), "time", utils.Read_Ini(inipath, (id + 2).ToString(), "time", " "));
                    id++;
                    System.Windows.Forms.Application.DoEvents();
                }
                utils.Write_Ini(inipath, (id + 1).ToString(), "name", " ");
                utils.Write_Ini(inipath, (id + 1).ToString(), "command", " ");
                utils.Write_Ini(inipath, (id + 1).ToString(), "time", " ");
                App.scheduleitems.RemoveAt(id);
            }
            else
                App.Notify(new noticeitem(utils.Get_Transalte("alarmmissing"), 2));
            
        }

            public static void Delay_Alarm(int id)
            {
                if (id > -1)
                    App.scheduleitems[id].time = DateTime.Now.AddMinutes(5.0).ToString("yyyy/MM/dd HH:mm:ss");
                else
                    App.Notify(new noticeitem(utils.Get_Transalte("alarmmissing"), 2));
            }
        #endregion
    }


}
