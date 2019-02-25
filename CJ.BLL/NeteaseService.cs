using CJ.Infrastructure;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using CJ.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace CJ.Services
{
    /// <summary>
    /// web服务
    /// </summary>
    public class NeteaseService : ITransientDependency
    {
        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void LoginApi(string username, string password)
        {
            var requestdata = new
            {
                phone = username,
                password = MD5Encrypt.Getmd5(password),
                rememberLogin = true
            };

            string data = RestHelper.SendPostRequest("http://music.163.com/weapi/login/cellphone", requestdata, true);

        }

        /// <summary>
        /// 搜索接口
        /// </summary>
        /// <param name="searchWord"></param>
        /// <param name="SearchType"></param>
        /// <returns></returns>
        public dynamic SearchApi(string searchWord, int SearchType)
        {
            var requestdata = new
            {
                s = searchWord,
                type = SearchType,
                limit = 30,
                offset = 0,
            };

            string data = RestHelper.SendPostRequest("http://music.163.com/weapi/search/get", requestdata, false);

            // 1: 单曲, 10: 专辑, 100: 歌手, 1000: 歌单, 1002: 用户, 1004: MV, 1006: 歌词, 1009: 电台, 1014: 视频
            if (SearchType == 1)
            {
                return JsonConvert.DeserializeObject<NeteaseApiModel<NeteaseSongsModel>>(data);
            }else if (SearchType == 10)
            {
                return JsonConvert.DeserializeObject<NeteaseApiModel<NeteaseAlbumsModel>>(data);
            }
            else if (SearchType == 100)
            {
                return JsonConvert.DeserializeObject<NeteaseApiModel<NeteaseArtistModel>>(data);
            }
            return null;
        }
        
        /// <summary>
        /// 每日推荐歌曲
        /// </summary>
        public NeteaseRecommendSong RecommendDaily()
        {
            var requestdata = new
            {
                limit =  20,
                offset = 0,
                total = true
            };

            string data = RestHelper.SendPostRequest("http://music.163.com/weapi/v1/discovery/recommend/songs", requestdata, false);
            NeteaseRecommendSong model = JsonConvert.DeserializeObject<NeteaseRecommendSong>(data);
            return JsonConvert.DeserializeObject<NeteaseRecommendSong>(data);
        }

        public bool CheckUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            throw new NotImplementedException();
        }
    }

   
}
