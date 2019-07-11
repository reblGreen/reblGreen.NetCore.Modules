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
using System.Collections.ObjectModel;
using reblGreen.NetCore.Modules.Classes;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules
{
    public abstract class ModuleHost : IModuleHost
    {
        readonly ModuleCollection _ModuleCollection;
        readonly EventCollection _EventCollection;
        readonly Dictionary<string, IEvent> _EventsInProgress;
        readonly ReadOnlyDictionary<string, IEvent> _ReadOnlyEventsInProgress;

        /// <summary>
        /// Creates a new instance of ModuleHost. When using startup args in your application, you can pass these args through to ModuleHost
        /// so they can be inspected by any module which may require them.
        /// </summary>
        /// <param name="args">Arguments to pass to modules.</param>
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

            // Create the events tracking list and a readonly wrapper to pass in the IModuleHost.EventsInProgress property.
            _EventsInProgress = new Dictionary<string, IEvent>();
            _ReadOnlyEventsInProgress = new ReadOnlyDictionary<string, IEvent>(_EventsInProgress);
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


        public virtual IReadOnlyDictionary<string, IEvent> EventsInProgress {
            get
            {
                return _ReadOnlyEventsInProgress;
            }
        }


        public virtual bool CanHandle(IEvent e)
        {
            return _ModuleCollection.CanHandle(e);
        }


        public virtual void Handle(IEvent e)
        {
            // We generate a unique ID for the event and add it to the IEvent.Meta dictionary. This unique ID can be used to
            // Track and monitor the event during the handling process through the exposed EventsInProgress property.

            var id = string.Format("{0}-{1}", e.GetHashCode().ToString("X2").ToLowerInvariant(), Guid.NewGuid());
            e.SetMetaValue("id", id);

            lock (_EventsInProgress)
            {
                _EventsInProgress.Add(id, e);
            }

            // Pass the event over to the ModuleCollection for handling. This keeps things more readable in this class.
            _ModuleCollection.Handle(e);

            // Once the event is completed we need to remove it from the EventsInProgress list.
            lock (_EventsInProgress)
            {
                _EventsInProgress.Remove(id);
            }
        }
    }
}
