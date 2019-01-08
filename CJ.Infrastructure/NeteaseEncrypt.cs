using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace CJ.Infrastructure
{
    public class NeteaseEncrypt
    {
        private static string _MODULUS = "00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7";
        public static string _NONCE = "0CoJUm6Qyw8W8jud";
        private static string _PUBKEY = "010001";
        private static string _VI = "0102030405060708";

        private static BigInteger BCHexDec(string hex)
        {
            BigInteger dec = new BigInteger(0);
            int len = hex.Length;
            for (int i = 0; i < len; i++)
            {
                dec += BigInteger.Multiply(new BigInteger(Convert.ToInt32(hex[i].ToString(), 16)), BigInteger.Pow(new BigInteger(16), len - i - 1));
            }
            return dec;
        }

        public static string CreateSecretKey(int length)
        {
            var str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var r = "";
            var rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                r += str[rnd.Next(0, str.Length)];
            }
            return r;
        }

        public static string RSAEncode(string text)
        {
            string srtext = new string(text.Reverse().ToArray()); ;
            var a = BCHexDec(BitConverter.ToString(Encoding.Default.GetBytes(srtext)).Replace("-", ""));
            var b = BCHexDec(_PUBKEY);
            var c = BCHexDec(_MODULUS);
            var key = BigInteger.ModPow(a, b, c).ToString("x");
            key = key.PadLeft(256, '0');
            if (key.Length > 256)
                return key.Substring(key.Length - 256, 256);
            else
                return key;
        }

        public static string AESEncode(string secretData, string secret = "TA3YiYCfY2dDJQgg")
        {
            byte[] encrypted;
            byte[] IV = Encoding.UTF8.GetBytes(_VI);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(secret);
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                using (var encryptor = aes.CreateEncryptor())
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var cstream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cstream))
                            {
                                sw.Write(secretData);
                            }
                            encrypted = stream.ToArray();
                        }
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }
    }
}
