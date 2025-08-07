using Unity.Entities;

namespace RTS.Infantry
{
    /// <summary>Indicates that the unit is currently selected.</summary>
    public struct SelectedTag : IComponentData, IEnableableComponent { }
}
