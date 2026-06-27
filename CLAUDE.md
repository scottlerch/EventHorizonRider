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
| `EventHorizonRider.Shared` | n/a | **Shared Project** (`.shproj`/`.projitems`) holding ~all game code. Not a standalone assembly. |
| `EventHorizonRider.Windows` | `net10.0-windows` | Windows head (`WinExe`, WindowsDX). Also hosts the WinForms `DevelopmentToolsForm`. |
| `EventHorizonRider.Android` | `net10.0-android` | Android head. |
| `EventHorizonRider.iOS` | `net10.0-ios` | iOS head (**requires a Mac to build**). |
| `EventHorizonRider.Web` | `net10.0` | ASP.NET static-file host for the web build. |

The solution is `EventHorizonRider.slnx` (XML solution format). Common settings live in
`Directory.Build.props` (copyright, `Newtonsoft.Json`, `AllowUnsafeBlocks`, the `Debug;Release;Ad-Hoc;AppStore`
configurations).

### The shared-project model (important)

`EventHorizonRider.Shared` is an **old-style Shared Project**. Its `.cs` files are compiled *directly into
each head* via `<Import .../EventHorizonRider.Shared.projitems />` — there is no shared DLL. Consequences:

- The root namespace of shared code is **`EventHorizonRider.Core`** (despite the folder being named `Shared`).
- The CLI cannot load `.shproj` directly. To build/format/analyze the shared code, target a **head** project.
  The Windows head is the most convenient because it compiles every shared file on this platform.
- Adding a new shared `.cs` file requires adding a `<Compile Include="..." />` entry to
  `EventHorizonRider.Shared.projitems`.

## Build & run

The MonoGame content pipeline needs the `dotnet-mgcb` local tool. **Restore it first or the content build
fails:**

```bash
dotnet tool restore
```

Then build a head (this also compiles all shared code):

```bash
dotnet build EventHorizonRider.Windows/EventHorizonRider.Windows.csproj   # Windows + all shared code
dotnet build EventHorizonRider.Web/EventHorizonRider.Web.csproj
dotnet build EventHorizonRider.Android/EventHorizonRider.Android.csproj
# iOS must be built on a Mac.
```

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
- Run `dotnet format <head>.csproj` to format. The shared code is formatted via a head (the `.shproj` can't be
  loaded directly). Note: `dotnet format --severity info` can crash inside the WinForms source generator on the
  Windows head — use the default severity, or format the shared files through a non-WinForms head.

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

- iOS can only be built on a Mac; verify shared-code changes on Windows instead.
- After cloning or when content fails to build, run `dotnet tool restore`.
- `Motion.Acceleration` is currently always 0 (the acceleration branch in `Motion.Update` is dead code); don't
  rely on it without revisiting that branch.
