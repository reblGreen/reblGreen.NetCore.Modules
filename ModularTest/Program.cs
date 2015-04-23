/*
    The MIT License (MIT)

    Copyright (c) 2015  The Modular Project (https://bitbucket.org/juanshaf/modular)

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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using Modular;

namespace ModularTest
{
    /// <summary>
    /// This is a very simple console application to show how the Modular system works.
    /// 
    /// Basically all you need to do to use Modular is add a reference to the Modular library, then create a new Modular.ModuleHost object
    /// in your application. Reference Modular in a class library project and inherit from Modular.IModule (or Modular.BaseModule which
    /// implements th IModule interface for you) and then add your own code. Drop your new module into the folder you're importing modules from with
    /// the IModuleHost.ImportModules() method and run your project.
    /// 
    /// It's advised to use threading in your constuctor, OnLoad and RunMethod methods and invoke the callback from RunMethod whenever possible to reduce
    /// thread blocking in your application.
    /// 
    /// If designed correctly, your application should become just a skeleton for a module powered heaven :P
    /// </summary>
    class Program
    {
        static string runningLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static ModuleHost host;

        static void Main(string[] args)
        {
            // Create the ModuleHost object.
            host = new ModuleHost(args);

            Console.WriteLine("Machine Details:");
            Console.WriteLine(host.MachineDetails.ToString());

            // Import modules from the desired location and run LoadModules to tell all modules to load
            host.ImportModules(runningLocation, true);
            host.LoadModules();

            if (host.LoadedModules.Count > 0)
            {
                Console.WriteLine("Loaded Modules:");
                foreach (var module in host.LoadedModules)
                {
                    Console.WriteLine(module.Name + " - " + module.Version);
                    Console.WriteLine(module.Description);
                    Console.WriteLine(module.Usage);
                    Console.WriteLine();
                    Console.WriteLine("Module methods:");
                    foreach (var meth in module.Methods)
                    {
                        Console.WriteLine(meth.Name + " - " + meth.Usage);
                    }
                }
                Console.WriteLine();
                Console.WriteLine();

                RunPluginTest();
            }
            else
            {
                Console.WriteLine("No module loaded");
            }

            Console.ReadLine();
        }

        
        static void RunPluginTest()
        {
            Console.WriteLine();
            Console.WriteLine("Say something to the test module: ");
            var s = Console.ReadLine();


            host.RunModuleMethod("TestModule", "Talk", s, new ModuleRunMethodCallback((response, outStr, message) =>
            {
                Console.WriteLine(outStr);
            }));


            System.Threading.Thread.Sleep(1000);
            RunPluginTest();
        }

        
    }
}
