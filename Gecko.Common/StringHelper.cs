using System;
using System.Text;
using System.Web;
using System.Data;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace Gecko.Common
{
    /// <summary>
    /// StringHelper 用于字符串的公用方法
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StringHelper() { }

        public static string ObfuscateIdCard(string idCard)
        {
            if (idCard == null)
            {
                return string.Empty;
            }

            if (idCard.Length > 14)
            {
                idCard = idCard.Replace(idCard.Substring(3, 11), "***********");
            }
            else if (idCard.Length > 5)
            {
                idCard = idCard.Replace(idCard.Substring(1, 3), "***");
            }

            return idCard;
        }

        public static string ObfuscateMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return string.Empty;
            }

            if (mobile.Length > 7)
            {
                return string.Format("{0}****{1}", mobile.Substring(0, 3), mobile.Substring(7, mobile.Length - 7));
            }

            return mobile;
        }

        public static string GetStringWithOutHtml(string str)
        {
            str = Regex.Replace(str, @"<\/?[^>]*>", "", RegexOptions.Multiline);
            str = Regex.Replace(str, @"&nbsp;", "", RegexOptions.Multiline);
            str = Regex.Replace(str, @"&ldquo;", "“", RegexOptions.Multiline);
            str = Regex.Replace(str, @"&rdquo;", "”", RegexOptions.Multiline);
            return str;
        }

        /// <summary>
        /// 替换空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringWithOutSpace(string str)
        {
            str = Regex.Replace(str, @"\s", "");
            return str;
        }

        /// <summary>
        /// 验证字符串是否为纯数字!
        /// </summary>
        /// <param name="s">需要验证的字符串</param>
        /// <returns></returns>
        public static bool IsNum(string s)
        {
            string strReg = "^\\d+$";
            Regex r = new Regex(strReg, RegexOptions.IgnoreCase);
            Match m = r.Match(s);
            return m.Success;
        }
        /// <summary>
        /// 验证字符串是否是浮点类型！
        /// </summary>
        /// <param name="s">需要验证的字符串</param>
        /// <returns></returns>
        public static bool IsFloat(string s)
        {
            string pattern = "^\\d+\\.\\d+$";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(s);
            return m.Success;
        }
        /// <summary>
        /// 获取字符串中的字母和数字！
        /// </summary>
        /// <param name="s">需要获取的字符串</param>
        /// <returns></returns>
        public static string GetNLString(string s)
        {
            if (s == null || s == String.Empty)
                return string.Empty;
            return new Regex("[^0-9a-zA-Z]").Replace(s, String.Empty);
        }

        #region 加密算法
        /// <summary>
        /// MD5加密算法
        /// </summary>
        /// <param name="source">源字串</param>
        /// <param name="blnPwd">是否为密码加密</param>
        /// <returns></returns>
        public static string EncryptMD5(string source, bool blnPwd)
        {
            //如果为密码加密.
            if (blnPwd)
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5");
            byte[] data = Encoding.Unicode.GetBytes(source.ToCharArray());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string sResult = Encoding.Unicode.GetString(result);
            return sResult;
        }
        /// <summary>
        /// MD5函数2
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = String.Empty;
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        /// <summary>
        /// DEC 加密过程 
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <param name="sIV"></param>
        /// <returns></returns>
        public static string EncryptHttpString(string pToEncrypt, string sKey, string sIV)
        {
            //把字符串放到byte数组中
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            //建立加密对象的密钥和偏移量
            des.Key = Encoding.ASCII.GetBytes(sKey);
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法 
            des.IV = Encoding.ASCII.GetBytes(sIV);
            //使得输入密码必须输入英文文本 
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
                ret.AppendFormat("{0:X2}", b);
            ret.ToString();
            return ret.ToString();
        }
        /// <summary>
        /// DEC 解密过程 
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <param name="sIV"></param>
        /// <returns></returns>
        public static string DecryptHttpString(string pToDecrypt, string sKey, string sIV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(sKey); //建立加密对象的密钥和偏移量，此值重要，不能修改 
            des.IV = Encoding.ASCII.GetBytes(sIV);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder(); //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象 
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        #endregion

        /// <summary>
        /// 返回字串的字节长度.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static int GetStringByteLength(string src)
        {
            byte[] sarr = System.Text.Encoding.Default.GetBytes(src);
            return sarr.Length;
        }
        /// <summary>
        /// 将全角字符转换为半角字符
        /// </summary>
        /// <param name="QJstr">源字符串</param>
        /// <returns>源字符串所对应的半角字串</returns>
        public static string TransToDBC(string QJstr)
        {
            if (QJstr.Trim() == "") return "";

            char[] c = QJstr.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length == 2)
                {
                    if (b[1] == 255)
                    {
                        b[0] = (byte)(b[0] + 32);
                        b[1] = 0;
                        c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                    }
                }
            }
            string strNew = new string(c);
            return strNew;
        }

        public static string Abbreviate(string s, int length)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length > length)
            {
                return s.Substring(0, length - 1) + "..";
            }

            return s;
        }
        public static string Abbreviate(string s, int length, string ReplaceStr)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length > length)
            {
                return s.Substring(0, length - 1) + ReplaceStr;
            }

            return s;
        }

        /// <summary>
        /// 根据长度获取随机字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            length = length < 6 ? 6 : length;
            string radStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rand = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int random = rand.Next(36);
                sb.Append(radStr.Substring(random, 1));
                System.Threading.Thread.Sleep(12);
            }
            return sb.ToString();
        }

        public static string AbbreviateMid(string s, int length)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length > length)
            {
                int excess = s.Length - length;

                try
                {
                    string head = s.Substring(0, s.Length / 2 - excess / 2 - 2);
                    string foot = s.Substring(s.Length / 2 + excess / 2 + 2);
                    return head + "..." + foot;
                }
                catch { }
            }
            return s;
        }
        /// <summary>
        /// 获取安全字符串，并限制长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string InputText(string text, int maxLength)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            return InputText(text);
        }
        /// <summary>
        /// 限制显示长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string substr(string text, int maxLength)
        {
            if (text == null) return string.Empty;

            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            return text;
        }

        /// <summary>
        /// 显示字符串
        /// </summary>
        /// <param name="text">要截取的字符串</param>
        /// <param name="length">长度</param>
        /// <param name="replace">替换为</param>
        /// <returns></returns>
        public static string substr(string text, int length, string replace)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = text.Trim();
            if (text.Length < length)
                return text;
            return text.Substring(0, length) + replace;
        }


        /// <summary>
        /// 区分中、英文以及数字的 字符截取函数
        /// </summary>
        /// <param name="text">需要获取的字符</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="isState">随意给一个true或false都可以。无任何实际意义，主要用于实现之前已存在的 Substr 函数的重载</param>
        /// <example></example>
        /// <remarks>邢智， 2011年1月18日 新增</remarks>
        /// <returns></returns>
        public static string substr(string text, int maxLength, bool isState)
        {
            text = text.Trim();
            if (text == null || text == String.Empty)
                return String.Empty;

            int i = 0;  //计数器
            int j = 0;  //计数器
            foreach (char chr in text)
            {
                //判断ASCII码值
                if ((int)chr > 127)
                    i += 2;
                else
                    i++;
                //验证是否超出指定长度
                if (i > maxLength)
                {
                    text = text.Substring(0, j);
                    break;
                }
                j++;
            }
            return text;
        }

        public static int GetStringLength(string str, bool isDouble)
        {
            if (str == String.Empty)
                return 0;
            else
            {
                int i = 0;
                if (isDouble)
                {
                    foreach (char c in str)
                    {
                        if ((int)c > 127)
                            i += 2;
                        else
                            i++;
                    }
                    return i;
                }
                else
                    return str.Length;
            }
        }
        /// <summary>
        /// 过滤安全字符串
        /// </summary>
        /// <param name="text">需要过滤的源字符串</param>
        /// <returns></returns>
        public static string InputText(string text)
        {
            if (text == null)
                return String.Empty;
            text = text.Trim();
            if (String.IsNullOrEmpty(text))
                return String.Empty;
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//any other tags
            text = Regex.Replace(text, "◇|◢|▼|▽|⊿|♡|■|▓|╝|╚|╔|╗|╬|╓|╩|┠|┨|┯|┷|┏|┓|┳|⊥|﹃|﹄|┌|┐|└|┘|∟|「|」|↑|↓|→|←|↘|↙|┇|┅|﹉|﹊", string.Empty);
            text = Regex.Replace(text, "﹍|﹎|╭|╮|╰|╯|∵|︴|﹏|﹋|﹌|︵|︶|︹|︺|【|】|〖|〗|﹕|﹗|·|√|∪|∩|∈|の|℡|ぁ|§|ミ|灬|№|＊|||≮|≯|÷|±|∫|∝|∧|∨|∏", string.Empty);
            text = Regex.Replace(text, "∥|∠|≌|∽|≒|じ|⊙|●|★|☆|『|』|◆|◣|▲|Ψ|◤|◥|㊣|∑|⌒|ξ|ζ|ω|□|∮|〓|※|∴|ぷ|卐|△|¤|々|♀|♂|∞|①|ㄨ|≡|▃|▄|▅|▆|▇|█|┗|┛", "");
            text = Regex.Replace(text, "var|truncate|delete|insert|select|drop|alter|update|exec|char|fetch|declare|cast|convert|sysobjects|syscolumns|create|%20|exists|trace|xp_cmdshell", "", RegexOptions.IgnoreCase);
            text = text.Replace("'", "＇");
            text = text.Replace(";", "；");
            text = text.Replace("--", "－－");
            return text;
        }

        private int rep = 0;
        /// <summary>
        /// 生成随机数字字符串
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成的数字字符串</returns>
        public string GenerateCheckCodeNum(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// 待生成的位数
        /// 生成的字母字符串
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public string GenerateCheckCode(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + this.rep;
            this.rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> this.rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// 剔除 Html 页面上的javascript、style标签以及其中内容！
        /// </summary>
        /// <param name="input">需要剔除的字符串</param>
        /// <returns></returns>
        public static string NoTagText(string input)
        {
            string ret = input;
            ret = System.Text.RegularExpressions.Regex.Replace(ret, "<script(.*?)</script>", "");
            ret = System.Text.RegularExpressions.Regex.Replace(ret, "<style(.*?)</style>", "");
            ret = System.Text.RegularExpressions.Regex.Replace(ret, "<(.*?)>", "");
            return ret;
        }


        public static string InputTitle(string text)
        {
            if (text == null)
                return String.Empty;
            text = Regex.Replace(text, @"[^\u4e00-\u9fa5\da-zA-Z\.\-]", "");
            text = Regex.Replace(text, "var|truncate|delete|insert|select|drop|alter|update|exec|char|fetch|declare|cast|convert|sysobjects|syscolumns|create|%20|exists|trace|xp_cmdshell", "", RegexOptions.IgnoreCase);
            return text;
        }
        /// <summary>
        /// 转换一,二,三,四等为阿拉伯数字
        /// </summary>
        /// <param name="s">一,二,三,四等</param>
        /// <returns>num</returns>
        public static int TransNum(string s)
        {
            string[] arr = new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            int num = -1;
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] == s.Trim())
                    num = i + 1;
            return num;
        }

        /// <summary>
        /// i 
        /// </summary>
        /// <param name="i">0-10的整数</param>
        /// <returns></returns>
        public static string TransNum(int i)
        {
            string[] arr = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
            int num = -1;
            if (i > 0 && i < 10)
            {
                return arr[i];
            }
            return "";
        }

        /// <summary>
        /// 将字符串编码为Base64字符串
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        public static string Base64Encode(string str)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 将Base64字符串解码为普通字符串
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        public static string Base64Decode(string str)
        {
            byte[] barray;
            barray = Convert.FromBase64String(HttpUtility.UrlDecode(str, Encoding.UTF8));
            return Encoding.UTF8.GetString(barray);
        }

        /// <summary>
        /// 判断一个字符串是否在字符串数组里面。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strarry"></param>
        /// <returns></returns>
        public static bool StrInArray(string str, string[] strarry)
        {
            if (str == null)
                return false;
            if (strarry == null || strarry.Length == 0)
                return false;
            for (int i = 0; i < strarry.Length; i++)
            {
                if (strarry[i] == null)
                    continue;
                if (str == strarry[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 替换特殊字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string InputSMS(string text)
        {
            if (text == null)
                return "";

            string s = System.Text.RegularExpressions.Regex.Replace(text.Trim(), "(^86)|(^8#)", "");

            s = System.Text.RegularExpressions.Regex.Replace(s, "(^qz)|(^q)", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            s = System.Text.RegularExpressions.Regex.Replace(s, "^\\,|^\\s+|^\\，", "");

            return s;
        }

        public static string SqlPrefix(string sql)
        {
            System.Web.HttpContext web = System.Web.HttpContext.Current;

            StringBuilder sb = new StringBuilder();

            sb.Append("/* #" + web.Request.ServerVariables["LOCAL_ADDR"]);
            sb.Append("#" + web.Request.ServerVariables["REMOTE_ADDR"]);
            sb.Append("#" + web.Request.ServerVariables["URL"]);
            sb.Append("# */");
            sb.Append(sql);


            web = null;
            return sb.ToString();
        }

        #region 自定义 URLEncode 与 UrlDecode

        public static string UrlEncode(string s)
        {
            if (s == null || s == string.Empty) return string.Empty;

            s = s.Replace(".", "");

            return HttpUtility.UrlEncode(s, Encoding.UTF8).Replace("%", "-");
        }

        public static string UrlDecode(string s)
        {
            if (s == null || s == string.Empty) return string.Empty;

            return HttpUtility.UrlDecode(s.Replace("-", "%")).Replace("%", "-");
        }

        public static string UrlDecode(string s, bool filterHtml)
        {
            if (filterHtml)
                return RegexHelper.ReplaceHtml("", UrlDecode(s), "");
            else
                return UrlDecode(s);
        }
        //针对只有cookie名，没有key的情况。
        public static string getCookies(string cookieName)
        {
            HttpCookie cookie;
            cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                return HttpContext.Current.Server.HtmlEncode(cookie.Value);
            }
            else
            {
                return "";
            }
        }
        #endregion

        public static string GetConfigApp(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get(key);
        }
        public static string GetLongStringByTime()
        {
            string day = DateTime.Now.Year.ToString("0000") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00");
            string time = DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00");
            string ms = DateTime.Now.Millisecond.ToString("00");
            return day + time + ms;

        }


        //public static string ToJson<T>(T t)
        //{
        //    try
        //    {
        //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(t.GetType());
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            serializer.WriteObject(ms, t);
        //            return Encoding.UTF8.GetString(ms.ToArray());
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public static T FromJson<T>(string strJson) where T : class
        //{
        //    if (string.IsNullOrEmpty(strJson)) return Activator.CreateInstance(typeof(T)) as T;

        //    try
        //    {
        //        DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));

        //        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson));
        //        return ds.ReadObject(ms) as T;
        //    }
        //    catch
        //    {
        //        return Activator.CreateInstance(typeof(T)) as T;
        //    }
        //}

        /// <summary>
        /// 替换瘦文本框中的空格、大于号、小于号以及软回车
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string EncodeText(string content)
        {
            content = content.Replace(" ", "&nbsp;");//处理空格   
            content = content.Replace("<", "&lt;");//处理小于号   
            content = content.Replace(">", "&gt;");//处理大于号   
            content = content.Replace("\n", "<br/>");//处理换行   

            return content;
        }

        /// <summary>
        /// 与EncodeText方法作用相反
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string DecodeText(string content)
        {
            content = DropHtmlTags(content);
            content = content.Replace("&nbsp;", " ");//处理空格
            content = content.Replace("&lt;", "<");//处理小于号
            content = content.Replace("&gt;", ">");//处理大于号
            content = content.Replace("<br/>", "\n");//处理换行
            return content;
        }
        /// <summary>
        /// 去除Html"标签"
        /// </summary>
        /// <param name="HtmlText"></param>
        /// <returns></returns>
        public static string DropHtmlTags(string HtmlText)
        {
            if (string.IsNullOrEmpty(HtmlText))
                return string.Empty;
            else
            {
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"<.*?>", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.Compiled);
                return reg.Replace(HtmlText, string.Empty);
            }
        }

        /// <summary>
        /// 获取用星号替换后的字符串, 保留指定showchar长度的不替换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="showchar"></param>
        /// <returns></returns>
        public static string GetHiddenString(string source, int showchar)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            string s = source;
            int length = source.Length;
            if (length > showchar)
            {
                s = source.Substring(0, showchar);
                for (int i = 0; i < length - showchar; i++)
                    s += "*";
            }
            return s;
        }

        /// <summary>
        /// 获取从start用*号替换后面保留leave个字符的字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="leave"></param>
        /// <returns></returns>
        public static string GetHiddenString(string source, int start, int leave)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            int length = source.Length;
            if (leave + start >= length) return GetHiddenString(source, start);
            if (length > leave + start)
            {
                char[] chs = source.ToCharArray();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < chs.Length; i++)
                {
                    if (i < start || i > length - leave - 1)
                    {
                        sb.Append(chs[i]);
                    }
                    else
                    {
                        sb.Append("*");
                    }
                }
                return sb.ToString();
            }
            return source;
        }

        /// <summary>
        /// 计算两个字符串相似度
        /// (编辑距离)
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static int GetStringDistance(string str1, string str2)
        {
            str1 = string.IsNullOrEmpty(str1) ? string.Empty : str1;
            str2 = string.IsNullOrEmpty(str2) ? string.Empty : str2;
            Regex reg = new Regex(@"\s|\W");
            str1 = reg.Replace(str1, "");
            str2 = reg.Replace(str2, "");
            string tmp = str1; //tmp临时存放
            Console.WriteLine(str1);
            Console.WriteLine(str2);
            str1 = str1.Length > str2.Length ? str2 : str1; //str1存放短字符串
            str2 = tmp == str1 ? str2 : tmp; //str2存放长字符串
            int distance = str2.Length - str1.Length;
            Console.WriteLine(distance);
            char[] charsA = str1.ToCharArray();
            char[] charsB = str2.ToCharArray();
            for (int i = 0; i < charsA.Length; i++)
            {
                if (charsA[i] != charsB[i])
                {
                    distance++;
                }
            }
            return distance;
        }

        /// <summary>
        /// 获取分页的字符串 ， getPagerStr("/ask/daikuan", "/ask/daikuan-{1}.html", 12, 1000, 10);
        /// </summary>
        /// <param name="urlPageFirst">第一页的完整链接</param>
        /// <param name="urlPattern">其它页的链接</param>
        /// <param name="PageIndex"></param>
        /// <param name="count">总记录条数</param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static string getPagerStr(string urlPageFirst, string urlPattern, int PageIndex, int count, int PageSize)
        {
            string reStr = "";

            if (PageSize >= count)
                return reStr;

            string TempletStr = TempletStr = "  <a href='" + urlPattern + "' class='pagerlink'>{0}</a>  ";
            int ForCount = 9;
            int AllCount = ((count % PageSize) == 0) ? (count / PageSize) : ((count / PageSize) + 1);
            int start = ((PageIndex - 4) < 1) ? 1 : (PageIndex - 4);
            if (AllCount < 10)
            {
                ForCount = AllCount - start + 1;
            }
            start = ((PageIndex + 9) > AllCount && (AllCount - 8) > 0) ? (AllCount - 8) : start;
            start = (PageIndex - 4) > 0 ? (PageIndex - 4) : start;
            if (AllCount < start + 10)
            {
                ForCount = AllCount - start + 1;
            }
            int end = start + ForCount;

            if (start > 1)
            {
                if (urlPageFirst != "")
                    reStr += "  <a href='" + urlPageFirst + "' class='pagerlink'>第一页</a>  ";
                else
                    reStr += string.Format(TempletStr, "第一页", "1");
            }
            for (int i = 0; i < ForCount; i++)
            {
                if (start + i == PageIndex)
                {
                    reStr += "<span class='index'>" + PageIndex.ToString() + "</span>";
                }
                else
                {
                    if (start + i == 1)
                    {
                        if (urlPageFirst != "")
                            reStr += "  <a href='" + urlPageFirst + "' class='pagerlink'>" + ((start + i).ToString()) + "</a>  ";
                        else
                            reStr += string.Format(TempletStr, ((start + i).ToString()), (start + i).ToString());
                    }
                    else
                    {
                        reStr += string.Format(TempletStr, ((start + i).ToString()), (start + i).ToString());
                    }
                }
            }
            if (end < AllCount)
            {
                reStr += string.Format(TempletStr, "最后一页", AllCount.ToString());
            }
            return reStr;
        }


        /// <summary>
        /// 获取分页的字符串 ， getPagerStr("/dikuan/geren", "/dikuan/geren-{1}.html", 12, 1000, 10);
        /// </summary>
        /// <param name="urlPageFirst">第一页的完整链接</param>
        /// <param name="urlPattern">其它页的链接</param>
        /// <param name="PageIndex"></param>
        /// <param name="count">总记录条数</param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static string getPagerDaikuan(string urlPageFirst, string urlPattern, int PageIndex, int count, int PageSize)
        {
            string reStr = "";

            if (PageSize >= count)
                return reStr;

            string TempletStr = TempletStr = "  <a href='" + urlPattern + "' class='pagerlink'>{0}</a>  ";
            int ForCount = 9;
            int AllCount = ((count % PageSize) == 0) ? (count / PageSize) : ((count / PageSize) + 1);
            int start = ((PageIndex - 4) < 1) ? 1 : (PageIndex - 4);
            if (AllCount < 10)
            {
                ForCount = AllCount - start + 1;
            }
            start = ((PageIndex + 9) > AllCount && (AllCount - 8) > 0) ? (AllCount - 8) : start;
            start = (PageIndex - 4) > 0 ? (PageIndex - 4) : start;
            if (AllCount < start + 10)
            {
                ForCount = AllCount - start + 1;
            }
            int end = start + ForCount;

            if (start > 1)
            {
                if (urlPageFirst != "")
                    reStr += "  <a href='" + urlPageFirst + "' class='pageindex'>第一页</a>  ";
                else
                    reStr += string.Format(TempletStr, "第一页", "1");
            }
            for (int i = 0; i < ForCount; i++)
            {
                if (start + i == PageIndex)
                {
                    reStr += "<span class='pageindex seld'>" + PageIndex.ToString() + "</span>";
                }
                else
                {
                    if (start + i == 1)
                    {
                        if (urlPageFirst != "")
                            reStr += "  <a href='" + urlPageFirst + "' class='pageindex'>" + ((start + i).ToString()) + "</a>  ";
                        else
                            reStr += string.Format(TempletStr, ((start + i).ToString()), (start + i).ToString());
                    }
                    else
                    {
                        reStr += string.Format(TempletStr, ((start + i).ToString()), (start + i).ToString());
                    }
                }
            }
            if (end < AllCount)
            {
                reStr += string.Format(TempletStr, "最后一页", AllCount.ToString());
            }
            return reStr;
        }




        public static string GetContents(string cont, int len)
        {
            return GetContents(cont, len, false);
        }

        public static string GetContents(string cont, int len, bool hasdot)
        {
            if (cont == String.Empty || cont == null)
                return String.Empty;
            else
            {
                cont = cont.Trim();
                cont = Gecko.Common.RegexHelper.Replace(cont, @"</?[a-zA-Z]+[^><]*>", String.Empty);
            }
            string strReturn = Gecko.Common.StringHelper.substr(cont, len);
            return (cont.Length > len && hasdot) ? strReturn + "..." : strReturn;
        }

        public static string GetNewsPath(string tid, string id)
        {
            string strRet = "http://www.Gecko.com/news/";
            switch (tid)
            {
                case "1":
                    strRet = strRet + "notice/" + id + ".html";
                    break;
                case "11":
                    strRet = strRet + "media/" + id + ".html";
                    break;
                case "17":
                    strRet = strRet + "industry/" + id + ".html";
                    break;
                case "20":
                    strRet = strRet + "shequ/" + id + ".html";
                    break;
                case "21":
                    strRet = strRet + "read/" + id + ".html";
                    break;
                case "22":
                    strRet = strRet + "article/" + id + ".html";
                    break;
                case "23":
                    strRet = strRet + "todaytopic/" + id + ".html";
                    break;
            }
            return strRet;
        }

        /// <summary>2016-04-07-sk
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetNewsPath2(string tid, string id)
        {
            int newstype = Convert.ToInt32(tid);
            string strRet = "/news/";
            switch (newstype)
            {
                case (int)Gecko.Common.Logic.NewsType.安心公告 :
                    strRet = strRet + "notice/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.媒体报道:
                    strRet = strRet + "media/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.行业资讯:
                    strRet = strRet + "industry/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.理财故事:
                    strRet = strRet + "shequ/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.理财要闻:
                    strRet = strRet + "read/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.微信文章:
                    strRet = strRet + "article/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.原创精品:
                    strRet = strRet + "original/" + id + ".html";
                    break;
                case (int)Gecko.Common.Logic.NewsType.今日话题:
                    strRet = strRet + "todaytopic/" + id + ".html";
                    break;
            }
            return strRet;
        }
        public static string GetNewsPath2(string tid)
        {
            int newstype = Convert.ToInt32(tid);
            string strRet = "/news/";
            switch (newstype)
            {
                case (int)Gecko.Common.Logic.NewsType.安心公告:
                    strRet = strRet + "notice/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.媒体报道:
                    strRet = strRet + "media/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.行业资讯:
                    strRet = strRet + "industry/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.理财故事:
                    strRet = strRet + "shequ/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.理财要闻:
                    strRet = strRet + "read/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.微信文章:
                    strRet = strRet + "article/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.原创精品:
                    strRet = strRet + "original/";
                    break;
                case (int)Gecko.Common.Logic.NewsType.今日话题:
                    strRet = strRet + "todaytopic/";
                    break;
            }
            return strRet;
        }
        public static int GetNewsTypeID(string typename)
        {
            int typeid = (int)Gecko.Common.Logic.NewsType.安心公告;
            switch (typename)
            {
                case "notice"://公告
                    typeid = (int)Gecko.Common.Logic.NewsType.安心公告;
                    break;
                case "media"://媒体
                    typeid = (int)Gecko.Common.Logic.NewsType.媒体报道;
                    break;
                case "industry"://行业
                    typeid = (int)Gecko.Common.Logic.NewsType.行业资讯;
                    break;
                case "shequ"://社区
                    typeid = (int)Gecko.Common.Logic.NewsType.理财故事;
                    break;
                case "read"://推荐
                    typeid = (int)Gecko.Common.Logic.NewsType.理财要闻;
                    break;
                case "article"://微信
                    typeid = (int)Gecko.Common.Logic.NewsType.微信文章;
                    break;
                case "original"://原创
                    typeid = (int)Gecko.Common.Logic.NewsType.原创精品;
                    break;
                case "todaytopic"://今日话题
                    typeid = (int)Gecko.Common.Logic.NewsType.今日话题;
                    break;
            }
            return typeid;
        }
        public static string GetNewsTypeName(int typeid)
        {
            string typename = "notice";
            switch (typeid)
            {
                case (int)Gecko.Common.Logic.NewsType.安心公告://公告
                    typename = "notice";
                    break;
                case (int)Gecko.Common.Logic.NewsType.媒体报道://媒体
                    typename = "media";
                    break;
                case (int)Gecko.Common.Logic.NewsType.行业资讯://行业
                    typename = "industry";
                    break;
                case (int)Gecko.Common.Logic.NewsType.理财故事://社区
                    typename = "shequ";
                    break;
                case (int)Gecko.Common.Logic.NewsType.理财要闻://推荐
                    typename = "read";
                    break;
                case (int)Gecko.Common.Logic.NewsType.微信文章://微信
                    typename = "article";
                    break;
                case (int)Gecko.Common.Logic.NewsType.原创精品://原创
                    typename = "original";
                    break;
                case (int)Gecko.Common.Logic.NewsType.今日话题://话题
                    typename = "todaytopic";
                    break;
            }
            return typename;
        }
        public static int GetCommentCount(string tid)
        {
            var bbspuburl = @"http://bbs.Gecko.com/pub/mobilebbstopic.aspx?page=1&tpp=1&topicid=" + tid;
            XmlHelper xmlList = new XmlHelper();
            try
            {
                xmlList.LoadXml(HttpHelper.HttpGetHTML(bbspuburl));
                int intTotalCount = Convert.ToInt32(xmlList.Doc.DocumentElement.SelectSingleNode("postcount").InnerText);
                // strTotalCount = intTotalCount.ToString() + "条评论";
                // int intTotalPage = Convert.ToInt32(xmlList.Doc.DocumentElement.SelectSingleNode("pagecount").InnerText);
                if (intTotalCount == 1)
                {
                    return intTotalCount;
                }
                return intTotalCount;
            }
            catch (Exception exp)
            {
                return 0;
            }
        }
        //2016-04-07-sk 结束

        /// <summary>
        /// 安心贷款daikuan
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetDaikuanPath(int tid, string id)
        {
            string path = "";
            string strRet = "";
            Dictionary<int, string> Dic = Logic.getDaikuanTypePinyin();
            if (Dic.ContainsKey(tid))
            {
                path = Dic[tid].ToString();

                switch (tid / 100)
                {
                    case 1:
                        strRet = "/" + path + "/daikuan-" + id + ".html";
                        break;
                    case 2:
                        strRet = "/" + path + "/daikuan-" + id + ".html";
                        break;
                    case 3:
                        strRet = "/" + path + "/daikuan-" + id + ".html";
                        break;
                    case 4:
                        strRet = "/" + path + "/daikuan-" + id + ".html";
                        break;
                    case 5:
                        strRet = "/" + path + "/daikuan-" + id + ".html";
                        break;
                    case 6:
                        strRet = "/" + path + "/daikuan-" + id + ".html";
                        break;
                }
            }
            return strRet;
        }
        ///   去除HTML标记   
        ///   </summary>   
        ///   <param   name="NoHTML">包括HTML的源码   </param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.|\n)+?>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n]|\n)[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = Htmlstring.Replace("\n", "");
            Htmlstring = Htmlstring.Replace("&lsquo;", "‘").Replace("&rsquo;", "’").Replace("&ldquo;", "“").Replace("&rdquo;", "”");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        /// <summary>
        /// 校验18位身份证
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckIDCard(string str)
        {
            string number17 = str.Substring(0, 17);
            string number18 = str.Substring(17);
            string check = "10X98765432";
            int[] num = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int sum = 0;
            for (int i = 0; i < number17.Length; i++)
            {
                sum += Convert.ToInt32(number17[i].ToString()) * num[i];
            }
            sum %= 11;
            if (number18.Equals(check[sum].ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// 获取中文大写数值, 只能两位小数
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string GetMoneyChineseNumber(double amount)
        { 
            var fraction = new string[] { "角", "分" };
            var digit = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[,] unit = new string[,] { { "元", "万", "亿", "万亿" }, { "", "拾", "佰", "仟" } };


            var head = amount < 0 ? "欠" : "";
            amount = Math.Abs(amount);
            var s = "";
            for (var i = 0; i < fraction.Length; i++)
            {
                s += (digit[(int)(Math.Floor(amount * 10d * Math.Pow(10, i)) % 10)] + fraction[i]);
            }

            s = Regex.Replace(s, "零.", "");
            s = string.IsNullOrEmpty(s) ? "整" : s;
            amount = Math.Floor(amount);
            for (var i = 0; i < 4 && amount > 0; i++)
            {
                var p = "";
                for (var j = 0; j < 4 && amount > 0; j++)
                {
                    p = digit[(long)amount % 10] + unit[1, j] + p;

                    amount = Math.Floor(amount / 10);
                }
                p = string.IsNullOrEmpty(p) ? "零" : p;
                s = Regex.Replace(p, "(零.)*零$", "") + unit[0, i] + s;
            }
            s = s.Replace("亿亿", "亿");
            string result = head + s;
            result = Regex.Replace(result, "(零.)*零元", "元");
            result = Regex.Replace(result, "零.", "零");
            result = result == "整" ? "零元整" : result;
            return result;
        }

    }
}