# TinyFmod
A .NET6 library that acts as a tiny wrapper around the FMOD C# Wrapper and exposes friendly methods for using the FMOD Studio API. 

**Note that not all FMOD Studio API interfaces are wrapped and are still avaialble through this library by using the `FMOD` namespace**

## How to Install
As of this moment, I do not have a build dll to provide or a nuget package.  You will need to clone this repository and add the `TinyFmod.csproj` as a refernece to your project.

As part of the FMOD user agreement, I am unable to distribute the reference FMOD dll/dylibs that are needed to use this.  You will need to download them yourself by visiting https://www.fmod.com/download. To download the appropriate files, choose the download option under **FMOD STudio** and download the one for your operating system.

Once downlaoded, within the downloaded files you will find the following directories
*   api/core/lib
*   api/studio/lib

within these folders you will find the libfmod, libfmodL, libfmodstudio, and libfmodstudio DLLs or DYLIB files (depending on your platform).  Copy these files to the root directory of your project.  If you are using Visual Studio, you can set the build action of these files to "Copy" so they are copied to your output directory on build.  If you are using Visual Studio Code or similar editor where you need to edit the .csproj file manually, you can add the following into your .csproj file to have the files copied on build

```xml
<ItemGroup>
    <None Include="libfmod.dll" CopyToOutputDirectory="PreserveNewest"/>
    <None Include="libfmodL.dll" CopyToOutputDirectory="PreserveNewest"/>
    <None Include="libfmodstudio.dll" CopyToOutputDirectory="PreserveNewest"/>
    <None Include="libfmodstudioL.dll" CopyToOutputDirectory="PreserveNewest"/>
</ItemGroup>
```
*Note: replace **.dll** with **.dylib** if the files you have based on your operating system are **.dylib***

## Usage

### Create a new instance
To create a new instance of `TinyFmod.FmodStudio`, call the constructor with no paramters, or alternativly you can pass a `true` value to indicate that Live Update should be enabled to you can connect with Live Update in the FMOD Studio application.

```cs
//  Add namspace
using TinyFmod;

//  Create instance somewhere in your code
FmodStudio studio = new();

//  Alternativly you can pass true to enable live update so you can use the live update feature within the FMOD Studio application.
FmodStudio studio = new(true);
```

### Load Banks
Once you have created an instance of `TinyFmod.FmodStudio`, you need to load the bank files.  Ensure that you also load the **Master.strings.bank** file to ensure that event, vca, bus, etc paths can be used correctly.

```cs
//  Load the master bank first
studio.LoadBank("path/to/Master.bank");

//  Load the master strings bank so that event, vca, bus, etc paths work correctly
studio.LoadBank("path/to/Master.strings.bank")

//  Load the rest of your banks
studio.LoadBank("path/to/myBank.bank");
```

The `LoadBank` method can also be given a string value that represents a key as a second paramter which will indicate that the `FmodStudio` instance should internally cache the bank which can be retreived using `FmodStudio.TryGetCachedBank(string key, out FMOD.Studio.Bank bank)` method.

### Play a Sound Effect or Music
While what one might consider a "sound effect" vs "music" are both considered an **EventInstance** in the FMOD Studio API, TinyFmod differentiates the concepts.

### Playing Sound Effects
When caling the `TinyFmod.FmodStudio.PlaySoundEffect` methods, the `Studio::EventInstance` that is created is started and then immediatly released.  This in line with the FMOD Documentation since Sound Effects should be a one shot play and not looped. (https://www.fmod.com/docs/2.02/api/studio-api-eventinstance.html#studio_eventinstance_start)

```cs
//  Play a sound effect. You have to pass in the event string for the event.
studio.PlaySoundEffect("event:/mySoundEffect");
```

Additionally when playing sound effects, there are multiple method overloads that allow you to set a single paramter or multiple paramters before playback is started, as well as setting the values for 3D attributes if the `Studio::EventDescription` for the instance is considered a 3D event.

```cs
//  Setting a single paramter
studio.PlaySoundEffect("event:/mySoundEffect", "myParamter", 1.0f);

//  Setting multiple paramters
Dictionary<string, float> myParameters = new Dictionary<string, float>();
myParameters.Add("paramter1", 1.0f);
myParameters.Add("parameter2", 2.0f);
studio.PlaySoundEffect("event:/mySoundEffect", myParameters);

//  Setting 3D Attributes
studio.PlaySoundEffect("event:/mySoundEffect", x: 1.0f, y: 1.0f, originX: 0.0f, originY: 0.0f);

//  Setting a single paramter and 3D attributs
studio.PlaySoundEffect("event:/mySoundEffect", parameter: "myParameter", value: 1.0f, x: 1.0f, y: 1.0f, originX: 0.0f, originY: 0.0f);

//  Setting multiple parameters and 3D attributes
Dictionary<string, float> myParameters = new Dictionary<string, float>();
myParameters.Add("paramter1", 1.0f);
myParameters.Add("parameter2", 2.0f);
studio.PlaySoundEffect("event:/mySoundEffect", parameters: myParameters, value: 1.0f, x: 1.0f, y: 1.0f, originX: 0.0f, originY: 0.0f);
```
### Playing Music
There is only one method provided to play music; the `TinyFmod.FmodStudio.PlayMusic(string eventPath, bool start = true, bool fadeCurrent = true)` method.

At minimim, you only need to supply the event path to the music event to play it.  Alternativly you can provided values to indicate if the music should start immediatly and if the current music playing should fade out (if not, then the current music is immediatly stopped).

```cs
//  Play the music, start it immediatly and fade out the current music playing (if any)
studio.PlayMusic("event:/myMusic", start: true, fadeCurrent: true);
```

## Additional Utility Methods
There are additionaly utility methods provided in the `TinyFmod.FmodStudio` class, all of which are well documented within the code and provide detailed intellesense when using them.  I will provided documentation on them at a future time, for now, please refer to the source code.

## License
```
MIT License

Copyright (c) 2022 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

