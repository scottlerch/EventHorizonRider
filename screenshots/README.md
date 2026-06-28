# App Store screenshots

Captured from the iOS Simulator by GitHub Actions, then rotated to landscape (the game is
landscape-only) and sized for App Store Connect.

| File | Scene | Device | Pixels |
| --- | --- | --- | --- |
| `iphone-6.7-inch-1.png` | Title / START | iPhone 6.7" | 2778 × 1284 |
| `iphone-6.7-inch-2-gameplay.png` | Mid-game (rings closing in) | iPhone 6.7" | 2778 × 1284 |
| `iphone-6.7-inch-3-gameover.png` | Crash / new best | iPhone 6.7" | 2778 × 1284 |
| `ipad-13-inch-1.png` / `-2.png` | Title / START | iPad Pro 13" | 2752 × 2064 |

Upload under **App Store Connect → App Store → 1.3 → Previews and Screenshots**, and remove the
obsolete 2016 captures (3.5", 4", and 9.7" iPad display types).

## How they're generated

- **[`ios-screenshots.yml`](../.github/workflows/ios-screenshots.yml)** — boots a 6.9" iPhone and a
  13" iPad, launches the game, captures the title screen.
- **[`ios-gameplay-screenshots.yml`](../.github/workflows/ios-gameplay-screenshots.yml)** — uses
  **idb** to inject touches (tap START, then press-and-hold to steer) and fires a burst of
  screenshots to catch the mid-game and crash/game-over scenes. idb runs in a Python 3.11 venv
  (the package's `asyncio.get_event_loop()` call fails on the runner's Python 3.14).

Post-processing (done off-CI for orientation control): rotate the raw portrait captures 90° CCW to
true landscape, then downscale the iPhone ones from the 6.9" native size (2868 × 1320) to 6.7"
(2778 × 1284) — the size App Store Connect's iPhone slot accepts.
