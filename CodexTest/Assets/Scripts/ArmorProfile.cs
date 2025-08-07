using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TankArmor", menuName = "Ballistics/ArmorProfile")]
public class ArmorProfile : ScriptableObject
{
    [System.Serializable]
    public struct ArmorPlate
    {
        public string id;        // Identifier, must match collider name
        public float thickness;  // millimetres
        public float slope;      // degrees from vertical
        public float hardness;   // optional material modifier
    }

    public List<ArmorPlate> plates = new List<ArmorPlate>();

    /// <summary>Find plate data by id.</summary>
    public bool TryGetPlate(string id, out ArmorPlate plate)
    {
        int index = plates.FindIndex(p => p.id == id);
        if (index >= 0)
        {
            plate = plates[index];
            return true;
        }
        plate = default;
        return false;
    }
}
