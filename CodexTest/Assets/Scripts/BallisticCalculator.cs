using UnityEngine;

public static class BallisticCalculator
{
    public enum HitResult { Penetration, Ricochet, Stopped }

    public struct HitData
    {
        public HitResult result;
        public float impactAngle;      // degrees
        public float effectiveThickness;
        public float penetrationRemaining;
    }

    public static HitData EvaluateHit(
        ProjectileData projectile,
        ArmorProfile.ArmorPlate plate,
        float distance,
        Vector3 shotDir,
        Vector3 plateNormal)
    {
        float velocity = projectile.GetVelocityAtDistance(distance);
        float penetration = projectile.GetPenetrationAtDistance(distance);
        float angle = Vector3.Angle(-shotDir.normalized, plateNormal.normalized);
        float effectiveThickness = plate.thickness / Mathf.Cos(angle * Mathf.Deg2Rad);

        var hit = new HitData
        {
            impactAngle = angle,
            effectiveThickness = effectiveThickness,
            penetrationRemaining = penetration - effectiveThickness
        };

        if (angle > projectile.maxRicochetAngle)
        {
            hit.result = HitResult.Ricochet;
        }
        else if (penetration >= effectiveThickness)
        {
            hit.result = HitResult.Penetration;
        }
        else
        {
            hit.result = HitResult.Stopped;
        }
        return hit;
    }
}
