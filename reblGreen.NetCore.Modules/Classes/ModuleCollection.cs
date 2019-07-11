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
using System.Linq;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Classes
{
    [Serializable]
    public class ModuleCollection : List<Type>, IModuleCollection,
        IEventHandler,
        IEventPreHandler<IEvent>,
        IEventPostHandler<IEvent>
    {
        private List<IModuleContainer> Containers;
        private bool Imported;


        public IModuleHost Host { get; private set; }


        public ModuleCollection(IModuleHost host)
        {
            Containers = new List<IModuleContainer>();
            Host = host;
        }


        public virtual IList<IModule> GetLoadedModules(IEvent e = null)
        {
            if (e == null)
            {
                return Containers.Where(c => c.Initialized && c.Module.Loaded).Select(c => c.Module as IModule).ToList();
            }

            return Containers.Where(c => c.Initialized && c.Module.Loaded && c.Module.CanHandle(e)).Select(c => c.Module as IModule).ToList();
        }


        public virtual IList<IModule> GetModulesByType<T>() where T : IModule
        {
            return Containers.Where(c => c.ModuleType.IsAssignableFrom(typeof(T))).Select(c => c.Module as IModule).ToList();
        }


        public virtual bool HasModule(ModuleName name, Version min = null, Version max = null)
        {
            if (min == null && max == null)
            {
                return Containers.Any(c => c.ModuleDetails.Name == name);
            }
            else if (min == null)
            {
                return Containers.Any(c => c.ModuleDetails.Name == name && c.ModuleDetails.Version <= max);
            }
            else if (max == null)
            {
                return Containers.Any(c => c.ModuleDetails.Name == name && c.ModuleDetails.Version >= min);
            }

            return Containers.Any(c => c.ModuleDetails.Name == name && (c.ModuleDetails.Version >= min && c.ModuleDetails.Version <= max));
        }


        public virtual void ImportModules()
        {
            if (Imported)
            {
                throw new Exception("Modules already imported. Importing modules multiple times would cause multiple instances of all known modules.");
            }

            // We add the containers and sort them by the order in which they should be loaded, which is specified using the
            // ModuleDetails.ModuleLoadPriority property.

            var containers = TypeManager.FindModules<Module>(Host, Host.WorkingDirectory);
            Containers.AddRange(containers.OrderBy(module => module.ModuleDetails.LoadPriority));


            AddRange(containers.OrderBy(module => module.ModuleDetails.HandlePriority).Select(module => module.ModuleType));
            Imported = true;
        }


        public virtual void LoadModules(IList<ModuleName> modules = null)
        {
            // We first need to initialize each module inside its container. The loading process of an array of modules is as follows:
            // 1) Ensure all module dependencies exist within the module ModuleDetails.Dependencies array
            // 2) Initialize each module so we have a useable module type and can invoke its methods.
            // 3) Invoke OnLoading on each module based on ModuleDetails.ModuleLoadPriority property. This allows the module to initialize
            //    and configure any requirements within itself. which may require communication with other modules.
            // 4) Invoke OnLoaded on each module based on ModuleDetails.ModuleLoadPriority property.

            // When we imported modules we ordered the containers by ModuleDetails.LoadPriority which makes it nice and easy to just
            // itterate each container and invoke the requirements in order.

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleDetails.Name))
                {
                    continue;
                }

                var missing = c.ModuleDetails.Dependencies.Where(d => !HasModule(d));
                if (missing.Count() > 0)
                {
                    throw new Exception(string.Format("{0} module has missing module dependencies: {1}", c.ModuleDetails.Name, string.Join(' ', missing)));
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleDetails.Name))
                {
                    continue;
                }

                if (!c.Initialized)
                {
                    c.InitializeModule();
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleDetails.Name))
                {
                    continue;
                }

                if (!c.Module.Loaded)
                {
                    c.Module.OnLoading();
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleDetails.Name))
                {
                    continue;
                }

                if (!c.Module.Loaded)
                {
                    c.Module.OnLoaded();
                    c.Module.Loaded = true;
                }
            }
        }


        public virtual void UnloadModules(IList<ModuleName> modules = null)
        {
            // Mostly unloading modules is like loading modules but in reverse. Set the LoadModules method above...

            foreach (var c in Containers)
            {
                if (modules!= null && !modules.Contains(c.ModuleDetails.Name))
                {
                    continue;
                }

                if (c.Module.Loaded)
                {
                    c.Module.OnUnloading();
                }
            }

            foreach (var c in Containers)
            {
                if (modules != null && !modules.Contains(c.ModuleDetails.Name))
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
                if (modules != null && !modules.Contains(c.ModuleDetails.Name))
                {
                    continue;
                }

                if (c.Initialized)
                {
                    c.DeinitializeModule();
                }

                c.Module.Loaded = false;
            }
        }


        public virtual void ReloadModules(IList<ModuleName> modules = null)
        {
            UnloadModules(modules);
            LoadModules(modules);
        }


        public virtual IList<IEventPreHandler> GetPreHandlers(IEvent e)
        {
            var type = e.GetType();
            var handler = typeof(IEventPreHandler<>);
            var handlers = Containers.Where(c =>
                c.ModuleType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handler && i.GenericTypeArguments[0].IsAssignableFrom(type)));

            return handlers.Select(h => h.Module as IEventPreHandler).ToList();
        }


        public virtual IList<IEventPostHandler> GetPostHandlers(IEvent e)
        {
            var type = e.GetType();
            var handler = typeof(IEventPostHandler<>);
            var handlers = Containers.Where(c =>
                c.ModuleType.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == handler && i.GenericTypeArguments[0].IsAssignableFrom(type)));

            return handlers.Select(h => h.Module as IEventPostHandler).ToList();
        }


        public virtual bool CanHandle(IEvent e)
        {
            return GetLoadedModules(e).Count() > 0;
        }


        public virtual void Handle(IEvent e)
        {
            OnBeforeHandle(e);

            var handlers = GetLoadedModules(e);

            foreach(var handler in handlers)
            {
                handler.Handle(e);

                if (e.Handled == true)
                {
                    break;
                }
            }

            OnHandled(e);
        }


        public virtual void OnBeforeHandle(IEvent e)
        {
            var handlers = GetPreHandlers(e);
            foreach (var handler in handlers)
            {
                handler.OnBeforeHandle(e);
            }
        }


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
