# Modular #

A MIT licensed .NET 4.0 C# Class Library which offers an open-source system for creating modular (plugin based) applications. Modular is built using the [Managed Extensibility Framework (MEF)](https://msdn.microsoft.com/en-us/magazine/ee291628.aspx) which is included in .NET Framework since version 4.0 but is also available as an external component at [codeplex](https://mef.codeplex.com/).

Although the Modular project is built using MEF, currently not many elements of MEF are actually used in this project. In fact MEF is mostly just used to load the modules instead of rolling our own Reflection based loader, but this minimal use of MEF is certainly subject to change through future commitment if time sees the project right.

The aim of Modular project is to make building plugin based applications as simple as possible with all the hard work done by the Modular library.
___

### What and why is Modular? ###

* Modular is a simple, clean and compact .NET class library for making plugin based applications.

The Modular project exists due to a closed source plugin based project which is in development at my workplace. I originally wrote the Modular library to be used as part of this project but as I wrote Modular in my own time, out of work hours and on my own computer, and it is a self contained library, I decided to open-source it for anyone who wishes to use it in their own projects.
___

### How do I get set up? ###

Check the other projects in the source code for an example module and a test project which loads modules. It's all quite easy to use and should be pretty self explanatory but the basics of how it works is below.
  
1. Compile and reference (or include) the source, Add a using Modular, Create a Modular.ModuleHost object somewhere in your project and tell it where to load modules from.
2. Create a new project (generally a Class Library dll) which references Modular, Inherit from Modular.BaseModule abstract class and override its properties and methods with your own functionality.
3. Drop the compiled project binary to the location where the other project is looking for modules to load then run the first project. You'll see that the module binary has been loaded by the first project.
4. Make plugins communicate with each other through the Modular.ModuleHost object you created in the first project.
5. If you need any help, drop me a line and I'll do my best.

####Project 1
```javascript
// Create the ModuleHost object.
string runningLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
ModuleHost host = new ModuleHost(args);

// Import modules from the desired location and run LoadModules to tell all modules to load.
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
}

host.RunModuleMethod("ModuleName", "MethodName", Input_Object, new ModuleRunMethodCallback((response, Output_Object, message) =>
{
    Console.WriteLine(Output_Object.ToString());
}));
```

####Project 2
```javascript
using Modular;
public class Module : BaseModule
{
    public override string Name { get { return "ModuleName"; } }
    public override string Description { get { return "A description"; } }
    public override string UsageInstructions { get { return "Some usage instructions"; } }

    public override ModuleMethod[] Methods
    {
        get
        {
            return new ModuleMethod[]
            {
                new ModuleMethod("MethodName", "Method usage instructions"),
            };
        }
    }

    public override ModuleResponse RunMethod(string method, object inData, ModuleRunMethodCallback callback)
    {
        if (method.Equals("methodname", StringComparison.InvariantCultureIgnoreCase))
        {
            Host.QueueAction(() =>
            {
                callback.Invoke(ModuleResponse.MethodSuccess, Output_Object, "Optional string");
            });

            return ModuleResponse.MethodAccepted;
        }
        
        return ModuleResponse.MethodNotFound;
    }
```
___

### Contribution guidelines ###

* Fork Modular, make some changes, make a pull request. Simple!
* Code will be reviewed when a pull request is made.
___

### Who do I talk to? ###

* Modular repo owner.
___

### License ###

* The MIT License (MIT) - Do with it what you will.
* If you do use Modular in your own project I would love to hear about it so drop me a line (and even a credit to Modular in your project if you feel like being really nice).