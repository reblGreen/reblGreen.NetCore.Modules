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
using reblGreen.NetCore.Modules.Classes;
using reblGreen.NetCore.Modules.Events;
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


        /// <summary>
        /// CanHandle can be called by the IEvent host to see if this handler is able to handle the requested event. This
        /// allows the handler to inspect the IEvent and inform the requester if it can be handled without further processing.
        /// </summary>
        /// <param name="e">The IEvent to inspect for handling.</param>
        /// <returns></returns>
        public abstract bool CanHandle(IEvent e);


        /// <summary>
        /// Pass an IEvent to this EventHandler for it to be processed. All EventHandlers should handle any IEvents which are
        /// known to the EventHandler within this method.
        /// </summary>
        /// <param name="e">The IEvent to be handled.</param>
        public abstract void Handle(IEvent e);



        /// <summary>
        /// Implemented due to demand. An object implementing IModule may often require the ability to retrieve configurable settings. This method is implemented in Module
        /// and acts as a wrapper for pushing a GetSettingEvent into the IModuleHost for handling. If a module is not provided to handle the GetSettingEvent then retrieving
        /// the requested setting will fail and attempt to log using this Module's Log method. Functionality can be overridden. For security reasons this IModule should not
        /// be allowed to access another Module's settings.
        /// </summary>
        /// <typeparam name="T">The type of the required setting.</typeparam>
        /// <param name="name">The identifier string for the required setting. This is passed to the generated LoggingEvent for handling.</param>
        /// <param name="@default">The default setting to use if a configured setting is not available or if the returned setting is the wrong type.</param>
        public virtual T GetSetting<T>(string name, T @default = default(T))
        {
            /*
             * This overridable method acts is a wrapper for creating a GetSettingEvent and invoking IModuleHost.Handle on it. If no module exists to handle the
             * GetSettingEvent or if the return type is unexpected we do some logging using the IModule.Log method. See below.
             */
            var getSettingEvent = new GetSettingEvent();
            getSettingEvent.Input.ModuleName = ModuleAttributes.Name;
            getSettingEvent.Input.SettingName = name;
            Host.Handle(getSettingEvent);

            if (getSettingEvent.Handled && getSettingEvent.Output != null)
            {
                var setting = getSettingEvent.Output.Setting;
                var type = typeof(T);

                if (setting != null)
                {
                    if (setting is T s)
                    {
                        return s;
                    }

                    Log(LoggingEvent.Severity.Warning,
                        new InvalidCastException(
                            string.Format("The object returned in GetSettingEvent.Output.Setting is {0}. Expected return type is {1}.", setting.GetType(), type)),
                        getSettingEvent);

                    return @default;
                }

                Log(LoggingEvent.Severity.Debug, new NullReferenceException("GetSettingEvent.Output.Setting is null."), getSettingEvent);

                return @default;
            }

            Log(LoggingEvent.Severity.Error,
                new NotImplementedException(
                    string.Format("No module exists to handle event type of {0} or the handling module is not marking the event as handled.", getSettingEvent.Name)),
                getSettingEvent);

            return @default;
        }


        /// <summary>
        /// Implemented due to demand. A Module may often require the ability to log debug data and analytics. This method is implemented in Module and acts as a wrapper
        /// for pushing a LoggingEvent into the IModuleHost for handling. If a module is not provided to handle the LoggingEvent then logging will fail silently. This
        /// Functionality can be overridden on a per-module basis.
        /// </summary>
        /// <param name="severity">The severity of logging required. This is passed to the generated LoggingEvent for handling.</param>
        /// <param name="arguments">The arguments to be logged. These are passed to the generated LoggingEvent for handling.</param>
        public virtual void Log(LoggingEvent.Severity severity, params object[] arguments)
        {
            /*
             * This overridable method creates a LoggingEvent, populates its properties and passes it to IModuleHost for handling.
             * If a module does not exist to handle an IEvent of type LoggingEvent logging will fail silently. If we were to attempt
             * logging of a failed LoggingEvent we would create an infinite loop of logging fails... I hear StackOverflowException.
             */
            var loggingEvent = new LoggingEvent();
            loggingEvent.Input.Severity = severity;
            loggingEvent.Input.Arguments = arguments.ToList();
            Host.Handle(loggingEvent);
        }
        

        /// <summary>
        /// This method should be invoked by IModuleHost when the module is first loading. It allows for a module to initialize or request handling of an IModuleEvent
        /// by a module whose loading priority is higher than its own. Attempting to request data from another module in a module's constructor will fail and requesting
        /// data from a module in this invocation may fail if the required module is not yet loaded.
        /// </summary>
        public virtual void OnLoading()
        {
            // Empty method is designed to be overriden if required.
        }


        /// <summary>
        /// This method should be invoked by IModuleHost when a module is loaded. Any cross-module activity should only occur when this method is invoked to allow for
        /// each module to initialize itself during the OnLoading method. Attempting to request that a module handles an event before now may fail if the reuired plugin
        /// is not yet completelyloaded.
        /// </summary>
        public virtual void OnLoaded()
        {
            // Empty method is designed to be overriden if required.
        }


        /// <summary>
        /// This method should be invoked by the IModuleHost just before a module is going to be unloaded and the host should attempt to delay any final application
        /// termination for a number of seconds to allow for any per-module finalization.
        /// </summary>
        public virtual void OnUnloading()
        {
            // Empty method is designed to be overriden if required.
        }


        /// <summary>
        /// This method should be invoked by the IModuleHost when all modules are being unloaded and you should do any final cleanup of resources here. It is not advised
        /// to try and access or request data from other modules inside this method as this may be a global unload of all loaded modules.
        /// </summary>
        public virtual void OnUnloaded()
        {
            // Empty method is designed to be overriden if required.
        }


        #region Overrides


        public override string ToString()
        {
            return ModuleAttributes.Name;
        }


        public override bool Equals(object obj)
        {
            if (obj is Module module)
            {
                return module.ModuleAttributes.Name.Equals(ModuleAttributes.Name);
            }

            return base.Equals(obj);
        }


        public override int GetHashCode()
        {
            return ModuleAttributes.Name.GetHashCode();
        }


        public static implicit operator string(Module m)
        {
            return m.ToString();
        }

        #endregion
    }
}
