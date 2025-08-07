using UnityEngine;
using UnityEngine.Events;

public class ShotSimulator : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    public ArmorProfile tankArmor;
    public ProjectileData[] projectiles;

    [Header("Events")]
    public UnityEvent<string> onLog;

    private int currentProjectileIndex;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentProjectileIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentProjectileIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentProjectileIndex = 2;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
                ProcessHit(ray, hit);
        }
    }

    void ProcessHit(Ray ray, RaycastHit hit)
    {
        if (projectiles.Length == 0)
            return;

        ProjectileData proj = projectiles[Mathf.Clamp(currentProjectileIndex, 0, projectiles.Length - 1)];
        string plateId = hit.collider.gameObject.name;

        if (!tankArmor.TryGetPlate(plateId, out var plate))
        {
            onLog.Invoke($"No armor data for {plateId}");
            return;
        }

        var result = BallisticCalculator.EvaluateHit(
            proj,
            plate,
            hit.distance,
            ray.direction,
            hit.normal);

        onLog.Invoke(
            $"{proj.projectileName} vs {plate.id} -> {result.result} | " +
            $"Angle: {result.impactAngle:F1}° | EffThk: {result.effectiveThickness:F1}mm | " +
            $"PenLeft: {result.penetrationRemaining:F1}mm");
    }
}
