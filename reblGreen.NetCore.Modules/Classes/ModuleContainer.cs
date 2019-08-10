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
using reblGreen.NetCore.Modules.Events;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Classes
{
    [Serializable]
    public class ModuleContainer : IModuleContainer, IModule
    {
        public IModuleHost Host { get; private set; }
        public bool Initialized {  get { return Module != null; } }

        public Uri Path
        {
            get; private set;
        }


        public AssemblyLoader LoadContext
        {
            get; internal set;
        }


        public Type ModuleType
        {
            get; private set;
        }


        public Module Module { get; private set; }

        public IModuleAttribute ModuleAttributes { get; internal set; }

        public Uri WorkingDirectory { get { return Module.WorkingDirectory; } }

        public bool Loaded { get { return Initialized && Module.Loaded; } }

        public ModuleContainer(Uri path, Type type, IModuleAttribute attribute, IModuleHost host)
        {
            Host = host;
            Path = path;
            ModuleType = type;
            ModuleAttributes = attribute;
            LoadContext = new AssemblyLoader(path);
        }



        public void InitializeModule()
        {
            LoadContext.Load();
            var module = TypeManager.InstantiateModule(ModuleType, LoadContext);
            module.ModuleAttributes = ModuleAttributes;
            module.Host = Host;
            Module = module;
        }

        public void DeinitializeModule()
        {
            Module = null;
            LoadContext.Unload();
        }

        public T GetSetting<T>(string name, T @default = default(T))
        {
            return Module.GetSetting<T>(name, @default);
        }

        public void Log(LoggingEvent.Severity severity, params object[] args)
        {
            Module.Log(severity, args); ;
        }

        public void OnLoading()
        {
            Module.OnLoading();
        }

        public void OnLoaded()
        {
            Module.OnLoaded();
        }

        public void OnUnloading()
        {
            Module.OnUnloading();
        }

        public void OnUnloaded()
        {
            Module.OnUnloaded();
        }

        public bool CanHandle(IEvent e)
        {
            return Module.CanHandle(e);
        }

        public void Handle(IEvent e)
        {
            Module.Handle(e);
        }

        public override bool Equals(object obj)
        {
            if (obj is ModuleContainer container)
            {
                container.ModuleAttributes.Name.Equals(ModuleAttributes.Name);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ModuleAttributes.Name.GetHashCode();
        }
    }
}
