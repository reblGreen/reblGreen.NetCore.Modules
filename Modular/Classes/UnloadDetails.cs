using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modular
{
    public class UnloadDetails
    {
        public UnloadReason UnloadReason { get; private set; }
        public IModule CallingModule { get; private set; }
        public Exception UnloadException { get; private set; }

        public UnloadDetails(UnloadReason unloadReason, IModule callingModule = null)
        {
            this.UnloadReason = unloadReason;
            this.CallingModule = callingModule;
        }

        public UnloadDetails(UnloadReason unloadReason, IModule callingModule = null, Exception unloadException = null)
        {
            this.UnloadReason = unloadReason;
            this.CallingModule = callingModule;
            this.UnloadException = unloadException;
        }
    }
}
