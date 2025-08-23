namespace JoG {

    public interface IHealable {

        void AddHealing(HealingMessage message);

        void SubmitHealing();
    }
}