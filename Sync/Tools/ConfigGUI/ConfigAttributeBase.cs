﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools.ConfigGUI
{
    [System.AttributeUsage(System.AttributeTargets.Property,AllowMultiple = false)]
    public abstract class ConfigAttributeBase:Attribute
    {
        public string Description { get; set; } = "No Description";
    }

    public class ConfigBoolAttribute : ConfigAttributeBase
    {
    }

    public class ConfigIntegerAttribute : ConfigAttributeBase
    {
        public int MinValue { get; set; } = int.MinValue;
        public int MaxValue { get; set; } = int.MaxValue;

        public bool Check(int i)
        {
            return (MinValue <= i && i <= MaxValue);
        }
    }

    public class ConfigFloatAttribute : ConfigAttributeBase
    {
        public float MinValue { get; set; } = float.MinValue;
        public float MaxValue { get; set; } = float.MaxValue;

        public bool Check(float i)
        {
            return (MinValue <= i && i <= MaxValue);
        }
    }

    public class ConfigStringAttribute : ConfigAttributeBase { }

    public class ConfigListAttribute : ConfigAttributeBase
    {
        public string[] ValueList { get; set; } = null;
        public bool IgnoreCase { get; set; } = false;

        public bool Check(string val)
        {
            val = IgnoreCase ? val.ToLower() : val;

            if (ValueList?.Length == 0)
                return false;
            return ValueList.Where((str) => (IgnoreCase?str.ToLower():str)==val).Count() != 0;
        }
    }
}
