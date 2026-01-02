namespace JoG {

    public interface IHealth : IIdentity {
        int Health { get; }
        int MaxHealth { get; }
        float HealthRatio { get; }
        bool IsAlive { get; }
        bool IsDead { get; }
    }
}