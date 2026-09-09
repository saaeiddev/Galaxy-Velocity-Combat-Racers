# Architecture

`GameSession` owns selection → countdown → racing → finished, with a reversible pause state. `Racer` owns local combat and driving state. `RaceRules` owns ordered forward progress; a finished race requires 12 checkpoint crossings. Recovery rewinds progress only to the last passed gate.

Movement is deliberately constrained to track coordinates (longitudinal distance, lateral offset, jump height). `CanyonTrack` converts those coordinates into 3D positions and tangents on a closed elevated curve. This provides predictable arcade navigation, natural track following and prevents shortcuts. It is not a free-roaming rigid-body flight simulation. Course distance is a gameplay parameter; it is not a measured arc-length mapping of the spline.

`CombatSystem` advances projectiles and checks the complete movement segment against vehicle/drone radii to prevent high-speed tunnelling. Laser assist selects a target in a forward cone; missiles steer toward the selected target. Damage drains shields before hull. Drones fire when the player is within range and respawn after destruction. Shots are capped at 160; full pooling remains a performance-roadmap item.

`PickupSystem` separates collectible cooldowns from racer resource state. Pickups restore ammo, shield or hull. Track events are crossed-distance checks, so boost and ramp events cannot be skipped between frames at high speed. AI competitors use the same racer resources, track and weapons, with simple periodic steering/throttle decisions.

`ProceduralArt` creates replaceable vehicle and pilot placeholders. `CanyonWorld` constructs the track, city, rocks, gates and planets. The first-import editor script persists URP assets and three racer prefabs. `ChaseCamera`, `GalaxyHUD` and `GameAudio` consume race state. The runtime-generated scene is intentional; runtime meshes and objects are created once per world, not every frame.

The Web companion is an independent implementation. `core.js` has no DOM or graphics dependencies. `render.js` batches static geometry into a single WebGL buffer and separately draws vehicles, drones, collectibles and projectiles. `game.js` maps keyboard/mouse input to a 60 Hz fixed-step simulation and presents DOM HUD elements. Device pixel ratio is capped at 1.5. There are no telemetry or external network calls.

Limitations: Unity uses a capped variable timestep while the browser uses a fixed timestep; exact trajectories and timing are not bit-identical. World platforms are decorative; there is one traversable race circuit. The translucent HUD is a prototype glass-style treatment, not refractive liquid glass. No network multiplayer, save system, localization, production assets, formal accessibility certification or verified frame-rate target is included.
