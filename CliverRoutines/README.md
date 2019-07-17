# CliverRoutines

## Overview
A cross-platform C# lib that exposes generally used routines: 
- serializing which replaces standard .NET settings for desktop applications; 
- logging with threading and session support;
- auxiliary routines;

## Framework
.NET Standard 2.0

## Supported platforms
It is supposed to run anywhere with .NET Standard lib supported. 
Presumably it will run on any Xamarin platforms (probably with minor updates required). 

The most concern is peculiarities of the target file system because serializing and logging routines write/read files and do everything as automatically as possible.

### Tested on:
- Windows 7, 10 in C# projects of any configuration built in Visual Studio;
- macOS High Sierra 10.12 in Xamarin.Mac projects built in Visual Studio for Mac;


## Serializing 
Classes: Cliver.Config, Cliver.Settings 

### Description
It is more powerful and flexible than the built-in .NET settings for desktop.

Features:
- saving to disk and restoring from disk of values of class members that need it;
- serialized types are flexibly defined in the application;
- serialized types can inherit from another serialized types which may be abstract;

### How to use:
Define classes that is to be serialized and make them a subclass of Cliver.Settings class. Anywhere declare public class fields of these types. Add the following calls in the beginning of the app: 

(optionally) Cliver.Config.Initialize(); 

(mandatory) Cliver.Config.Reload();

That's all. Now the fields will be set to the previously serialized values if any, otherwise keep the values they are initialized with.

To serialize current value of a field, call Save() on it.

Review my C# projects in github to see live examples.


## Logging 
Classes: Cliver.Log

### Description
Writting logs on disk.

Features:
- thread-safe;
- (option) writting log per thread;
- (option) writting logs in sessions that an app can open and close many times during its work;
- (option) automatic old log cleanup; 

### How to use:
Add the following calls in the beginning of the app: 

(optionally) Cliver.Log.Initialize();

To write to log call either Cliver.Log.Write() or Cliver.Log.Main.Write() or more specific methods.

Review my C# projects in github to see live examples.

## Auxiliary routines 
### Description
Anything handy that is needed in general development.

### How to use:
Usually it is clear from their code. 

Review my C# projects in github to see live examples.
