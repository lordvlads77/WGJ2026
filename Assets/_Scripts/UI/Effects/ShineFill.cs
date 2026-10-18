using UnityEngine;

public class ShineFill : MonoBehaviour
{
    private UIGradientAnimator _shine;

    private void OnEnable()
    {
        _shine.Play();
    }

    private void OnDisable()
    {
        _shine.Stop();
    }
}
