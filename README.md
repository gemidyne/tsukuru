![Tsukuru](https://raw.githubusercontent.com/stsvtf-productions/tsukuru/master/applogo.png)

Augment your Source Engine development workflow with Tsukuru, a handy Windows tool which provides useful features for SourceMod scripting and Source Engine map compilation.
![Screenshot](https://user-images.githubusercontent.com/3583562/80633531-d4c63e00-8a50-11ea-8155-6417517d0a04.png)

## Features

### Source Engine Map Compiler
* Easy to use UI for VBSP, VVIS and VRAD settings 
* Automatic map versioning support with choice of versioning format - Version with date (yyyyMMdd) or incrementing build number
* Supports patched VRAD for improved multi-core performance
* BSPZIP Resource Packing UI with optional intelligent packing mode - allows for only used files to be packed into a BSP
* Automatic file/asset templating - allowing for map specific files to be generated and contents to be replaced via the use of tokens
* BSP Repack support - to further compress data within a BSP 
* Shareable compiler settings support - allows you to share your map compile settings with your team
* Realtime output from compiler processes, organised into its own process tab so you can view logs easier
* Integrated game launcher support - once your map compiles you can load the map into the game engine directly from Tsukuru

### SourceMod Plugin Compiler
* Easy to use UI for batch plugin compiles
* Ability to execute a CMD file after a build completes for automation
* Automatic build / version number management 
* Copy plugins to clipboard on successful compile

### Translations 
* Import your SourceMod translations into a single project file - this allows for easier maintainability
* Export from a translation project into SourceMod translation files - this allows for you to export translations back into SourceMod from a translation project file. An example implementation of this is the Gemidyne Contributor Portal which allows contributors to edit translations.

## Download
The latest release can be downloaded from here: https://github.com/gemidyne/tsukuru/releases/latest. Click on the Tsukuru zip file to start the download. Once downloaded, double click Tsukuru.exe.

## License
Licensed under the [GNU GPL License](LICENSE.md)

## Having issues with your VPROJECT environment variable?
Please ensure that a User environment variable is set with the name "VPROJECT". Tsukuru expects a User environment variable, instead of a System environment variable.

The VPROJECT user env variable must be set to the game directory. 

For example: G:\SteamLibrary\steamapps\common\Team Fortress 2\tf
