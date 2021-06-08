﻿/*
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
using System.Linq;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Classes
{
    [Serializable]
    public class ModuleCollection : List<IModule>, IModuleCollection,
        IEventHandler,
        IEventPreHandler<IEvent>,
        IEventPostHandler<IEvent>
    {
        /// <summary>
        /// The list of imported modules ordered by <see cref="ModuleAttribute.LoadPriority"/>. When <see cref="LoadModules(IList{ModuleName})"/>
        /// is invoked, the modules are loaded in the order they appear in this list.
        /// </summary>
        private List<IModuleContainer> Containers;


        /// <summary>
        /// Internal accessor to the IModuleHost instasnce.
        /// </summary>
        private IModuleHost Host;


        /// <summary>
        /// Creates a new instance of ModuleCollection.
        /// </summary>
        public ModuleCollection(IModuleHost host)
        {
            Containers = new List<IModuleContainer>();
            Host = host;
        }


        /// <summary>
        /// Returns a list containing the names of all modules which have been discovered through <see cref="ImportModules"/>.
        /// Modules are returned in the order in which they would be loaded by invoking <see cref="LoadModules(IList{ModuleName})"/>.
        /// </summary>
        public virtual IList<ModuleName> GetModuleNames()
        {
            return Containers.Select(c => c.ModuleAttributes.Name).ToList();
        }


        /// <summary>
        /// Returns a list of <see cref="IModule"/> containing all modules where <see cref="Module.Loaded"/> is true. This list is
        /// ordered by <see cref="ModuleAttribute.HandlePriority"/> and is returned in the order in which the modules will handle
        /// events. If an <see cref="IEvent"/> is passed into this method, only a list of modules which can handle the specified
        /// event object are returned.
        /// </summary>
        public virtual IList<IModule> GetLoadedModules(IEvent e = null)
        {
            if (e == null)
            {
                return this.Where(module => module.Loaded).ToList();
            }

            return this.Where(module => module.Loaded && module.CanHandle(e)).ToList();
        }


        /// <summary>
        /// Returns a descovered instance of <see cref="IModule"/> from a specified type. The module type must have been previously
        /// discovered by invoking <see cref="ImportModules"/>.
        /// </summary>
        public virtual IList<IModule> GetModulesByType<T>() where T : IModule
        {
            return Containers.Where(c => c.ModuleType.IsAssignableFrom(typeof(T))).Select(c => c.Module as IModule).ToList();
        }


        /// <summary>
        /// Allows to check if a module has been discovered using <see cref="ImportModules"/>. This can be used to check for module
        /// dependencies.
        /// </summary>
        public virtual bool HasModule(ModuleName name, Version min = null, Version max = null)
        {
            if (min == null && max == null)
            {
                return Containers.Any(c => c.ModuleAttributes.Name == name);
            }
            else if (min == null)
            {
                return Containers.Any(c => c.ModuleAttributes.Name == name && c.ModuleAttributes.Version <= max);
            }
            else if (max == null)
            {
                return Containers.Any(c => c.ModuleAttributes.Name == name && c.ModuleAttributes.Version >= min);
            }

            return Containers.Any(c => c.ModuleAttributes.Name == name && (c.ModuleAttributes.Version >= min && c.ModuleAttributes.Version <= max));
        }


        /// <summary>
        /// This method is used to discover and import modules using the <see cref="IModuleHost.WorkingDirectory"/> property.
        /// The default <see cref="ModuleCollection"/> implementation will search for modules in this directory and in all
        /// directories 1 level deep from this directory. This allows you to have nested modules which also may implement
        /// <see cref="IModuleHost"/> with their own nested child modules. The default <see cref="ModuleHost"/> attempts to
        /// import modules in its own constructor so directly invoking this method may not be required. You can periodically
        /// check for and import newly added modules at any time.
        /// </summary>
        public virtual void ImportModules()
        {
            // We add the containers and sort them by the order in which they should be loaded, which is specified using the
            // ModuleAttributes.ModuleLoadPriority property.

            var containers = TypeManager.FindModules<Module>(Host, Host.WorkingDirectory, 1);
            
            // Seems long but allows importing newly installed modules provided they are unique and an instance is not already loaded.
            // Tried to use Linq Exclude method here but didn't seem to be working even though GetHashCode and Equals are correctly
            // overidden???
            Containers.AddRange(
                containers.Where(
                    @new => !Containers.Any(
                        current => @new.ModuleAttributes.Name.Equals(current.ModuleAttributes.Name)
                    )
                ).OrderByDescending(module => module.ModuleAttributes.LoadPriority));

            // The modules are initially added to ModuleCollection in the order that they should handle modules. This is specified
            // using the ModuleAttributes.EventHandlePriority property. Since ModuleCollection inherits from List<Type> it means that
            // the ModuleCollection List<Type> can simply be reordered to change the event handle order. A useage example where this
            // may be required could be if reblGreen.NetCore.Modules is being used to build a plugin based effects mixer for an audio
            // stream event. You may wish to allow the user to change the order in which the effects modules process the audio stream
            // event via an GUI with drag/drop. The modules in ModuleCollection would then be programmatically reordered to suit the
            // user selection.
            Clear();
            AddRange(Containers.OrderByDescending(container => container.ModuleAttributes.HandlePriority).Select(container => container as IModule));
        }


        /// <summary>
        /// This method must be invoked to load modules ready for handling instances of <see cref="IEvent"/>. This allows the module or
        /// modules being loaded to initialize internally. Passing a list of <see cref="ModuleName"/> as an argument to this method will
        /// only attempt to load the listed modules. This may be useful in an environment where you may wish to load modules individually
        /// or a subset of modules only when they are required.
        /// </summary>
        public virtual void LoadModules(IList<ModuleName> modules = null)
        {
            // We first need to initialize each module inside its container. The loading process of an array of modules is as follows:
            // 1) Ensure all module dependencies exist within the module ModuleAttributes.Dependencies array
            // 2) Initialize each module so we have a useable module type and can invoke its methods.
            // 3) Invoke OnLoading on each module based on ModuleAttributes.ModuleLoadPriority property. This allows the module to initialize
            //    and configure any requirements within itself. which may require communication with other modules.
            // 4) Invoke OnLoaded on each module based on ModuleAttributes.ModuleLoadPriority property.

            // When we imported modules we ordered the containers by ModuleAttributes.LoadPriority which makes it nice and easy to just
            // itterate each container and invoke the requirements in order.

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                // Check dependencies...
                var missing = c.ModuleAttributes.Dependencies.Where(d => !HasModule(d));
                if (missing.Count() > 0)
                {
                    throw new Exception(string.Format("{0} module has missing module dependencies: {1}", c.ModuleAttributes.Name, string.Join(' ', missing)));
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                // Initialize the container. This creates an instance of the module using Activator.CreatInstance.
                if (!c.Initialized)
                {
                    c.InitializeModule();
                }
            }


            // Look for IModules with the IModuleAttribute.LoadFirst property set to true and trigger these to fully load before other modules.
            var loadFirst = Containers.Where(c => c.ModuleAttributes.LoadFirst == true);

            if (loadFirst != null && loadFirst.Count() > 0)
            {
                foreach (var c in loadFirst)
                {
                    if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                    {
                        continue;
                    }

                    // Invoke the OnLoading method on each module in the order in which they should be loaded.
                    // We set the loaded property to true here so that any dependant module can raise events to
                    // its dependencies while loading.
                    if (!c.Module.Loaded)
                    {
                        c.Module.OnLoading();
                        c.Module.Loaded = true;
                    }
                }

                foreach (var c in loadFirst)
                {
                    if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                    {
                        continue;
                    }

                    // Finally we invoke OnLoaded on each module in the order in which they should be loaded. This is decided
                    // in the ImportModules method and when the imported modules are added to this collection.
                    if (c.Module.Loaded)
                    {
                        c.Module.OnLoaded();
                    }
                }
            }


            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                // Invoke the OnLoading method on each module in the order in which they should be loaded.
                // We set the loaded property to true here so that any dependant module can raise events to
                // its dependencies while loading.
                if (!c.Module.Loaded)
                {
                    c.Module.OnLoading();
                    c.Module.Loaded = true;
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                // Finally we invoke OnLoaded on each module in the order in which they should be loaded. This is decided
                // in the ImportModules method and when the imported modules are added to this collection.
                if (c.Module.Loaded)
                {
                    c.Module.OnLoaded();
                }
            }
        }


        /// <summary>
        /// Allows you to unload all modules or a single or subset of loaded modules by passing a list of
        /// <see cref="ModuleName"/> as an argument to this method. By default, invoking this method will invoke
        /// the <see cref="IModule.OnUnloading"/> and <see cref="IModule.OnUnloaded"/> methods for each module.
        /// By default, this does not unload or unlock any loaded assemblies. It is possible to implement
        /// you own IModuleCollection, IModuleContainer and IModule system using shadow loading or cross-process
        /// where more advanced control over module unloading is required.
        /// </summary>
        public virtual void UnloadModules(IList<ModuleName> modules = null)
        {
            // Mostly unloading modules is like loading modules but in reverse. Set the LoadModules method above...

            foreach (var c in Containers)
            {
                if (modules!= null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                // We set the Module.Unloaded property to false here so that the module will no longer be returned
                // when ModuleHost calls GetLoadedModules method.
                if (c.Module.Loaded)
                {
                    c.Module.OnUnloading();
                    c.Module.Loaded = false;
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                if (c.Module.Loaded)
                {
                    c.Module.OnUnloaded();
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleAttributes.Name))
                {
                    continue;
                }

                // Finally we would deinitialize the loaded module on the ModuleContainer and unload the referenced assembly.#
                // This is not implemented in .NET Core 2.0 implementation of AssemblyLoadContext and our default implementation
                // of IModuleContainer does not use AppDomains or cross-process communication so by default we do not unload and
                // unlock any loaded assemblies.
                if (c.Initialized)
                {
                    c.DeinitializeModule();
                }
            }
        }


        /// <summary>
        /// This wrapper method invokes <see cref="UnloadModules(IList{ModuleName})"/> and <see cref="LoadModules(IList{ModuleName})"/>
        /// in series.
        /// </summary>
        public virtual void ReloadModules(IList<ModuleName> modules = null)
        {
            UnloadModules(modules);
            LoadModules(modules);
        }


        /// <summary>
        /// Returns a list of <see cref="IEventPreHandler{e}"/>. Event pre-handlers can inspect or manipulate an <see cref="IEvent"/>
        /// before it is passed to the <see cref="IEventHandler.Handle"/> method of each module which can handle it.
        /// </summary>
        public virtual IList<IEventPreHandler> GetPreHandlers(IEvent e)
        {
            var type = e.GetType();
            var handler = typeof(IEventPreHandler<>);
            var handlers = Containers.Where(c =>
                c.Initialized && c.Module.Loaded && c.ModuleType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handler && i.GenericTypeArguments[0].IsAssignableFrom(type)));

            return handlers.Select(h => h.Module as IEventPreHandler).ToList();
        }


        /// <summary>
        /// Returns a list of <see cref="IEventPostHandler{e}"/>. Event post-handlers can inspect or manipulate an <see cref="IEvent"/>
        /// after it is passed to the <see cref="IEventHandler.Handle"/> method of each module which can handle it.
        /// </summary>
        public virtual IList<IEventPostHandler> GetPostHandlers(IEvent e)
        {
            var type = e.GetType();
            var handler = typeof(IEventPostHandler<>);
            var handlers = Containers.Where(c =>
                c.Initialized && c.Module.Loaded && c.ModuleType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handler && i.GenericTypeArguments[0].IsAssignableFrom(type)));

            return handlers.Select(h => h.Module as IEventPostHandler).ToList();
        }


        /// <summary>
        /// <see cref="ModuleCollection"/> implements <see cref="IEventHandler"/> and must implement this method as part of this interface.
        /// This returns true if any module in the collection is loaded which can handle the <see cref="IEvent"/> argument.
        /// </summary>
        public virtual bool CanHandle(IEvent e)
        {
            return GetLoadedModules(e).Any();
        }


        /// <summary>
        /// <see cref="ModuleCollection"/> implements <see cref="IEventHandler"/> and must implement this method as part of this interface.
        /// This method invokes <see cref="OnBeforeHandle(IEvent)"/>, followed by each loaded module's <see cref="IEventHandler.Handle(IEvent)"/>
        /// method, followed by <see cref="OnHandled(IEvent)"/>. This method is used internally when invoking <see cref="ModuleHost.Handle(IEvent)"/>
        /// and it is not recommended to call this method directly as this will create a ghost event which is not tracked by <see cref="IModuleHost"/>.
        /// </summary>
        public virtual void Handle(IEvent e)
        {
            DateTime now;
            TimeSpan then;

            OnBeforeHandle(e);

            var meta = new Dictionary<string, string>();
            var handlers = GetLoadedModules(e);

            foreach(var handler in handlers)
            {
                now = DateTime.UtcNow;
                handler.Handle(e);
                then = DateTime.UtcNow.Subtract(now);

                meta.Add(handler.ModuleAttributes.Name, then.ToString());

                if (e.Handled == true)
                {
                    break;
                }
            }

            e.SetMetaValue("handlers", meta);

            OnHandled(e);
        }


        /// <summary>
        /// This method invokes <see cref="GetPreHandlers(IEvent)"/> to get a list of pre-handlers and passes
        /// the <see cref="IEvent"/> argument to them in series. pre-handlers are not designed to process an
        /// event in any particular order like <see cref="ModuleAttribute.HandlePriority"/>.
        /// </summary>
        public virtual void OnBeforeHandle(IEvent e)
        {
            var handlers = GetPreHandlers(e);
            foreach (var handler in handlers)
            {
                handler.OnBeforeHandle(e);
            }
        }


        /// <summary>
        /// This method invokes <see cref="GetPostHandlers(IEvent)"/> to get a list of post-handlers and passes
        /// the <see cref="IEvent"/> argument to them in series. post-handlers are not designed to process an
        /// event in any particular order like <see cref="ModuleAttribute.HandlePriority"/>.
        /// </summary>
        public virtual void OnHandled(IEvent e)
        {
            var handlers = GetPostHandlers(e);
            foreach (var handler in handlers)
            {
                handler.OnHandled(e);   
            }
        }
    }
}
