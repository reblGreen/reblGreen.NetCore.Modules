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

namespace reblGreen.NetCore.Modules.Interfaces
{
    /// <summary>
    /// An instance which implements IEventHandler can handle objects which implement IEvent.
    /// A handler can be strictly types using the generic arguments or can handle any type of
    /// event by strictly typing the interfaces directly. 
    /// </summary>
    public interface IEventHandler<E, I, O>
        where E : IEvent<I, O>
        where I : IEventInput
        where O : IEventOutput
    {
        ///// <summary>
        ///// CanHandle can be called by the IEvent host to see if this handler is able to handle the requested event. This
        ///// allows the handler to inspect the IEvent and inform the requester if it can be handled without further processing.
        ///// </summary>
        ///// <returns></returns>
        //bool CanHandle<T>() where T : IEvent<IEventInput, IEventOutput>;


        /// <summary>
        /// CanHandle can be called by the IEvent host to see if this handler is able to handle the requested event. This
        /// allows the handler to inspect the IEvent and inform the requester if it can be handled without further processing.
        /// </summary>
        /// <param name="e">The IEvent to inspect for handling.</param>
        /// <returns></returns>
        bool CanHandle(IEvent e);


        ///// <summary>
        ///// Pass an IEvent to this EventHandler for it to be processed. All EventHandlers should handle any IEvents which are
        ///// known to the EventHandler within this method.
        ///// </summary>
        ///// <param name="e">The IEvent to be handled.</param>
        //void Handle<T>(T e) where T : IEvent<IEventInput, IEventOutput>;


        /// <summary>
        /// Pass an IEvent to this EventHandler for it to be processed. All EventHandlers should handle any IEvents which are
        /// known to the EventHandler within this method.
        /// </summary>
        /// <param name="e">The IEvent to be handled.</param>
        void Handle(IEvent e);
    }
}
