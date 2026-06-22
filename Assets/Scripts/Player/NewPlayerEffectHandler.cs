
using UnityEngine;
using UnityEngine.Events;

public class NewPlayerEffectHandler : MonoBehaviour
{
    public NewBasicPlatformerController2D eventSource;
    public UnityEvent OnJump, OnLand, OnHardLand;

    public void Awake()
    {
        if (eventSource == null)
            return;

        eventSource.OnLand += OnLand.Invoke;
        eventSource.OnJump += OnJump.Invoke;
        eventSource.OnHardLand += OnHardLand.Invoke;
    }
}
