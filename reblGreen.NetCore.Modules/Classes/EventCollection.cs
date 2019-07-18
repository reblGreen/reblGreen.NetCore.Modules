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
using System.Linq;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Classes
{
    [Serializable]
    public class EventCollection : List<Type>, IEventCollection
    {
        internal IModuleHost Host { get; set; }
        private bool Imported;

        IDictionary<Type, IEvent> Instantiated;

        internal EventCollection(IModuleHost host)
        {
            Host = host;
            Instantiated = new Dictionary<Type, IEvent>();
        }


        internal void ImportEvents()
        {
            if (Imported)
            {
                throw new Exception("Events already imported. Importing events multiple times would cause multiple instances of all known events.");
            }

            var events = TypeManager.FindEvents<IEvent>(Host.WorkingDirectory, 1);
            AddRange(events);

            foreach (var @event in events)
            {
                Instantiated.Add(@event, TypeManager.InstantiateEvent(@event));
            }

            Imported = true;
        }


        public IList<EventName> GetKnownEvents()
        {
            return Instantiated.Values.Select(i => i.Name).ToList();
        }


        public IList<Type> GetKnownEventTypes()
        {
            return Instantiated.Keys.ToList();
        }


        public IEvent GetSolidEventFromName(EventName name)
        {
            var instantiated = Instantiated.FirstOrDefault(i => i.Value.Name == name);
            if (instantiated.Equals(default(KeyValuePair<Type, IEvent>)))
            {
                return null;
            }

            return TypeManager.InstantiateEvent(instantiated.Key);
        }


        public IEvent GetSolidEventFromType(Type type)
        {
            var instantiated = Instantiated.FirstOrDefault(i => i.Key == type);
            if (instantiated.Equals(default(KeyValuePair<Type, IEvent>)))
            {
                return null;
            }

            return TypeManager.InstantiateEvent(instantiated.Key);
        }


        public IEvent GetSolidEventFromType<T>(T type) where T : IEvent<IEventInput, IEventOutput>
        {
            var instantiated = Instantiated.FirstOrDefault(i => i.Key == typeof(T));
            if (instantiated.Equals(default(KeyValuePair<Type, IEvent>)))
            {
                return null;
            }

            return TypeManager.InstantiateEvent(instantiated.Key);
        }
    }
}
