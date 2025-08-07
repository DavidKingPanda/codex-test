# codex-test

## Setup
This Unity 2022.3 project relies on packages that are not included by default.
Add them via the Package Manager or ensure `Packages/manifest.json` lists:

- `com.unity.entities` – DOTS Entities 1.0 runtime
- `com.unity.entities.graphics` – Entities rendering components
- `com.unity.inputsystem` – new Input System used by the samples
- `com.unity.ai.navigation` – provides the `NavMeshSurface` component

After installing **AI Navigation**, add a `NavMeshSurface` to a GameObject and bake
the NavMesh to enable infantry movement.

### Scene Preparation
1. Add the `RTSCameraController` component to your main camera.
2. Create infantry prefabs with a `NavMeshAgent` and an `EntityReference` component.
3. Place these prefabs in the scene; systems will convert them to entities at runtime.
4. Bake the NavMesh and press Play – selected units should navigate on right click.

### Controls
- **Left Click**: select a single unit. Drag to marquee-select multiple units.
- **Shift + Click/Drag**: add or remove units from the current selection.
- **Right Click**: move all selected units to the clicked position.

These scripts use Entities 1.0 with the new Input System and hybrid NavMesh navigation.
