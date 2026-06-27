Event Horizon Rider
===================

Multiplatform (mobile first) game inspired by [Super Hexagon](http://www.superhexagon.com) using MonoGame.

[![Event Horizon Rider](https://cloud.githubusercontent.com/assets/1789883/26028224/03049e54-37d1-11e7-8d54-569d0d378b68.gif)](http://www.eventhorizonrider.com)

http://www.eventhorizonrider.com

Build Requirements
------------------
 * Visual Studio 2022 17.14+ (or the .NET 10 SDK) with the iOS, Android, and Windows workloads installed

Build Instructions
------------------
Restore the local content-pipeline tool once after cloning (otherwise the MonoGame content build fails):

    dotnet tool restore

Then open the EventHorizonRider solution, select the desired platform project as *Set as StartUp Project*, select
the appropriate build configuration and platform, and hit F5.

You can also build from the command line (iOS requires a Mac):

    dotnet build EventHorizonRider.Windows/EventHorizonRider.Windows.csproj
    dotnet run --project EventHorizonRider.Windows

Development Notes
-----------------
The following software was used to help develop the content:

 * [Audacity](http://audacity.sourceforge.net/)
 * [Paint.NET](http://www.getpaint.net/)
 * [Gimp](http://www.gimp.org/)

The following sites were used to obtain royalty free content:

 * http://www.soundjay.com/
 * http://teknoaxe.com/
 * https://www.freesound.org

Video captures were made using the following:

 * [WM Capture 7](http://wmrecorder.com/products/wm-capture/)
