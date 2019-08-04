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

using System.Collections.Generic;

namespace reblGreen.NetCore.Modules.Interfaces
{
    /// <summary>
    /// An object which implements IEvent can be handled by an instance which implements IEventHandler.
    /// </summary>
    public interface IEvent<I, O> : IEvent
        where I : IEventInput
        where O : IEventOutput
    {
        /// <summary>
        /// The Input property must inherit IEventInput and acts as a placeholder for any properties, fields or other data which can be passed to
        /// an IEventHandler as arguments.
        /// </summary>
        I Input { get; set; }


        /// <summary>
        /// The Output object inherits from IEventOutput and is used for returning any properties, fields or other data while an IEvent is being
        /// handled by an IEventHandler.
        /// </summary>
        O Output { get; set; }
    }


    /// <summary>
    /// An object which implements IEvent can be handled by an instance which implements IEventHandler.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Each IEvent which is loaded into ModuleHost should have a unique name which can be used to identify the event type where the concrete
        /// type of the IEvent object is unknown.
        /// </summary>
        EventName Name { get; }

        /// <summary>
        /// The Meta dictionary can be used to hold and transfer any generic or event specific data between modules.
        /// </summary>
        Dictionary<string, object> Meta { get; set; }

        /// <summary>
        /// Handled should return true only if the event was completed.
        /// </summary>
        bool Handled { get; set; }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to IModuleEvent.Input where available.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done using <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. return ((dynamic)this).Input as IEventInput;
        /// </summary>
        IEventInput GetEventInput();


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to IModuleEvent.Output where available.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done using <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. return ((dynamic)this).Output as IEventOutput;
        /// </summary>
        IEventOutput GetEventOutput();


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to the setter for IModuleEvent.Output.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done using <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. ((dynamic)this).Output = output;
        /// </summary>
        void SetEventOutput(IEventOutput output);
    }
}
