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
        public LoggingEventInput Input { get; set; } = new LoggingEventInput();

        public LoggingEventOutput Output { get; set; }

        public EventName Name { get; } = "reblGreen.NetCore.Modules.Events.LoggingEvent";

        public Dictionary<string, object> Meta { get; set; }

        public bool Handled { get; set; }


        public enum Severity
        {
            Analytics,
            Debug,
            Error,
            Warning
        }
    }

}
