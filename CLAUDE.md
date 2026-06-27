# CLAUDE.md

Guidance for working in this repository.

## What this is

Event Horizon Rider is a mobile-first arcade game (inspired by Super Hexagon) built on
**MonoGame 3.8.4.1** and targeting **.NET 10**. The player rides the event horizon of a black hole,
threading gaps in incoming rings of debris. One shared codebase runs on Android, iOS, Windows, and
the web.

## Solution layout

| Project | TFM | Purpose |
| --- | --- | --- |
| `EventHorizonRider.Shared` | `net10.0-windows;net10.0-android;net10.0-ios` | Multi-targeted SDK class library holding ~all game code; referenced by each head. |
| `EventHorizonRider.Windows` | `net10.0-windows` | Windows head (`WinExe`, WindowsDX). Also hosts the WinForms `DevelopmentToolsForm`. |
| `EventHorizonRider.Android` | `net10.0-android` | Android head. |
| `EventHorizonRider.iOS` | `net10.0-ios` | iOS head (**requires a Mac to build**). |
| `EventHorizonRider.Web` | `net10.0` | The marketing website (static HTML/CSS). Hosted on AWS S3 + CloudFront + ACM and deployed via `scripts/Deploy-Website.ps1` — see [its README](EventHorizonRider.Web/README.md). The `.csproj`/`Program.cs` is only a local `dotnet run` preview. |

The solution is `EventHorizonRider.slnx` (XML solution format). Common settings live in
`Directory.Build.props` (copyright, `Newtonsoft.Json`, `AllowUnsafeBlocks`, the `Debug;Release;Ad-Hoc;AppStore`
configurations).

### The shared class library (important)

`EventHorizonRider.Shared` is a **multi-targeted SDK class library** (`net10.0-windows;net10.0-android;net10.0-ios`).
The same source is compiled once per platform against that platform's MonoGame framework assembly
(WindowsDX / Android / iOS), which is why it multi-targets rather than producing a single portable DLL.
Each head `<ProjectReference>`s it. Consequences:

- The root namespace of shared code is **`EventHorizonRider.Core`** (despite the folder being named `Shared`).
- Source files are picked up by the SDK's default globbing — **no manual `<Compile>` list**; just add a `.cs` file.
- Because it's a real library, `internal` members no longer leak into the heads. The Windows head's
  `DevelopmentToolsForm` inspects internal game state, so the library grants it access via
  `<InternalsVisibleTo Include="EventHorizonRider.Windows" />`.
- The MonoGame content pipeline (`Content/Content.mgcb`) is still referenced and built **by each head**
  (`<MonoGameContentReference>` + `MonoGame.Content.Builder.Task`), so content output flows into the app
  package exactly as before; the library itself does not build content.
- You can compile the shared code for a specific platform directly, e.g.
  `dotnet build EventHorizonRider.Shared/EventHorizonRider.Shared.csproj -f net10.0-ios`. This compiles the
  **iOS C# on Windows** (only the final app link needs a Mac), which catches iOS-only errors early.

## Build & run

The MonoGame content pipeline needs the `dotnet-mgcb` local tool. **Restore it first or the content build
fails:**

```bash
dotnet tool restore
```

Then build a head (this also builds the referenced shared library):

```bash
dotnet build EventHorizonRider.Windows/EventHorizonRider.Windows.csproj   # Windows + shared library
dotnet build EventHorizonRider.Web/EventHorizonRider.Web.csproj
dotnet build EventHorizonRider.Android/EventHorizonRider.Android.csproj
# The full iOS app must be built on a Mac, but the shared C# compiles for iOS on Windows:
dotnet build EventHorizonRider.Shared/EventHorizonRider.Shared.csproj -f net10.0-ios
```

If a content build intermittently fails with a `.mgcontent ... being used by another process` race under
parallel MSBuild, re-run that build with `-m:1`.

Run on Windows:

```bash
dotnet run --project EventHorizonRider.Windows
# Pass "Development" as the first arg to open the live property/scene-graph inspector window.
```

## Architecture

```
Platform head (Android/iOS/Windows/Web)
  └─ builds a Platform (detail/input settings) → DeviceInfo.InitializePlatform(platform)
  └─ new MainGame().Run()
       └─ MainGame : Game            (Update/Draw loop; owns the SpriteBatch + InputState)
            └─ GameContext           (top-level container)
                 ├─ GameStateBase    (state machine: Initialize → Start → Menu → Running → Over)
                 ├─ Root : ComponentBase  (root of the visual component tree)
                 │    ├─ Space (background, blackhole, rings, ship, shockwave …)
                 │    ├─ Menu, Foreground (HUD: timer, buttons, title …)
                 │    └─ Music
                 ├─ PlayerData        (persisted best time / level; JSON in LocalApplicationData)
                 └─ LevelCollection   (5 hand-authored levels; the last is infinite)
```

- **`DeviceInfo` / `Platform`** are global statics. `InitializePlatform` must run **before** `MainGame` is
  constructed; `InitializeGraphics` runs inside `MainGame.Initialize`. `DeviceInfo` also holds the
  logical→native scaling matrix (the game is authored at a fixed logical resolution and scaled to the device).
- **Component composite pattern (`ComponentBase`)**: every visual element has children; `Update`/`Draw`
  walk the tree and call the subclass hooks `UpdateCore`/`DrawCore`/`LoadContentCore`/`OnBeforeDraw`/`OnAfterDraw`.
- **Physics** (`Physics/`): pooled particle system (`Emitter` owns a `Particle[]`; `Particle` is a `struct`),
  pixel-perfect `CollisionDetection`, plus `Motion`/`Spring` easing helpers.
- **Graphics** (`Graphics/`): `Space` renders to `RenderTarget2D`s and applies a two-pass `GaussianBlur`
  post-process when the blur amount is > 0; `TextureProcessor` does alpha/crop/scale work at load time.
- **Content**: authored in `EventHorizonRider.Shared/Content/Content.mgcb`, built by
  `MonoGame.Content.Builder.Task` (via the `dotnet-mgcb` tool).

## Conventions

- Style is enforced by `.editorconfig`: 4-space indent, CRLF, file-scoped namespaces, `_camelCase` private
  fields, PascalCase constants, `I`-prefixed interfaces, `var` preferred. Modern idioms (primary constructors,
  collection expressions, target-typed `new`, expression-bodied members) are used throughout.
- Run `dotnet format <project>.csproj` to format. The shared library can be formatted directly
  (`dotnet format EventHorizonRider.Shared/EventHorizonRider.Shared.csproj`). Note: `dotnet format --severity info`
  can crash inside the WinForms source generator on the Windows head — use the default severity, or format the
  shared library (no WinForms) directly.

## Performance notes (the hot path)

`Update(GameTime)` and `Draw(SpriteBatch)` run for the **entire component tree at ~60 fps**. Code on these
paths is deliberately allocation-free, and should stay that way:

- `ComponentBase.Update`/`Draw` use hand-rolled `for` loops (not `ForEach`/LINQ) to avoid per-frame
  closure/delegate allocations. Don't "simplify" them back to LINQ.
- Avoid in per-frame code: LINQ, lambdas/closures, `new` arrays/lists, boxing (e.g. calling LINQ on the
  `TouchCollection` struct), and per-frame string formatting. Prefer `KeyboardState.IsKeyDown(key)` over
  `GetPressedKeys().Contains(key)` (the latter allocates an array each call).
- UI strings (e.g. the timer) are cached and only re-formatted when their value changes.

## Gotchas

- The full iOS **app** build needs a Mac, but the shared **C#** compiles on Windows via
  `dotnet build EventHorizonRider.Shared/EventHorizonRider.Shared.csproj -f net10.0-ios` — use it to catch
  iOS-only errors before handing off to a Mac.
- On iOS, `MediaPlayer` is also an Apple framework namespace. Reference MonoGame's player with the fully
  qualified `Microsoft.Xna.Framework.Media.MediaPlayer` (see `Components/Music.cs`) or the unqualified name is
  ambiguous and the iOS build fails.
- The MonoGame content build (`mgcb`) can intermittently fail under parallel MSBuild with
  `.mgcontent ... being used by another process`. It's a content-pipeline race, not a code issue — rebuild the
  head with `-m:1` (or clear `EventHorizonRider.Shared/Content/obj`) if it happens.
- After cloning or when content fails to build, run `dotnet tool restore`.
- `Motion.Acceleration` is currently always 0 (the acceleration branch in `Motion.Update` is dead code); don't
  rely on it without revisiting that branch.
