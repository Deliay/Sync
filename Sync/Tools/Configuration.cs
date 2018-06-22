﻿using Sync.MessageFilter;
using Sync.Tools.ConfigurationAttribute;

namespace Sync.Tools
{
    /// <summary>
    /// Default plugin confiuration
    /// </summary>
    public class DefaultConfiguration : IConfigurable
    {
        public const string DEFAULT_LANGUAGE = "LocalSettings";

        [ClientList(NoCheck = true)]
        public ConfigurationElement Client { get; set; } = "";

        [SourceList(NoCheck = true)]
        public ConfigurationElement Source { get; set; } = "";

        public ConfigurationElement Language { get; set; } = "zh-CN";

        [Path(IsDirectory = false)]
        public ConfigurationElement LoggerFile { get; set; } = "Log.txt";

        [Bool]
        public ConfigurationElement EnableViewersChangedNotify { get; set; } = "False";

        [Bool]
        public ConfigurationElement EnableGiftChangedNotify { get; set; } = "False";

        [List(ValueList = new[] { "Auto", "ForceAll", "OnlySendCommand", "DisableAll" }, IgnoreCase = true)]
        public ConfigurationElement MessageManagerDefaultOption { get; set; } = "Auto";

        public void onConfigurationLoad()
        {
            MessageManager.SetOption(MessageManagerDefaultOption);
        }

        public void onConfigurationReload()
        {
            MessageManager.SetOption(MessageManagerDefaultOption);
        }

        public void onConfigurationSave()
        {
            MessageManagerDefaultOption = MessageManager.Option.ToString();
        }

        public static readonly DefaultConfiguration Instance = new DefaultConfiguration();
        private static readonly PluginConfigurationManager config = new PluginConfigurationManager("Sync");
        static DefaultConfiguration()
        {
            config.AddItem(Instance);
        }

    }
}