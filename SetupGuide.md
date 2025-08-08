# Setup Guide

This repository demonstrates a minimal server-authoritative ECS framework for **Unity 2022.3.33f1**. Follow these steps to set up the project and run both client and server on the same machine.

## 1. Install Unity Packages
Open **Window → Package Manager** and install:

- `com.unity.transport` – Unity Transport for networking
- `com.unity.inputsystem` – Input System for capturing player input

## 2. Create Folder Structure
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
    Presentation/
    Utils/        (reserved)
```

Place the provided `.cs` files into their matching folders.

## 3. Server Bootstrap (Headless)
1. Create an empty scene for the server.
2. Add a `GameObject` named **Server** and attach `ServerBootstrap`.
3. `ServerBootstrap` wires together:
   - `EventBus`
   - `World`
   - `NetworkManager` (`StartServer(port)`)
   - `ServerCommandDispatcher`
   - `MovementSystem`
   - `ReplicationSystem`
4. Open **File → Build Settings** and enable **Server Build** to produce a headless executable.

## 4. Client Scene
1. Create a new scene for the client.
2. Add an empty `GameObject` named **Client** and attach `ClientBootstrap`.
3. `ClientBootstrap` references:
   - `ClientInputSender`
   - `ClientSnapshotReceiver`
   - a player visual `Transform` used for rendering
   - `CameraFollow` on the main camera for top‑down tracking
4. Add a `PlayerInput` component with an action **Move** bound to WASD/left stick and set it to call `ClientInputSender.OnMove`.
5. The client only renders state and sends input; all gameplay logic runs on the server.

## 5. Running Locally
1. Start the headless server build or play the server scene in the Editor.
2. Play the client scene in the Editor or as a standalone build.
3. Use `localhost` and the same port so both run on one machine.

## 6. Project Overview
- **Domain/ECS** – core ECS types (`Entity`, `World`, `ISystem`, `IComponent`).
- **Components** – data-only components such as `PositionComponent`.
- **Systems** – server logic like `MovementSystem` responding to events.
- **Networking** – `NetworkManager` wrapper around Unity Transport.
- **Infrastructure** – glue code (`EventBus`, `ClientInputSender`, `ServerCommandDispatcher`, `ClientBootstrap`, `ServerBootstrap`).
- **Presentation** – visuals and camera scripts (`CameraFollow`).

This setup yields a clean, event-driven, server‑authoritative ECS base where a player entity moves with WASD, the server replicates the position, and the client camera follows from a top‑down perspective.
