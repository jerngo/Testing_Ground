using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class PlayerSpineEventHandler : MonoBehaviour
{
    public event Action OnAttackHit;

    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] PlayerSpineEventHandler spineEventHandler;

    const int ActionTrack = 1;
    const string AttackHitEventName = "AttackHit";

    void Awake()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        skeletonAnimation.AnimationState.Event += HandleSpineEvent;
    }

    void OnDestroy()
    {
        if (skeletonAnimation != null)
            skeletonAnimation.AnimationState.Event -= HandleSpineEvent;
    }

    void HandleSpineEvent(TrackEntry entry, Spine.Event e)
    {
        Debug.Log($"Event: {e.Data.Name}");

        if (entry.TrackIndex != ActionTrack)
            return;

        if (e.Data.Name == AttackHitEventName)
            OnAttackHit?.Invoke();
    }
}