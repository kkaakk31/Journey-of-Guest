using Unity.Netcode;
using UnityEngine;

namespace JoG.Audio {

    public class NetworkAudioSource : NetworkBehaviour {
        [SerializeField] private AudioSource _audioSource;

        public AudioSource AudioSource => _audioSource;
        public bool IsPlaying => _audioSource.isPlaying;

        public void Play() {
            PlayRpc();
        }

        public void PlayOnShot() {
            PlayOnShotRpc();
        }

        public void Stop() {
            StopRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayRpc() {
            _audioSource.Play();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayOnShotRpc() {
            _audioSource.PlayOneShot(_audioSource.clip);
        }

        [Rpc(SendTo.Everyone)]
        private void StopRpc() {
            _audioSource.Stop();
        }
    }
}