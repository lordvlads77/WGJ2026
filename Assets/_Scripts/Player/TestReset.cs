using System;
using UnityEngine;

public class TestReset : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out ResetPlayer resetPlayer))
        {
            resetPlayer.PlayerReset();
        }
    }
}
