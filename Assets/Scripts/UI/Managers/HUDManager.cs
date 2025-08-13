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
        [SerializeField, Required] private BuffIconPanel _buffIconPanel;
        [SerializeField, Required] private GameObjectInfomationPanel _screenTipPanel;
        [SerializeField, Required] private Crosshair _crosshair;
        [SerializeField, Required] private GameObject _spawnPanel;
        private CharacterBody _source;
        private CharacterInteractor _interactor;
        private float _nextTickTime = 0;
        private IDisposable _disposable;

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            _source = message.body;
            switch (message.changeType) {
                case CharacterBodyChangeType.None:
                    _interactor = null;
                    _spawnPanel.SetActive(true);
                    enabled = false;
                    return;

                case CharacterBodyChangeType.Lose:
                    if (_source.IsLocalPlayer) {
                        _interactor = null;
                        _spawnPanel.SetActive(true);
                        enabled = false;
                        CursorManager.Instance.RequestShowCursor();
                    }
                    break;

                case CharacterBodyChangeType.Get:
                    if (_source.IsLocalPlayer) {
                        _interactor = _source.GetComponent<CharacterInteractor>();
                        _spawnPanel.SetActive(false);
                        enabled = true;
                        CursorManager.Instance.ReleaseShowCursor();
                    }
                    break;
            }
        }

        [Inject]
        private void Construct(IBufferedSubscriber<CharacterBodyChangedMessage> subscriber) {
            _disposable = subscriber.Subscribe(this);
        }

        private void Update() {
            _healthBar.UpdateView(_source.PercentHp);
            if ((Time.frameCount & 0b11) is 0b11) {
                _buffIconPanel.UpdateView(_source.Buffs, _source.ActiveBuffCount);
            }
            if (Time.time < _nextTickTime) return;
            _nextTickTime = Time.time + 0.1f;
            _interactor.FindInteractableObject(out var result);
            _screenTipPanel.UpdateView(result);
        }

        private void OnDestroy() {
            _disposable?.Dispose();
        }
    }
}