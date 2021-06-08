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

using System;

namespace reblGreen.NetCore.Modules.Interfaces
{
    public interface IModuleAttribute
    {
        /// <summary>
        /// The Name of this IModule is used to identify it and should be unique across all instances of IModule.
        /// </summary>
        ModuleName Name { get; }


        /// <summary>
        /// Can be used to check for compatibility by other instances implementing IModule in case of a specific version dependancy.
        /// </summary>
        Version Version { get; }


        /// <summary>
        /// The Description property allows you to briefly explain what this instance of IModule is used for.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// AdditionalInformation can be used to provide any further information and instructions for usage.
        /// </summary>
        string[] AdditionalInformation { get; }

        /// <summary>
        /// Dependencies can contain a list of <see cref="Name"/> in string format which must be available and loaded
        /// instances of IModule for this instance of IModule to be able to successfully handle object implementing
        /// IEvent.
        /// </summary>
        string[] Dependencies { get; }


        /// <summary>
        /// This allows the instance of IModule to inform IModuleHost of the priority in which it would like to handle
        /// IEvents. 
        /// </summary>
        short HandlePriority { get; }

        /// <summary>
        /// This allows the instance of IModule to inform IModuleHost of the priority in which it should be loaded.
        /// </summary>
        short LoadPriority { get; }


        /// <summary>
        /// This property tells the ModuleHost to fully load the instance of IModule before loading other modules.
        /// This will work alongside LoadPriority but should be considered as loading of primary and secondary modules
        /// where an IModule instance with LoadFirst set to true would trigger both OnLoading() and Onloaded() before
        /// other module instances.
        /// </summary>
        bool LoadFirst { get; }
    }
}
