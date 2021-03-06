﻿using System.Configuration;
using SharpExtensionsUtil.Converter;

namespace SharpExtensionsUtil.Util
{
    public static class AppConfig
    {
        public static string GetStringParam(string key, string defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }

        public static int GetIntParam(string key, int defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue : 
                int.Parse(str);
        }

        public static ulong GetULongParam(string key, ulong defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue :
                ulong.Parse(str);
        }

        public static bool GetBooleanParam(string key, bool defaultValue)
        {
            var str = ConfigurationManager.AppSettings.Get(key);
            return string.IsNullOrEmpty(str) ? defaultValue :
                str.ToBool();
        }
    }
}
