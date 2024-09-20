using System.Security.Cryptography;
using System.Text;

namespace SimpleWeb.TelegramBot;

public class Md5SecurityHelper
{
    /// <summary>
    /// 进行MD5加密
    /// </summary>
    /// <param name="inputString">需要加密的字符串</param>
    /// <param name="encoding">字符集</param>
    /// <returns>加密后的字符串</returns>
    public static String Encrypt(String inputString, Encoding encoding, bool isToUpper = false)
    {
        if (String.IsNullOrWhiteSpace(inputString))
        {
            return String.Empty;
        }
        using (MD5 md5 = MD5.Create())
        {
            Byte[] encryptedBytes = md5.ComputeHash(encoding.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            foreach (Byte t in encryptedBytes)
            {
                sb.AppendFormat("{0:x2}", t);
            }
            return isToUpper ? sb.ToString().ToUpper() : sb.ToString();
        }
    }

    public static String Utf8Encrypt(String inputString, bool isToUpper = false)
    {
        return Encrypt(inputString, Encoding.UTF8, isToUpper);
    }

    public static String AsciiEncrypt(String inputString, bool isToUpper = false)
    {
        return Encrypt(inputString, Encoding.ASCII, isToUpper);
    }

    public static String UnicodeEncrypt(String inputString)
    {
        return Encrypt(inputString, Encoding.Unicode, true);
    }
}