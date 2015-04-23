using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modular
{
    public delegate void ModuleStartCallback(ModuleResponse response, string message = null);
    public delegate void ModuleRunMethodCallback(ModuleResponse response, object outData, string message = null);
    public delegate void ModuleStopCallback(ModuleResponse response, string message = null);
}
