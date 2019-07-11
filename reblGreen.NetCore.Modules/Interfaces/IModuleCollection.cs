/*
    The MIT License (MIT)

    Copyright (c) 2019 reblGreen Software Ltd. (https://reblgreen.com/)
    Repository Url: https://bitbucket.org/reblgreen/reblgreen.netcore.modules/

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

namespace reblGreen.NetCore.Modules.Interfaces
{
    public interface IModuleCollection
    {
        IModuleHost Host { get; }

        /// <summary>
        /// This should return a list of loaded modules. Pipeline can check this list for other modules that they may depend on. If an IEvent is passed
        /// to this method, a list containing only loaded modules which are able to process the event should be returned.
        /// </summary>
        IList<IModule> GetLoadedModules(IEvent @event = null);


        ///// <summary>
        ///// This method should tell the IModuleHost to import modules. Modules are imported from the ModuleHost working directory.
        ///// </summary>
        //void ImportModules();


        /// <summary>
        /// This should invoke the OnLoad() method on each imported module followed the OnLoaded() method on each module. This allows for things like module
        /// dependency checking, initialization, threaded loops etc... This should enable the Handle() event for each loaded module. If modules is null, all
        /// imported modules should be loaded.
        /// </summary>
        /// <param name="modules"></param>
        void LoadModules(IList<ModuleName> modules = null);


        /// <summary>
        /// Disable the modules
        /// </summary>
        /// <param name="modules"></param>
        void UnloadModules(IList<ModuleName> modules = null);


        /// <summary>
        /// Disable the modules
        /// </summary>
        /// <param name="modules"></param>
        void ReloadModules(IList<ModuleName> modules = null);


        /// <summary>
        /// This should check to see if a module is available (imported) and can be used by modules to check for other module dependancy etc...
        /// </summary>
        /// <param name="name">Name of the module to look for.</param>
        /// <param name="min">(Optional) Specify a minimum module version.</param>
        /// <param name="max">(Optional) Specify a maximum module version.</param>
        bool HasModule(ModuleName name, Version min = null, Version max = null);


        /// <summary>
        /// This should return a list of modules which implement a specific type. Pipeline can request a list of modules by inheritance and use
        /// casting to call methods which would be unavailable through the standard handle stream.
        /// </summary>
        IList<IModule> GetModulesByType<T>() where T : IModule;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        IList<IEventPreHandler> GetPreHandlers(IEvent e);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        IList<IEventPostHandler> GetPostHandlers(IEvent e);
    }
}
