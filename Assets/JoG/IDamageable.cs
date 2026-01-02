namespace JoG {

    public interface IDamageable : ITeamed {

        void TakeDamage(in DamageMessage message);
    }
}