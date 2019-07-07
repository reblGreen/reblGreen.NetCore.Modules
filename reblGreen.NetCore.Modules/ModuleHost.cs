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
    public abstract class ModuleHost : IModuleHost
    {
        readonly ModuleCollection _ModuleCollection;
        readonly EventCollection _EventCollection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public ModuleHost(string[] args = null)
        {

            if (args != null)
            {
                Arguments = args;
            }
            else
            {
                Arguments = new List<string>();
            }

            var eventCollection = new EventCollection(this);
            eventCollection.ImportEvents();
            _EventCollection = eventCollection;

            var moduleCollection = new ModuleCollection(this);
            moduleCollection.ImportModules();
            _ModuleCollection = moduleCollection;
        }

        ~ModuleHost()
        {
            Modules.UnloadModules(null);
        }

        public virtual IModuleCollection Modules
        {
            get
            {
                return _ModuleCollection;
            }
        }

        public virtual IEventCollection Events
        {
            get
            {
                return _EventCollection;
            }
        }

        public virtual IList<string> Arguments { get; } = new List<string>();


        Uri _WorkingDirectory;
        public virtual Uri WorkingDirectory
        {
            get
            {
                if (_WorkingDirectory == null)
                {
                    _WorkingDirectory = AssemblyTools.GetPathToAssembly(GetType());
                }

                return _WorkingDirectory;
            }
        }

        string _ApplicationName;
        public virtual string ApplicationName
        {
            get
            {
                if (_ApplicationName == null)
                {
                    _ApplicationName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;
                }

                return _ApplicationName;
            }
        }


        public virtual IDictionary<string, IEvent> EventsInProgress { get; } = new Dictionary<string, IEvent>();


        public virtual bool CanHandle(IEvent e)
        {
            return _ModuleCollection.CanHandle(e);
        }


        public virtual void Handle(IEvent e)
        {
            e.SetMetaValue("id", Guid.NewGuid());

            EventsInProgress.Add(e.GetMetaValue("id", Guid.NewGuid()).ToString(), e);

            _ModuleCollection.Handle(e);
            
            EventsInProgress.Remove(e.GetMetaValue("id", Guid.NewGuid()).ToString());
        }
    }
}
