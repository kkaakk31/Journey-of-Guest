using EditorAttributes;
using JoG.InteractionSystem;
using JoG.Localization;
using UnityEngine;

namespace JoG.Audio {

    public class RadioController : MonoBehaviour, IInteractable, IInformationProvider {
        [SerializeField, Required] private NetworkAudioSource _audioSource;
        public string Name => Localizer.GetString("radio.name");

        public string Description => Localizer.GetString("radio.desc");

        public bool CanInteract(Interactor interactor) {
            return true;
        }

        public string GetProperty(string key) {
            return Localizer.GetString(key);
        }

        public void PreformInteraction(Interactor interactor) {
            if (_audioSource.IsPlaying) {
                _audioSource.Stop();
            } else {
                _audioSource.Play();
            }
        }
    }
}