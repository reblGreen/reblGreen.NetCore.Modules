/*
    The MIT License (MIT)

    Copyright (c) 2015  The Modular Project (https://bitbucket.org/juanshaf/modular)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Modular
{
    [InheritedExport]
    public interface IModuleHost
    {
        /// <summary>
        /// This should return the IModuleHost's version number and can be used by modules to check for any compatibility issues.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// This should return a working directory which may be accessed by a requesting module.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Expose the main application's name and make it available to IModule.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// This should be set to the arguments that are used when launching the application.
        /// </summary>
        string[] ApplicationStartArguments { get; }

        /// <summary>
        /// This exposes ways to identify the computer system running the application. This can be used by modules which may require access to this information.
        /// </summary>
        SystemDetails MachineDetails { get; }

        /// <summary>
        /// This should return a list of imported modules. Modules can check this list for other modules that they may depend on.
        /// </summary>
        List<ModuleDetails> LoadedModules { get; }

        /// <summary>
        /// This method should tell the IModuleHost to search for and import modules from a specific directory. The directory path should be absolute.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="includeSubDirs"></param>
        void ImportModules(string directory, bool includeSubDirs = false);

        /// <summary>
        /// This should invoke the OnLoad() method on each imported module and then the OnLoaded() method on each module. This allows for things like module
        /// dependancy checking, initialization, threaded loops etc...
        /// </summary>
        void LoadModules();

        /// <summary>
        /// This should invoke the OnBeforeUnload() on all imported methods to allow any cross module finalization and then invoke OnUnload() on all
        /// imported modules shortly after.
        /// </summary>
        void UnloadModules(UnloadDetails unloadDetails);

        /// <summary>
        /// This should check to see if a module is available (imported) and can be used by modules to check for other module dependancy etc...
        /// </summary>
        /// <param name="moduleName">Name of the module to look for.</param>
        /// <param name="minVersion">(Optional) Specify a minimum module version.</param>
        /// <param name="maxVersion">(Optional) Specify a maximum module version.</param>
        /// <param name="methodName">(Optional) Check for a specific method name.</param>
        bool HasModule(string moduleName, Version minVersion = null, Version maxVersion = null, string methodName = null);
        
        /// <summary>
        /// This method should search all imported modules for the module name that is being requested for invoke, then invoke the RunMethod() on the
        /// found module passing in the other parameters in the method call.
        /// </summary>
        /// <param name="module">The name used to identify the module to invoke.</param>
        /// <param name="method">The method identifier to be invoked on the module</param>
        /// <param name="data">Any in data you wish to pass to the module.</param>
        /// <param name="callback">A delegate callback to be executed when the processing has completed. This allows for the use of Threading.</param>
        /// <returns>A response indicating whether the invoke was successful or not.</returns>
        ModuleResponse RunModuleMethod(string module, string method, object inData, ModuleRunMethodCallback callback);
        ModuleResponse RunModuleMethod(string module, string method, object inData);

        int AvailableThreads { get; }
        System.Threading.Tasks.Task QueueAction(Action method);
        System.Threading.Tasks.Task QueueAction(Func<object> method);
    }
}
