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

namespace reblGreen.NetCore.Modules.Interfaces
{
    public interface IEventCollection
    {
        /// <summary>
        /// Return a instance of an Event object which implements <see cref="IEvent{I, O}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Solid instance of the relevant type based on the string.</returns>
        IEvent GetSolidEventFromType(Type type);


        /// <summary>
        /// Return a instance of an Event object which implements <see cref="IEvent{I, O}"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Solid instance of the relevant type based on the string.</returns>
        IEvent GetSolidEventFromType<T>(T type) where T : IEvent<IEventInput, IEventOutput>;


        /// <summary>
        /// Return a instance of an Event object which implements <see cref="IEvent{I, O}"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Solid instance of the relevant type based on the name of the event.</returns>
        IEvent GetSolidEventFromName(EventName name);


        /// <summary>
        /// Return a list of all Events known to the ModuleHost.
        /// </summary>
        IList<EventName> GetKnownEvents();


        /// <summary>
        /// Should return a list of all event types known to the ModuleHost.
        /// </summary>
        IList<Type> GetKnownEventTypes();
    }
}
