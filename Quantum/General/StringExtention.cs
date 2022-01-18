﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extentions
{
    public static class StringExtention
    {
        /// <summary>
        /// Превращает строку в заданный тип T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T To<T>(this string text)
        {
            try
            {
                return (T)Convert.ChangeType(text, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Превращает строку в целое число int
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int ToInt(this string text)
        {
            return To<int>(text);
        }

        /// <summary>
        /// Превращает строку в число с плавающей точной double
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static double ToDouble(this string text)
        {
            return To<double>(text);
        }

        /// <summary>
        /// Превращает строку в тип bool. Принимает "true", "1" и "да" в качестве true;
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ToBool(this string text)
        {
            switch (text.ToLower().Trim())
            {
                case "true":
                case "1":
                case "on":
                case "да": return true;
                default: return false;
            }
        }
    }
}
