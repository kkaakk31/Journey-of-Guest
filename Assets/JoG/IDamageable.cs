using Unity.Netcode;

namespace JoG {

    public interface IDamageable {

        void AddDamage(DamageMessage message);

        void SubmitDamage();
    }
}