# Validation report — 2026-09-09

## Executed

- JavaScript syntax checks: `Web/core.js`, `Web/render.js`, `Web/game.js` passed `node --check`.
- **12/12 automated Node simulation tests passed.** Raw output is in `node-test-results.txt`.
- Each of the three pilots completed a three-lap simulated race against five AI racers, with combat enabled. This was a headless simulation, not a rendered human playtest.
- Regression cases cover ordered gates, reverse-progress rejection, seam crossing, vehicle movement, drift, jump/air steering, boost drain/regeneration/pads, brake priority, shield/hull damage, checkpoint recovery, abilities, projectile/drone hits, missile inventory/rate limiting, pickups, pause and restart.
- C# source was reviewed for correspondence with implemented mechanics; this is not compilation evidence.

## Blocked / not executed

| Requirement | Result | Reason |
|---|---|---|
| Unity import and C# compilation | Not verified | No Unity editor or C# compiler installed |
| Unity Play Mode driving/combat/UI | Not verified | No Unity runtime |
| Unity EditMode tests (5 included) | Not executed | No Unity test runner |
| PC `.exe` build and launch | Not produced | No licensed Unity editor / Windows build module |
| Browser rendered gameplay and screenshots | Not verified | Local Chromium executable missing; cloud browser explicitly rejected local-file navigation under its URL security policy |
| GitHub repository creation | Completed by owner | Owner supplied the new repository URL in the follow-up |
| GitHub publication | Source upload in this follow-up | Remote commit and file integrity checked after upload; no Unity runtime claim |
| AAA visual quality / production readiness | Not achieved | Procedural prototype assets; no graphics playtest or hardware profiling |

The browser-policy rejection was respected. No alternate browser route or policy workaround was attempted after that rejection. The earlier local bundle preserves authoring history. This follow-up adds the source to the owner-created GitHub repository; hosting source does not validate a Unity build.

## Manual acceptance checklist for a Unity-capable PC

- [ ] Import cleanly, confirm URP pipeline assigned, no red Console errors.
- [ ] Open AsterionCanyon, press Play, select all three pilots, inspect original models and stat differences.
- [ ] Complete countdown; test throttle, simultaneous brake/throttle priority, steering and edge scrape damage.
- [ ] Drain and replenish boost; cross a center pad; verify speed change and meter.
- [ ] Hold Space while turning; verify drift; launch from both ramps; steer in the air.
- [ ] Fire laser and missile at drone and rival; verify hull/shield, ammo and cooldown changes.
- [ ] Collect all three pickup types; verify cooldown and respawn.
- [ ] Trigger each ability and verify effect/cooldown. Lose all hull and verify checkpoint recovery.
- [ ] Complete three laps; check gate order, position, finish time, restart and return to selection.
- [ ] Pause, resume, switch application focus, resize window; check HUD and controls.
- [ ] Run included EditMode tests; build Windows x64, launch build, repeat checks on actual hardware.
- [ ] Capture genuine menu/gameplay screenshots and record frame-time measurements.
- [ ] Publish repository; clone into a clean folder and reproduce import/build.
