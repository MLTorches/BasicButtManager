
# Basic Butt Manager ![Smiling Peach](../resources/favicon24.png)

A basic wrapper around qdot's amazing [haptics library](https://github.com/buttplugio/buttplug-csharp), with the aim of providing a simple interface for integrating adult toy controls into Unity games / .NET applications (e.g. through mods). Not suitable for complex projects!

## Usage

To set all connected toys to half intensity:

1. Launch [Intiface :copyright: Central](https://intiface.com/central/) (with default server settings).
2. Add this [.dll](https://github.com/MLTorches/BasicButtManager/releases/tag/v1.0.3) to your project's references (or install from [NuGet](https://www.nuget.org/packages/BasicButtManager/)).
3. Somewhere in your code...

```
BasicButtManager manager = new BasicButtManager("Love Rhythm");
manager.Set(0.5f);
manager.Exit();
```

## WARNING

If you are MODDING an existing project, please use [ButtServer](https://github.com/MLTorches/ButtServer) + [ButtClient](https://www.nuget.org/packages/ButtClient) instead.

### Explanation:

***This is a lower-level, framework-dependent (.NET 4) package meant for developers working on their own projects where they can control the underlying framework.*** For example, there are some compatibility issues between certain packages used by (the dependencies of) the underlying Buttplug package and some versions of the .NET framework.

This concern is taken care off under the hood automatically by the ***Butt Server***, which is a standalone executable tested and known to work with [Buttplug](https://github.com/buttplugio/buttplug-csharp). The ***Butt Client*** (which you as a modder will hook into the existing game/application) then uses bare minimum socket logic to communicate with ButtServer, independent of any potential framework-dependent libraries.

### Communication Path

The typical communication path goes like this:

```text
Existing Unity Game --> BepinEx + Harmony --> ButtClient --> ButtServer --> BasicButtManager --> Intiface Central --> Toys
      (C# hook)                                 (.dll)         (.exe)            (.dll)               (.exe)    
```           

So once again, DO NOT expect to just plug this .dll into an existing Unity game on Steam and just expect it to work!
It works just wonderfully fine though if you are creating your own .NET 4 project from scratch. Have fun modding! <3

## Wiki
Click [here](https://mltorches.github.io/BasicButtManager/api/BasicButtManager.BasicButtManager.html) to view the documentation!

## Credits
Buttplug: [qdot@github.com](https://github.com/qdot) | Favicon: [freepik@flaticon.com](https://www.flaticon.com/authors/frdmn) <br/>

## License

This project is BSD 3-Clause licensed.

```text
Copyright (c) 2023, MLTorches
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```