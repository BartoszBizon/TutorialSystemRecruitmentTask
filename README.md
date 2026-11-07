# Tutorial System

A data-driven tutorial system for Unity with a server-authoritative progress model.
Tutorial sequences are built in the Inspector without code, progress survives a game restart, and the client cannot skip or bypass blocking steps.

**Unity 6000.5.6f1** · URP · uGUI · Input System

## Quick start

1. Open `Assets/Scenes/TutorialDemo.unity` and press **Play**.
2. Press **Stop → Play** to simulate killing the game. Progress is restored to the same step.
3. **Reset progress** (top-left corner) clears the save.

Server delay, hint timeout and the demo economy are set on the `TutorialDemoBootstrap` object.

## Features

- Dialogs (left / right) and pointers (Additive / Cutout) to UI and world objects
- Blocking steps: only a tap on the target gets through
- Optional **Skip** per sequence
- Steps completed by a game action or by UI
- Delayed hint after 5 s of inactivity
- Progress saved after every command and restored after a restart

## Architecture

| Assembly | Responsibility |
|---|---|
| `TutorialSystem.Shared` (no Unity) | Commands, state, tutorial definitions and rules |
| `TutorialSystem.Server` (no Unity) | `GameServer`: authoritative execution and save |
| `TutorialSystem.Client` | Model, ViewModel, Binding, View, tutorial content |
| `TutorialSystem.Demo` | Composition Root, mini-game, local server |

Key decisions:

- **Progress lives in Shared logic.** The server validates and saves it, so Skip and blocking can't be bypassed on the client.
- **Game code knows nothing about the tutorial.** A post-processor advances the tutorial in the same transaction as the game command.
- **Idempotent completion.** UI steps send the step index, so a double tap never skips a step.
- **Client-side prediction.** Commands run locally and on the server. If the server rejects one, the client takes the server state and replays pending commands.
- **Targets by Id, not by reference.** `TutorialAnchor` marks UI or world objects, so pointers appear and disappear with their screen.

## Adding a sequence

All content lives in `Assets/Content/TutorialContent.asset`.

1. Add a sequence in the Inspector: Id, Skip flag, start conditions and steps (dialog, pointer, blocking, completion action).
2. Add `TutorialAnchor` with a `TargetId` to each target object.

`TutorialContentValidator` rejects content that would soft-lock the player.

## Tests

*Window → General → Test Runner → EditMode → Run All* (16 tests in `Assets/Tests/EditMode`).
They cover Skip rules, blocking, step completion, idempotency, sequence chaining, rejecting cheats on the server and restoring state after a restart.

## Out of scope

- Real network transport and JSON content export for the server
- Localization and a DI container (a manual Composition Root is used)
- Final art
- Dropdowns in the editor for `TargetId` and action keys
