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
using reblGreen.NetCore.Modules.ChatBot.Events;
using reblGreen.NetCore.Modules.Interfaces;

namespace reblGreen.NetCore.Modules.TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new module host
            ModuleHost host = new BasicModuleHost();

            // Invoking ModuleHost.Modules.GetModuleNames method tells us which modules have been imported. We
            // are calling this method for information only. Modules have not been loaded yet.
            var names = host.Modules.GetModuleNames();

            // Now we load modules. Modules must be loaded otherwise they can not handle events.
            host.Modules.LoadModules();

            // Importing module happens by default when the default ModuleHost is initialized but you can call
            // ImportModules any time and any newly added modules will be loaded.
            host.Modules.ImportModules();

            var modulesList = host.Modules.GetModulesByType<IModule>();

            // Writing console lines here has nothing to do with the functionality of reblGreen.NetCore.Modules
            Console.WriteLine("...");
            Console.WriteLine("...");

            Console.WriteLine("Hello World! I'm a chatbot, {0}", Chat.GetOpener());

            Console.WriteLine("...");
            Console.WriteLine("...");

            // Purely for testing to ensure we have an event.
            var testSolidEvent = host.Events.GetSolidEventFromName("reblGreen.NetCore.Modules.ChatBot.ChatModuleEvent");
            
            if (testSolidEvent == null)
            {
                throw new Exception("We have no chat event loaded.");
            }

            while (true)
            {
                // Waiting for user input...
                var request = Console.ReadLine();
                var e = new ChatModuleEvent();

                // We created a new chat event and added the input text to the IEvent.Input object.
                e.Input = new ChatModuleEventInput()
                {
                    Request = request
                };

                // We don't really need to call CanHandle but currently used good for debugging.
                if (host.CanHandle(e))
                {
                    // Here's the magic...
                    host.Handle(e);
                }
                else
                {
                    throw new Exception("Unable to handle the event.");
                }

                // Testing to see if we can get the input and output from the IEvent object... 
                var input = e.GetEventInput();
                var output = e.GetEventOutput();
                e.SetEventOutput(output);

                // Was our event hndled in the call to host.method Handle above?
                if (e.Handled /*&& e.Output != null*/)
                {
                    // Yes, so write out the response to the console...
                    Console.WriteLine(e.Output.Response);
                }
                
                // More random and not required console writing.
                Console.WriteLine("...");
                Console.WriteLine("...");
            }
        }
    }
}
