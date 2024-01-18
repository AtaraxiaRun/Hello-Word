using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ2
{
    public static class ConvertHelper
    {


        public static int ToInt32(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return 0;
            }

            return Convert.ToInt32(obj);
        }
        public static bool IsNullOrEmpty(this object data)
        {
            if (data == null)
            {
                return true;
            }

            if (data.GetType() == typeof(string) && string.IsNullOrEmpty(data.ToString().Trim()))
            {
                return true;
            }

            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            return false;
        }

        public static bool IsNullOrEmpty<T>(this T data)
        {
            if (data == null)
            {
                return true;
            }

            if (data.GetType() == typeof(string) && string.IsNullOrEmpty(data.ToString().Trim()))
            {
                return true;
            }

            if (data is DBNull)
            {
                return true;
            }

            return false;
        }
    }
}
