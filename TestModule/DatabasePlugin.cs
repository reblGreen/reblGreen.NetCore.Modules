using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using MSW.Modules;

namespace TestPlugin
{
    public class DatabasePlugin : BaseModule
    {
        public DatabasePlugin()
        {
            Console.WriteLine("DatabasePlugin constructor");
        }

        public override ModuleResponse RunMethod(string method, string data, ModuleRunMethodCallback callback)
        {
            MethodNames methodName = (MethodNames)Enum.Parse(typeof(MethodNames), method, true);

            switch (methodName)
            {
                case MethodNames.FindDocument:
                    return FindDocument(data, callback);
                    
                case MethodNames.LoadDocument:
                    return LoadDocument(data, callback);

                case MethodNames.SaveDocument:
                    return SaveDocument(data, callback);

                default:
                    return ModuleResponse.REJECTED;
            }
        }


        ModuleResponse FindDocument(string data, ModuleRunMethodCallback callback)
        {
            new Thread(() =>
            {
                callback.Invoke(ModuleResponse.OK, "blah! And here's the document...", "some message if needed");
            }).Start();

            return ModuleResponse.ACCEPTED;
        }

        ModuleResponse LoadDocument(string data, ModuleRunMethodCallback callback)
        {
            new Thread(() =>
            {
                callback.Invoke(ModuleResponse.OK, "blah! And here's the document...", "some message if needed");
            }).Start();

            return ModuleResponse.ACCEPTED;
        }

        ModuleResponse SaveDocument(string data, ModuleRunMethodCallback callback)
        {
            new Thread(() =>
            {
                callback.Invoke(ModuleResponse.OK, "blah! And here's the saveddocument...", "some message if needed");
            }).Start();

            return ModuleResponse.ACCEPTED;
        }
    }
}
