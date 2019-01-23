using System;
using System.Collections.Generic;
using System.Text;

namespace CJ.Infrastructure
{
    public class RandomHelper
    {
        public static string RandomNum(int length)
        {
            StringBuilder randomStr = new StringBuilder();
            Random random = new Random();
            for (int i=0; i<length; i++)
            {
                randomStr.Append(random.Next(0, 10));
            }
            return randomStr.ToString();
        }
    }
}
