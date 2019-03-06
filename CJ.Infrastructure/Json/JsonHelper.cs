using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CJ.Infrastructure.Json
{
    /// <summary>
    /// Json文件操作
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 写Json文件
        /// </summary>
        /// <param name="JsonObj">json对象</param>
        /// <param name="Path">输出路径</param>
        /// <param name="FileName">文件名</param>
        public static void Export(object JsonObj, string Path, string FileName)
        {
            string path = Directory.GetCurrentDirectory();

            string directoryPath = path + "\\" + Path;

            string FilePath = directoryPath + "\\" + FileName;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(FilePath))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
            }

            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;

                //构建Json.net的写入流
                JsonWriter writer = new JsonTextWriter(sw)
                {
                    Formatting = Formatting.Indented,//格式化缩进
                    Indentation = 4,  //缩进四个字符
                    IndentChar = ' '  //缩进的字符是空格
                };

                //把模型数据序列化并写入Json.net的JsonWriter流中
                serializer.Serialize(writer, JsonObj);
                //ser.Serialize(writer, ht);
                writer.Close();
                sw.Close();
            }
        }

        public static JObject Import(string Path)
        {
            StreamReader file = File.OpenText(Path);
            JsonTextReader reader = new JsonTextReader(file);
            JObject jsonObject = (JObject)JToken.ReadFrom(reader);
            //CAN_Communication = (bool)jsonObject["CAN"];
            //AccCode = (uint)jsonObject["AccCode"];
            //Id = (uint)jsonObject["Id"];

            //// Configure Json
            //BPointMove = (bool)jsonObject["BPointMove"];
            //_classLeft.DelayBPointMove = (int)jsonObject["L_BPointMoveDelay"];
            //_classRight.DelayBPointMove = (int)jsonObject["R_BPointMoveDelay"];
            file.Close();
            return jsonObject;
        }
    }
}
