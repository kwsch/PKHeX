# PKHeX.Web third-party notices

PKHeX, including PKHeX.Web and PKHeX.Core, is distributed under the GNU General Public License version 3: see `LICENSE.txt` next to this file in a published site, or `LICENSE` in the source repository.

The build label at the bottom of the page shows the git commit the site was built from. The source for published releases is at https://github.com/kwsch/PKHeX; build instructions are in `PKHeX.Web/README.md` there. A local build can include uncommitted changes that the commit does not reflect, and a build made without git history shows `unknown`.

A published site delivers the components below to the browser. Each package keeps its own license and notices. The upstream `THIRD-PARTY-NOTICES` files are published unchanged under `licenses/`; the table names the one that covers each package.

## Published in the browser bundle

Packages with at least one file in the Release publish.

| Component | Version | License | Upstream notices |
|---|---|---|---|
| PKHeX.Core (this repository) | same commit | GPL-3.0-or-later, as declared by PKHeX.Core | — |
| Microsoft.NETCore.App.Runtime.Mono.browser-wasm (.NET runtime, base class libraries, `dotnet.*.js`, `dotnet.native.wasm`, ICU data) | 10.0.12 | MIT | `licenses/dotnet-runtime.txt` |
| Microsoft.AspNetCore.App.Internal.Assets (carries an identical copy of `blazor.webassembly.js`) | 10.0.12 | MIT | `licenses/aspnetcore.txt` |
| Microsoft.AspNetCore.Components | 10.0.12 | MIT | `licenses/aspnetcore-components.txt` |
| Microsoft.AspNetCore.Components.Web | 10.0.12 | MIT | `licenses/aspnetcore-components.txt` |
| Microsoft.AspNetCore.Components.WebAssembly (including `blazor.webassembly.js`) | 10.0.12 | MIT | `licenses/aspnetcore-components.txt` |
| Microsoft.Extensions.Configuration | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.Configuration.Json | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.DependencyInjection | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.Logging | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.Logging.Abstractions | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.Options | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.Extensions.Primitives | 10.0.12 | MIT | `licenses/dotnet-extensions.txt` |
| Microsoft.JSInterop | 10.0.12 | MIT | `licenses/aspnetcore.txt` |
| Microsoft.JSInterop.WebAssembly | 10.0.12 | MIT | `licenses/aspnetcore-components.txt` |

## Restored for the browser but removed by trimming

These packages are part of the browser build, but trimming removes all of their code, so no file of theirs is published. A package moves to the table above, with its upstream notices, once the app uses it.

| Component | Version | License | Upstream notices |
|---|---|---|---|
| Microsoft.AspNetCore.Authorization | 10.0.12 | MIT | — |
| Microsoft.AspNetCore.Components.Forms | 10.0.12 | MIT | — |
| Microsoft.AspNetCore.Metadata | 10.0.12 | MIT | — |
| Microsoft.Extensions.Configuration.Binder | 10.0.12 | MIT | — |
| Microsoft.Extensions.Configuration.FileExtensions | 10.0.12 | MIT | — |
| Microsoft.Extensions.Diagnostics | 10.0.12 | MIT | — |
| Microsoft.Extensions.Diagnostics.Abstractions | 10.0.12 | MIT | — |
| Microsoft.Extensions.FileProviders.Abstractions | 10.0.12 | MIT | — |
| Microsoft.Extensions.FileProviders.Physical | 10.0.12 | MIT | — |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.12 | MIT | — |
| Microsoft.Extensions.Options.ConfigurationExtensions | 10.0.12 | MIT | — |
| Microsoft.Extensions.Validation | 10.0.12 | MIT | — |

## Build-time only, not distributed

| Component | Version | License | Upstream notices |
|---|---|---|---|
| Microsoft.AspNetCore.Components.Analyzers | 10.0.12 | MIT | — |
| Microsoft.NET.ILLink.Tasks | 10.0.12 | MIT | — |
| Microsoft.NET.Sdk.WebAssembly.Pack | 10.0.12 | MIT | — |

Test tooling (xUnit, FluentAssertions, Playwright and its browsers) is used only by `Tests/PKHeX.Web.Tests` and is not part of a published site.

## Not included

- No sprites, images or fonts are distributed yet. Sprite redistribution waits on a maintainer decision about PokeSprite assets on a public host, and will be listed here with its attribution when it lands.
- No runtime CDN, analytics or other remote resources are used.

## Keeping this list current

The tables must list every package in the Web restore graph once, with its exact version; the E2E tier (`NoticesRestoreGraphTests`) fails otherwise. It also checks the publish against the tables: every published package file must belong to a package in the first table, every package there must have a published file, and no package in the second table may have one.

The runtime pack, ILLink and WebAssembly SDK pack versions come from the installed .NET SDK, not from a package reference, so an SDK with a different runtime patch changes them. Update the versions here when moving to a new SDK. To review the graph:

```sh
dotnet list PKHeX.Web/PKHeX.Web.csproj package --include-transitive
```

## .NET license

The Microsoft packages above are licensed as follows.

```text
The MIT License (MIT)

Copyright (c) .NET Foundation and Contributors

All rights reserved.

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
