using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action<bool, Vector2> OnMove;
    public static event Action<bool, float> OnLook;
    public static event Action<bool> OnJump;
    public static event Action<bool> onDeath;
    public static event Action<bool> onAttack;
    public static event Action<bool> onGamePaused;

  
    public static void RaiseMovePerformed(bool isMoving, Vector2 direction)
    {
        OnMove?.Invoke(isMoving, direction);
    }

    public static void RaiseJumpPerformed(bool isJumping)
    {
        OnJump?.Invoke(isJumping);
    }

    public static void RaisePlayerDeath(bool isDead)
    {
        onDeath?.Invoke(isDead);
    }

    public static void RaisePlayerAttack(bool isAttacking)
    {
        onAttack?.Invoke(isAttacking);
    }

    public static void RaiseLookPerformed(bool isLooking, float direction)
    {
        OnLook?.Invoke(isLooking, direction);
    }

    public static void RaiseGamePaused(bool isGamePaused)
    {
        onGamePaused?.Invoke(isGamePaused);
    }
}
