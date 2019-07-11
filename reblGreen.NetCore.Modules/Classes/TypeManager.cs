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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Classes
{
    [Serializable]
    public sealed class TypeManager
    {
        private TypeManager() { }


        /// <summary>
        /// Find Modules. Direcotry depth can be controlled and is set to maximum depth by default. If you would like to create modules which
        /// are also ModuleHosts themselves, it is a good idea nest modules one directory level deep followed by nesting submodules another
        /// level deep. This will isolate parent modules from submodules.
        /// </summary>
        /// <param name="path">Where to search for DLL's which export IModule.</param>
        /// <param name="directoryDepth">How many subdirectories deep of path be searched.</param>
        public static IList<IModuleContainer> FindModules<T>(IModuleHost host, Uri path, int directoryDepth = int.MaxValue) where T : Module
        {
            var types = DllTypeSearch(typeof(T), path, directoryDepth);
            var containers = new List<IModuleContainer>();

            foreach (var t in types)
            {
                var attribute = t.GetCustomAttribute<ModuleAttribute>();

                if (attribute == null)
                {
                    throw new NotImplementedException(string.Format("{0} Module must be decorated with a ModuleAttribute.", t.Name));
                }

                attribute.Name = string.Format("{0}.{1}", t.Namespace, t.Name);
                attribute.Version = t.GetAssemblyInfo().Version;
                var location = t.GetPathToAssembly(false);
                containers.Add(new ModuleContainer(location, t, attribute, host));
            }

            return containers;
        }


        /// <summary>
        /// Find Events. Direcotry depth can be controlled and is set to maximum depth by default. If you would like to create modules which
        /// are also ModuleHosts themselves, it is a good idea nest modules one directory level deep followed by nesting submodules another
        /// level deep. This will isolate parent module events from submodule events.
        /// </summary>
        /// <param name="path">Where to search for DLL's which export IEvent.</param>
        /// <param name="directoryDepth">How many subdirectories deep of path be searched.</param>
        public static IList<Type> FindEvents<T>(Uri path, int directoryDepth = int.MaxValue) where T : IEvent
        {
            var types = DllTypeSearch(typeof(T), path, directoryDepth);
            var events = new List<Type>();

            foreach (var t in types)
            {
                if (!t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEvent<,>)))
                {
                    throw new NotImplementedException(string.Format("{0} must implement generic interface type of IEvent<IEventInput, IEventOutput>.", t.Name));
                }

                events.Add(t);
            }

            return events;
        }


        private static List<Type> DllTypeSearch(Type @type, Uri path, int directoryDepth = int.MaxValue)
        {
            var startingDepth = Count(Path.DirectorySeparatorChar, path.LocalPath);

            if (!Directory.Exists(path.LocalPath))
            {
                throw new ArgumentException(string.Format("Invalid basePath specified (does not exist): {0}", path.LocalPath));
            }

            var dlls = Directory.GetFiles(path.LocalPath, "*.dll", SearchOption.AllDirectories).ToList();
            dlls.AddRange(Directory.GetFiles(path.LocalPath, "*.exe", SearchOption.AllDirectories));

            dlls = dlls.Where(d => Count(Path.DirectorySeparatorChar, d) <= startingDepth + directoryDepth).ToList();
            var useable = dlls.SelectMany((dll) => GetUseableTypes(new Uri(dll), @type)).ToList();
            return useable;
        }


        private static IList<Type> GetUseableTypes(Uri assemblyPath, Type type)
        {
            var loader = new AssemblyLoader(assemblyPath);
            var assembly = loader.Load();

            var ret = new List<Type>();
            
            foreach (Type t in assembly.GetExportedTypes())
            {
                if ((type == t || type.IsAssignableFrom(t)) && !t.IsAbstract && !t.IsInterface)
                {
                    ret.Add(t);
                }
            }

            loader.Unload();
            return ret;
        }


        private static int Count(char needle, string haystack)
        {
            if (haystack == null)
            {
                return 0;
            }

            return haystack.Count(c => c == needle);
        }
    }
}
