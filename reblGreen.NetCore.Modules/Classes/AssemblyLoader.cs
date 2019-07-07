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
using System.Runtime.Loader;
using System.Collections.Generic;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.Classes
{
    [Serializable]
    public class AssemblyLoader : AssemblyLoadContext
    {
        string AssemblyPath;
        Assembly LoadedAssembly;
        List<Assembly> ReferencedAssemblies;

        public AssemblyLoader(Uri path)
        {
            if (path == null || !path.IsFile)
            {
                throw new Exception("AssemblyContainer path parameter must be a System.Uri which targets a local file.");
            }

            AssemblyPath = path.LocalPath;
            Resolving += ModuleLoader_Resolving;
            Unloading += AssemblyLoader_Unloading;
        }

        public IEnumerable<Type> GetExportedTypes()
        {
            if (LoadedAssembly != null)
            {
                return LoadedAssembly.GetExportedTypes();
            }

            return null;
        }

        private void AssemblyLoader_Unloading(AssemblyLoadContext obj)
        {
            throw new NotImplementedException();
        }


        private Assembly ModuleLoader_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            return Load(arg2);
        }


        protected override Assembly Load(AssemblyName assemblyName)
        {
            return Load(assemblyName.Name);
        }


        protected Assembly Load(string assemblyName)
        {
            if (LoadedAssembly != null && LoadedAssembly.Location == assemblyName)
            {
                return LoadedAssembly;
            }

            Assembly assembly = null;
            var current = AppDomain.CurrentDomain.GetAssemblies();
            var loaded = current.Where(x => x.Location == assemblyName);

            if (loaded.Count() > 0)
            {
                assembly = loaded.First();
            }
            else if (ReferencedAssemblies != null)
            {
                loaded = ReferencedAssemblies.Where(x => x.Location == assemblyName);
                if (loaded.Count() > 0)
                {
                    assembly = loaded.First();
                }
            }

            if (assembly != null)
            {
                if (LoadedAssembly == null)
                {
                    LoadedAssembly = assembly;
                }

                return assembly;
            }

            string assemblyPath = GetPossibleReferencePath(assemblyName);
            if (assemblyPath != null)
            {
                assembly = LoadFromAssemblyPath(assemblyPath);
            }

            if (assembly != null)
            {
                if (LoadedAssembly == null && assembly.Location == AssemblyPath)
                {
                    LoadedAssembly = assembly;
                }
                else
                {
                    if (ReferencedAssemblies == null)
                    {
                        ReferencedAssemblies = new List<Assembly>();
                    }

                    ReferencedAssemblies.Add(assembly);
                }

                return assembly;
            }

            return null;
        }


        public Assembly Load()
        {
            return Load(AssemblyPath);
        }


        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = GetPossibleReferencePath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }


        public void Unload()
        {
            if (LoadedAssembly !=  null)
            {
                LoadedAssembly = null;
            }

            if (ReferencedAssemblies != null)
            {
                ReferencedAssemblies = null;
            }
        }


        string GetPossibleReferencePath(string assemblyName)
        {
            if (assemblyName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) || assemblyName.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
            {
                assemblyName = assemblyName.Substring(0, assemblyName.Length - 4);
            }

            var dir = Path.GetDirectoryName(AssemblyPath);
            var relative = Path.Combine(dir, assemblyName);
            string parent;

            try
            {
                parent = Directory.GetParent(dir).ToString();
                parent = Path.Combine(parent, assemblyName);
            }
            catch
            {
                parent = assemblyName;
            }

            var fileNames = new string[]
            {
                relative + ".dll",
                relative + ".exe",
                parent + ".dll",
                parent + ".exe"
            };

            foreach (var f in fileNames)
            {
                if (File.Exists(f))
                {
                    return f;
                }
            }

            return null;
        }


        public Module InstantiateModule(Type module)
        {
            var type = typeof(Module);

            if (LoadedAssembly == null)
            {
                LoadedAssembly = Load();
            }

            foreach (Type t in LoadedAssembly.GetTypes())
            {
                if ((type == t || type.IsAssignableFrom(t)) && !t.IsAbstract && !t.IsInterface)
                {
                    if (t == module)
                    {
                        var result = Activator.CreateInstance(t) as Module;
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }


        public static IEvent InstantiateEvent(Type t)
        {
            try
            {
                if (Activator.CreateInstance(t) is IEvent result)
                {
                    if (!t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEvent<,>)))
                    {
                        throw new Exception("A type implementing IEvent interface must use generic arguments IEvent<IEventInput, IEventOutput> to identify the event input and output types.");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("A type implementing IEvent<IEventInput, IEventOutput> interface must contain a public parameterless constructor.", ex);
            }

            return null;
        }
    }
}
