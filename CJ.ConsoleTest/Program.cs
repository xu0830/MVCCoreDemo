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
        public static void Main(string[] args)
        {
            Console.WriteLine(RandomHelper.RandomNum(6));
        }
    }
}
