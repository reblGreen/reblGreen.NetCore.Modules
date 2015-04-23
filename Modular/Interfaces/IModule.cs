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
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace Modular
{
    [InheritedExport]
    public interface IModule
    {
        /// <summary>
        /// Allows communication and calling methods in the module which hosts modules. In your class that implements IModule, you must decorate this property
        /// with the [System.ComponentModel.Composition.Import(typeof(IModuleHost))] flag to access the host.
        /// </summary>
        IModuleHost Host { get; }

        /// <summary>
        /// Briefly explain what this module is used for. This property is included in the "IModuleHost.GetLoadedModuleDetails()" return object.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Give any instructions for usage here. This allows people creating new plugins to more easily interact with your plugin if neccessary.
        /// </summary>
        string UsageInstructions { get; }

        /// <summary>
        /// The name of the module is used to identify it when invoking a method on the module using the RunMethod function in the module host.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// This should be set to an absolute path to the directory that the module resides in. This can be used by a module to access external files which it may
        /// depend on. Eg. a config file or an external executable.
        /// </summary>
        string Location { get; }
        
        /// <summary>
        /// Return an array of methods that can be invoked on the module. This array will be accessed by the RunMethod function in the IModuleHost to find the
        /// correct method to run.
        /// </summary>
        ModuleMethod[] Methods { get; }

        /// <summary>
        /// This can be used by other modules to check compatibility if there is a dependancy.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Other modules may depend on other modules. A dependant module should check its dependancy's started property to make sure it is fully
        /// initialized before calling its methods.
        /// </summary>
        bool Loaded { get; }

        /// <summary>
        /// This method should be invoked by IModuleHost when all modules have been imported. other modules should only be accessed after this method is invoked
        /// on each module. This can be done from the OnLoaded method. Attempting to access another module in a module's constructor or in this method may fail 
        /// if the requested plugin is not loaded yet. A module should set its started property to true after code is executed.
        /// </summary>
        ModuleResponse OnLoad();

        /// <summary>
        /// This method should be invoked by IModuleHost when all modules are loaded. other modules should only be accessed when this method is invoked or from the 
        /// IModuleHost's RunModuleMethod function. Attempting to access another module in a module's before now may fail if the requested plugin is not loaded yet.
        /// A module should set its started property to true after code is executed.
        /// </summary>
        ModuleResponse OnLoaded();

        /// <summary>
        /// The IModuleHost should invoke this method when it or another module requires this module to processs some data.
        /// </summary>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="data">The input data to be processed.</param>
        /// <param name="callback">A delegate method to invoked when the in data has been processed.</param>
        /// <returns>A response indicating the status of the request.</returns>
        ModuleResponse RunMethod(string method, object inData, ModuleRunMethodCallback callback);

        /// <summary>
        /// This method should be invoked by the IModuleHost just before all modules are going to be unloaded and the host should attempt to delay termination for a few
        /// seconds to allow any finalization. If you need to do some last minute cross module access, do it now!
        /// </summary>
        ModuleResponse OnBeforeUnload(UnloadDetails unloadDetails);

        /// <summary>
        /// This method should be invoked by the IModuleHost when all modules are being unloaded and you should do any final cleanup of resources here. It is not advised
        /// to try and access other modules inside this method.
        /// </summary>
        ModuleResponse OnUnload();
    }
}
