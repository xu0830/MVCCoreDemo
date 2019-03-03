using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CJ.Infrastructure.Json
{
    public class JsonHelper
    {
        public static void JsonExport(object jsonObj)
        {
            string path = Directory.GetCurrentDirectory();

            string directoryPath = path + "\\json";

            string FilePath = directoryPath + "\\station.json";

            if (!Directory.Exists(directoryPath))
            {
                // Create the directory it does not exist.
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(FilePath))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);

                fs1.Close();
            }

            File.WriteAllText(FilePath, JsonConvert.SerializeObject(jsonObj));

        }
    }
}
