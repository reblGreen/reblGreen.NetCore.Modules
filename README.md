# reblGreen.NetCore.Modules #

This repository has been moved from BitBucket [https://bitbucket.org/reblgreen/reblgreen.netcore.modules/](https://bitbucket.org/reblgreen/reblgreen.netcore.modules/) to GitHub [https://github.com/reblGreen/reblGreen.NetCore.Modules](https://github.com/reblGreen/reblGreen.NetCore.Modules) and as of 2019/09/05 the BitBucket repository is no longer updated or synchronized with the latest commit history. For latest the source code please use the GitHub repository.

In a one-liner, the aim of the reblGreen.NetCore.Modules project is to make developing customizable applications as simple as possible with all the hard work done by the reblGreen.NetCore.Modules architecture.

This project is open source so please feel free to contribute suggestions, make modifications and pull requests back to the repository.
___

### What and why is reblGreen.NetCore.Modules? ###

reblGreen.NetCore.Modules is an [MIT license](https://tldrlegal.com/license/mit-license) .NET Core 2.0 C# Class Library which offers an open-source system for creating [event-driven](https://en.wikipedia.org/wiki/Event-driven_architecture), modular and [plugin-based](https://en.wikipedia.org/wiki/Plug-in_(computing)) applications. 

This project was started as a example project which demonstrated an implementation of [Managed Extensibility Framework (MEF)](https://msdn.microsoft.com/en-us/magazine/ee291628.aspx). A framework included in Microsoft .NET Framework 4.0 and is also available as an external component at [codeplex](https://mef.codeplex.com/) for older versions of .NET Framework.

It was to demonstrate the architecture and benifits of a strict modular design pattern for developing complex applications.

Although the project was originally built with MEF, not many elements of it were are actually used. MEF was used to load the modules and privately import the ModuleHost into each module, instead of rolling our own Reflection based loader. Since porting the project to .NET Core, removing MEF references in favor of Reflection was required. (THIS PROJECT NO LONGER DEPENDS ON MICROSOFT EXTENSIBILITY FRAMEWORK).

The project has since grown into an advanced framework designed to simplify the development of cross-platform applications using a modular or plugin-based design pattern. It is compatible with .NET Core 2.0 and platforms which support .NET Core class libraries. We now use this framework as the groundwork for all client projects.

* reblGreen.NetCore.Modules is a simple, clean and compact .NET Core 2.0 class library for making plugin-based or customizable applications.

The reblGreen.NetCore.Modules project exists due to a closed source plugin based project which is in development at my workplace. I originally wrote the reblGreen.NetCore.Modules library to be used as part of this project but as I wrote reblGreen.NetCore.Modules in my own time, out of work hours and on my own computer, and it is a self contained library, I decided to open-source it for anyone who wishes to use it in their own projects.
___

### How do I get set up? ###

Take a look at the [reblGreen.NetCore.Modules.ChatModule](https://github.com/reblGreen/reblGreen.NetCore.Modules/tree/master/reblGreen.NetCore.Modules.ChatBot) project in the source code. The ChatModule demonstrates the implementation of a Module in the form of a late 1900s style chatbot. This module shows how to handle a [reblGreen.NetCore.Modules.ChatModule.Events.ChatModuleEvent](https://github.com/reblGreen/reblGreen.NetCore.Modules/tree/master/reblGreen.NetCore.Modules.ChatBot.Events) which implements the [IEvent](https://github.com/reblGreen/reblGreen.NetCore.Modules/blob/master/reblGreen.NetCore.Modules/Interfaces/IEvent.cs) interface.

The [reblGreen.NetCore.Modules.ChatModule](https://github.com/reblGreen/reblGreen.NetCore.Modules/tree/master/reblGreen.NetCore.Modules.ChatBot) and [reblGreen.NetCore.Modules.ChatModule.Events](https://github.com/reblGreen/reblGreen.NetCore.Modules/tree/master/reblGreen.NetCore.Modules.ChatBot.Events) class libraries are referneced in the demo project located at [reblGreen.NetCore.Modules.TestApplication](https://github.com/reblGreen/reblGreen.NetCore.Modules/tree/master/reblGreen.NetCore.Modules.TestApplication), which is a console application. If you run this application without any modification, a console window will be displayed in which you can chat with the ChatBot module by entering text and pressing the return key.

The reblGreen.NetCore.Modules.TestApplication contains a [BasicModuleHost](https://github.com/reblGreen/reblGreen.NetCore.Modules/tree/master/reblGreen.NetCore.Modules.TestApplication/Classes) class which inherits from [reblGreen.NetCore.Modules.ModuleHost](https://github.com/reblGreen/reblGreen.NetCore.Modules/blob/master/reblGreen.NetCore.Modules/ModuleHost.cs), which implements the [reblGreen.NetCore.Modules.Interfaces.IModuleHost](https://github.com/reblGreen/reblGreen.NetCore.Modules/blob/master/reblGreen.NetCore.Modules/Interfaces/IModuleHost.cs) interface. This class and implemented interfaces are used for loading modules and invoking the handling of events. There are no other dependency requirements in this project to keep it as simple as possible.

The layout of the projects within the reblGreen.NetCore.Modules directory and solution is to demonstrate the design pattern of keeping modules and corresponding events is seperate class libraries, and then referencing the class libraries from within an application which depends on them.
  
The core project and examples are well documented and if you get stuck or have any questions, please contact me and I'll be glad to help out.

For further documentation please see the [repository wiki](https://github.com/reblGreen/reblGreen.NetCore.Modules/wiki).
___

### Contribution guidelines ###

* Fork [reblGreen.NetCore.Modules](https://github.com/reblGreen/reblGreen.NetCore.Modules), make some changes, make a pull request. Simple!
* Code will be reviewed when a pull request is made.
___

### Who do I talk to? ###

* reblGreen.NetCore.Modules repo owner via message or the [issues board](https://github.com/reblGreen/reblGreen.NetCore.Modules/issues).
* Or contact us via our website at [reblgreen.com](https://reblgreen.com/).
___

### License ###

* [The MIT License (MIT)](https://tldrlegal.com/license/mit-license) - You are free to use reblGreen.NetCore.Modules in commercial projects and modify/redistribute the source code provided the copyright notice is not removed.
* If you use reblGreen.NetCore.Modules in your own project we would love to hear about it, so drop us a line (and even a credit to reblGreen.NetCore.Modules in your project if you feel like being really generous). We would be very happy to hear about your experiences using our reblGreen.NetCore.Modules class library in your projects and any suggestions you may have for us to make it better.