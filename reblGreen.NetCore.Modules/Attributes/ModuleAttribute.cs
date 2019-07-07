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
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules
{
    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModuleAttribute : Attribute, IModuleAttribute
    {
        /// <summary>
        /// This is used along with the Name property to create a unique ID which can be used while loading and for identifying modules.
        /// </summary>
        readonly string Unique = Guid.NewGuid().ToString();


        /// <summary>
        /// The Name of the module is used to identify it and should be unique across all imported modules.
        /// </summary>
        public ModuleName Name { get; internal set; }


        /// <summary>
        /// 
        /// </summary>
        public Version Version { get; internal set; }


        /// <summary>
        /// The Dscription property allows you to briefly explain what this module is used for.
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// AdditionalInformation can be used to provide any further information and instructions for usage.
        /// </summary>
        public string[] AdditionalInformation { get; set; } = new string[] { };


        /// <summary>
        /// Dependencies can contain a list of modules that must be available and loaded for this module to handle events.
        /// </summary>
        public string[] Dependencies { get; set; } = new string[] { };

        
        /// <summary>
        /// This allows the instance of IModule to inform IModuleHost of the priority in which it would like to handle
        /// IEvents. It means that if 2 or more instances of IModule handle the same types which implement IEvent, you
        /// can control the order in which IModule handles the IEvent first. This can be overridden via
        /// <see cref="IModuleHost.EventHandlePriority"/>.
        /// </summary>
        public short HandlePriority { get; set; }


        /// <summary>
        /// This allows the instance of IModule to inform IModuleHost of the priority in which it should be loaded.
        /// <see cref="IModuleHost.ModuleLoadPriority"/>.
        /// </summary>
        public short LoadPriority { get; set; }

        
        /// <summary>
        /// Returns a unique ID for the IModule instance.
        /// </summary>
        public string ID { get { return string.Format("{0}-{1}", Name, Unique); } }
    }
}
