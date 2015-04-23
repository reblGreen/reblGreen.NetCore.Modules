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
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Modular
{
    /// <summary>
    /// A ready to use ModuleHost that you can use as is or derive from and override :) Just instantiate it in your code and call it's methods. 
    /// </summary>
    public class ModuleHost : IModuleHost
    {
        private AggregateCatalog _catalog;

        [ImportMany(typeof(IModule))]
        private IEnumerable<IModule> _modules = null;

        private string[] _args;
 
        /// <summary>
        /// Create a new ModuleHost. You should pass in the arguments that passed in to Program.Main when your application is launched. This
        /// allows any modules to access these arguments if neccessary.
        /// </summary>
        /// <param name="args"></param>
        public ModuleHost(string[] args)
        {
            this._args = args;
        }


        /// <summary>
        /// Tell this ModuleHost to get modules from a specific location. This should be an absolute path to your modules/plugins directory.
        /// </summary>
        /// <param name="path">Absolute module directory path.</param>
        public virtual void ImportModules(string path, bool includeSubDirs = false)
        {
            if (_catalog == null)
            {
                _catalog = new AggregateCatalog();
                _catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            }

            _catalog.Catalogs.Add(new DirectoryCatalog(path));

            if (includeSubDirs)
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    _catalog.Catalogs.Add(new DirectoryCatalog(dir));
                }
            }

            CompositionContainer container = new CompositionContainer(_catalog);
            container.ComposeParts(this);
        }

        /// <summary>
        /// Run this method after you have imported your modules, it will run the OnLoad method on each loaded module then run OnLoaded. Each module will run its cunstructor
        /// as it is imported, when writing modules you should not do any cross-module communication in a module's constructor or the OnLoad method as the requested module may
        /// not be loaded. Instead you should wait for the module's OnLoaded() method before doing any cross-module communication.
        /// </summary>
        public virtual void LoadModules()
        {
            foreach (IModule m in _modules)
                m.OnLoad();
            foreach (IModule m in _modules)
                m.OnLoaded();
        }

        /// <summary>
        /// You should run this method when the application is going to be closed. This invokes OnBeforeUnload() on each imported module, shortly followed by OnUnload() on each
        /// imported module to allow cross-module finalization any then any cleanup of resources.
        /// </summary>
        public virtual void UnloadModules(UnloadDetails unloadDetails)
        {
            foreach (IModule m in _modules)
                m.OnBeforeUnload(unloadDetails);

            Thread.Sleep(1000);

            foreach (IModule m in _modules)
                m.OnUnload();
        }

        /// <summary>
        /// Returns the current version number of this IModuleHost. This can be used by modules to check for any dependancy requirements.
        /// </summary>
        public virtual Version Version
        {
            get { return AssemblyName.GetAssemblyName(this.GetType().Assembly.Location).Version; }
        }

        /// <summary>
        /// Exposes the main application's name and make it available to IModule.
        /// </summary>
        public virtual string ApplicationName
        {
            get { return Process.GetCurrentProcess().ProcessName; }
        }

        /// <summary>
        /// Returns the arguments that were used when launching the application. Access to these arguments can be used by modules when processing data.
        /// </summary>
        public virtual string[] ApplicationStartArguments { get { return _args; } }

        /// <summary>
        /// Returns an absolute path to the working of the executing application.
        /// </summary>
        public virtual string WorkingDirectory
        {
            get
            {
                //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        private SystemDetails _machineDetails;
        public SystemDetails MachineDetails
        {
            get
            {
                if (_machineDetails == null)
                    _machineDetails = new SystemDetails();

                return _machineDetails;
            }
        }

        /// <summary>
        /// Returns a List of all modules which have been imported. Modules can check this list for other modules and requirements that they may depend on.
        /// </summary>
        public virtual List<ModuleDetails> LoadedModules
        {
            get
            {
                List<ModuleDetails> l = new List<ModuleDetails>();

                foreach (IModule m in _modules)
                {
                    ModuleDetails d = new ModuleDetails(m.Name, m.Description, m.Version, m.UsageInstructions, m.Methods.ToList());
                    l.Add(d);
                }
                return l;
            }
        }

        /// <summary>
        /// This method searches imported modules for the module name that is being requested for invoke, then invoke the RunMethod() on the
        /// found module passing in the other parameters into the method call.
        /// </summary>
        /// <param name="module">The name used to identify the module to invoke.</param>
        /// <param name="method">The method identifier to be invoked on the module</param>
        /// <param name="data">Any in data you wish to pass to the module.</param>
        /// <returns>A response indicating whether the invoke was successful or not.</returns>
        public virtual ModuleResponse RunModuleMethod(string module, string method, object inData)
        {
            return RunModuleMethod(module, method, inData, new ModuleRunMethodCallback((r,o,m)=>{}));
        }

        /// <summary>
        /// This method searches imported modules for the module name that is being requested for invoke, then invoke the RunMethod() on the
        /// found module passing in the other parameters into the method call.
        /// </summary>
        /// <param name="module">The name used to identify the module to invoke.</param>
        /// <param name="method">The method identifier to be invoked on the module</param>
        /// <param name="data">Any in data you wish to pass to the module.</param>
        /// <param name="callback">A delegate callback to be executed when the processing has completed. This allows for the use of Threading.</param>
        /// <returns>A response indicating whether the invoke was successful or not.</returns>
        public virtual ModuleResponse RunModuleMethod(string module, string method, object inData, ModuleRunMethodCallback callback)
        {
            foreach (IModule m in _modules)
            {
                if (m.Name.Equals(module, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (m.Methods.Contains(new ModuleMethod(method, "")))
                    {
                        return m.RunMethod(method.ToLowerInvariant(), inData, callback);
                    }
                }
            }

            return ModuleResponse.ModuleNotFound;
        }

        /// <summary>
        /// This checks to see if a module is available (imported) and can be used by modules to check for other module dependancy etc...
        /// </summary>
        /// <param name="moduleName">Name of the module to look for.</param>
        /// <param name="minVersion">(Optional) Specify a minimum module version.</param>
        /// <param name="maxVersion">(Optional) Specify a maximum module version.</param>
        /// <param name="methodName">(Optional) Check for a specific method name.</param>
        public virtual bool HasModule(string pluginName, Version minVersion = null, Version maxVersion = null, string methodName = null)
        {
            foreach (IModule p in _modules)
            {
                if (p.Name.Equals(pluginName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (minVersion != null && p.Version < minVersion)
                        continue;

                    if (maxVersion != null && p.Version > maxVersion)
                        continue;

                    if (methodName != null && !p.Methods.Contains(new ModuleMethod(methodName, "")))
                        continue;

                    return true;
                }
            }

            return false;
        }

        public int AvailableThreads
        {
            get
            {
                int avT = 0, avIO;
                System.Threading.ThreadPool.GetAvailableThreads(out avT, out avIO);
                return avT;
            }
        }

        public Task QueueAction(Action method)
        {
            return Task.Factory.StartNew(method);
        }

        public Task QueueAction(Func<object> method)
        {
            return Task.Factory.StartNew(method);
        }
       
    }
}