using CJ.Infrastructure.Cache;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Infrastructure
{
    public class RestHelper
    {
        /// <summary>
        /// 发送HttpPost请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <param name="addCookies"></param>
        /// <returns></returns>
        public static string SendPostRequest(string url, object formData, bool addCookies)
        {
            string secretKey = NeteaseEncrypt.CreateSecretKey(16);

            string aesEncrypt = NeteaseEncrypt.AESEncode(JsonConvert.SerializeObject(formData), NeteaseEncrypt._NONCE);

            string _paramsUnencode = NeteaseEncrypt.AESEncode(aesEncrypt, secretKey);
            string _params = HttpHelper.CharEncode(NeteaseEncrypt.AESEncode(aesEncrypt, secretKey));

            string rsaEncrypt = HttpHelper.CharEncode(NeteaseEncrypt.RSAEncode(secretKey));
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");



            IList<RestResponseCookie> cookies = (IList<RestResponseCookie>)CacheHelper.GetCache("NeteaseCookies");

            if (cookies != null && cookies.Count > 0)
            {
                foreach (var item in cookies)
                {
                    request.AddParameter(item.Name, item.Value, ParameterType.Cookie);
                }
            }
            request.AddParameter("undefined", $"params={_params}&encSecKey={rsaEncrypt}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (addCookies)
            {
                CacheHelper.SetCache("NeteaseCookies", response.Cookies) ;
            }
            return response.Content;

        }

        public static IRestResponse SendGetRequest(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);

            return client.Execute(request);
        }

    }
}
