using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

using Modular;

namespace TestModule
{
    public class TestModule : BaseModule
    {
        private bool unloading = false;

        public override string Name
        {
            get { return "TestModule"; }
        }

        public override string Description
        {
            get { return "A test plugin that you can have a conversation with!"; }
        }

        public override string UsageInstructions
        {
            get
            {
                return "Just talk to me..! Use my \"Talk\" method and pass a string to get a reply.";
            }
        }

        public override ModuleMethod[] Methods
        {
            get
            {
                return new ModuleMethod[]
                {
                    new ModuleMethod("Talk", "A test method which accepts strings such as \"hello\" & \"goodbye\""),
                };
            }
        }

        public TestModule()
        {
            Console.WriteLine(this.Name + "'s costructor has been run as soon as the module was imported.");
        }

        public override ModuleResponse RunMethod(string method, object inData, ModuleRunMethodCallback callback)
        {
            Host.QueueAction(() =>
            {
                if (method.Equals(Methods[0].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    string input = inData.ToString();

                    if (input.StartsWith("hello"))
                        callback.Invoke(ModuleResponse.MethodSuccess, "Hi there, how are you? I'm a test module!");
                    else if (input.StartsWith("goodbye"))
                        callback.Invoke(ModuleResponse.MethodSuccess, "Are you going somewhere? see ya then!");
                    else if (input.Contains("how are you"))
                        callback.Invoke(ModuleResponse.MethodSuccess, "I'm good, thanks :)");
                    else
                        callback.Invoke(ModuleResponse.MethodSuccess, "I'm sorry, I don't know what to say!");
                }
            });

            return ModuleResponse.MethodAccepted;
        }
        
        public override ModuleResponse OnLoad()
        {
            Console.WriteLine(Name + "'s Onload method was invoked. It is starting a\r\nthread to do some stuff...\r\nBut you need to wait 30 seconds to see a console write!\r\n");

            Host.QueueAction(() =>
            {
                while (!unloading)
                {
                    Thread.Sleep(30000);
                    Console.WriteLine("Talk to me please, I'm board!!");
                }
            });

            return base.OnLoad();
        }

        public override ModuleResponse OnLoaded()
        {
            Console.WriteLine(Name + "'s OnLoaded method was invoked. This module is ready to be\r\naccessed by other modules using a Host.RunModuleMethod() call!\r\n");
            return base.OnLoad();
        }

        public override ModuleResponse OnBeforeUnload(UnloadDetails unloadDetails)
        {
            Console.WriteLine(Name + " is setting its unloaded property to true, this stops\r\nthe thread which was started in OnLoad() from doing stuff...");
            unloading = true;
            return base.OnBeforeUnload(unloadDetails);
        }

        public override ModuleResponse OnUnload()
        {
            Console.WriteLine(Name + " Stopped doing some stuff...");
            return ModuleResponse.MethodSuccess;
        }
    }
}
