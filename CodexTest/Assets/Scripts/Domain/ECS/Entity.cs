namespace Game.Domain.ECS
{
    /// <summary>
    /// Represents a unique identifier of an entity in the ECS world.
    /// </summary>
    public readonly struct Entity
    {
        public readonly int Id;

        public Entity(int id)
        {
            Id = id;
        }

        public override string ToString() => Id.ToString();
    }
}
