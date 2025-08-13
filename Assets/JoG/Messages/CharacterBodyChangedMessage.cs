using JoG.Character;

namespace JoG.Messages {

    public enum CharacterBodyChangeType {
        None,
        Get,
        Lose,
    }

    public struct CharacterBodyChangedMessage {
        public CharacterBodyChangeType changeType;
        public CharacterBody body;
    }
}