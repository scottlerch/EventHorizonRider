Event Horizon Rider
===================

Multiplatform game inspired by Hexagon using MonoGame.

Build Requirements
------------------
 * Visual Studio 2013 with NuGet 2.8+
 * [XNA 4.0 Refresh (Visual Studio 2013)](https://msxna.codeplex.com/releases/view/117230)
 * [MonoGame Content Processors version 3.2+](http://www.monogame.net/2014/04/07/monogame-3-2/) located at $(MSBuildExtensionsPath)/MonoGame/v3.0/
 * [Xamarin iOS for Windows](http://xamarin.com/ios)
 * [Windows Phone 8.0 SDK](http://dev.windowsphone.com/en-us/downloadsdk)

Build Instructions
------------------
Each platform has it's own solution file but all solutions share same portable core game library
and XNA/MonoGame content library.  There is very little platform specific code in each solution. 
Just load up a solution and build.  When switching between platform solutions be sure to do
a full rebuild so platform specific preprocessor directives are taken into consideration.