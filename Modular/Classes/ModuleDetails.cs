using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Modular
{
    public class ModuleDetails
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Version Version;
        public readonly string Usage;
        public ReadOnlyCollection<ModuleMethod> Methods { get; set; }

        public ModuleDetails(string name, string description, Version version, string usage, List<ModuleMethod> methods)
        {
            this.Name = name;
            this.Description = description;
            this.Version = version;
            this.Usage = usage;
            this.Methods = methods.AsReadOnly();
        }
    }
}
