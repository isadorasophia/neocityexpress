<p align="center">
<img width="450" src=".github/images/murder_logo.png" alt="Murder logo">
</p>

<h1 align="center">A game made on Murder Engine</h1>

<p align="center">
<img width="800" src=".github/images/screenshot1.png" alt="Screenshot of Neo City Express">
</p>

This is the source repository for the "Neo City Express", an entry for Ludum Dare 53.

### How can I play it?
We have an [itch.io page](https://saint11.itch.io/neo-city-express) where you can download the game. 

It currently runs on Windows, Linux and macOS.

### How to build it?
I manually copied the binaries built with debug symbols in a horrible way. This is because a real release will imply nuget package management and I just can't afford to do this now and I still want to do plenty of tweaks and improvements on my engine before actually releasing it properly. 

BUT! You know what? The project builds! If you want to build the **editor**:
```
cd src/LDGame.Editor
dotnet run
```
or open `LDGame.sln` on Visual Studio 2022, set `LDGame.Editor` as startup project and hit F5. 🎉

If you want to build the **game**, run:
```
cd src/LDGame
dotnet run
```
or set `LDGame` as startup project on the steps above.

The sounds require fmod, which I am not really allowed to ship here. If you want to get sounds working, run the editor and check the log error for instructions. You can get the fmod binaries [here](https://www.fmod.com/download), under "Fmod Engine". You just manually add the assemblies on all the architectures you want to target (macOS, Windows, Linux).

### Other tools!
The dialogue itself is written in [gum](https://github.com/isadorasophia/gum), a narrative language designed to integrate with the engine. 

All the logic is around an ECS supported by [bang](https://github.com/isadorasophia/bang), a C# ECS framework. 

The rendenring and graphics of the engine are pulled from Monogame, although we don't rely on any of the MonoGame Content Builder because it's not fast enough (we need to hot reload everything!).

Anyway, I hope this code helps on any references around ECS or on the engine. Feel free to reach out on any questions!

### Team
_Pedro Medeiros (@saint11)_ \
_Isadora Rodopoulos (me)_ \
_Davey Wreden (@HelloCakebread)_ \
_Ryan Roth (@DualRyan)_ \
_Voice Acting by Jack Hicks_

<p align="center">
<img width="800" src=".github/images/game_logo.png" alt="Screenshot of Neo City Express"><br>
</p>
