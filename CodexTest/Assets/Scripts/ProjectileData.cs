using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Ballistics/Projectile")]
public class ProjectileData : ScriptableObject
{
    [Header("Basic Specs")]
    public string projectileName;
    public float caliber;        // mm
    public float mass;           // kg
    public float muzzleVelocity; // m/s

    [Header("Penetration")]
    public AnimationCurve penetrationByDistance; // distance (m) -> penetration (mm)
    public float maxRicochetAngle = 70f; // degrees

    /// <summary>Simple linear velocity drop-off.</summary>
    public float GetVelocityAtDistance(float distance)
    {
        float drop = distance * 0.1f;
        return Mathf.Max(0f, muzzleVelocity - drop);
    }

    /// <summary>Lookup penetration value from curve.</summary>
    public float GetPenetrationAtDistance(float distance)
    {
        return penetrationByDistance.Evaluate(distance);
    }
}
