# codex-test

## Setup
This Unity 2022.3 project relies on packages that are not included by default.
Add them via the Package Manager or ensure `Packages/manifest.json` lists:

- `com.unity.entities` – DOTS Entities 1.0 runtime
- `com.unity.inputsystem` – new Input System used by the samples
- `com.unity.ai.navigation` – provides the `NavMeshSurface` component

After installing **AI Navigation**, add a `NavMeshSurface` to a GameObject and bake
the NavMesh to enable infantry movement.
