﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sync.Tools
{
    /// <summary>
    /// 该类提供直接读取配置文件的方法
    /// </summary>
    static class ConfigurationReader
    {
        /// <summary>
        /// 配置文件枚举项
        /// </summary>
        public enum DefaultConfig
        {
            LiveRoomID,
            TargetIRC,
            BotIRC,
            BotIRCPassword,
            Provider
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public readonly static string ConfigFile = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
        /// <summary>
        /// 实现读取配置文件
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="column">索引</param>
        /// <returns>配置信息</returns>
        private static string IniReadValue(string key, string column = "config")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(column, key, "", temp, 255, ConfigFile);
            return temp.ToString();
        }
        /// <summary>
        /// 按需读取配置文件
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="column">索引</param>
        /// <returns>配置信息</returns>
        public static string Read(string key, string column = "config")
        {
            return IniReadValue(key, column);
        }
        /// <summary>
        /// 按枚举读取配置文件
        /// </summary>
        /// <param name="key">指定配置节</param>
        /// <returns>配置信息</returns>
        public static string ReadConfig(DefaultConfig key)
        {
            return IniReadValue(Enum.GetName(typeof(DefaultConfig), key));
        }
    }
}
