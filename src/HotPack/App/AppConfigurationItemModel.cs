using HotPack.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPack.App
{
    public class AppConfigurationItemModel
    {
        public AppConfigurationType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public Castable? Value { get; set; }

        public AppConfigurationItemModel()
        {

        }

        public AppConfigurationItemModel(AppConfigurationType type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = new Castable(value);
        }
    }
}
