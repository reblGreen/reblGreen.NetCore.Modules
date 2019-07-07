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

namespace reblGreen.NetCore.Modules.ChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ModuleHost host = new BasicModuleHost();
            host.Modules.LoadModules();

            Console.WriteLine("...");
            Console.WriteLine("...");

            Console.WriteLine("Hello World! I'm a chatbot, {0}", Chat.GetOpener());

            Console.WriteLine("...");
            Console.WriteLine("...");

            var testSolidEvent = host.Events.GetSolidEventFromName("reblGreen.NetCore.Modules.ChatBot.ChatModuleEvent");

            while (true)
            {
                var request = Console.ReadLine();
                var e = new ChatModuleEvent();

                e.Input.Request = request;

                if (host.CanHandle(e))
                {
                    host.Handle(e);
                }

                if (e.Handled && e.Output != null)
                {
                    Console.WriteLine(e.Output.Response);
                }
                
                Console.WriteLine("...");
                Console.WriteLine("...");
            }
        }
    }
}
