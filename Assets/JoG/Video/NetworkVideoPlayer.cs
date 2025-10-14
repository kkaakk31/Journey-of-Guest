using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;

namespace JoG.Video {

    public class NetworkVideoPlayer : NetworkBehaviour {
        public NetworkVariable<bool> isPlaying = new(false, writePerm: NetworkVariableWritePermission.Owner);
        public NetworkVariable<double> videoTime = new(0.0, writePerm: NetworkVariableWritePermission.Owner);

        private const double ThresholdSeconds = 0.2;
        [field: SerializeField] public VideoPlayer VideoPlayer { get; private set; }

        public void TogglePlay() {
            if (isPlaying.Value) {
                StopRpc();
            } else {
                PlayRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        public void PlayRpc() {
            videoTime.Value = VideoPlayer.time;
            isPlaying.Value = true;
        }

        [Rpc(SendTo.Owner)]
        public void StopRpc() {
            videoTime.Value = VideoPlayer.time;
            isPlaying.Value = false;
        }

        protected void Awake() {
            isPlaying.OnValueChanged = (oldValue, newValue) => {
                if (newValue) {
                    VideoPlayer.Play();
                } else {
                    VideoPlayer.Stop();
                }
            };

            videoTime.OnValueChanged = (oldValue, newValue) => {
                var diff = Mathf.Abs((float)(VideoPlayer.time - newValue));
                if (diff > ThresholdSeconds) {
                    VideoPlayer.time = newValue;
                }
            };
        }

        protected override void OnNetworkSessionSynchronized() {
            VideoPlayer.time = videoTime.Value;
            if (isPlaying.Value) {
                VideoPlayer.Play();
            } else {
                VideoPlayer.Stop();
            }
        }
    }
}