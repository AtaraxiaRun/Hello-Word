using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace RabbitMQ1
{
    public static class ConvertHelper
    {
        public static bool FloatEqualTo(this decimal numberA, decimal numberB)
        {
            if (Math.Abs(numberA - numberB).CompareTo(0.0001.ToDecimal()) > 0)
            {
                return true;
            }

            return false;
        }

        public static bool FloatEqualTo(this float numberA, float numberB)
        {
            if (Math.Abs(numberA - numberB).CompareTo(0.0001.ToDecimal()) > 0)
            {
                return true;
            }

            return false;
        }

        public static bool FloatEqualTo(this double numberA, double numberB)
        {
            if (Math.Abs(numberA - numberB).CompareTo(0.0001.ToDecimal()) > 0)
            {
                return true;
            }

            return false;
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

        public static int ToInt32(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return 0;
            }

            return Convert.ToInt32(obj);
        }

        public static DateTime ToDateTime(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return DateTime.MinValue;
            }

            return Convert.ToDateTime(obj);
        }

        public static float ToFloat(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return 0f;
            }

            return Convert.ToSingle(obj);
        }

        public static double ToDouble(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return 0.0;
            }

            return Convert.ToDouble(obj);
        }

        public static bool ToBoolean(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return false;
            }

            return Convert.ToBoolean(obj);
        }

        public static string ToStr(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return string.Empty;
            }

            return Convert.ToString(obj);
        }

        public static decimal ToDecimal(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return 0m;
            }

            return Convert.ToDecimal(obj);
        }

        public static Guid ToGuid(this string obj)
        {
            return obj.IsNullOrEmpty() ? default(Guid) : new Guid(obj);
        }

        public static byte[] StringToBytes(string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }

        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        public static string CmycurD(decimal num)
        {
            string text = "零壹贰叁肆伍陆柒捌玖";
            string text2 = "万仟佰拾亿仟佰拾万仟佰拾元角分";
            string text3 = "";
            string text4 = "";
            string text5 = "";
            string text6 = "";
            string text7 = "";
            int num2 = 0;
            num = Math.Round(Math.Abs(num), 2);
            text4 = ((long)(num * 100m)).ToString();
            int length = text4.Length;
            if (length > 15)
            {
                return "溢出";
            }

            text2 = text2.Substring(15 - length);
            for (int i = 0; i < length; i++)
            {
                text3 = text4.Substring(i, 1);
                int startIndex = Convert.ToInt32(text3);
                if (i != length - 3 && i != length - 7 && i != length - 11 && i != length - 15)
                {
                    if (text3 == "0")
                    {
                        text6 = "";
                        text7 = "";
                        num2++;
                    }
                    else if (text3 != "0" && num2 != 0)
                    {
                        text6 = "零" + text.Substring(startIndex, 1);
                        text7 = text2.Substring(i, 1);
                        num2 = 0;
                    }
                    else
                    {
                        text6 = text.Substring(startIndex, 1);
                        text7 = text2.Substring(i, 1);
                        num2 = 0;
                    }
                }
                else if (text3 != "0" && num2 != 0)
                {
                    text6 = "零" + text.Substring(startIndex, 1);
                    text7 = text2.Substring(i, 1);
                    num2 = 0;
                }
                else if (text3 != "0" && num2 == 0)
                {
                    text6 = text.Substring(startIndex, 1);
                    text7 = text2.Substring(i, 1);
                    num2 = 0;
                }
                else if (text3 == "0" && num2 >= 3)
                {
                    text6 = "";
                    text7 = "";
                    num2++;
                }
                else if (length >= 11)
                {
                    text6 = "";
                    num2++;
                }
                else
                {
                    text6 = "";
                    text7 = text2.Substring(i, 1);
                    num2++;
                }

                if (i == length - 11 || i == length - 3)
                {
                    text7 = text2.Substring(i, 1);
                }

                text5 = text5 + text6 + text7;
                if (i == length - 1 && text3 == "0")
                {
                    text5 += "整";
                }
            }

            if (num == 0m)
            {
                text5 = "零元整";
            }

            return text5;
        }

        public static long ToInt64(this object obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return 0L;
            }

            return Convert.ToInt64(obj);
        }

        public static T ConvertTo<T>(object obj)
        {
            Type typeFromHandle = typeof(T);
            return (T)ConvertToObject(obj, typeFromHandle);
        }

        public static object ConvertToObject(object obj, Type type)
        {
            if (type == null)
            {
                return obj;
            }

            if (obj == null)
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType()))
            {
                return obj;
            }

            if ((underlyingType ?? type).IsEnum)
            {
                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString()))
                {
                    return null;
                }

                return Enum.Parse(underlyingType ?? type, obj.ToString());
            }

            if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return (underlyingType == null) ? Activator.CreateInstance(type) : null;
                }
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(obj.GetType()))
            {
                return converter.ConvertFrom(obj);
            }

            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor != null)
            {
                object obj3 = constructor.Invoke(null);
                PropertyInfo[] properties = type.GetProperties();
                Type type2 = obj.GetType();
                PropertyInfo[] array = properties;
                foreach (PropertyInfo propertyInfo in array)
                {
                    PropertyInfo property = type2.GetProperty(propertyInfo.Name);
                    if (propertyInfo.CanWrite && property != null && property.CanRead)
                    {
                        propertyInfo.SetValue(obj3, ConvertToObject(property.GetValue(obj, null), propertyInfo.PropertyType), null);
                    }
                }

                return obj3;
            }

            return obj;
        }
    }
}
