# reblGreen.NetCore.Modules # 

An MIT license .NET Core 2.0 C# Class Library which offers an open-source system for creating event driven modular (plugin based) applications. reblGreen.NetCore.Modules was started as a sample project I put together which demonstrated an implementation of Microsoft .NET [Managed Extensibility Framework (MEF)](https://msdn.microsoft.com/en-us/magazine/ee291628.aspx) which is included in .NET Framework since version 4.0 but is also available as an external component at [codeplex](https://mef.codeplex.com/). This project was to demonstrate the benifits of a strict modular design pattern for complex applications to my peers 

Although the project was built using MEF, not many elements of MEF were are actually used in this project. In fact MEF was mostly just used to load the modules and import the ModuleHost into each module, instead of rolling our own Reflection based loader. Since porting the project over to .NET Core removing MEF in favor of Reflection was required. It has since grown into an advanced framework designed to simplify building cross-platform plugin-based applications using C# and platforms which support .NET Core class libraries. We use this framework as the groundwork for many client projects.

The aim of reblGreen.NetCore.Modules project is to make building plugin based applications as simple as possible with all the hard work done by the reblGreen.NetCore.Modules library.
___

### What and why is reblGreen.NetCore.Modules? ###

* reblGreen.NetCore.Modules is a simple, clean and compact .NET Core 2.0 class library for making plugin based applications.

The reblGreen.NetCore.Modules project exists due to a closed source plugin based project which is in development at my workplace. I originally wrote the reblGreen.NetCore.Modules library to be used as part of this project but as I wrote reblGreen.NetCore.Modules in my own time, out of work hours and on my own computer, and it is a self contained library, I decided to open-source it for anyone who wishes to use it in their own projects.
___

### How do I get set up? ###

Check out the reblGreen.NetCore.Modules.ChatModule project in the source code. This project shows the basic implementation of a Module in the form of a 1980s style chatbot. There is also a basic ModuleHost for loading modules and handling events. There are no other dependency requirements in this project to keep it simple. When using reblGreen.NetCore.Modules in production I prefer to keep modules and corresponding events is seperate class libraries, but in this case for simplicity the ChatModule module, its corresponding ChatModuleEvent and a BasicModuleHost are contained in a single project.
  
The project and example are well documented and should be self explanatory but if you get stuck or have any questions, please contact me and I'll be glad to help out.
___

### Contribution guidelines ###

* Fork reblGreen.NetCore.Modules, make some changes, make a pull request. Simple!
* Code will be reviewed when a pull request is made.
___

### Who do I talk to? ###

* reblGreen.NetCore.Modules repo owner.
___

### License ###

* The MIT License (MIT) - Do with it what you will.
* If you do use reblGreen.NetCore.Modules in your own project I would love to hear about it so drop me a line (and even a credit to reblGreen.NetCore.Modules in your project if you feel like being really nice).