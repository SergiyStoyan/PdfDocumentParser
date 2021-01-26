# CliverRoutines

A cross-platform C# lib which provides frequently used routines: 
- application settings engine which replaces the Visual Studio application settings routine; 
- logging with multi-threading and session support;
- auxiliary routines;

It is developed on .NET Standard 2.0 and supposed to run anywhere. 

Tested on:
- Windows 7, 10 in C# projects of any configuration built in Visual Studio;
- macOS High Sierra 10.12 in Xamarin.Mac projects built in Visual Studio for Mac;

## Application settings engine
It is easy to use aplication settings engine which is much more powerful and flexible than the Visual Studio's one.

Features:
- automatically serialazing/deserialazing values of class members which need it;
- serializable types are tailored in the application according to needs;

## Logger 
Features:
- thread-safe;
- writting log per thread;
- simultaneous multiple log sessions;
- automatic old log cleanup; 

## Auxiliary routines 
Anything handy that is needed in general development.


## [More details...](https://sergeystoyan.github.io/CliverRoutines/#1)

Also review my C# projects in github to see live examples.
