using EditorAttributes;
using GuestUnion;
using JoG.Character;
using JoG.Messages;
using JoG.UI.Buff;
using JoG.UI.HealthBars;
using MessagePipe;
using System;
using UnityEngine;
using VContainer;

namespace JoG.UI.Controllers {

    public class CharacterViewController : MonoBehaviour, IMessageHandler<CharacterBodyChangedMessage> {
        [SerializeField, Required] private BuffBar _buffBar;
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private CanvasGroup _viewGroup;
        [SerializeField, Required] private HealthBar _healthBar;
        [SerializeField, Required] private Transform _viewTransform;
        private Camera _mainCamera;
        private Camera _uiCamera;
        private CharacterBody _body;
        private CharacterModel _model;
        private IDisposable _disposable;

        public CharacterBody Body {
            get => _body;
            set {
                _body = value;
                _model = value == null
                    ? null
                    : value.Model;
            }
        }

        public CanvasGroup ViewGroup => _viewGroup;
        public Transform ViewTransform => _viewTransform;
        public BuffBar BuffBar => _buffBar;
        public HealthBar HealthBar => _healthBar;

        void IMessageHandler<CharacterBodyChangedMessage>.Handle(CharacterBodyChangedMessage message) {
            Body = message.changeType is CharacterBodyChangeType.Get
                ? message.body
                : null;
        }

        [Inject]
        internal void Construct(IBufferedSubscriber<CharacterBodyChangedMessage> subscriber) {
            _mainCamera = Camera.main;
            if (_canvas.worldCamera == null) {
                _uiCamera = Camera.allCameras.Find(c => c.name is "UICamera");
                _canvas.worldCamera = _uiCamera;
            } else {
                _uiCamera = _canvas.worldCamera;
            }
            _disposable = subscriber.Subscribe(this);
        }

        protected void OnEnable() {
            _canvas.gameObject.SetActive(true);
        }

        protected void Update() {
            if (_body == null || !_model.IsMainRendererVisible) {
                _viewGroup.alpha = 0;
                return;
            }
            _viewGroup.alpha = 1;
            var screenPoint = _mainCamera.WorldToScreenPoint(_body.Top);
            _viewTransform.position = _uiCamera.ScreenToWorldPoint(screenPoint);
            _healthBar.UpdateView(_body.PercentHp);
            if ((Time.frameCount & 0b11) is 0b11) {
                _buffBar.UpdateView(_body.Buffs, _body.ActiveBuffCount);
            }
        }

        protected void OnDisable() {
            _canvas.gameObject.SetActive(false);
        }

        protected void OnDestroy() {
            _disposable?.Dispose();
        }
    }
}