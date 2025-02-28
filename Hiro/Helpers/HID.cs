﻿using Hiro.Resources;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using static Hiro.Helpers.HSet;

namespace Hiro.Helpers
{
    internal class HID
    {
        internal static string version = "v1";
        #region 个人资料操作
        public static string Login(string account, string pwd, bool token = false, string? saveto = null)
        {
            var url = "https://id.rexio.cn/login.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(HText.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                string t = token ? "token" : "pwd";
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"account\"" + Enter + Enter + "" + account + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"pwd\"" + Enter + Enter + "" + pwd + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"lang\"" + Enter + Enter + "" + App.lang + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + t + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[eof.Length];
                eof.CopyTo(ndata, 0);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    if (saveto != null)
                    {
                        try
                        {
                            using (Stream stream = response.Content.ReadAsStream())
                            {
                                using (var fileStream = File.Create(saveto))
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                            return "success";
                        }
                        catch (Exception ex)
                        {
                            HLogger.LogError(ex, "Hiro.Exception.Login.Save");
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            string result = string.Empty;
                            using (StreamReader sr = new(stream))
                            {
                                result = sr.ReadToEnd();
                                return result;
                            }
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Login");
                return "error";
            }
        }

        public static void Logout()
        {
            App.Logined = false;
            App.loginedToken = string.Empty;
            App.username = App.eUserName;
            App.CustomUsernameFlag = 0;
            Write_Ini(App.dConfig, "Config", "Token", string.Empty);
            Write_Ini(App.dConfig, "Config", "AutoLogin", "false");
            Write_Ini(App.dConfig, "Config", "CustomUser", "false");
            Write_Ini(App.dConfig, "Config", "CustomName", string.Empty);
            Write_Ini(App.dConfig, "Config", "CustomSign", string.Empty);
            Write_Ini(App.dConfig, "Config", "UserAvatarStyle", string.Empty);
        }

        public static string UploadProfileImage(string file, string user, string token, string type)
        {
            var url = "https://hi.rex.as/chat/upload.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(HText.Get_Translate("webnotinitial"));
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                byte[] bytebuffer;
                bytebuffer = new byte[fs.Length];
                fs.Read(bytebuffer, 0, (int)fs.Length);
                fs.Close();
                string filename = Path.GetFileName(file);
                filename = filename ?? "null";
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    "--" + boundary + Enter + "Content-Type: application/octet-stream" + Enter + "Content-Disposition: form-data; filename=\"" + "" + filename + "" + "\"; name=\"file\"" + Enter + Enter
                    );
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"type\"" + Enter + Enter + "" + type + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"md5\"" + Enter + Enter + "" + HFile.GetMD5(file).Replace("-", "") + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[send.Length + bytebuffer.Length + eof.Length];
                send.CopyTo(ndata, 0);
                bytebuffer.CopyTo(ndata, send.Length);
                eof.CopyTo(ndata, send.Length + bytebuffer.Length);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    using (Stream stream = response.Content.ReadAsStream())
                    {
                        string result = string.Empty;
                        using (StreamReader sr = new(stream))
                        {
                            result = sr.ReadToEnd();
                            if (App.dflag)
                                HLogger.LogtoFile($"Web result: {result}");
                            return result;
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Update.Profile");
                return "error";
            }
        }

        public static string UploadProfileSettings(string user, string token, string name, string signature, string avatar, string iavatar, string back, string verifiedID, string verifiedPic, string affID, string affPic, string method = "update", string? saveto = null)
        {
            var url = "https://hi.rex.as/chat/update.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(HText.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"name\"" + Enter + Enter + "" + name + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"sign\"" + Enter + Enter + "" + signature + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"avatar\"" + Enter + Enter + "" + avatar + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"iavatar\"" + Enter + Enter + "" + iavatar + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"back\"" + Enter + Enter + "" + back + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"verifiedID\"" + Enter + Enter + "" + verifiedID + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"verifiedPic\"" + Enter + Enter + "" + verifiedPic + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"affID\"" + Enter + Enter + "" + affID + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"affPic\"" + Enter + Enter + "" + affPic + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"method\"" + Enter + Enter + "" + method + "" + Enter + "--" + boundary + "--"
                    );
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(send);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    if (method.Equals("check") && saveto != null)
                    {
                        try
                        {
                            using (Stream stream = response.Content.ReadAsStream())
                            {
                                using (var fileStream = File.Create(saveto))
                                {
                                    stream.CopyTo(fileStream);
                                }
                            }
                            return "success";
                        }
                        catch (Exception ex)
                        {
                            HLogger.LogError(ex, "Hiro.Exception.Update.Profile.Settings.Save");
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            string result = string.Empty;
                            using (StreamReader sr = new(stream))
                            {
                                result = sr.ReadToEnd();
                                return result;
                            }
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Update.Profile.Settings");
                return "error";
            }
        }

        public static string SyncProfile(string user, string token)
        {
            var url = "https://hi.rex.as/chat/sync.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(HText.Get_Translate("webnotinitial"));
                var usrAvatar = "<hiapp>\\images\\avatar\\" + user + ".hap";
                var usrBack = "<hiapp>\\images\\background\\" + user + ".hpp";
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] send = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"version\"" + Enter + Enter + "" + version + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"iavatar\"" + Enter + Enter + "" + HFile.GetMD5(HText.Path_PPX(usrAvatar)).Replace("-", string.Empty) + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"back\"" + Enter + Enter + "" + HFile.GetMD5(HText.Path_PPX(usrBack)).Replace("-", string.Empty) + "" + Enter + "--" + boundary + "--" + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"name\"" + Enter + Enter + "" + Read_DCIni("CustomName", string.Empty) + "" + Enter + "--" + boundary + "--" + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"sign\"" + Enter + Enter + "" + Read_DCIni("CustomSign", string.Empty) + "" + Enter + "--" + boundary + "--" + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"avatar\"" + Enter + Enter + "" + Read_DCIni("UserAvatarStyle", string.Empty) + "" + Enter + "--" + boundary + "--"
                    );
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(send);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    var saveto = Path.GetTempFileName();
                    try
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            using (var fileStream = File.Create(saveto))
                            {
                                stream.CopyTo(fileStream);
                            }
                        }
                        if (App.dflag)
                            HLogger.LogtoFile(File.ReadAllText(saveto));
                        Write_Ini(App.dConfig, "Config", "CustomUser", "true");
                        UpdateUserInfo(saveto, "Name", "CustomName");
                        UpdateUserInfo(saveto, "Sign", "CustomSign");
                        UpdateUserInfo(saveto, "Avatar", "UserAvatarStyle");
                        App.username = Read_DCIni("CustomName", string.Empty);
                        App.CustomUsernameFlag = 1;
                        Write_Ini(App.dConfig, "Config", "UserAvatar", usrAvatar);
                        Write_Ini(App.dConfig, "Config", "UserBackground", usrBack);
                        usrAvatar = HText.Path_Prepare(usrAvatar);
                        usrBack = HText.Path_Prepare(usrBack);
                        HFile.CreateFolder(usrAvatar);
                        HFile.CreateFolder(usrBack);
                        var _link = Read_Ini(saveto, "Profile", "Iavatar", string.Empty);
                        var _bak = usrAvatar + ".bak";
                        if (!_link.Equals(string.Empty))
                        {
                            if (File.Exists(_bak))
                                File.Delete(_bak);
                            if (File.Exists(usrAvatar))
                                File.Move(usrAvatar, _bak);
                            if (HNet.GetWebContent(_link, true, usrAvatar).Equals("saved"))
                                File.Delete(_bak);
                            else if (File.Exists(_bak))
                                File.Move(_bak, usrAvatar);
                        }
                        _link = Read_Ini(saveto, "Profile", "Back", string.Empty);
                        _bak = usrBack + ".bak";
                        if (!_link.Equals(string.Empty))
                        {
                            if (File.Exists(_bak))
                                File.Delete(_bak);
                            if (File.Exists(usrBack))
                                File.Move(usrBack, _bak);
                            if (HNet.GetWebContent(_link, true, usrBack).Equals("saved"))
                                File.Delete(_bak);
                            else if (File.Exists(_bak))
                                File.Move(_bak, usrBack);
                        }
                        if (File.Exists(saveto))
                            File.Delete(saveto);
                        return "success";
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Update.Profile.Sync.Save");
                        if (File.Exists(saveto))
                            File.Delete(saveto);
                        throw new Exception(ex.Message);
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Update.Profile.Sync");
                return "error";
            }
        }

        private static void UpdateUserInfo(string saveto, string saveSection, string userSection)
        {
            var a = Read_Ini(saveto, "Profile", saveSection, string.Empty);
            if (!a.Equals(string.Empty))
            {
                Write_Ini(App.dConfig, "Config", userSection, a);
            }
        }

        public static string SendMsg(string user, string token, string to, string content)
        {
            var url = "https://hi.rex.as/chat/send.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(HText.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"to\"" + Enter + Enter + "" + to + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"content\"" + Enter + Enter + "" + content + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[eof.Length];
                eof.CopyTo(ndata, 0);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    using (Stream stream = response.Content.ReadAsStream())
                    {
                        string result = string.Empty;
                        using (StreamReader sr = new(stream))
                        {
                            result = sr.ReadToEnd();
                            return result;
                        }
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Chat.Send");
                return "error";
            }
        }


        public static string GetChat(string user, string token, string to, string saveto)
        {
            var url = "https://hi.rex.as/chat/log.php";
            try
            {
                if (App.hc == null)
                    throw new Exception(HText.Get_Translate("webnotinitial"));
                string boundary = DateTime.Now.Ticks.ToString("X");
                string Enter = "\r\n";
                byte[] eof = Encoding.UTF8.GetBytes(
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"user\"" + Enter + Enter + "" + user + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"token\"" + Enter + Enter + "" + token + "" + Enter + "--" + boundary + "--" +
                    Enter + "--" + boundary + Enter + "Content-Type: text/plain" + Enter + "Content-Disposition: form-data; name=\"to\"" + Enter + Enter + "" + to + "" + Enter + "--" + boundary + "--"
                    );
                byte[] ndata = new byte[eof.Length];
                eof.CopyTo(ndata, 0);
                HttpRequestMessage request = new(HttpMethod.Post, url);
                request.Headers.Add("UserAgent", Hiro_Resources.AppUserAgent);
                request.Content = new ByteArrayContent(ndata);
                request.Content.Headers.Remove("Content-Type");
                request.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data;boundary=" + boundary);
                HttpResponseMessage response = App.hc.Send(request);
                if (response.Content != null)
                {
                    try
                    {
                        using (Stream stream = response.Content.ReadAsStream())
                        {
                            using (var fileStream = File.Create(saveto))
                            {
                                stream.CopyTo(fileStream);
                            }
                        }
                        return "success";
                    }
                    catch (Exception ex)
                    {
                        HLogger.LogError(ex, "Hiro.Exception.Chat.Get.Save");
                        throw new Exception(ex.Message);
                    }
                }
                else
                    return "null";
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Chat.Get");
                return "error";
            }
        }
        #endregion
    }
}
