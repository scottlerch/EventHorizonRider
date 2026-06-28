# App Store screenshots

Captured from the iOS Simulator by GitHub Actions and sized for App Store Connect.

| File | Scene | Device | Pixels |
| --- | --- | --- | --- |
| `iphone-6.7-inch-1.png` | Title / START | iPhone 6.7" | 2778 × 1284 |
| `iphone-6.7-inch-2-gameplay.png` | Mid-game (rings closing in) | iPhone 6.7" | 2778 × 1284 |
| `iphone-6.7-inch-3-gameover.png` | Crash / new best | iPhone 6.7" | 2778 × 1284 |
| `ipad-13-inch-1.png` | Title / START | iPad Pro 13" | 2752 × 2064 |
| `ipad-13-inch-2-gameplay.png` | Mid-game (rings closing in) | iPad Pro 13" | 2752 × 2064 |
| `ipad-13-inch-3-gameover.png` | Crash / new best | iPad Pro 13" | 2752 × 2064 |

Upload under **App Store Connect → App Store → 1.3 → Previews and Screenshots**, and remove the
obsolete 2016 captures (3.5", 4", and 9.7" iPad display types).

## How they're generated

One workflow, **[`ios-screenshots.yml`](../.github/workflows/ios-screenshots.yml)**, does everything:

1. Builds the Simulator app and boots a 6.9" iPhone and a 13" iPad.
2. Uses **idb** to inject touches — tap START, then press-and-hold to steer the ship into a crash —
   while capturing the title, a mid-game frame, and a burst of game-over frames. (idb runs in a
   Python 3.11 venv: `fb-idb` calls `asyncio.get_event_loop()`, which raises on the runner's Python 3.14.)
3. Rotates the raw portrait captures 90° CCW to true landscape and downscales the iPhone ones to
   6.7" (2778 × 1284) — the size App Store Connect's iPhone slot accepts — all in-CI.

The `appstore-screenshots` artifact has upload-ready PNGs in `out/` (and the originals in `raw/`).
The committed set above was hand-picked from that artifact (best mid-game + game-over per device).
