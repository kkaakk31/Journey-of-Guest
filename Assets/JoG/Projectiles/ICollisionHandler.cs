namespace JoG.Projectiles {

    public interface ICollisionHandler {

        void Handle(CollisionMessage message);
    }
}