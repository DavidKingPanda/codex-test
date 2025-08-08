# Setup Guide

This project targets **Unity 2022.3.33f1** and implements a minimal server-authoritative ECS
foundation.

## Required Unity Packages
Install via *Window → Package Manager*:

- `com.unity.transport` – Unity Transport for networking
- `com.unity.inputsystem` – New Input System for capturing player input

## Folder Structure
Create the following folders under `Assets/`:

```
Assets/
  Scripts/
    Components/
    Systems/
    Networking/
    Domain/
      Commands/
      ECS/
    Infrastructure/
    Utils/ (reserved)
```

Place the provided `.cs` files into the matching folders.

## Headless Server Build
1. Open **File → Build Settings**.
2. Check **Server Build** to create a headless executable.
3. In a bootstrap script, create and wire:
   - `EventBus`
   - `World`
   - `NetworkManager` (call `StartServer(port)`)
   - `ServerCommandDispatcher`
   - `MovementSystem`
4. On every tick call `networkManager.Update()` and `movementSystem.Update(world, deltaTime)`.

## Client Scene Setup
1. In the client scene create a `GameObject` named `Client`.
2. Add components:
   - `NetworkManager` (call `StartClient(address, port)` at start)
   - `ClientInputSender` – reference the `NetworkManager`.
   - `PlayerInput` (from Input System) with an action *Move* mapped to WASD or stick.
     Set the action to call `ClientInputSender.OnMove`.
3. Runtime-created entities should be mirrored on the client for rendering only.

## Local Network Testing
1. Build and run the server headless (or run a `ServerBootstrap` script in the Editor).
2. Play the client scene in the Editor or as a standalone build.
3. Both can run on the same machine using `localhost` and the same port.

## Project Structure Overview
- **Domain/ECS** – core ECS types (`Entity`, `World`, `ISystem`, `IComponent`).
- **Components** – data-only components like `PositionComponent`.
- **Systems** – server systems such as `MovementSystem` reacting to events.
- **Networking** – `NetworkManager` wrapper around Unity Transport.
- **Infrastructure** – plumbing: `EventBus`, `ClientInputSender`, `ServerCommandDispatcher`.
- **Domain/Commands** – command structs (e.g., `MoveCommand`).

This foundation establishes a clean separation of concerns and an event-driven,
server-authoritative workflow ready for further expansion.
