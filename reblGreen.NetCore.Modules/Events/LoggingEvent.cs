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
using System.Text;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Events
{
    /// <summary>
    /// This event is built in to the reblGreen.NetCore.Modules core library and requires a module to be imported and loaded
    /// which can handle this event type for any logging to occur. If no module is loaded to handle this event logging will
    /// fail silently.
    /// </summary>
    [Serializable]
    public class LoggingEvent : IEvent<LoggingEventInput, LoggingEventOutput>
    {
        public enum Severity
        {
            Analytics,
            Debug,
            Error,
            Warning
        }


        public LoggingEventInput Input { get; set; } = new LoggingEventInput();

        public LoggingEventOutput Output { get; set; }

        public EventName Name { get; } = "reblGreen.NetCore.Modules.Events.LoggingEvent";

        public Dictionary<string, object> Meta { get; set; }

        public bool Handled { get; set; }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to IModuleEvent.Input where available.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done with <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. return ((dynamic)this).Input as IEventInput;
        /// </summary>
        public IEventInput GetEventInput()
        {
            return ((dynamic)this).Input as IEventInput;
        }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to IModuleEvent.Output where available.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done with <see cref="System.Reflection"/> or more efficiently using dynamic casting.
        /// Eg. return ((dynamic)this).Output as IEventOutput;
        /// </summary>
        public IEventOutput GetEventOutput()
        {
            return ((dynamic)this).Output as IEventOutput;
        }


        /// <summary>
        /// It has been required by modules which are designed to handle generic types of IEvent and need access to the setter for IModuleEvent.Output.
        /// Since directly casting to <see cref="IEvent{I, O}"/> in not allowed we must expose and handle this implementation through <see cref="IEvent"/>
        /// directly. This can be done with <see cref="System.Reflection"/>.
        /// </summary>
        public void SetEventOutput(IEventOutput output)
        {
            // This purposely assumes that output exists on the event type and that typeof(output) is equal to typeof(this.Output).
            // It will throw an exception where any of the above conditions fail. This is by design.
            GetType().GetProperty("Output").SetValue(this, output);
        }
    }

}
