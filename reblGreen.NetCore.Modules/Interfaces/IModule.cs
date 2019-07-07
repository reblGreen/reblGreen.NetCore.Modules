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
    /// <summary>
    /// An IModule is an <see cref="IEventHandler">IEventHandler</see> which can be manipulated by an <see cref="IModuleHost">IModuleHost</see>.
    /// This interface exposes members to the IModuleHost which identify both the IModule itself and what IEvents it is able to handle.
    /// </summary>
    public interface IModule : IEventHandler<IEvent<IEventInput, IEventOutput>, IEventInput, IEventOutput>
    {
        /// <summary>
        /// Each module requires a reference to the IModuleHost which parents the module. Make sure you set this property!
        /// </summary>
        IModuleHost Host { get; }

        /// <summary>
        /// Attributes contains the standard information about the instance implementing IModule such as its name, description and
        /// any dependencies.
        /// </summary>
        IModuleAttribute ModuleAttributes { get; }


        /// <summary>
        /// Can be used to check for compatibility by other instances implementing IModule in case of a specific version dependancy.
        /// </summary>
        Version Version { get; }


        /// <summary>
        /// The WorkingDirectory property should return the absolute path to the directory the module was loaded from.
        /// This is used by the IModule to access external files which it may depend on such as a required file,
        /// external executable or resource.
        /// </summary>
        Uri WorkingDirectory { get; }


        /// <summary>
        /// Contains the types implementing IEvent which can be handled by this IModules IEventHandler.
        /// </summary>
        IList<IEvent> Events { get; }

        
        /// <summary>
        /// The IModule should be marked as loaded and this property should be checked before any object which implements IEvent can be handled.
        /// </summary>
        bool Loaded { get; }


        /// <summary>
        /// This method should be invoked by IModuleHost to start it up and this module should not be accessed until this has been called. 
        /// Attempting to access another module in a module's constructor or in this method may fail 
        /// if the requested plugin is not loaded yet. A module should set its started property to true after code is executed.
        /// </summary>
        void OnLoading();


        /// <summary>
        /// This method should be invoked by IModuleHost when all modules are loaded. other modules should only be accessed when this method is invoked or from the 
        /// IModuleHost's RunModuleMethod function. Attempting to access another module in a module's before now may fail if the requested plugin is not loaded yet.
        /// </summary>
        void OnLoaded();


        /// <summary>
        /// This method should be invoked by the IModuleHost just before all modules are going to be unloaded and the host should attempt to delay termination for a few
        /// seconds to allow any finalization. If you need to do some last minute cross module access, do it now!
        /// </summary>
        void OnUnloading();


        /// <summary>
        /// This method should be invoked by the IModuleHost when all modules are being unloaded and you should do any final cleanup of resources here. It is not advised
        /// to try and access other modules inside this method.
        /// </summary>
        void OnUnloaded();


        /// <summary>
        /// An object implementing IModule may often require the ability to retrieve configurable settings. This method is provided and should be used for requesting
        /// module settings. For security reasons this IModule should not be allowed to access another IModule instances settings.
        /// </summary>
        /// <typeparam name="T">The type of the required setting.</typeparam>
        /// <param name="settingId">The identifier string for the required setting.</param>
        /// <param name="default">The default setting to use if a configured setting is not available.</param>
        T GetSetting<T>(string settingId, T @default = default(T));


        /// <summary>
        /// This method is similar to <see cref="GetSetting{T}(string, T)"/> but returns a resource instead. This could be a file or shared configuration, it allows
        /// for fine granular control of where resources are loaded from.
        /// </summary>
        /// <typeparam name="T">The type of the required resource.</typeparam>
        /// <param name="settingId">The identifier string for the required resource.</param>
        /// <param name="defaultSetting">The default to return if the required resource is not available.</param>
        /// <returns></returns>
        T GetResource<T>(string resourceId, T @default = default(T));
    }
}
