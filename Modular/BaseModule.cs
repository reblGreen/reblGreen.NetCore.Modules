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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Modular
{
    /// <summary>
    /// A readymade base module abstract class that you can derive your module from.
    /// </summary>
    public abstract class BaseModule : IModule
    {
        // This flag automatically imports the IModuleHost using Managed Extensibility Framework (MEF).
        [System.ComponentModel.Composition.Import(typeof(IModuleHost))]
        IModuleHost _PluginHost = null;


        /// <summary>
        /// Allows the module to communication with the IModuleHost. This can be used to check for other modules and run their methods.
        /// </summary>
        public IModuleHost Host
        {
            get { return _PluginHost; }
        }


        // IModule's Started property is readonly so we declare a bool which we can modify and return.
        private bool _loaded = false;

        /// <summary>
        /// Other modules may depend on other modules. A dependant module should check its dependancy's started property to make sure it is fully
        /// initialized before calling its methods.
        /// </summary>
        public virtual bool Loaded
        {
            get { return _loaded; }
        }

        /// <summary>
        /// This returns the Assembly Version from "Assembly Information".
        /// </summary>
        public virtual Version Version
        {
            get { return AssemblyName.GetAssemblyName(this.GetType().Assembly.Location).Version; }
        }

        /// <summary>
        /// The BaseModule description should be overridden to provide a brief or detailed description of your module.
        /// </summary>
        public virtual string Description
        {
            get { return "A base module to derive from."; }
        }

        /// <summary>
        /// The BaseModule name should be overridden to provide the name of your module. A module's name is what's used to identify it by IModuleHost when
        /// another module wants to call its method.
        /// </summary>
        public virtual string Name
        {
            get { return "BaseModule"; }
        }

        /// <summary>
        /// The BaseModule usage instructions should be overridden to provide details of how to use your module. Eg. The data object type it accepts and the
        /// data object type it returns etc... This can be used by another module developer to find out how to integrate with your module.
        /// </summary>
        public virtual string UsageInstructions
        {
            get
            {
                return "This module is designed to be derived from and has no usage instructions. Override its properties and methods to create your own module.";
            }
        }

        /// <summary>
        /// This returns the assembly absolute path directory of the module. This can be used by the module to access external files which it may
        /// depend on. Eg. a config file or an external executable.
        /// </summary>
        public virtual string Location
        {
            get
            {
                return Path.GetDirectoryName(this.GetType().Assembly.Location);
            }
        }

        /// <summary>
        /// Return an array of methods that can be invoked on the module. This array will be accessed by the RunMethod function in the IModuleHost to find the
        /// correct method to run.
        /// </summary>
        public virtual ModuleMethod[] Methods
        {
            get 
            {
                return new ModuleMethod[]
                {
                    new ModuleMethod("AllYourMethodsAreBelongToUs", "All methods in the base module are rejected, Override its properties and methods to create your own module."),
                };
            }
        }


        /// <summary>
        /// The module constructor will be invoked on any module that implements it as soon as the module is imported by an IModuleHost. Don't call other modules from inside
        /// the constructor, they may not be loaded yet. Instead use the modules OnLoad() method, this method is invoked when all modules are loaded. Use the OnBeforeUnload()
        /// method to do any cross-module finalization and the OnUnload() cleanup as the module is being unloaded.
        /// </summary>
        public BaseModule() { }


        /// <summary>
        /// The IModuleHost will invoke this method when it or another module requires this module to processs some data.
        /// </summary>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="data">The input data to be processed.</param>
        /// <param name="callback">A delegate method to invoked when the in data has been processed.</param>
        /// <returns>A response indicating the status of the request.</returns>
        public virtual ModuleResponse RunMethod(string method, object inData, ModuleRunMethodCallback callback)
        {
            callback.Invoke(ModuleResponse.MethodNotFound, "", "All methods in the base module are rejected, Override its properties and methods to create your own module.");
            return ModuleResponse.MethodNotFound;
        }


        /// <summary>
        /// This method will be invoked by IModuleHost when all modules are loaded. other modules should only be accessed after this method is invoked. Attempting
        /// to access another module in a module's constructor or in this method may fail if the requested plugin is not loaded yet. A module should set its started
        /// property to true after code is executed.
        /// </summary>
        public virtual ModuleResponse OnLoad()
        {
            return ModuleResponse.MethodSuccess;
        }

        /// <summary>
        /// This method will be invoked by IModuleHost when all modules are loaded. other modules should only be accessed when this method is invoked or from the 
        /// IModuleHost's RunModuleMethod function. Attempting to access another module in a module's before now may fail if the requested plugin is not loaded yet.
        /// A module should set its started property to true after code is executed.
        /// </summary>
        public virtual ModuleResponse OnLoaded()
        {
            _loaded = true;
            return ModuleResponse.MethodSuccess;
        }

        /// <summary>
        /// This method will be invoked by the IModuleHost just before all modules are going to be unloaded and will attempt to delay termination for a few
        /// seconds to allow any finalization. If you need to do some last minute cross module access, do it now!
        /// </summary>
        public virtual ModuleResponse OnBeforeUnload(UnloadDetails unloadInformation)
        {
            return ModuleResponse.MethodSuccess;
        }

        /// <summary>
        /// This method will be invoked by the IModuleHost when all modules are being unloaded. You should do any final cleanup of resources here. It is not advised
        /// to try and access other modules inside this method.
        /// </summary>
        public virtual ModuleResponse OnUnload()
        {
            return ModuleResponse.MethodSuccess;
        }
    }
}
