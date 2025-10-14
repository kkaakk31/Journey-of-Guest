using UnityEngine;

namespace JoG {

    public static class LayerMasks {
        public static LayerMask @default = LayerMask.GetMask("Default");
        public static LayerMask interactable = LayerMask.GetMask("Interactable");
        public static LayerMask character = LayerMask.GetMask("Character");
        public static LayerMask item = LayerMask.GetMask("Item");
        public static LayerMask characterPart = LayerMask.GetMask("CharacterPart");
        public static LayerMask ignoreRaycast = LayerMask.GetMask("Ignore Raycast");
        public static LayerMask ui = LayerMask.GetMask("UI");
        public static LayerMask water = LayerMask.GetMask("Water");
        public static LayerMask trigger = LayerMask.GetMask("Trigger");
        public static LayerMask dynamic = LayerMask.GetMask("Dynamic");
        public static LayerMask transparentFX = LayerMask.GetMask("TransparentFX");
        public static LayerMask projectile = LayerMask.GetMask("Projectile");
        public static LayerMask postProcessing = LayerMask.GetMask("PostProcessing");
    }
}