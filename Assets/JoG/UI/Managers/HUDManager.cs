using EditorAttributes;
using JoG.Character;
using JoG.InteractionSystem;
using JoG.Messages;
using JoG.UI.Buff;
using JoG.UI.HealthBars;
using MessagePipe;
using System;
using UnityEngine;
using VContainer;

namespace JoG.UI.Managers {

    public class HUDManager : MonoBehaviour, IMessageHandler<CharacterBodyChangedMessage> {
        [SerializeField, Required] private HealthBar _healthBar;
        [SerializeField, Required] private BuffBar _buffIconPanel;
        [SerializeField, Required] private TipView _tipView;
        [SerializeField, Required] private Crosshair _crosshair;
        [SerializeField, Required] private CanvasGroup _hudGroup;
        private CharacterBody _body;
        private CharacterInteractor _interactor;
        private float _nextTickTime = 0;
        private IDisposable _disposable;

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            _body = message.body;
            _interactor = message.changeType is CharacterBodyChangeType.Get 
                ? _body.GetComponent<CharacterInteractor>()
                : null;
        }

        [Inject]
        private void Construct(IBufferedSubscriber<CharacterBodyChangedMessage> subscriber) {
            _disposable = subscriber.Subscribe(this);
        }

        private void Update() {
            if (_body == null) {
                _hudGroup.alpha = 0;
                return;
            }
            _hudGroup.alpha = 1;
            _healthBar.UpdateView(_body.PercentHp);
            if ((Time.frameCount & 0b11) is 0b11) {
                _buffIconPanel.UpdateView(_body.Buffs, _body.ActiveBuffCount);
            }
            if (Time.time < _nextTickTime) return;
            _nextTickTime = Time.time + 0.1f;
            _interactor.FindInteractableObject(out var result);
            _tipView.UpdateView(result);
        }

        private void OnDisable() {
            _hudGroup.alpha = 0;
        }

        private void OnDestroy() {
            _disposable?.Dispose();
        }
    }
}