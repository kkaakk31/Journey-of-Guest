using ANU.IngameDebug.Console;
using EditorAttributes;
using UnityEngine;
using UnityUtils;

namespace JoG {

    [DebugCommandPrefix("cursor")]
    public class CursorManager : Singleton<CursorManager> {
        [SerializeField, ReadOnly] protected int _showCursorCount = 1;

        [DebugCommand]
        public void ShowCursor() {
            _showCursorCount++;
            UpdateCursorState();
        }

        [DebugCommand]
        public void HideCursor() {
            _showCursorCount--;
            UpdateCursorState();
        }

        protected void UpdateCursorState() {
            if (_showCursorCount > 0) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        protected void OnApplicationFocus(bool focus) {
            if (focus) {
                UpdateCursorState();
            }
        }

        protected override void Awake() {
            base.Awake();
            UpdateCursorState();
        }

        protected void OnDestroy() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}