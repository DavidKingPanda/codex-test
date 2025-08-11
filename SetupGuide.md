# Setup Guide

This repository demonstrates a minimal server-authoritative ECS framework for **Unity 2022.3.33f1**. Follow these steps to set up the project and run both the headless server and client locally or across a LAN.

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
   - `JumpSystem`
   - `ReplicationSystem`
4. Open **File → Build Settings** and enable **Dedicated Server/Server Build** to produce a headless executable. (Leave this unchecked for client builds.)

## 4. Client Scene
1. Create a new scene for the client.
2. Add an empty `GameObject` named **Client** and attach `ClientBootstrap`.
3. `ClientBootstrap` references:
   - `ClientInputSender`
   - `ClientSnapshotReceiver`
   - a player visual `Transform` used for rendering
   - `CameraFollow` on the main camera for top‑down tracking
4. Add a `PlayerInput` component:
   - Create an **Input Actions** asset with an action map `Gameplay` containing:
     - a `Vector2` action **Move** bound to WASD/left stick,
     - a `Button` action **Jump** bound to Space.
   - In `PlayerInput`, assign this asset and choose **Invoke Unity Events**.
   - Under **Move (performed)**, hook up `ClientInputSender → OnMove`.
   - Under **Jump (performed)**, hook up `ClientInputSender → OnJump`.
5. The client only renders state and sends input; all gameplay logic runs on the server.

## 5. Running on One Machine
1. Start the headless server build or play the server scene in the Editor.
2. Play the client scene in the Editor or as a standalone build on the same computer.
3. Use `localhost` and the same port for both processes.

## 6. Testing on a Local Network
1. Build and run the headless server on one machine within your LAN.
2. Determine its local IP address (for example, `192.168.x.x`).
3. On another machine, run the client scene and configure it to connect to that IP and port.
4. Ensure any firewalls allow traffic on the chosen port.

## 7. Project Overview
- **Domain/ECS** – core ECS types (`Entity`, `World`, `ISystem`, `IComponent`).
- **Components** – data-only components such as `PositionComponent`.
- **Systems** – server logic like `MovementSystem` and `JumpSystem` responding to events.
- **Networking** – `NetworkManager` wrapper around Unity Transport.
- **Infrastructure** – glue code (`EventBus`, `ClientInputSender`, `ServerCommandDispatcher`, `ClientBootstrap`, `ServerBootstrap`).
- **Presentation** – visuals and camera scripts (`CameraFollow`).

This setup yields a clean, event-driven, server‑authoritative ECS base where a player entity moves with WASD, the server replicates the position, and the client camera follows from a top‑down perspective.
