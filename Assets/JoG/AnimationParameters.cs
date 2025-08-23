using UnityEngine;

namespace JoG {

    public static class AnimationParameters {
        public static readonly int forwardSpeed = Animator.StringToHash(nameof(forwardSpeed));
        public static readonly int isAimming = Animator.StringToHash(nameof(isAimming));
        public static readonly int isCrouching = Animator.StringToHash(nameof(isCrouching));
        public static readonly int isDead = Animator.StringToHash(nameof(isDead)); 
        public static readonly int isGrounded = Animator.StringToHash(nameof(isGrounded));
        public static readonly int isMoving = Animator.StringToHash(nameof(isMoving));
        public static readonly int isSpawning = Animator.StringToHash(nameof(isSpawning)); 
        public static readonly int isSprinting = Animator.StringToHash(nameof(isSprinting));
        public static readonly int maxMoveSpeed = Animator.StringToHash(nameof(maxMoveSpeed)); 
        public static readonly int rightSpeed = Animator.StringToHash(nameof(rightSpeed));
        public static readonly int upSpeed = Animator.StringToHash(nameof(upSpeed));
    }
}