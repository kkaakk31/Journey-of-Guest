namespace JoG {

    public interface IHealable : ITeamed {

        void TakeHeal(in HealMessage message);
    }
}