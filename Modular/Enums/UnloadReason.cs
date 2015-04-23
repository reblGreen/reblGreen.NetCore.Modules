using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modular
{
    public enum UnloadReason
    {
        Closing,
        UnexpectedTermination,
        ModuleException,
    }
}
