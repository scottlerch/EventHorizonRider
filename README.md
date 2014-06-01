Event Horizon Rider
===================

Multiplatform (mobile first) game inspired by [Super Hexagon](http://www.superhexagon.com) using MonoGame.

http://www.eventhorizonrider.com

Build Requirements
------------------
 * Visual Studio 2013 Update 2 with NuGet 2.8+
 * [XNA 4.0 Refresh (Visual Studio 2013)](https://msxna.codeplex.com/releases/view/117230)
 * [MonoGame Content Processors version 3.2+](http://www.monogame.net/2014/04/07/monogame-3-2/) located at $(MSBuildExtensionsPath)/MonoGame/v3.0/
 * [Xamarin iOS for Windows](http://xamarin.com/ios)
 * [Windows Phone 8.0 SDK](http://dev.windowsphone.com/en-us/downloadsdk)
 * [PlayStation Mobile SDK](https://psm.playstation.net) (still a work-in-progress)

Build Instructions
------------------
Each platform has it's own solution file but all solutions share same portable core game library
and XNA/MonoGame content library.  There is very little platform specific code in each solution. 
Just load up a solution and build.  When switching between platform solutions be sure to do
a full rebuild so platform specific preprocessor directives are taken into consideration.

Development Notes
-----------------
MonoGame libs are compiled from this fork https://github.com/scottlerch/MonoGame

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
