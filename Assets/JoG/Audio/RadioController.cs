using EditorAttributes;
using GuestUnion.UI;
using JoG.InteractionSystem;
using JoG.Localization;
using UnityEngine;

namespace JoG.Audio {

    public class RadioController : MonoBehaviour, IInteractable, ITooltipSource {
        [SerializeField, Required] private NetworkAudioSource _audioSource;
        public string Name => Localizer.GetString("radio.name");

        public string Description => Localizer.GetString("radio.desc");

        void ITooltipSource.BuildTooltip(TooltipView view) {
        }

        public bool CanInteract(Interactor interactor) => true;

        public void PreformInteraction(Interactor interactor) {
            if (_audioSource.IsPlaying) {
                _audioSource.Stop();
            } else {
                _audioSource.Play();
            }
        }
    }
}