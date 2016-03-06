Event Horizon Rider
===================

Multiplatform (mobile first) game inspired by [Super Hexagon](http://www.superhexagon.com) using MonoGame.

http://www.eventhorizonrider.com

Build Requirements
------------------
 * Visual Studio 2015 Update 1
 * [MonoGame Content Processors version 3.5-develop](http://teamcity.monogame.net/repository/download/MonoGame_PackagingWindows/4682:id/MonoGameSetup.exe) located at $(MSBuildExtensionsPath)/MonoGame/v3.0/
 * [Xamarin iOS for Windows](http://xamarin.com/ios)
 * [Windows Phone 8.0 SDK](http://dev.windowsphone.com/en-us/downloadsdk)

Build Instructions
------------------
Add the following NuGet feed to get the MonoGame develop branch builds:
http://teamcity.monogame.net/guestAuth/app/nuget/v1/FeedService.svc/

Each platform has it's own solution file but all solutions include the same shared source project
There is very little platform specific code in each solution.   Just load up a solution and build.

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
