﻿using Sync.Plugins.BuildInPlugin;
using static Sync.Tools.ConfigurationIO;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public class Configuration:IConfigurable
    {
        public const string DEFAULT_LANGUAGE = "LocalSettings";

        public ConfigurationElement Client { get; set; } = "OsuBotTransferClient";

        public ConfigurationElement Source { get; set; } = "Bilibili";

        public ConfigurationElement Language { get; set; } = "zh-CN";

        public ConfigurationElement LoggerFile { get; set; } = "Log.txt";

        public ConfigurationElement EnableViewersChangedNotify { get; set; } = "False";

        public ConfigurationElement EnableGiftChangedNotify { get; set; }= "False";

        public void onConfigurationLoad()
        {

        }

        public void onConfigurationReload()
        {

        }

        public void onConfigurationSave()
        {

        }

        static Configuration instance;
        static PluginConfigurationManager config;

        public static Configuration Instance
        {
            get {
                if (instance==null)
                {
                    config = new PluginConfigurationManager(typeof(InternalPlugin).Name);
                    instance = new Configuration();
                    config.AddItem(instance);
                }
                return instance;
            }
        }
    }
}
