# Galaxy Velocity: Combat Racers

An original single-player anti-gravity combat racing prototype for **Unity 6 / C# / URP**, with a separate dependency-free WebGL browser companion.

Race six armed hovercraft through Asterion Canyon. Boost over elevated roads, drift between energy guards, launch from ramps, shoot drones and rivals, collect weapons, and finish three laps.

**Status:** prototype source complete; browser simulation tests pass. Unity compilation, Editor play mode, Windows build and rendered browser playtesting were **not verified in the authoring environment**. No Windows executable is included. This is not an AAA-quality or production-certified release.

## Play the browser companion

Extract the archive, then double-click **`Web/index.html`** in a desktop browser with WebGL enabled. No installation, server, CDN, package download, account, or network connection is needed. A standalone `Galaxy-Velocity-Play.html` is also included at the repository root. Download it before opening it in your browser; GitHub’s file view does not execute the game.

Select a pilot, press **Launch Race**, and hold **W** after the countdown. Sound is optional; use the top-right toggle. The browser companion implements the same feature set in JavaScript; it is **not a Unity WebGL export**, and passing its tests does not validate the C# build.

## Open the Unity project

1. Install Unity Hub and **Unity 6.0 (`6000.0.23f1`)**, with Windows Build Support. This pinned editor is a project target, not a recommendation of the latest patch.
2. Extract the project and choose **Add project from disk** in Unity Hub. Select the directory containing `Assets`, `Packages`, and `ProjectSettings`.
3. Open it and let Package Manager resolve **URP 17.0.3** and the Unity Test Framework. Internet access is needed for this initial import.
4. The editor setup creates the URP pipeline, renderer, persistent materials, three editable vehicle prefabs, and build-scene registration. If needed, run **Galaxy Velocity → Prepare Project**.
5. Open **`Assets/Scenes/AsterionCanyon.unity`** and press **Play**. The scene is intentionally empty on disk; the runtime bootstrap creates the track, camera, characters, UI and systems on scene load.
6. Choose a pilot and launch the race.

## Windows build

Choose **Galaxy Velocity → Build Windows x64**. Output: `Builds/Windows/GalaxyVelocity.exe` plus its companion data files. Distribute the entire `Builds/Windows` folder, not the `.exe` alone.

For a licensed local editor, use `Scripts/build-windows.ps1`. The script accepts the editor path explicitly. It returns the editor process exit code and writes `Builds/windows-build.log`.

## Controls

| Action | Keyboard | Mouse |
|---|---|---|
| Accelerate | W / ↑ | — |
| Brake | S / ↓ | — |
| Steer | A / D or ← / → | — |
| Boost | Left Shift | — |
| Drift while steering | Space | — |
| Laser | J | Left button |
| Homing missile | K | Right button |
| Pilot ability | E | — |
| Recover at last checkpoint | R | — |
| Pause / resume | Esc | — |

Desktop keyboard controls are the supported target. Touch controls, gamepads, remapping and network multiplayer are not implemented.

## Pilots

| Pilot | Vehicle | Speed | Handling | Ability |
|---|---|---:|---:|---|
| **Rook Solar** — orange/white vulpine commander with green visor | Solaris R-01, red/white | 78 | 20 | Aegis pulse: +45 shield, 3 seconds immunity |
| **Kite Azure** — blue falcon interceptor with tactical armor | Azure K-02, blue/white | 86 | 17 | Overdrive: 5 seconds enhanced speed and rapid fire |
| **Nyx Vesper** — pale lunar lynx with violet armor | Vesper N-03, purple/white | 75 | 26 | Phase shift: full boost, 4 seconds immunity |

Speed and handling values are gameplay tuning units. Each ability has a 14-second cooldown. All pilots, vehicles and UI portraits are original procedural placeholders.

## Implemented features

- Chase camera with boost field-of-view change; steering, acceleration, braking, boost resource, lateral drift and airborne steering.
- Three-lap race with four ordered checkpoints per lap, progress-based ranking, countdown, finish, restart, pause and focus-loss pause.
- Five AI competitors; competitive presentation without online networking.
- Forward lasers with assisted aim, homing missiles, finite ammo, fire cooldowns and swept projectile hit checks.
- Eight hostile drones, hull/shield separation, delayed shield regeneration, checkpoint recovery and brief respawn protection.
- Missile, shield and repair pickups with respawn timers; reusable center-lane boost pads.
- Elevated canyon circuit, bridge piers, launch ramps, energy barriers, alien city, gateway, planets and decorative floating platforms.
- Speed, lap, rank, health, shield, boost, weapon and ability displays, pilot portraits and menus. Browser version adds minimap and impact particles.
- Original synthesized sound effects. Unity also has a simple synthesized engine loop.

## Project layout

| Path | Purpose |
|---|---|
| `Assets/Scripts` | Modular C# runtime: session, racer, race rules, track, world, combat, pickups, camera, art, audio, HUD |
| `Assets/Scenes` | Runtime-generated canyon bootstrap scene |
| `Assets/Editor` | First-import setup and Windows build entry point |
| `Assets/Prefabs` | Three generated editable racer prefabs after first Unity import |
| `Assets/Materials` | Generated URP assets and materials after first import |
| `Assets/UI` | HUD notes; implementation is in `GalaxyHUD.cs` |
| `Assets/Tests/Editor` | Unity EditMode regression tests — included, not executed here |
| `Packages`, `ProjectSettings` | Unity project configuration |
| `Web` | Offline, independent WebGL companion |
| `Tests` | Executable Node simulation tests and optional browser smoke test |
| `Scripts` | Build, verification and explicit GitHub publication helpers |
| `Documentation` | Architecture, acceptance matrix, roadmap and test evidence |

## Verification

Run the executed simulation suite with Node 20+:

```sh
node --test Tests/core.test.cjs
```

Run Unity tests in **Window → General → Test Runner → EditMode**, or use `Scripts/test-unity.ps1` with a licensed Unity editor.

The authoring environment had no Unity editor or local browser executable. Cloud-browser local navigation was blocked by policy. See **[Validation](Documentation/VALIDATION.md)** for the exact distinction between implemented, tested and blocked. Do not interpret a browser test as proof the Unity project compiles.

## Visual reference and screenshots

The user supplied a high-detail canyon combat-racing concept. Its composition informs the warm canyon palette, blue/orange energy, anthropomorphic pilots, armed hovercraft and distant alien city.

Actual gameplay screenshots are **pending a permitted graphics runtime**. None are fabricated or substituted with concept artwork. Run `Tests/browser-smoke.cjs` on a machine with Playwright and Chromium to capture menu and gameplay screenshots in `Documentation/Screenshots`.

## GitHub publishing

Target requested by the owner: `saaeiddev/Galaxy-Velocity-Combat-Racers`.

The owner created this repository after the initial authoring session. This source upload preserves the owner’s initial commit and includes the Unity project, offline WebGL companion, tests and documentation. Native builds and GitHub Pages deployment are not included. Instructions are in **[Publishing](Documentation/PUBLISHING.md)**.

## Roadmap

1. Import and compile in Unity; fix any editor/package issues; execute EditMode tests and manual acceptance checklist.
2. Produce and validate the Windows x64 build on an actual PC; capture screenshots and hardware/frame-time measurements.
3. Replace placeholders with sculpted, rigged characters, authored vehicles, textures, animation and VFX.
4. Replace track-coordinate arcade movement with a more physical anti-gravity controller if free flight and off-track exploration are desired.
5. Add gamepad controls, remapping, accessibility settings, music, difficulty settings, race balancing and additional tracks.
6. Consider multiplayer only after implementing authoritative networking and synchronization tests.

## Credits and rights

Project for **Amir Saeid Dehghan**. Original code, generated meshes and synthesized audio are included. No ripped models, copyrighted franchise characters or third-party game assets are used. No open-source license is asserted on the owner's behalf; see `NOTICE.md`.
