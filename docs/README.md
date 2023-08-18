
# Basic Butt Manager ![Smiling Peach](../resources/favicon24.png)

A basic wrapper around qdot's amazing [haptics library](https://github.com/buttplugio/buttplug-csharp), with the aim of providing a simple interface for integrating adult toy controls into Unity games / .NET applications (e.g. through mods). Not suitable for complex projects!

## Usage

To set all connected toys to half intensity:

1. Launch Intiface :copyright: Central (with default server settings).
2. Add this .dll to your project's references.
3. Somewhere in your code...

```
BasicButtManager manager = new BasicButtManager("Love Rhythm");
manager.Set(0.5f);
manager.Exit();
```

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