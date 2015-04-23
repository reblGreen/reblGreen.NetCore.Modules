using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modular
{
    public class ModuleMethod
    {
        /// <summary>
        /// The identifier used when calling the method using the RunMethod function in the module host.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Usage details explaining how to use this method, eg. if the method accepts a Json encoded Dictionary or some other string encoded object, explain this here.
        /// </summary>
        public readonly string Usage;


        public ModuleMethod(string name, string usage)
        {
            this.Name = name;
            this.Usage = usage;
        }

        public override bool Equals(object obj)
        {
            ModuleMethod other = obj as ModuleMethod;

            if (other != null && other.Name.Equals(this.Name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
