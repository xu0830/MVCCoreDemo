using CJ.Infrastructure.Cache;
using CJ.Infrastructure.Log;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CJ.Infrastructure
{
    /// <summary>
    /// 网站配置
    /// </summary>
    public class WebConfig
    {
        private static string settingKey = "appsetting";

        private static string jsonFileName = "appsettings.json";

        private static IConfigurationRoot GetConfigurationRoot()
        {
            IConfigurationRoot configurationRoot = (IConfigurationRoot)CacheHelper.GetCache(settingKey);
            if (configurationRoot == null)
            {
                try
                {
                    string path = Directory.GetCurrentDirectory();

                    configurationRoot = new ConfigurationBuilder().SetBasePath(path).AddJsonFile("appsettings.json").Build();

                    CacheHelper.SetCache(settingKey, configurationRoot, path, jsonFileName);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                }
            }
            return configurationRoot;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DBConnectionString
        {
            get
            {
                try
                {
                    return GetConfigurationRoot().GetSection("DBConnectionString").Value;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                    return "";
                }
            }
        }

        /// <summary>
        /// 前端地址
        /// </summary>
        public static string ClientRootAddress
        {
            get
            {
                try
                {
                    return GetConfigurationRoot().GetSection("ClientRootAddress").Value;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                    return "";
                }
            }
        }

        public static string ServerRootAddress
        {
            get
            {
                try
                {
                    return GetConfigurationRoot().GetSection("ServerRootAddress").Value;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                    return "";
                }
            }
        }

        /// <summary>
        /// Rsa公钥
        /// </summary>
        public static string PublicKey
        {
            get
            {
                try
                {
                    return GetConfigurationRoot().GetSection("PublicKey").Value;
                }
                catch(Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                    return "";
                }
            }
        }

        /// <summary>
        /// Rsa私钥
        /// </summary>
        public static string PrivateKey
        {
            get
            {
                try
                {
                    return GetConfigurationRoot().GetSection("PrivateKey").Value;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                    return "";
                }
            }
        }

        public static bool RefererVerify
        {
            get
            {
                try
                {
                    bool result;
                    bool.TryParse(GetConfigurationRoot().GetSection("RefererVerify").Value, out result);
                    return result;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("读取配置文件失败");
                    return false;
                }
            }
        }
    }
}
