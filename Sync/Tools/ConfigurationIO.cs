﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sync.Tools
{
    /// <summary>
    /// INI File reader
    /// </summary>
    static class ConfigurationIO
    {
        /// <summary>
        /// InI key
        /// </summary>
        public enum DefaultConfig
        {
            EnableViewersChangedNotify,
            EnableGiftChangedNotify,
            Client,
            Source,
            Language,
            LoggerFile,
        }

        private static FileSystemWatcher watcher;

        static ConfigurationIO()
        {
            watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += (s, e) => {
                if (PluginConfigurationManager.InSaving) return;
                if (!e.Name.StartsWith("config.ini")) return;

                watcher.EnableRaisingEvents = false;
                Task.Run(()=> {
                    Thread.Sleep(500);
                    watcher.EnableRaisingEvents = true;
                });

                Thread.Sleep(100);

                foreach (var item in PluginConfigurationManager.ConfigurationSet)
                {
                    item.ReloadAll();
                }
                Plugins.PluginEvents.Instance.RaiseEventAsync(new Plugins.PluginEvents.ConfigurationChange());
            };
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        /// <summary>
        /// Config path
        /// </summary>
        public readonly static string ConfigFile = AppDomain.CurrentDomain.BaseDirectory + "config.ini";

        /// <summary>
        /// The ini read buffer(thread safe)
        /// </summary>
        private readonly static ThreadLocal<StringBuilder> iniReadBuffer = new ThreadLocal<StringBuilder>(()=>new StringBuilder(65535));

        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="column">Section</param>
        /// <returns>Value</returns>
        internal static string IniReadValue(string FilePath, string key, string column = "config")
        {
            StringBuilder temp = iniReadBuffer.Value;
            int length = GetPrivateProfileString(column, key, "", temp, 65535, FilePath);
            return temp.ToString(0,length);
        }

        internal static bool IniWriteValue(string FilePath, string key, string value, string column = "config")
        {
            return WritePrivateProfileString(column, key, value, FilePath);
        }
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="column">Section</param>
        /// <returns>Value</returns>
        public static string Read(string key, string column = "config")
        {
            return IniReadValue(ConfigFile, key, column);
        }
        /// <summary>
        /// Read via enum
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static string ReadConfig(DefaultConfig key)
        {
            return IniReadValue(ConfigFile, Enum.GetName(typeof(DefaultConfig), key));
        }
        /// <summary>
        /// Write <see cref="value"/> to <see cref="key"/>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="column">Section</param>
        /// <returns></returns>
        public static bool Write(string key, string value, string column = "config")
        {
            return IniWriteValue(ConfigFile, key, value, column);
        }
        /// <summary>
        /// Write value via enum
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static bool WriteConfig(DefaultConfig key, string value)
        {
            return Write(Enum.GetName(typeof(DefaultConfig), key), value);
        }
    }
}
