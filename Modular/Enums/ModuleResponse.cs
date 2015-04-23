using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modular
{
    /// <summary>
    /// ModuleResponse allows a module to identify whether a call to another module has been successful or not.
    /// </summary>
    public enum ModuleResponse
    {
        /// <summary>
        /// Should be returned from IModuleHost's RunModuleMethod method if a module is not found, otherwise the IModuleHost should return the response from the requested module.
        /// </summary>
        ModuleNotFound,

        /// <summary>
        /// Should be returned from a module's RunMethod method if the module is accepting the method call.
        /// </summary>
        MethodAccepted,

        /// <summary>
        /// Should be returned from a module's RunMethod method if the module is rejecting the method call.
        /// </summary>
        MethodNotFound,

        /// <summary>
        /// Should be returned in the module's RunMethod ModuleRunMethodCallback response field if the method call was successful.
        /// </summary>
        MethodSuccess,

        /// <summary>
        /// Should be returned in the module's RunMethod ModuleRunMethodCallback response field if the method call was unsuccessful.
        /// </summary>
        MethodFail,
    }
}
