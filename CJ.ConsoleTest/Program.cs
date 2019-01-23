using CJ.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GEEKiDoS.MusicPlayer.NeteaseCloudMusicApi
{
    public class Program
    {
          private static readonly string _privateKey = @"MIICWwIBAAKBgQCKdCeiDDVVRZNMOk0zZzK4v4CjoxhLmoWXuVhhKoRUUoQ/bRRA
UdT+PKOMOFzPyALg6OM9dRo7e6VIWjJlgzJ3dy5ASdiXSyc1Ptl0Xvy8/BJPDlLp
jLUFvcj2prdAxZtnKbVGNwPCTJ+exULXgw5ZTAucZOY37cDavaL5tiebTQIDAQAB
AoGAfL0bvAatwk6137ajOU2fyA1Y85UMXYkxFTo6owgwQtw5I/+9gBl6AThWzQ02
qUj1Nvb7TLKFWNQUXHRO9WBXhS/V97EnnXv97Un25Kf8a0pqPjIJYDn7Lr5lXmXe
hn0hW0lc5T4mDEspxFh0MUrjGOUDjlMBmnl/tmYTdFjVry0CQQCremTHmm+yjNbR
pHZQrO19PbMFSaTYRD/vOW7wae3jW6pKDOM79s2+SVnJLMv3Wpio05o4ybJqWrxV
BklJmbvnAkEAzrKmXIwOKivqkbH8vQazfxEQQ8//TnR44phn+VPdYB1EcVt8iPKF
6bhDIMokOnaA2t6cFhzNIAznbckAlK0oqwJAK1BNKIX/9M/Saz3pjNNBYcM19v31
H5ONurV9Kkj3h9hdmTrMIxdiPNB2V3RzSNWfffWFHRcFdAvbSna+CFNGvQJAC7A6
jB03Z9cX6qk/+4h3egYC/3Kxo0Qe2eF4b7b4W8kL58Ueo7fjLrZGxYHozo2I99eC
yBVU3C0eoSyupbmtBQJAG8X+x5kMVvqKezyM7ppjpZCfS24jbDvQAEHS1dIirhXa
PepCj2qOhtpQVDMPPzCOQN/qTE9AcYea/NECSh0s5Q==".Replace("\n", "");

        //openssl rsa -pubout -in rsa_1024_priv.pem -out rsa_1024_pub.pem
        private static readonly string _publicKey = @"".Replace("\n", "");

        public static void Main(string[] args)
        {
            var jsonObj = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string publicKey = jsonObj.GetSection("privateKey").Value;
            string privateKey = jsonObj.GetSection("publicKey").Value;
            string plainText = "xucanjie";
            Console.WriteLine(publicKey);
            Console.WriteLine(privateKey);
            var cipher = RSAHelper.Encrypt(publicKey, plainText);
            Console.WriteLine($"密文: {cipher}");
            var plainStr = RSAHelper.Decrypt(privateKey, cipher);
            Console.WriteLine($"解密后的明文: {plainStr}");
        }


        private static RSA CreateRsaFromPrivateKey(string privateKey)
        {
            var privateKeyBits = System.Convert.FromBase64String(privateKey);
            var rsa = RSA.Create();
            var RSAparams = new RSAParameters();

            using (var binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(RSAparams);
            return rsa;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static RSA CreateRsaFromPublicKey(string publicKeyString)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, SeqOID))
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);

                    if (binr.ReadByte() != 0x02)
                        return null;
                    int expbytes = (int)binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);

                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }

            }
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
    }

}
