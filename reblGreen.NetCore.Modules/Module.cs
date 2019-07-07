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
using reblGreen.NetCore.Modules.Classes;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules
{
    /// <summary>
    /// A Module is an <see cref="IEventHandler">IEventHandler</see> which can be loaded & manipulated by an <see cref="IModuleHost">IModuleHost</see>.
    /// This class exposes members to the IModuleHost which identify both the IModule itself and what IEvents it is able to handle.
    /// </summary>
    /// <typeparam name="E">A type implementing IEvent which can be handled by this Modules IEventHandler.</typeparam>
    /// <typeparam name="I">The IEventInput of the IEvent.</typeparam>
    /// <typeparam name="O">The IEventOutput of the IEvent.</typeparam>
    [Serializable]
    [Module(Description = "This is an abstract module which can be inherited while creating a new Module Type.")]
    public abstract class Module : IModule
    {
        public virtual IModuleHost Host { get; internal set; }


        public virtual IModuleAttribute ModuleAttributes
        {
            get; internal set;
        }


        Version version;

        public virtual Version Version
        {
            get
            {
                if (version == null)
                {
                    version = GetType().GetAssemblyInfo().Version;
                }

                return version;
            }
        }


        Uri path;

        public virtual Uri WorkingDirectory
        {
            get
            {
                if (path == null)
                {
                    path = AssemblyTools.GetPathToAssembly(GetType());
                }

                return path;
            }
        }


        public virtual IList<IEvent> Events
        {
            get
            {
                return new List<IEvent>();
            }
        }


        public virtual bool Loaded
        {
            get; internal set;
        }


        public abstract bool CanHandle(IEvent e);


        public abstract void Handle(IEvent e);


        public virtual T GetResource<T>(string resourceId, T @default = default(T))
        {
            throw new NotImplementedException();
        }


        public virtual T GetSetting<T>(string settingId, T @default = default(T))
        {
            throw new NotImplementedException();
        }


        public virtual void OnLoaded()
        {
            // Empty method is designed to be overriden if required.
        }


        public virtual void OnLoading()
        {
            // Empty method is designed to be overriden if required.
        }


        public virtual void OnUnloaded()
        {
            // Empty method is designed to be overriden if required.
        }


        public virtual void OnUnloading()
        {
            // Empty method is designed to be overriden if required.
        }


        #region Overrides

        public override string ToString()
        {
            return ModuleAttributes.Name;
        }

        public static implicit operator string(Module m)
        {
            return m.ToString();
        }

        #endregion
    }
}
