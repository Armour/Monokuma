/*
 * Monokuma by Magimagi with love @@ C# (.NET 4.0/4.5)
 * Thank Jennings Wu ( http://jenningswu.net ) For add-ons @@ Java
 * Thank QB10032 For Question Library
 * Thank all kind people love & teach Monokuma
 * Finished @ Jan, 30th, 2014
 * All right shared.
 * WARNING: UTF-8 USED ONLY PLZ !
 * AND PARTLY DEPEND ON Ruby & Java & C DYNAMICLY
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Security;
using System.Security.Permissions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Diagnostics;

//=======================Check VerifyImg=============================//
// GET https://ssl.ptlogin2.qq.com/check?uin=$QQNUMBER&appid=1003903&js_ver=10067&js_type=0&login_sig=EPcCP3Vlw6IeqpdkjwwQjnONKu83rutIiMmtDZVEAXbvksFVuX5723G6fC2am*lA&u1=http%3A%2F%2Fweb2.qq.com%2Floginproxy.html&r=0.4277090684045106 HTTP/1.1
// Host: ssl.ptlogin2.qq.com
// Connection: keep-alive
// Accept: */*
// User-Agent: Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36
// Referer: https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=5&mibao_css=m_webqq&appid=1003903&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fweb2.qq.com%2Floginproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131202001
// Accept-Encoding: gzip,deflate,sdch
// Accept-Language: zh-CN,zh;q=0.8
// Cookie: pt2gguin=o0408181916; ptui_loginuin=408181916; RK=yHV7VbtA2p; o_cookie=408181916; pt2gguin=o0408181916; ptcz=7c9256d151c1877b9d3c674b8fdd4e81eafadf9bb6d25045d79dd6a5ecaa04f5; pgv_pvid=9549369484; pgv_info=pgvReferrer=&ssid=s167571296; chkuin=408181916
//===================================================================//

namespace PerfectSlumbers
{
    public partial class Form1 : Form
    {
        // =====================Settings======================= //
        //private bool Release = false;
        private string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
        private string[] IntiFlag = new string[] { "!None", "!Low", "!Mid", "!High", "!VeryHigh", "!Max", "!Overflow" };
        private string[] UnhappyString = new string[] { "你妈", "傻逼", "煞笔", "逗比", "逗逼", "二逼", "傻比", "傻屄", "傻比", "滚", "垃圾", "鸡巴", "去死", "CNM", "cnm", "mlgb", "MLGB", "老子", "蠢熊", "我操", "我草", "卧槽" };
        private int[] IntiNeed = new int[] { 0, 20, 40, 60, 80, 100, 200 };
        private IDictionary<string, int> FlagToInt = new Dictionary<string, int>();
        private int NowKumaMode = KumaMode.NormalMode;
        // ==================================================== //

        // ======================Threads======================= //
        Thread Poll2Thread;
        Thread SolveMsgThread;
        Thread GetLogoThread;
        Thread ClockReportThread;
        bool bPoll2Thread = false;
        bool bSolveMsgThread = false;
        bool bGetLogoThread = false;
        // ==================================================== //

        // =======================Vars========================= //
        private CookieContainer SetCookies = new CookieContainer();
        private string VerifyCode = null;
        private string Pt_Uin = null;
        private string Ptwebqq = null;
        private string NewLocation = null;
        private string QQName = null;
        private LoginRetInfo LoginInfo = null;
        private PollRetInfo PollMsg = null;
        private int ClientID;
        private int MsgID;
        private int IgnoreTime;
        private int RetryTime = 0;
        private Random ResponseRand = new Random();
        private int Seconds = 0;
        private int Calc24Seconds = -1;
        private int ProblemSeconds = -1;
        private string ProblemUserQQ = "";
        private int ProblemUserInti = 0;
        private int Vitality = 100;
        private bool Calc24Solved = false;
        private int A, B, C, D = 0;
        private string Answer = "";
        private string AnswerLine = "";
        private string ChooseFrom = "";
        private string MessageTail = "";
        private List<Equation> Calc24Ans = new List<Equation>();
        private List<string> ChattedGroupUin = new List<string>();
        // ==================================================== //

        // ======================Knowledges==================== //
        private IDictionary<string, string> UintoQQ = new Dictionary<string, string>();
        private IDictionary<string, List<KnowledgePack>> FullMatchKnowledges = new Dictionary<string, List<KnowledgePack>>();
        private IDictionary<List<string>, List<KnowledgePack>> WordsMatchKnowledges = new Dictionary<List<string>, List<KnowledgePack>>();
        private IDictionary<string, int> Intimacy = new Dictionary<string, int>();
        private IDictionary<string, string> Nickname = new Dictionary<string, string>();
        // ==================================================== //

        public Form1()
        {
            InitializeComponent();
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 进行Http请求
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="parameters">POST Data</param>
        /// <param name="method">Method: POST/GET</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="userAgent">UserAgent</param>
        /// <param name="requestEncoding">Set Encoding</param>
        /// <param name="cookies">cookies</param>
        /// <param name="dosetcookie">是否处理response的set-cookie</param>
        /// <param name="allowredirect">是否允许自动跳转</param>
        /// <returns></returns>
        public byte[] CreateHttpRequest(string url, IDictionary<string, string> parameters, string method, int? timeout, string userAgent, Encoding requestEncoding, CookieContainer cookies, bool dosetcookie, bool allowredirect)
        {
            //Check Url
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            //Init Encoding
            Encoding encoding = requestEncoding == null ? Encoding.UTF8 : requestEncoding;

            //Create WebRequest
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(Form1.CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }

            //Set Method
            if (method != "POST" && method != "GET")
                throw new ArgumentException("method");
            else
            {
                request.Method = method;
                if (method == "GET")
                    request.ContentType = null;
            }

            //Set Accept
            request.Accept = "*/*";

            //Set Accept-Encoding
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");

            //Set Accept-Language
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-CN,zh;q=0.8");

            //Set AutomaticDecompression
            request.AutomaticDecompression = DecompressionMethods.GZip;

            //Set User-Agent
            request.UserAgent = string.IsNullOrEmpty(userAgent) ? this.DefaultUserAgent : userAgent;

            //Set TimeOut ?
            if (timeout.HasValue)
                request.Timeout = timeout.Value;

            //Set Null Except
            request.Expect = null;

            //Set Cookies
            if (cookies != null)
                request.CookieContainer = cookies;

            //Set Parameters
            StringBuilder builder = new StringBuilder();
            if ((parameters != null) && (parameters.Count != 0))
            {
                int num = 0;
                foreach (string str in parameters.Keys)
                {
                    string str2 = HttpUtility.UrlEncode(parameters[str]);
                    if (num <= 0)
                        builder.AppendFormat("{0}={1}", str, str2);
                    else
                        builder.AppendFormat("&{0}={1}", str, str2);
                    ++num;
                }
            }

            //Set AllowAutoRedirect
            request.AllowAutoRedirect = allowredirect;

            //Set Proxy
            //if (!Release)
            //request.Proxy = new WebProxy("127.0.0.1", 6974);

            //Set Expect100Continue
            request.ServicePoint.Expect100Continue = false;

            //Set Origin
            if (url.Equals("https://d.web2.qq.com/channel/poll2") || url.Equals("https://d.web2.qq.com/channel/send_buddy_msg2")
                || url.Equals("https://d.web2.qq.com/channel/send_qun_msg2"))
                request.Headers.Add("Origin", "https://d.web2.qq.com");

            //Set Referer
            if (url.Equals("https://d.web2.qq.com/channel/login2"))
                request.Referer = "http://d.web2.qq.com/channel/login2";
            if (url.Equals("https://d.web2.qq.com/channel/poll2") || url.Equals("https://d.web2.qq.com/channel/send_buddy_msg2")
                 || url.Equals("https://d.web2.qq.com/channel/send_qun_msg2"))
                request.Referer = "https://d.web2.qq.com/cfproxy.html?v=20110331002&callback=1";
            if (url.StartsWith("http://s.web2.qq.com/api/get_friend_uin2"))
                request.Referer = "http://s.web2.qq.com/proxy.html?v=20110412001&callback=1&id=3";

            //Send Request & Get Response
            byte[] bytes;
            if (method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";
                bytes = encoding.GetBytes(builder.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            //Set Keep-Alive
            request.KeepAlive = true;

            //Send Request
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                this.Log("[连接]" + e.Message, Color.Red);
                return null;
            }
            Stream responseStream = response.GetResponseStream();
            int contentLength = (int)response.ContentLength;

            //Set Cookie
            string[] keys = response.Headers.AllKeys;
            foreach (string s in keys)
            {
                //Log("[Debug]Get Header { Name: [" + s + "] Value: [" + response.Headers.Get(s) + "] }", Color.Blue);
                if (s == "Location")
                    this.NewLocation = response.Headers.Get(s);
            }
            if (dosetcookie && response.Headers.Get("Set-Cookie") != null)
            {
                string[] commands = response.Headers.Get("Set-Cookie").Replace("Sun,", "Sun").Replace("Mon,", "Mon").Replace("Tue,", "Tue").Replace("Wed,", "Wed").Replace("Thu,", "Thu").Replace("Fri,", "Fri").Replace("Sat,", "Sat").Split(',');
                foreach (string c in commands)
                {
                    string[] s = new string[] { ";,", "; ", ";" };
                    string[] t = c.Split(s, StringSplitOptions.RemoveEmptyEntries);

                    //Remove Expires
                    if (t[1].Contains("EXPIRES="))
                    {
                        t[1] = t[2];
                        t[2] = t[3];
                    }
                    Cookie newcookie = new Cookie(t[0].Split('=')[0], t[0].Split('=')[1], t[1].Replace("PATH=", ""), t[2].Replace("DOMAIN=", ""));
                    //Log("[Debug]New Cookie: [" + t[0].Split('=')[0] + "] [" + t[0].Split('=')[1] + "] [" + t[1].Replace("PATH=", "") + "] [" + t[2].Replace("DOMAIN=", "") + "]", Color.Blue);
                    if (!String.IsNullOrEmpty(newcookie.Value))
                    {
                        SetCookies.Add(newcookie);
                        if (newcookie.Name == "ptwebqq")
                            this.Ptwebqq = newcookie.Value;
                    }
                }
            }

            bytes = null;
            int count = 0x64000;
            if (contentLength >= 0)
            {
                bytes = new byte[contentLength];
                int offset = 0;
                while (contentLength > 0)
                {
                    int num5 = responseStream.Read(bytes, offset, contentLength);
                    offset += num5;
                    contentLength -= num5;
                }
            }
            else
            {
                bytes = new byte[count];
                MemoryStream stream3 = new MemoryStream();
                for (int i = responseStream.Read(bytes, 0, count); i > 0; i = responseStream.Read(bytes, 0, count))
                {
                    stream3.Write(bytes, 0, i);
                }
                bytes = stream3.ToArray();
                stream3.Close();
            }
            response.Close();
            return bytes;
        }

        private string GetRandom16()
        {
            Random r = new Random();
            string res = "0.";
            while (res.Length < 16)
                res += r.Next(0, 10).ToString();
            return res;
        }

        delegate void SetTextCallBack(string message, Color color);
        /// <summary>
        /// 在richTextBoxLog上打印信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="color">颜色[一般:White 状态:Green 错误:Red 调试:Blue]</param>
        private void Log(string message, Color color)
        {
            //if (this.Release && color == Color.Blue)
            //    return;
            if (!this.checkBoxLog.Checked && (color == Color.White || color == Color.Red))
                return;
            if (this.richTextBoxLog.InvokeRequired)
            {
                SetTextCallBack d = new SetTextCallBack(Log);
                this.Invoke(d, new object[] { message, color });
            }
            else
            {
                this.richTextBoxLog.SelectionStart = this.richTextBoxLog.Text.Length;
                this.richTextBoxLog.SelectionColor = color;
                this.richTextBoxLog.AppendText(message + "\r\n");
            }
            return;
        }
        private long GetTimeStamp()
        {
            DateTime startDate = new DateTime(1970, 1, 1);
            DateTime endDate = DateTime.Now.ToUniversalTime();
            TimeSpan span = endDate - startDate;
            return (long)(span.TotalMilliseconds + 0.5);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Log("[系统]完成窗体载入", Color.Green);

            //Set Inti Settings
            for (int i = 0; i < IntiFlag.Length; ++i)
                FlagToInt.Add(IntiFlag[i], IntiNeed[i]);

            //FullMatchKnowledge
            int Sum = 0;
            FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.txt", FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string inci = "";
                string[] NewKnowledge = sr.ReadLine().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (NewKnowledge.Length > 2)
                    inci = NewKnowledge[2];
                if (!FullMatchKnowledges.ContainsKey(NewKnowledge[0]))
                    FullMatchKnowledges.Add(NewKnowledge[0], new List<KnowledgePack>());
                FullMatchKnowledges[NewKnowledge[0]].Add(new KnowledgePack(NewKnowledge[1], inci));
                ++Sum;
            }
            sr.Close();
            fs.Close();
            this.Log("[FullMatch]学会了" + Sum.ToString() + "句话", Color.Green);
            this.label12.Text = Sum.ToString();

            //WordsMatchKnowledge
            Sum = 0;
            fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.txt", FileMode.OpenOrCreate);
            sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string inci = "";
                string[] NewKnowledge = sr.ReadLine().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (NewKnowledge.Length > 2)
                    inci = NewKnowledge[2];
                List<string> wordslist = new List<string>(NewKnowledge[0].Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries));
                bool Flag = false;
                foreach (List<string> got in WordsMatchKnowledges.Keys)
                    if (got.Except(wordslist).Count() == 0 && wordslist.Except(got).Count() == 0)
                    {
                        WordsMatchKnowledges[got].Add(new KnowledgePack(NewKnowledge[1], inci));
                        Flag = true;
                        break;
                    }
                if (!Flag)
                {
                    WordsMatchKnowledges.Add(wordslist, new List<KnowledgePack>());
                    WordsMatchKnowledges[wordslist].Add(new KnowledgePack(NewKnowledge[1], inci));
                }
                ++Sum;
            }
            sr.Close();
            fs.Close();
            this.Log("[WordsMatch]学会了" + Sum.ToString() + "句话", Color.Green);
            this.label13.Text = Sum.ToString();

            //Intimacy
            Sum = 0;
            fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\Intimacy.txt", FileMode.OpenOrCreate);
            sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                try
                {
                    string[] NewKnowledge = sr.ReadLine().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Intimacy.Add(NewKnowledge[0], int.Parse(NewKnowledge[1]));
                    ++Sum;
                }
                catch
                {

                }
            }
            sr.Close();
            fs.Close();
            this.Log("[Intimacy]获得" + Sum.ToString() + "个亲密度数据", Color.Green);

            //Nickname
            Sum = 0;
            fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\Nickname.txt", FileMode.OpenOrCreate);
            sr = new StreamReader(fs);
            while (!sr.EndOfStream)
            {
                string[] NewKnowledge = sr.ReadLine().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                Nickname.Add(NewKnowledge[0], NewKnowledge[1]);
                ++Sum;
            }
            sr.Close();
            fs.Close();
            this.Log("[Nickname]获取了" + Sum.ToString() + "个昵称", Color.Green);


            this.textBoxQQ.Text = "484778406";
            this.textBoxPassword.Text = "";
            this.textBoxQQ.Select();
            this.textBoxQQ.Select(this.textBoxQQ.Text.Length, 0);

            Log("[系统]完成HTML文档" + Path.GetDirectoryName(Application.ExecutablePath) + "\\encodepwd.html的载入", Color.Green);
            this.webBrowser1.Navigate(Path.GetDirectoryName(Application.ExecutablePath) + "\\encodepwd.html");

            Random r = new Random();
            this.ClientID = r.Next(10000000, 100000000);
            //Log("[Debug]ClientID" + this.ClientID, Color.Blue);
        }

        private void textBoxQQ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
                return;
            }
            this.textBoxVerify.Enabled = true;
            this.label3.Enabled = true;
            this.pictureBox1.Enabled = true;
            this.pictureBox1.Image = null;
            return;
        }

        private string GetQQfromUin(string uin)
        {
            if (UintoQQ.ContainsKey(uin))
                return UintoQQ[uin];
            string url = "http://s.web2.qq.com/api/get_friend_uin2?tuin=" + uin + "&verifysession=&type=1&code=&vfwebqq=" + this.LoginInfo.result["vfwebqq"] + "&t=" + this.GetTimeStamp();
            string response = Encoding.UTF8.GetString(this.CreateHttpRequest(url, null, "GET", 0x1333, null, null, this.SetCookies, false, false));
            JavaScriptSerializer json = new JavaScriptSerializer();
            LoginRetInfo info;
            try
            {
                info = json.Deserialize<LoginRetInfo>(response);
                if (info.retcode != 0)
                    throw new Exception();
                UintoQQ.Add(uin, info.result["account"]);
                return info.result["account"];
            }
            catch
            {
                return "";
            }
        }

        private void GetVerifyImage()
        {
            if (!String.IsNullOrEmpty(this.textBoxQQ.Text))
            {
                //Log("[Debug]GetImage QQ:" + this.textBoxQQ.Text + " 开始获取请求验证字串", Color.Blue);
                //==============GET验证字串==============//
                string url = "https://ssl.ptlogin2.qq.com/check?uin=" + this.textBoxQQ.Text + "&appid=1003903&js_ver=10067&js_type=0&login_sig=EPcCP3Vlw6IeqpdkjwwQjnONKu83rutIiMmtDZVEAXbvksFVuX5723G6fC2am*lA&u1=http%3A%2F%2Fweb2.qq.com%2Floginproxy.html&r=" + GetRandom16();
                string response = System.Text.Encoding.UTF8.GetString(this.CreateHttpRequest(url, null, "GET", 0x2333, null, null, this.SetCookies, true, true));
                //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: GET\r\n[Debug]Response: " + response, Color.Blue);
                string[] info = response.Replace("ptui_checkVC('", "").Replace("');", "").Split(new string[] { "','" }, StringSplitOptions.RemoveEmptyEntries);
                //foreach (string t in info)
                // {
                //   Log("[Debug]ArrayInfo: " + t, Color.Blue);
                // }
                Pt_Uin = info[2];
                if (info[0].Equals("0"))
                {
                    this.VerifyCode = info[1];
                    this.textBoxVerify.Enabled = false;
                    this.label3.Enabled = false;
                    this.pictureBox1.Enabled = false;
                }
                else
                {
                    //===============获取验证码图片===============//
                    url = "https://ssl.captcha.qq.com/getimage?aid=1003903&r=" + GetRandom16() + "&uin=" + this.textBoxQQ.Text;
                    byte[] imgdata = this.CreateHttpRequest(url, null, "GET", 0x2333, null, null, this.SetCookies, true, true);
                    MemoryStream ms = new MemoryStream();
                    ms.Write(imgdata, 0, imgdata.Length);
                    ms.Flush();
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pictureBox1.Image = Image.FromStream(ms);
                    //=============================================//
                }
                //=======================================//
            }
            return;
        }

        private void textBoxQQ_LostFocus(object sender, EventArgs e)
        {
            this.GetVerifyImage();
        }

        private void richTextBoxLog_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.richTextBoxLog.Refresh();
                this.richTextBoxLog.SelectionStart = this.richTextBoxLog.Text.Length;
                this.richTextBoxLog.ScrollToCaret();
            }
            catch { }
        }

        private string PasswordToMD5()
        {
            string pw = this.textBoxPassword.Text;
            string vc = this.VerifyCode;
            string pt_uin = this.Pt_Uin;
            string res = this.webBrowser1.Document.InvokeScript("EncodePwd", new string[] { pt_uin, pw, vc }).ToString();
            // Log("[Script]EncodePwd\r\n[Script]pt_uin: [" + pt_uin + "]\r\n[Script]password: [" + pw + "]\r\n[Script]verifycode: [" + vc + "]\r\n[Script]result: [" + res + "]", Color.Blue);
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.textBoxQQ.Text) || String.IsNullOrEmpty(this.textBoxPassword.Text) || String.IsNullOrEmpty(this.VerifyCode))
            {
                System.Windows.Forms.MessageBox.Show("账号，密码或验证码不能为空！");
                return;
            }
            this.textBoxQQ.Enabled = false;
            this.textBoxPassword.Enabled = false;
            this.textBoxVerify.Enabled = false;
            this.label3.Enabled = false;
            this.pictureBox1.Image = null;
            this.pictureBox1.Enabled = false;
            this.button1.Enabled = false;
            this.button2.Enabled = true;
            //================登录================//
            Log("[系统]开始验证", Color.Green);
            string url = "https://ssl.ptlogin2.qq.com/login?u=" + this.textBoxQQ.Text + "&p=" + this.PasswordToMD5() + "&verifycode=" + this.VerifyCode + "&webqq_type=10&remember_uin=1&login2qq=1&aid=1003903&u1=http%3A%2F%2Fweb2.qq.com%2Floginproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&h=1&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=3-15-9953&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10067&login_sig=EPcCP3Vlw6IeqpdkjwwQjnONKu83rutIiMmtDZVEAXbvksFVuX5723G6fC2am*lA";
            string response = System.Text.Encoding.UTF8.GetString(this.CreateHttpRequest(url, null, "GET", 0x2333, null, null, this.SetCookies, true, true));
            //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: GET\r\n[Debug]Response: " + response, Color.Blue);
            //====================================//
            string[] info = response.Split(new string[] { "ptuiCB('", "','", "', '", "');" }, StringSplitOptions.RemoveEmptyEntries);
            if (!info[0].Equals("0"))
            {
                Log("[错误]Code " + info[0] + ": " + info[3], Color.Red);
                CloseThreads();
                SetCookies = new CookieContainer();
                LoginInfo = null;
                this.textBoxQQ.Enabled = true;
                this.textBoxPassword.Enabled = true;
                this.textBoxVerify.Enabled = true;
                this.label3.Enabled = true;
                this.pictureBox1.Enabled = true;
                this.button1.Enabled = true;
                this.button2.Enabled = false;
                this.GetVerifyImage();
                return;
            }
            Log("[系统][" + info[5] + "]" + info[4], Color.Green);
            this.QQName = info[5];

            //Set Cookies Before Autoredirect
            url = info[2];
            response = System.Text.Encoding.UTF8.GetString(this.CreateHttpRequest(url, null, "GET", 0x2333, null, null, this.SetCookies, true, false));
            //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: GET\r\n[Debug]Response: " + response, Color.Blue);

            //Redirect
            url = this.NewLocation;
            response = System.Text.Encoding.UTF8.GetString(this.CreateHttpRequest(url, null, "GET", 0x2333, null, null, this.SetCookies, true, false));
            //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: GET\r\n[Debug]Response: " + response, Color.Blue);

            //Set pgv_pvid
            Random ran = new Random();
            string pgv_pvid = "9";
            for (int i = 1; i <= 9; ++i)
                pgv_pvid += ran.Next(0, 10).ToString();
            Cookie pgvcookie = new Cookie("pgv_pvid", pgv_pvid, "/", "qq.com");
            SetCookies.Add(pgvcookie);

            //Set hideusehttpstips
            Cookie hideusehttpstips = new Cookie("hideusehttpstips", "1", "/", "web2.qq.com");
            SetCookies.Add(hideusehttpstips);

            //Login WebQQ
            //r={"status":"online","ptwebqq":"adb5a8da6cc56e119b68df97a1d2617cb38b9471f3d7e49a532cacb44b114b97","passwd_sig":"","clientid":"75589933","psessionid":null}
            //&clientid=75589933
            //&psessionid=null
            Log("[系统]开始上线", Color.Green);
            url = "https://d.web2.qq.com/channel/login2";
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Clear();
            param.Add("r", "{\"status\":\"online\",\"ptwebqq\":\"" + this.Ptwebqq + "\",\"passwd_sig\":\"\",\"clientid\":\"" + this.ClientID + "\",\"psessionid\":null}");
            param.Add("clientid", "\"" + this.ClientID + "\"");
            param.Add("psessionid", "null");
            response = System.Text.Encoding.UTF8.GetString(this.CreateHttpRequest(url, param, "POST", 0x2333, null, null, this.SetCookies, true, true));
            //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: POST\r\n[Debug]Response: " + response, Color.Blue);
            //Login Response
            //{"retcode":0,"result":{"uin":2971219445,"cip":1851206896,"index":1074,"port":52932,"status":"online",
            //"vfwebqq":"30ac28c9a6e54c197cddbaeeafb5eda0f31a18273d5b0465158f93e074bbdfa36ff519de721c61e2",
            //"psessionid":"8368046764001e636f6e6e7365727665725f77656271714031302e3132382e36362e31313500006e21000003be036e0400f53519b16d0000000a40506b6c31706237765a6d0000002830ac28c9a6e54c197cddbaeeafb5eda0f31a18273d5b0465158f93e074bbdfa36ff519de721c61e2",
            //"user_state":0,"f":0}}
            JavaScriptSerializer json = new JavaScriptSerializer();
            LoginInfo = json.Deserialize<LoginRetInfo>(response);
            //Log("[Debug]LoginInfo \\GetLoginJson/\r\n" + LoginInfo.ToString(), Color.Blue);
            if (LoginInfo.retcode == 0)
            {
                Log("[系统]上线成功", Color.Green);
                this.groupBox2.Enabled = true;
                this.labelQQ.Text = this.LoginInfo.result["uin"];
                this.labelQQName.Text = this.QQName;
                this.timer1.Start();
            }
            else
            {
                Log("[错误]上线失败", Color.Red);
                CloseThreads();
                SetCookies = new CookieContainer();
                LoginInfo = null;
                this.textBoxQQ.Enabled = true;
                this.textBoxPassword.Enabled = true;
                this.textBoxVerify.Enabled = true;
                this.label3.Enabled = true;
                this.pictureBox1.Enabled = true;
                this.button1.Enabled = true;
                this.button2.Enabled = false;
                this.GetVerifyImage();
                return;
            }
            if (!bGetLogoThread)
            {
                bGetLogoThread = true;
                GetLogoThread = new Thread(new ThreadStart(threadGetLogo));
                GetLogoThread.Start();
            }
            else
            {
                Log("[错误]获取头像线程已被占用", Color.Red);
                return;
            }
            if (!bPoll2Thread)
            {
                bPoll2Thread = true;
                Poll2Thread = new Thread(new ThreadStart(threadPoll2));
                Poll2Thread.Start();
            }
            else
            {
                Log("[错误]Poll2线程已被占用", Color.Red);
                return;
            }
            return;
        }

        private void ReLoginQQ()
        {
            Log("[系统]开始上线", Color.Green);
            string url = "https://d.web2.qq.com/channel/login2";
            IDictionary<string, string> param = new Dictionary<string, string>();
            param.Clear();
            param.Add("r", "{\"status\":\"online\",\"ptwebqq\":\"" + this.Ptwebqq + "\",\"passwd_sig\":\"\",\"clientid\":\"" + this.ClientID + "\",\"psessionid\":null}");
            param.Add("clientid", "\"" + this.ClientID + "\"");
            param.Add("psessionid", "null");
            string response = System.Text.Encoding.UTF8.GetString(this.CreateHttpRequest(url, param, "POST", 0x2333, null, null, this.SetCookies, true, true));
            Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: POST\r\n[Debug]Response: " + response, Color.Blue);
            //Login Response
            //{"retcode":0,"result":{"uin":2971219445,"cip":1851206896,"index":1074,"port":52932,"status":"online",
            //"vfwebqq":"30ac28c9a6e54c197cddbaeeafb5eda0f31a18273d5b0465158f93e074bbdfa36ff519de721c61e2",
            //"psessionid":"8368046764001e636f6e6e7365727665725f77656271714031302e3132382e36362e31313500006e21000003be036e0400f53519b16d0000000a40506b6c31706237765a6d0000002830ac28c9a6e54c197cddbaeeafb5eda0f31a18273d5b0465158f93e074bbdfa36ff519de721c61e2",
            //"user_state":0,"f":0}}
            JavaScriptSerializer json = new JavaScriptSerializer();
            LoginInfo = json.Deserialize<LoginRetInfo>(response);
            //Log("[Debug]LoginInfo \\GetLoginJson/\r\n" + LoginInfo.ToString(), Color.Blue);
            if (LoginInfo.retcode == 0)
            {
                Log("[系统]上线成功", Color.Green);
                this.groupBox2.Enabled = true;
                this.labelQQ.Text = this.LoginInfo.result["uin"];
                this.labelQQName.Text = this.QQName;
            }
            else
            {
                Log("[错误]上线失败", Color.Red);
                CloseThreads();
                SetCookies = new CookieContainer();
                LoginInfo = null;
                this.textBoxQQ.Enabled = true;
                this.textBoxPassword.Enabled = true;
                this.textBoxVerify.Enabled = true;
                this.label3.Enabled = true;
                this.pictureBox1.Enabled = true;
                this.button1.Enabled = true;
                this.button2.Enabled = false;
                this.GetVerifyImage();
                return;
            }
        }

        private void threadPoll2()
        {
            while (true)
            {
                string url = "https://d.web2.qq.com/channel/poll2";
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Clear();
                param.Add("r", "{\"clientid\":\"" + this.ClientID + "\",\"psessionid\":\"" + this.LoginInfo.result["psessionid"] + "\",\"key\":0,\"ids\":[]}");
                param.Add("clientid", this.ClientID.ToString());
                param.Add("psessionid", this.LoginInfo.result["psessionid"]);
                Regex regex = new Regex(@"\\u(\w{4})");
                byte[] bytes;
                try
                {
                    bytes = this.CreateHttpRequest(url, param, "POST", 300000, null, null, this.SetCookies, false, false);
                }
                catch
                {
                    continue;
                }
                string response;
                try
                {
                    response = regex.Replace(Encoding.UTF8.GetString(bytes), delegate(Match m)
                    {
                        string hexStr = m.Groups[1].Value;
                        string charStr = ((char)int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber)).ToString();
                        return charStr;
                    });
                    regex = new Regex("nt\":" + "\\[(.*)\\]\\,");
                    response = regex.Replace(response, "nt\":").Replace(" \"", "\"").Replace("\"value\":{", "").Replace("]}}]}", "}]}");
                    //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: POST\r\n[Debug]Response: " + response, Color.Blue);
                }
                catch
                {
                    continue;
                }
                if (response.Contains("\"buddies_status_change\","))
                    continue;
                JavaScriptSerializer json = new JavaScriptSerializer();
                PollRetInfo message;
                try
                {
                    message = json.Deserialize<PollRetInfo>(response);
                }
                catch
                {
                    if (++this.RetryTime > 10)
                    {
                        Log("[系统]解析失败！", Color.Red);
                        this.bPoll2Thread = false;
                        FileStream file = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\Exit.txt", FileMode.Append);
                        StreamWriter sw = new StreamWriter(file);
                        sw.Write(DateTime.Now.Hour.ToString() + " " + DateTime.Now.Minute.ToString() + " " + DateTime.Now.Second.ToString() + "\r\n");
                        sw.Write(response + "\r\n");
                        sw.Close();
                        file.Close();
                        return;
                    }
                    continue;
                }
                //102: 没有信息
                if (message.retcode == 102)
                    continue;
                //121: 掉线
                if (message.retcode == 121)
                {
                    ReLoginQQ();
                    continue;
                }
                //116: 切换ptwebqq
                if (message.retcode == 116)
                {
                    this.Ptwebqq = message.p;
                    this.SetCookies.Add(new Cookie("ptwebqq", message.p, "/", "qq.com"));
                    continue;
                }
                try
                {
                    this.RetryTime = 0;
                    //this.Log("[Debug]Poll2 \\GetJson/\r\n" + message.ToString(), Color.Blue);
                    this.PollMsg = message;
                    this.Log("[信息]收到[" + message.result[0]["content"] + "]", Color.White);
                }
                catch
                {
                    FileStream file = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\Exit.txt", FileMode.Append);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write(response + "\r\n");
                    sw.Close();
                }
                if (message.result[0]["poll_type"] == "group_message" ||
                    message.result[0]["poll_type"] == "message")
                {
                    if (!this.bSolveMsgThread)
                    {
                        this.IgnoreTime = 0;
                        this.bSolveMsgThread = true;
                        SolveMsgThread = new Thread(new ThreadStart(SolveMsg));
                        SolveMsgThread.Start();
                    }
                    else
                        ++this.IgnoreTime;
                }
                //Thread.Sleep(2000);
            }
        }

        private int GetMsgID()
        {
            if (this.MsgID == 0 || this.MsgID > 99999999)
            {
                Random r = new Random();
                MsgID = r.Next(1000, 10000) * 10000;
            }
            return ++this.MsgID;
        }
		
		// TODO: PACK THEM UP !
        private string StudyNotSuccessful()
        {
            try
            {
                int Sum = FullMatchKnowledges["$StudyNotSuccessful$"].Count;
                return FullMatchKnowledges["$StudyNotSuccessful$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "烦死了烦死了学不会呀Kuma!!!";
            }
        }

        private string StudySuccessful()
        {
            try
            {
                int Sum = FullMatchKnowledges["$StudySuccessful$"].Count;
                return FullMatchKnowledges["$StudySuccessful$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "学会啦Kuma!!!";
            }
        }

        private string ForgetNotSuccessful()
        {
            try
            {
                int Sum = FullMatchKnowledges["$ForgetNotSuccessful$"].Count;
                return FullMatchKnowledges["$ForgetNotSuccessful$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "没有学过的东西怎么能忘记呢Kuma!!!";
            }
        }
        private string ForgetSuccessful()
        {
            try
            {
                int Sum = FullMatchKnowledges["$ForgetSuccessful$"].Count;
                return FullMatchKnowledges["$ForgetSuccessful$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "好吧我就装作什么都不知道哦Kuma!!!";
            }
        }
        private string NoTeachAccess()
        {
            try
            {
                int Sum = FullMatchKnowledges["$NoTeachAccess$"].Count;
                return FullMatchKnowledges["$NoTeachAccess$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "想对我指指点点没那么容易哦Kuma!!!";
            }
        }
        private string TeachNotCorrect()
        {
            try
            {
                int Sum = FullMatchKnowledges["$TeachNotCorrect$"].Count;
                return FullMatchKnowledges["$TeachNotCorrect$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "这样教我是学不会的Kuma!!!";
            }
        }
        private string NoForgetAccess()
        {
            try
            {
                int Sum = FullMatchKnowledges["$NoForgetAccess$"].Count;
                return FullMatchKnowledges["$NoForgetAccess$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "熊可不是那么容易忘记的Kuma!!!";
            }
        }
        private string ForgetNotCorrect()
        {
            try
            {
                int Sum = FullMatchKnowledges["$ForgetNotCorrect$"].Count;
                return FullMatchKnowledges["$ForgetNotCorrect$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "你麻麻没有教你怎么说话吗Kuma!!!";
            }
        }
        private string ClockReport()
        {
            try
            {
                int Sum = FullMatchKnowledges["$Clock" + DateTime.Now.Hour + "Report$"].Count;
                return FullMatchKnowledges["$Clock" + DateTime.Now.Hour + "Report$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "不知不觉到了" + DateTime.Now.Hour + "点了呢Kuma!!!";
            }
        }
        private string PostUrlInGroup()
        {
            try
            {
                int Sum = FullMatchKnowledges["$PostUrlInGroup$"].Count;
                return FullMatchKnowledges["$PostUrlInGroup$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "唔噗噗噗噗你看看你又在群里发小黄网了~Kuma!!!";
            }
        }
        private string SetSuccessful()
        {
            try
            {
                int Sum = FullMatchKnowledges["$SetSuccessful$"].Count;
                return FullMatchKnowledges["$SetSuccessful$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "唔噗噗噗噗你说的被我记下啦~";
            }
        }
        private string SetNotSuccessful()
        {
            try
            {
                int Sum = FullMatchKnowledges["$SetNotSuccessful$"].Count;
                return FullMatchKnowledges["$SetNotSuccessful$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "你在乱设置些什么呢？Kuma!!!";
            }
        }
        private string KumaUnhappy()
        {
            try
            {
                int Sum = FullMatchKnowledges["$KumaUnhappy$"].Count;
                return FullMatchKnowledges["$KumaUnhappy$"].ElementAt(ResponseRand.Next(Sum)).ResponseMsg;
            }
            catch
            {
                return "你刚刚说的话让我生气了Kuma!!!";
            }
        }

        private string RemoveUnnecessaryChar(string s)
        {
            return s.Replace(" ", "").Replace("　", "").Replace("\r", "").Replace("\n", "").Replace("	", "");
        }
        private string KumaTeach(string q, string a, string inti)
        {
            q = RemoveUnnecessaryChar(q);
            //Tencent Exception
            if (a.Contains("[]") ||
                a.Contains("{}"))
                return this.StudyNotSuccessful();
            bool fullMatchOnly = true;
            try
            {
                Log("[教学]Qestion[" + q + "]Answer[" + a + "][" + inti + "]", Color.White);
                string solveinci = "";
                if (!String.IsNullOrEmpty(inti))
                    for (int i = IntiFlag.Length - 1; i >= 0; --i)
                        if (inti.Contains(IntiFlag[i]))
                        {
                            solveinci = IntiFlag[i];
                            inti = inti.Replace(IntiFlag[i], "");
                        }
                if (inti.Contains("!"))
                {
                    inti = inti.Replace("!", "");
                    fullMatchOnly = false;
                }
                if (q.Contains('&'))
                    fullMatchOnly = false;

                if (!String.IsNullOrEmpty(inti) || (fullMatchOnly && q.Contains('&')))
                    return this.StudyNotSuccessful();

                //Fullmatch Study
                if (!q.Contains('&'))
                {
                    if (!FullMatchKnowledges.ContainsKey(q))
                        FullMatchKnowledges.Add(q, new List<KnowledgePack>());
                    FullMatchKnowledges[q].Add(new KnowledgePack(a, solveinci));
                    FileStream file = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.txt", FileMode.Append);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write(q + "|" + a);
                    if (String.IsNullOrEmpty(solveinci))
                        sw.Write("\r\n");
                    else
                        sw.Write("|" + solveinci + "\r\n");
                    this.label12.Text = (int.Parse(this.label12.Text) + 1).ToString();
                    sw.Close();
                    file.Close();
                }

                //Wordsmatch Study
                if (!q.Contains('$') && !fullMatchOnly)
                {
                    bool Flag = false;
                    List<string> qwlist = new List<string>(q.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (List<string> got in WordsMatchKnowledges.Keys)
                        if (got.Except(qwlist).Count() == 0 && qwlist.Except(got).Count() == 0)
                        {
                            WordsMatchKnowledges[got].Add(new KnowledgePack(a, solveinci));
                            Flag = true;
                            break;
                        }
                    if (!Flag)
                    {
                        WordsMatchKnowledges.Add(qwlist, new List<KnowledgePack>());
                        WordsMatchKnowledges[qwlist].Add(new KnowledgePack(a, solveinci));
                    }
                    FileStream file = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.txt", FileMode.Append);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write(q + "|" + a);
                    if (String.IsNullOrEmpty(solveinci))
                        sw.Write("\r\n");
                    else
                        sw.Write("|" + solveinci + "\r\n");
                    this.label13.Text = (int.Parse(this.label13.Text) + 1).ToString();
                    sw.Close();
                    file.Close();
                }
            }
            catch (Exception e)
            {
                this.Log(e.Message, Color.Red);
                return this.StudyNotSuccessful();
            }
            return this.StudySuccessful();
        }

        private string KumaForget(string q, string a)
        {
            q = RemoveUnnecessaryChar(q);
            bool Flag2 = false;
            try
            {
                //Remove FullMatch
                if (!q.Contains('&'))
                {
                    bool t = false;
                    foreach (KnowledgePack kp in FullMatchKnowledges[q])
                    {
                        if (kp.ResponseMsg == a)
                        {
                            t = FullMatchKnowledges[q].Remove(kp);
                            break;
                        }
                    }
                    Flag2 |= t;
                    File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.txt", Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.bak", true);
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.txt");
                    FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.bak", FileMode.OpenOrCreate);
                    StreamReader sr = new StreamReader(fs);
                    FileStream fs2 = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.txt", FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs2);
                    int sum = 0;
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        string[] NewKnowledge = s.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                        if (NewKnowledge[0] != q || NewKnowledge[1] != a)
                        {
                            sw.WriteLine(s);
                            ++sum;
                        }
                    }
                    this.label12.Text = sum.ToString();
                    sr.Close();
                    fs.Close();
                    sw.Close();
                    fs2.Close();
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\FullMatchKnowledge.bak");
                }
                //Remove Words
                if (!q.Contains('$'))
                {
                    bool Flag = false;
                    List<string> qwlist = new List<string>(q.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries));
                    foreach (List<string> got in WordsMatchKnowledges.Keys)
                        if (got.Except(qwlist).Count() == 0 && qwlist.Except(got).Count() == 0)
                        {
                            bool t = false;
                            foreach (KnowledgePack kp in WordsMatchKnowledges[got])
                            {
                                if (kp.ResponseMsg == a)
                                {
                                    t = WordsMatchKnowledges[got].Remove(kp);
                                    break;
                                }
                            }
                            Flag2 |= t;
                            Flag = true;
                            break;
                        }
                    if (!Flag && !Flag2)
                        throw new Exception();
                    File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.txt", Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.bak", true);
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.txt");
                    FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.bak", FileMode.OpenOrCreate);
                    StreamReader sr = new StreamReader(fs);
                    FileStream fs2 = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.txt", FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs2);
                    int sum = 0;
                    while (!sr.EndOfStream)
                    {
                        string s = sr.ReadLine();
                        string[] NewKnowledge = s.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (NewKnowledge[0] != q || NewKnowledge[1] != a)
                        {
                            sw.WriteLine(s);
                            ++sum;
                        }
                    }
                    this.label13.Text = sum.ToString();
                    sr.Close();
                    fs.Close();
                    sw.Close();
                    fs2.Close();
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\WordsMatchKnowledge.bak");
                }
            }
            catch (Exception e)
            {
                this.Log(e.Message, Color.Red);
                return this.ForgetNotSuccessful();
            }
            return this.ForgetSuccessful();
        }
		
		// This Function is another Software's KeyGen
		// I use it to autoproduce a key for users in group
        private string KumaAccess(string str)
        {
            string AccessKey = "BuWaiChuan000000";
            string res = null;
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] buffer2 = Encoding.UTF8.GetBytes(AccessKey);
            AesManaged managed2 = new AesManaged
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.ECB,
                Key = buffer2
            };
            ICryptoTransform transform = managed2.CreateEncryptor();
            try
            {
                byte[] inArray = transform.TransformFinalBlock(bytes, 0, bytes.Length);
                res = Convert.ToBase64String(inArray, 0, inArray.Length);
            }
            catch
            {
                return "黑白熊不知道你的AccessID啊 Kuma!!!";
            }
            if (!String.IsNullOrEmpty(res))
            {
                this.Log("[KeyGen][" + str + "][" + res + "]\r\n", Color.White);
                return res + "(在软件目录下用记事本新建一个名为access、没有后缀名的文件，把这个值独立一行粘贴保存就可以获得授权啦！)";
            }
            else
                return "黑白熊不知道你的AccessID啊 Kuma!!!";
        }

        private string GetNickname(string qq)
        {
            if (Nickname.ContainsKey(qq))
                return Nickname[qq];
            return qq;
        }

        private void SetNickname(string qq, string newNickname)
        {
            if (!Nickname.ContainsKey(qq))
                Nickname.Add(qq, "");
            Nickname[qq] = newNickname;
            FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\Nickname.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            foreach (KeyValuePair<string, string> i in Nickname)
                sw.WriteLine(i.Key + "|" + i.Value);
            sw.Close();
            fs.Close();
            return;
        }

        private void SetVitality(int vitality)
        {
            if (vitality > 100 || vitality < 0)
                throw new Exception();
            this.Vitality = vitality;
            return;
        }

		// TODO: Move responce sentences out of program code
        private string GetRemark(int inti)
        {
            if (inti > 100)
                return "唔噗噗噗噗…真是太喜欢你了…啊要溢出来了…我那纯白的…黏乎乎的棉花！！";
            if (inti == 100)
                return "唔噗噗噗噗…满满的爱…每天都很幸福…";
            if (inti > 80)
                return "我也想要叫做“Monokuma酱”呢！你看，只是加个酱字！可爱程度翻一番吧？";
            if (inti > 60)
                return "渐渐觉得和你说话有点意思呢~Kuma!!!";
            if (inti > 40)
                return "黑白熊的哲学剧场大放送…全部学完你就可以毕业啦…唔噗噗噗噗~";
            if (inti > 20)
                return "唔噗噗噗噗，你好像有几分天赋，和我学屠宰吧…";
            if (inti >= 0)
                return "和你不熟，你走开啊Kuma!!!";
            return "讨厌你Kuma!!!";
        }

        private string TeachString(string qq)
        {
            int intimacy = this.GetIntimacy(qq);
            string message = @"【一般回复教学】\\n";
            if (intimacy < 20)
                message += @"需要更高的亲密度解锁该指令\\n";
            else
            {
                message += @"指令1：KumaTeach|问题|答案|附加参数\\n";
                message += @"指令2：KumaTeach|关键词列表|答案|附加参数\\n";
                message += @"其中关键词列表用&连接，附加参数可以不填\\n";
                message += @"附加参数包括!None !Low !Mid !High !VeryHigh !Max !Overflow表示不同的亲密度等级\\n";
                message += @"附加参数额外加入!表示问题不需要全句匹配\\n";
            }
            message += @"【指令回复教学】\\n";
            if (intimacy < 60)
                message += @"需要更高的亲密度解锁该指令";
            else
            {
                message += @"指令：KumaTeach|参数|答案\\n";
                message += @"参数包括以下\\n";
                message += @"$StudySucessful$ 学习成功\\n";
                message += @"$StudyNotSuccessful$ 学习失败\\n";
                message += @"$ForgetSuccessful$ 遗忘成功\\n";
                message += @"$ForgetNotSuccessful$ 遗忘失败\\n";
                message += @"$SetSuccessful$ 设置成功\\n";
                message += @"$SetNotSuccessful$ 设置失败\\n";
                message += @"$NoTeachAccess$ 没有教学权限\\n";
                message += @"$TeachNotCorrect$ 教学指令错误\\n";
                message += @"$NoForgetAccess$ 没有遗忘权限\\n";
                message += @"$ForgetNotCorrect$ 遗忘指令错误\\n";
                message += @"$Clock？Report$ 对？时的整点报时\\n";
                message += @"$PostUrlInGroup$ 收到了网站地址\\n";
                message += @"$KumaUnhappy$ 不愉快";
            }
            return message;
        }
        private string ForgetString(string qq)
        {
            int intimacy = this.GetIntimacy(qq);
            string message = @"【一般回复遗忘】\\n";
            if (intimacy < 40)
                message += @"需要更高的亲密度解锁该指令\\n";
            else
                message += @"与教学指令对应，不添加附加参数，将KumaTeach替换为KumaForget\\n";
            message += @"【指令回复遗忘】\\n";
            if (intimacy < 80)
                message += @"需要更高的亲密度解锁该指令";
            else
                message += @"与教学指令对应，不添加附加参数，将KumaTeach替换为KumaForget";
            return message;
        }
        private string SetNicknameString()
        {
            return @"【设置昵称】\\n指令：KumaSetNickname|昵称";
        }

        private string SetModeString(string qq)
        {
            int intimacy = this.GetIntimacy(qq);
            string message = @"【设置黑白熊模式】\\n";
            if (intimacy < 60)
                message += @"需要更高的亲密度解锁该指令";
            else
            {
                message += @"指令：KumaSwitchMode|模式名称\\n";
                message += @"RepeatMode 复读模式\\n";
                message += @"NormalMode 一般模式\\n";
                message += @"QuietMode 安静模式";
            }
            return message;
        }
        private string SetVitalityString(string qq)
        {
            int intimacy = this.GetIntimacy(qq);
            string message = @"【设置黑白熊的活跃度】\\n";
            if (intimacy < 60)
                message += @"需要更高的亲密度解锁该指令";
            else
            {
                message += @"指令：KumaSetVitality|活跃度\\n";
                message += @"活跃度为0-100内的整数";
            }
            return message;
        }

        private string KumaAbout(string qq)
        {
            int intimacy = this.GetIntimacy(qq);
            string message = this.GetNickname(qq) + @"你好啊唔噗噗噗噗~\\n";
            message += @"这里是黑白熊，";
            message += @"请@以下关键词学习如何交流Kuma!!!\\n";
            message += @"@教学指令 @遗忘指令\\n";
            message += @"@设置昵称 @设置模式 @设置活跃度\\n";
            message += @"目前亲密度：" + intimacy + @"\\n";
            message += this.GetRemark(intimacy);
            //Log(message, Color.Blue);
            return message;
        }

        private int GetIntimacy(string qq)
        {
            if (!Intimacy.ContainsKey(qq))
            {
                SetIntimacy(qq, 30);
                return 30;
            }
            return Intimacy[qq];
        }
        private void Gen24Problem()
        {
            A = ResponseRand.Next(1, 14);
            B = ResponseRand.Next(1, 14);
            C = ResponseRand.Next(1, 14);
            D = ResponseRand.Next(1, 14);
            return;
        }
        private void Solve24Problem(double[] nums, List<Equation> step)
        {
            if (Calc24Solved)
                return;
            if (nums.Length == 1)
            {
                if (Math.Abs(nums[0] - 24.00) < 1e-7)
                {
                    this.Calc24Solved = true;
                    Calc24Ans.Clear();
                    Calc24Ans.AddRange(step);
                    //this.Log(Calc24Ans.Count.ToString(), Color.Red);
                }
                return;
            }
            // this.Log(nums.Length.ToString(), Color.Red);
            for (int i = 0; i < nums.Length; ++i)
                for (int j = 0; j < nums.Length; ++j)
                    if (i != j)
                    {
                        double t;
                        double[] tnums = new double[nums.Length - 1];
                        int p = -1;
                        List<Equation> tstep = new List<Equation>();
                        for (int k = 0; k < nums.Length; ++k)
                            if (k != i && k != j)
                                tnums[++p] = nums[k];
                        ++p;

                        if (i < j)
                        {
                            tstep.Clear();
                            tstep.AddRange(step);
                            t = nums[i] + nums[j];
                            tstep.Add(new Equation(nums[i], '+', nums[j], t));
                            tnums[p] = t;
                            Solve24Problem(tnums, tstep);
                        }
                        if (Calc24Solved)
                            return;

                        tstep.Clear();
                        tstep.AddRange(step);
                        t = nums[i] - nums[j];
                        tstep.Add(new Equation(nums[i], '-', nums[j], t));
                        tnums[p] = t;
                        Solve24Problem(tnums, tstep);
                        if (Calc24Solved)
                            return;

                        if (i < j)
                        {
                            tstep.Clear();
                            tstep.AddRange(step);
                            t = nums[i] * nums[j];
                            tstep.Add(new Equation(nums[i], '*', nums[j], t));
                            tnums[p] = t;
                            Solve24Problem(tnums, tstep);
                        }
                        if (Calc24Solved)
                            return;

                        if (Math.Abs(nums[j]) > 1e-7)
                        {
                            tstep.Clear();
                            tstep.AddRange(step);
                            t = nums[i] / nums[j];
                            tstep.Add(new Equation(nums[i], '/', nums[j], t));
                            tnums[p] = t;
                            Solve24Problem(tnums, tstep);
                        }
                        if (Calc24Solved)
                            return;
                    }
            return;
        }
        private void SolveMsg()
        {
            string receivedMsg = "";
            string responseMsg = null;
            string from_uin = null;
            string from_user_qq = null;
            int user_inti = 0;
            int receivedMsgType = 0;
            try
            {
                receivedMsg = PollMsg.result[0]["content"];
                from_uin = PollMsg.result[0]["from_uin"];
                if (!ChattedGroupUin.Contains(from_uin))
                    ChattedGroupUin.Add(from_uin);

                //QQ Get
                string from_user_uin = "";
                if (PollMsg.result[0]["poll_type"] == "message")
                {
                    receivedMsgType = MsgType.BuddyMsg;
                    from_user_uin = PollMsg.result[0]["from_uin"];
                }
                else if (PollMsg.result[0]["poll_type"] == "group_message")
                {
                    receivedMsgType = MsgType.GroupMsg;
                    from_user_uin = PollMsg.result[0]["send_uin"];
                }
                else
                    return;

                from_user_qq = this.GetQQfromUin(from_user_uin);

                user_inti = this.GetIntimacy(from_user_qq);

                //Ban User
                if (user_inti < 10)
                {
                    this.bSolveMsgThread = false;
                    return;
                }

                //Msg2
                string receivedMsg2 = RemoveUnnecessaryChar(receivedMsg);

                if (receivedMsg.StartsWith("KumaTeach"))
                {
                    if (!this.checkBoxTeach.Checked)
                    {
                        this.bSolveMsgThread = false;
                        return;
                    }
                    //Intimacy needed
                    if (user_inti < 20)
                        responseMsg = this.NoTeachAccess();
                    else if (user_inti < 60 && receivedMsg.Contains('$'))
                        responseMsg = this.NoTeachAccess();
                    else
                    {
                        string[] teachParam = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (teachParam.Length > 2)
                            teachParam[2] = teachParam[2].Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\n", "").Replace("\r", @"\\n").Replace("\"", "\\\\\\\"");
                        if (teachParam.Length == 3)
                            responseMsg = KumaTeach(teachParam[1], teachParam[2], "");
                        else if (teachParam.Length == 4)
                            responseMsg = KumaTeach(teachParam[1], teachParam[2], teachParam[3]);
                        else
                            responseMsg = this.TeachNotCorrect();
                    }
                    goto Label_SendMsg;
                }
                else if (receivedMsg.StartsWith("KumaForget"))
                {
                    if (!this.checkBoxForget.Checked)
                    {
                        this.bSolveMsgThread = false;
                        return;
                    }
                    //Intimacy needed
                    if (user_inti < 40)
                        responseMsg = this.NoForgetAccess();
                    else if (user_inti < 80 && receivedMsg.Contains('$'))
                        responseMsg = this.NoTeachAccess();
                    else
                    {
                        string[] teachParam = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (teachParam.Length > 2)
                            teachParam[2] = teachParam[2].Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\n", "").Replace("\r", @"\\n").Replace("\"", "\\\\\\\"");
                        if (teachParam.Length == 3)
                            responseMsg = KumaForget(teachParam[1], teachParam[2]);
                        else
                            responseMsg = this.ForgetNotCorrect();
                    }
                    goto Label_SendMsg;
                }
                else if (receivedMsg.StartsWith("KumaAccess"))
                {
                    string[] teachParam = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (teachParam.Length == 2)
                        responseMsg = KumaAccess(teachParam[1]);
                    else
                        responseMsg = "要正确输入游戏角色名哦Kuma!!!";
                    goto Label_SendMsg;
                }
                //Commands
                else if (receivedMsg2.Equals("KumaAbout") || receivedMsg2.Equals("@黑白熊") ||
                    receivedMsg2.Equals("@Monokuma") || receivedMsg2.Equals("@monokuma"))
                {
                    this.Log("[召唤]" + from_user_qq, Color.White);
                    responseMsg = this.KumaAbout(from_user_qq);
                    goto Label_SendMsg;
                }
                else if (receivedMsg2.Equals("@教学指令"))
                {
                    responseMsg = this.TeachString(from_user_qq);
                    goto Label_SendMsg;
                }
                else if (receivedMsg2.Equals("@遗忘指令"))
                {
                    responseMsg = this.ForgetString(from_user_qq);
                    goto Label_SendMsg;
                }
                else if (receivedMsg2.Equals("@设置昵称"))
                {
                    responseMsg = this.SetNicknameString();
                    goto Label_SendMsg;
                }
                else if (receivedMsg2.Equals("@设置模式"))
                {
                    responseMsg = this.SetModeString(from_user_qq);
                    goto Label_SendMsg;
                }
                else if (receivedMsg2.Equals("@设置活跃度"))
                {
                    responseMsg = this.SetVitalityString(from_user_qq);
                    goto Label_SendMsg;
                }
                //=====================================================================
                else if (receivedMsg.StartsWith("KumaSetIntimacy"))
                {
                    if (this.GetIntimacy(from_user_qq) <= 199)
                    {
                        responseMsg = this.SetNotSuccessful();
                        goto Label_SendMsg;
                    }
                    string[] param = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (param.Length == 3)
                    {
                        try
                        {
                            this.SetIntimacy(param[1], int.Parse(param[2]));
                            responseMsg = this.SetSuccessful();
                        }
                        catch
                        {
                            responseMsg = this.SetNotSuccessful();
                        }
                    }
                    else
                        responseMsg = this.SetNotSuccessful();
                    goto Label_SendMsg;
                }
                else if (receivedMsg.StartsWith("KumaSetNickname"))
                {
                    string[] param = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (param.Length == 2)
                    {
                        this.SetNickname(from_user_qq, param[1]);
                        responseMsg = this.SetSuccessful();
                    }
                    else
                        responseMsg = this.SetNotSuccessful();
                    goto Label_SendMsg;
                }
                else if (receivedMsg.StartsWith("KumaSwitchMode"))
                {
                    string[] param = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (user_inti < 60 || param.Length != 2)
                    {
                        responseMsg = this.SetNotSuccessful();
                        goto Label_SendMsg;
                    }
                    if (param[1] == "RepeatMode")
                    {
                        this.NowKumaMode = KumaMode.RepeatMode;
                        responseMsg = this.SetSuccessful();
                    }
                    else if (param[1] == "NormalMode")
                    {
                        this.NowKumaMode = KumaMode.NormalMode;
                        responseMsg = this.SetSuccessful();
                    }
                    else if (param[1] == "QuietMode")
                    {
                        this.NowKumaMode = KumaMode.QuietMode;
                        responseMsg = this.SetSuccessful();
                    }
                    else
                        responseMsg = this.SetNotSuccessful();
                    goto Label_SendMsg;
                }
                else if (receivedMsg.StartsWith("KumaSetVitality"))
                {
                    string[] param = receivedMsg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (user_inti < 60 || param.Length != 2)
                    {
                        responseMsg = this.SetNotSuccessful();
                        goto Label_SendMsg;
                    }
                    try
                    {
                        this.SetVitality(int.Parse(param[1]));
                        responseMsg = this.SetSuccessful();
                    }
                    catch
                    {
                        responseMsg = this.SetNotSuccessful();
                    }
                    goto Label_SendMsg;
                }
                else if (receivedMsg.StartsWith("#Ruby"))
                {
                    receivedMsg = receivedMsg.Replace('\r', '\n');
                    string inputinfo = "";
                    if (receivedMsg.Contains("#Input\n"))
                    {
                        inputinfo = receivedMsg.Split(new string[] { "#Input\n" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        receivedMsg = receivedMsg.Split(new string[] { "#Input\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    StreamWriter sw = new StreamWriter("r.rb");
                    sw.Write(receivedMsg);
                    sw.Flush(); sw.Close();
                    var info = new ProcessStartInfo("ruby", "r.rb");
                    info.CreateNoWindow = true;
                    info.UseShellExecute = false;
                    info.RedirectStandardInput = true;
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    info.StandardOutputEncoding = Encoding.UTF8;
                    info.StandardErrorEncoding = Encoding.UTF8;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process p = Process.Start(info);
                    p.StandardInput.WriteLine(inputinfo);
                    p.WaitForExit(10000);
                    if (!p.HasExited)
                        p.Kill();
                    else
                    {
                        try
                        {
                            responseMsg = p.StandardOutput.ReadToEnd();
                            responseMsg += p.StandardError.ReadToEnd();
                            if (responseMsg[responseMsg.Length - 1] == '\n')
                                responseMsg = responseMsg.Remove(responseMsg.Length - 1);
                            responseMsg = responseMsg.Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\r", "").Replace("\n", @"\\n").Replace("\"", "\\\\\\\"");
                            if (!String.IsNullOrEmpty(responseMsg))
                                goto Label_SendMsg;
                        }
                        catch (Exception e) { Log(e.Message , Color.Red); }
                    }
                }
				// TODO:  REWRITE THEM IT'S TOO LONG!!!
                else if (receivedMsg.StartsWith("#C"))
                {
                    receivedMsg = receivedMsg.Replace('\r', '\n').Replace("#C\n", "").Replace("#C", "");
                    string inputinfo = "";
                    if (receivedMsg.Contains("#Input\n"))
                    {
                        inputinfo = receivedMsg.Split(new string[] { "#Input\n" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        receivedMsg = receivedMsg.Split(new string[] { "#Input\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    if (File.Exists("c.exe"))
                        File.Delete("c.exe");
                    if (File.Exists("c.cpp"))
                        File.Delete("c.cpp");
                    StreamWriter sw = new StreamWriter("c.cpp");
                    sw.Write(receivedMsg);
                    sw.Flush(); sw.Close();
                    var compileinfo = new ProcessStartInfo("g++", " -o c.exe c.cpp");
                    compileinfo.CreateNoWindow = true;
                    compileinfo.UseShellExecute = false;
                    compileinfo.RedirectStandardError = true;
                    compileinfo.StandardErrorEncoding = Encoding.UTF8;
                    Process compile = Process.Start(compileinfo);
                    compile.WaitForExit(10000);
                    if (!compile.HasExited)
                    {
                        compile.Kill();
                        goto Label_SendMsg;
                    }
                    string compilelog = compile.StandardError.ReadToEnd();
                    if (compilelog.Contains("error:"))
                    {
                        responseMsg = compilelog.Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\r", "").Replace("\n", @"\\n").Replace("\"", "\\\\\\\"");
                        goto Label_SendMsg;
                    }
                    var info = new ProcessStartInfo("c.exe");
                    info.CreateNoWindow = true;
                    info.UseShellExecute = false;
                    info.RedirectStandardInput = true;
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    info.StandardOutputEncoding = Encoding.UTF8;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process p = Process.Start(info);
                    p.StandardInput.WriteLine(inputinfo);
                    p.WaitForExit(10000);
                    if (!p.HasExited)
                        p.Kill();
                    else
                    {
                        try
                        {
                            responseMsg = p.StandardOutput.ReadToEnd();
                            //responseMsg += p.StandardError.ReadToEnd();
                            if (responseMsg[responseMsg.Length - 1] == '\n')
                                responseMsg = responseMsg.Remove(responseMsg.Length - 1);
                            responseMsg = responseMsg.Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\r", "").Replace("\n", @"\\n").Replace("\"", "\\\\\\\"");
                            if (!String.IsNullOrEmpty(responseMsg))
                                goto Label_SendMsg;
                        }
                        catch (Exception e) { Log(e.Message, Color.Red); }
                    }
                }
                else if (receivedMsg.Contains("+") || receivedMsg.Contains("-") || receivedMsg.Contains("/") || receivedMsg.Contains("*")
                    || receivedMsg.Contains("÷") || receivedMsg.Contains("!") || receivedMsg.Contains("×") || receivedMsg.Contains("^") ||
                    ((receivedMsg.Contains("fib") || receivedMsg.Contains("p") || receivedMsg.Contains("c") || receivedMsg.Contains("fix")
                    || receivedMsg.Contains("gcd") || receivedMsg.Contains("lcm") || receivedMsg.Contains("log") || receivedMsg.Contains("exp")
                    || receivedMsg.Contains("sin") || receivedMsg.Contains("cos") || receivedMsg.Contains("tan") || receivedMsg.Contains("cot"))
                    && receivedMsg.Contains("(") && receivedMsg.Contains(")")))
                {
                    //this.Log("Math", Color.White);
                    //"java -jar calc.jar \"" + param[1] + "\""
                    long size = 0;
                    var info = new ProcessStartInfo("java", "-jar calc.jar \"" + receivedMsg + "\"");
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process CalcProcess = Process.Start(info);
                    CalcProcess.WaitForExit();
                    if (File.Exists("calculateResult.txt"))
                        size = new FileInfo("calculateResult.txt").Length;
                    if (size != 0)
                    {
                        bool Flag = false;
                        bool Flag2 = false;
                        StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + "\\calculateResult.txt");
                        string get = sr.ReadLine();
                        if (Calc24Seconds != -1 && receivedMsg.Contains(A.ToString()) && receivedMsg.Contains(B.ToString()) && receivedMsg.Contains(C.ToString()) && receivedMsg.Contains(D.ToString()))
                        {
                            string pd = RemoveUnnecessaryChar(receivedMsg);
                            if (A > 9)
                                pd = pd.Replace(A.ToString(), "");
                            if (B > 9)
                                pd = pd.Replace(B.ToString(), "");
                            if (C > 9)
                                pd = pd.Replace(C.ToString(), "");
                            if (D > 9)
                                pd = pd.Replace(D.ToString(), "");
                            pd = pd.Replace(A.ToString(), "").Replace(B.ToString(), "").Replace(C.ToString(), "").Replace(D.ToString(), "");
                            pd = pd.Replace("+", "").Replace("-", "").Replace("*", "").Replace("/", "").Replace("×", "").Replace("÷", "").Replace("(", "").Replace(")", "").Replace("（", "").Replace("）", "");
                            if (String.IsNullOrEmpty(pd))
                            {
                                responseMsg = receivedMsg + " = 24 Kuma!!! 耗时：" + Calc24Seconds + "秒";
                                if (user_inti < 99)
                                    SetIntimacy(from_user_qq, user_inti + 2);
                                this.Calc24Seconds = -1;
                                Flag = true;
                            }
                            else
                                Flag2 = true;
                        }
                        if (!Flag && !Flag2)
                            responseMsg = "机智的黑白熊算出来的答案是" + get + "~Kuma!!!";
                        sr.Close();
                        goto Label_SendMsg;
                    }
                }
                else if (receivedMsg.Equals("算24"))
                {
                    do
                    {
                        this.Calc24Solved = false;
                        Gen24Problem();
                        Calc24Ans.Clear();
                        Solve24Problem(new double[] { A, B, C, D }, new List<Equation>());
                    }
                    while (!this.Calc24Solved);
                    this.Calc24Seconds = 0;
                    responseMsg = "24点：" + A + " " + B + " " + C + " " + D;
                    goto Label_SendMsg;
                }
                else if (receivedMsg.Equals("算24答案") && this.Calc24Seconds != -1)
                {
                    responseMsg = "24点：" + A + " " + B + " " + C + " " + D + "的答案";
                    this.Calc24Seconds = -1;
                    foreach (Equation i in Calc24Ans)
                        responseMsg += @"\\n" + i.Output();
                    goto Label_SendMsg;
                }
                else if (receivedMsg.Equals("出题") && ProblemSeconds == -1)
                {
                    long size = 0;
                    var info = new ProcessStartInfo("java", "-jar quest.jar");
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process CalcProcess = Process.Start(info);
                    CalcProcess.WaitForExit();
                    if (File.Exists("question.txt"))
                        size = new FileInfo("question.txt").Length;
                    if (size != 0)
                    {
                        StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + "\\question.txt");
                        responseMsg = "题主" + this.GetNickname(from_user_qq) + @"得到的题目是\\n" + sr.ReadLine() + @"\\n选项：" + (ChooseFrom = sr.ReadLine());
                        AnswerLine = sr.ReadLine();
                        ChooseFrom = RemoveUnnecessaryChar(ChooseFrom);
                        Answer = RemoveUnnecessaryChar(AnswerLine);
                        sr.Close();
                        ProblemUserQQ = from_user_qq;
                        ProblemUserInti = user_inti;
                        this.ProblemSeconds = 0;
                        goto Label_SendMsg;
                    }
                }
                else if (!String.IsNullOrEmpty(Answer) && receivedMsg2.Equals(Answer))
                {
                    if (user_inti < 98)
                        this.SetIntimacy(from_user_qq, user_inti + 3);
                    if (ProblemUserInti < 100)
                        this.SetIntimacy(ProblemUserQQ, ProblemUserInti + 1);
                    responseMsg = "恭喜" + this.GetNickname(from_user_qq) + "~" + receivedMsg.Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\n", "").Replace("\r", @"\\n").Replace("\"", "\\\\\\\"") + "~回答正确Kuma!!!3点好感度到手啦~题主也顺便获得了1点~";
                    Answer = "";
                    this.ProblemSeconds = -1;
                    goto Label_SendMsg;
                }
                else if (!String.IsNullOrEmpty(Answer) && receivedMsg.Equals("答案"))
                {
                    if (user_inti <= 100)
                        this.SetIntimacy(from_user_qq, user_inti - 1);
                    responseMsg = "答案是~" + AnswerLine + "Kuma!!!";
                    Answer = "";
                    this.ProblemSeconds = -1;
                    goto Label_SendMsg;
                }
                else if (!String.IsNullOrEmpty(Answer) && !String.IsNullOrEmpty(receivedMsg2) && ChooseFrom.Contains(receivedMsg2))
                {
                    if (user_inti <= 100)
                        this.SetIntimacy(from_user_qq, user_inti - 1);
                    responseMsg = "恭喜" + this.GetNickname(from_user_qq) + "~" + receivedMsg.Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\n", "").Replace("\r", @"\\n").Replace("\"", "\\\\\\\"") + "~回答错误Kuma!!!1点好感度不见了~";
                    if (ProblemSeconds > 6)
                        this.ProblemSeconds -= 6;
                    goto Label_SendMsg;
                }

                //Repeat Mode
                if (NowKumaMode == KumaMode.RepeatMode)
                {
                    responseMsg = receivedMsg.Replace(@"\", @"\\\\").Replace("\t", "").Replace("\f", "").Replace("\b", "").Replace("\n", "").Replace("\r", @"\\n").Replace("\"", "\\\\\\\"");
                    goto Label_SendMsg;
                }

                //Quiet Mode
                if (NowKumaMode == KumaMode.QuietMode || !this.checkBoxReply.Checked || ResponseRand.Next(100) >= this.Vitality)
                {
                    this.bSolveMsgThread = false;
                    return;
                }

                /*
                if (DateTime.Now.Minute == 0)
                {
                    if (!ClockReported)
                    {
                        ClockReported = true;
                        responseMsg = this.ClockReport();
                    }
                }
                else
                    ClockReported = false;
                */

                //Http Url
                Regex regex = new Regex(@"(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&$%\$#\=~])*");
                foreach (Match i in regex.Matches(receivedMsg))
                {
                    if (receivedMsgType == MsgType.GroupMsg)
                        responseMsg = this.PostUrlInGroup();
                    break;
                }

                receivedMsg = RemoveUnnecessaryChar(receivedMsg);
                //FullMatch
                if (FullMatchKnowledges.ContainsKey(receivedMsg))
                {
                    int Repeat = 0;
                    int Sum = FullMatchKnowledges[receivedMsg].Count;
                    try
                    {
                        KnowledgePack Get = null;
                        do
                        {
                            if (++Repeat > 10)
                                break;
                            Get = FullMatchKnowledges[receivedMsg].ElementAt(ResponseRand.Next(Sum));
                        }
                        while (!String.IsNullOrEmpty(Get.Intimacy) && user_inti < FlagToInt[Get.Intimacy]);
                        if (Repeat <= 10)
                        {
                            responseMsg = Get.ResponseMsg;
                            goto Label_SendMsg;
                        }
                    }
                    catch
                    { }
                }

                //WordsMatch
                int MaxMatchLength = 0;
                List<string> UseKeywordList = null;
                foreach (List<string> qwlist in WordsMatchKnowledges.Keys)
                {
                    int NowMatchLength = 0;
                    bool Flag = true;
                    foreach (string keyword in qwlist)
                    {
                        if (!receivedMsg.Contains(keyword))
                        {
                            Flag = false;
                            break;
                        }
                        else
                            NowMatchLength += keyword.Length;
                    }
                    if (Flag &&
                        (NowMatchLength > MaxMatchLength || (NowMatchLength == MaxMatchLength && ResponseRand.Next() % 2 == 0)))
                    {
                        MaxMatchLength = NowMatchLength;
                        UseKeywordList = qwlist;
                    }
                }
                if (MaxMatchLength > 0)
                {
                    int Repeat = 0;
                    int Sum = WordsMatchKnowledges[UseKeywordList].Count;
                    try
                    {
                        KnowledgePack Get = null;
                        do
                        {
                            if (++Repeat > 10)
                                break;
                            Get = WordsMatchKnowledges[UseKeywordList].ElementAt(ResponseRand.Next(Sum));
                        }
                        while (!String.IsNullOrEmpty(Get.Intimacy) && user_inti < FlagToInt[Get.Intimacy]);
                        if (Repeat <= 10)
                            responseMsg = Get.ResponseMsg;
                    }
                    catch { }
                }

                if (!String.IsNullOrEmpty(responseMsg) && ResponseRand.Next(100) < 33 && user_inti < 100)
                    this.SetIntimacy(from_user_qq, user_inti + 1);
            }
            catch (Exception e)
            {
                Log("[错误]查询失败" + receivedMsg, Color.Red);
                Log(e.Message, Color.Red);
                this.bSolveMsgThread = false;
                return;
            }

        Label_SendMsg:
            //Unhappy
            bool unhappy = false;
            foreach (string s in UnhappyString)
            {
                if (receivedMsg.Contains(s))
                {
                    unhappy = true;
                    break;
                }
            }
            if (unhappy && user_inti > -1 && user_inti < 200)
            {
                this.SetIntimacy(from_user_qq, user_inti - ResponseRand.Next(6, 12));
                if (String.IsNullOrEmpty(responseMsg))
                    responseMsg = this.KumaUnhappy();
            }


            if (!String.IsNullOrEmpty(MessageTail))
            {
                responseMsg += MessageTail;
                MessageTail = "";
            }
            if (String.IsNullOrEmpty(responseMsg))
            {
                this.bSolveMsgThread = false;
                return;
            }
            if (responseMsg.Contains("\n") ||
                responseMsg.Contains("[]") ||
                responseMsg.Contains("{}"))
            {
                this.bSolveMsgThread = false;
                return;
            }
            try
            {
                Thread.Sleep(0x0666);
                SendMsg(new MsgPack(receivedMsgType, responseMsg, from_uin));
            }
            catch
            {
                Log("[错误]发送失败" + receivedMsg, Color.Red);
            }
            Thread.Sleep(0x1333);

            this.bSolveMsgThread = false;
            return;
        }

        private void SendMsg(MsgPack message)
        {
            //Buddy 1
            if (message.MsgType == 1)
            {
                string url = "https://d.web2.qq.com/channel/send_buddy_msg2";
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Clear();
                string r = "{\"to\":" + message.ToUin + ",\"face\":0,\"content\":\"[\\\"" + message.Msg + "\\\",\\\"\\\",[\\\"font\\\",{\\\"name\\\":\\\"微软雅黑\\\",\\\"size\\\":\\\"10\\\",\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"msg_id\":" + this.GetMsgID().ToString() + ",\"clientid\":\"" + this.ClientID.ToString() + "\",\"psessionid\":\"" + LoginInfo.result["psessionid"] + "\"}";
                //r = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.UTF8.GetBytes(r)));
                param.Add("r", r);
                param.Add("clientid", this.ClientID.ToString());
                param.Add("psessionid", LoginInfo.result["psessionid"]);
                string response = Encoding.UTF8.GetString(this.CreateHttpRequest(url, param, "POST", 0x1333, null, null, this.SetCookies, false, false)).Replace("{\"retcode\":", "").Replace(",\"result", " \"result");
                //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: POST\r\n[Debug]Response: " + response, Color.Blue);
                if (response[0] == '0')
                    Log("[信息]回复成功", Color.White);// + message.Msg + "]", Color.White);
                else
                    Log("[错误]发送失败" + response, Color.Red);
                return;
            }
            //Group 2
            else if (message.MsgType == 2)
            {
                string url = "https://d.web2.qq.com/channel/send_qun_msg2";
                IDictionary<string, string> param = new Dictionary<string, string>();
                param.Clear();
                string r = "{\"group_uin\":" + message.ToUin + ",\"content\":\"[\\\"" + message.Msg + "\\\",\\\"\\\",[\\\"font\\\",{\\\"name\\\":\\\"微软雅黑\\\",\\\"size\\\":\\\"10\\\",\\\"style\\\":[0,0,0],\\\"color\\\":\\\"000000\\\"}]]\",\"msg_id\":" + this.GetMsgID().ToString() + ",\"clientid\":\"" + this.ClientID.ToString() + "\",\"psessionid\":\"" + LoginInfo.result["psessionid"] + "\"}";
                //r = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.UTF8.GetBytes(r)));
                param.Add("r", r);
                param.Add("clientid", this.ClientID.ToString());
                param.Add("psessionid", LoginInfo.result["psessionid"]);
                string response = Encoding.UTF8.GetString(this.CreateHttpRequest(url, param, "POST", 0x1333, null, null, this.SetCookies, false, false)).Replace("{\"retcode\":", "").Replace(",\"result", " \"result");
                //Log("[Debug]url连接结束: " + url + "\r\n[Debug]方式: POST\r\n[Debug]Response: " + response, Color.Blue);
                if (response[0] == '0')
                    Log("[信息]回复成功", Color.White);//[" + message.Msg + "]", Color.White);
                else
                    Log("[错误]回复失败" + response, Color.Red);
                return;
            }
            else
                return;
        }

        private void threadGetLogo()
        {
            try
            {
                string url = "http://face2.web.qq.com/cgi/svr/face/getface?cache=1&type=11&fid=0&uin=" + this.LoginInfo.result["uin"] + "&vfwebqq=" + this.LoginInfo.result["vfwebqq"] + "&t=" + this.GetTimeStamp();
                byte[] data = this.CreateHttpRequest(url, null, "GET", 0x2333, null, null, this.SetCookies, false, true);
                MemoryStream ms = new MemoryStream(data);
                this.pictureBox2.Image = Image.FromStream(ms);
            }
            catch (Exception e)
            {
                Log("头像获取失败" + e.Message, Color.Red);
            }
            this.bGetLogoThread = false;
            return;
        }

        private void CloseThreads()
        {
            if (this.bPoll2Thread)
            {
                Log("[系统]正在关闭Poll2线程", Color.Green);
                Poll2Thread.Abort();
                this.bPoll2Thread = false;
            }
            if (this.bSolveMsgThread)
            {
                Log("[系统]正在关闭SendMsg线程", Color.Green);
                SolveMsgThread.Abort();
                this.bSolveMsgThread = false;
            }
            if (this.bGetLogoThread)
            {
                Log("[系统]正在关闭GetLogo线程", Color.Green);
                GetLogoThread.Abort();
                this.bGetLogoThread = false;
            }
            return;
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            CloseThreads();
            Environment.Exit(0);
            return;
        }

        private void textBoxVerify_TextChanged(object sender, EventArgs e)
        {
            this.VerifyCode = this.textBoxVerify.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CloseThreads();
            SetCookies = new CookieContainer();
            LoginInfo = null;
            this.textBoxQQ.Enabled = true;
            this.textBoxPassword.Enabled = true;
            this.textBoxVerify.Enabled = true;
            this.label3.Enabled = true;
            this.pictureBox1.Enabled = true;
            this.button1.Enabled = true;
            this.button2.Enabled = false;
            this.groupBox2.Enabled = false;
            this.labelQQ.Text = "";
            this.labelQQName.Text = "";
            this.pictureBox2.Image = null;
            this.textBoxQQ.Select();
            this.textBoxQQ.Select(this.textBoxQQ.Text.Length, 0);
            this.timer1.Stop();
            this.textBoxSetQQ.Text = "";
            this.textBoxSetIntimacy.Text = "";
            this.textBoxVitality.Text = "";
            this.label11.Text = "0:00";
            this.Seconds = 0;
            this.checkBoxForget.Checked = true;
            this.checkBoxLog.Checked = true;
            this.checkBoxReply.Checked = true;
            this.checkBoxTeach.Checked = true;
            return;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.GetVerifyImage();
        }

        private void SetIntimacy(string qq, int inti)
        {
            if (inti > 100 && inti < 110)
                inti = 100;
            if (!Intimacy.ContainsKey(qq))
                Intimacy.Add(qq, 0);
            Intimacy[qq] = inti;
            FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + "\\Intimacy.txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            foreach (KeyValuePair<string, int> i in Intimacy)
                sw.WriteLine(i.Key + "|" + i.Value);
            sw.Close();
            fs.Close();
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.textBoxSetQQ.Text) && !String.IsNullOrEmpty(this.textBoxSetIntimacy.Text))
            {
                try
                {
                    this.SetIntimacy(this.textBoxSetQQ.Text, int.Parse(this.textBoxSetIntimacy.Text));
                    this.Log("[设置亲密]QQ:" + this.textBoxSetQQ.Text + "的好感度为" + int.Parse(this.textBoxSetIntimacy.Text), Color.White);
                }
                catch
                {
                    this.Log("[设置失败]", Color.Red);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.richTextBoxLog.Text = "";
        }

        private void threadClockReport()
        {
            foreach (string uin in ChattedGroupUin)
            {
                try { SendMsg(new MsgPack(MsgType.GroupMsg, this.ClockReport(), uin)); }
                catch { }
                Thread.Sleep(3000);
            }
            this.bSolveMsgThread = false;
            return;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ++Seconds;
            this.label11.Text = (Seconds / 60).ToString() + ":" + (Seconds % 60 < 10 ? "0" : "") + (Seconds % 60).ToString();
            if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                while (bSolveMsgThread) ;
                this.bSolveMsgThread = true;
                ClockReportThread = new Thread(new ThreadStart(threadClockReport));
                ClockReportThread.Start();
            }
            else if (DateTime.Now.Minute == 50 && DateTime.Now.Second < 10)
                ChattedGroupUin.Clear();
            if (Calc24Seconds != -1)
                ++Calc24Seconds;
            if (ProblemSeconds != -1)
            {
                ++ProblemSeconds;
                if (ProblemSeconds > 30)
                {
                    ProblemSeconds = -2;
                    if (ProblemUserInti <= 100)
                        this.SetIntimacy(ProblemUserQQ, ProblemUserInti - 5);
                    this.MessageTail = "回答题目超时啦~题主的好感度5点不见了~";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int t = int.Parse(this.textBoxVitality.Text);
                this.SetVitality(t);
                this.Log("[设置活跃]" + t.ToString() + "\r\n", Color.White);
            }
            catch
            {
                this.Log("[设置失败]", Color.Red);
            }
        }

    }
    public class LoginRetInfo
    {
        public int retcode { get; set; }
        public Dictionary<string, string> result { get; set; }
        override public string ToString()
        {
            string res = "{\r\n    [Retcode]=[" + retcode + "]\r\n}\r\n{\r\n";
            foreach (string s in result.Keys)
                res += "    [" + s + "]=[" + result[s] + "]\r\n";
            return res + "}";
        }
    }

    public class PollRetInfo
    {
        public int retcode { get; set; }
        public Dictionary<string, string>[] result { get; set; }
        public string p { get; set; }
        override public string ToString()
        {
            string res = "{\r\n    [Retcode]=[" + retcode + "]\r\n}\r\n{\r\n";
            foreach (string s in result[0].Keys)
                res += "    [" + s + "]=[" + result[0][s] + "]\r\n";
            return res + "}";
        }
    }

    public class MsgType
    {
        public const int BuddyMsg = 1;
        public const int GroupMsg = 2;
    }
    /// <summary>
    /// 一个表示发送信息的包
    /// </summary>
    public class MsgPack
    {
        public int MsgType { get; set; }
        public string Msg { get; set; }
        public string ToUin { get; set; }
        public MsgPack(int msgtype, string msg, string touin)
        {
            this.MsgType = msgtype;
            this.Msg = msg;
            this.ToUin = touin;
            return;
        }
    }

    /// <summary>
    /// 答复Pack
    /// </summary>
    public class KnowledgePack
    {
        //Depend
        public string Intimacy { get; set; }

        //MsgBody
        public string ResponseMsg { get; set; }

        public KnowledgePack(string msg, string intimacy)
        {
            this.ResponseMsg = msg;
            this.Intimacy = intimacy;
        }
    }
    /// <summary>
    /// Kuma Mode
    /// </summary>
    public class KumaMode
    {
        public const int NormalMode = 0;
        public const int RepeatMode = 233;
        public const int QuietMode = 1;
    }

    public class Equation
    {
        public double A, B, C;
        public char Operator;
        public Equation(double a, char op, double b, double c)
        {
            A = a;
            Operator = op;
            B = b;
            C = c;
        }
        public string Output()
        {
            return A.ToString() + Operator.ToString() + B.ToString() + "=" + C.ToString();
        }
    }
}
