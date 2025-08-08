namespace Game.Domain.ECS
{
    /// <summary>
    /// Interface for all systems. Systems operate on components and
    /// are executed by the server.
    /// </summary>
    public interface ISystem
    {
        void Update(World world, float deltaTime);
    }
}
