using Spine;
using Spine.Unity;
using UnityEngine;

public class NewAudioEventHandler : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public NewBasicPlatformerController2D playerController;

    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string eventName;

    [Space]
    public AudioSource audioSource;
    public AudioClip audioClip;
    public float basePitch = 1f;
    public float randomPitchOffset = 0.1f;

    [Space]
    public bool logDebugMessage = false;

    Spine.EventData eventData;

    void OnValidate()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (playerController == null)
            playerController = GetComponentInParent<NewBasicPlatformerController2D>();
    }

    void Awake()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (playerController == null)
            playerController = GetComponentInParent<NewBasicPlatformerController2D>();
    }

    void Start()
    {
        if (audioSource == null) return;
        if (skeletonAnimation == null) return;

        skeletonAnimation.Initialize(false);

        if (!skeletonAnimation.IsValid) return;

        eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);

        skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
    }

    void OnDestroy()
    {
        if (skeletonAnimation != null && skeletonAnimation.AnimationState != null)
            skeletonAnimation.AnimationState.Event -= HandleAnimationStateEvent;
    }

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (logDebugMessage)
            Debug.Log("Event fired! " + e.Data.Name);

        bool eventMatch = eventData == e.Data;

        if (!eventMatch)
            return;

        if (playerController != null && !playerController.IsGrounded)
            return;

        Play();
    }

    public void Play()
    {
        if (audioClip == null)
            return;

        audioSource.pitch = basePitch + Random.Range(-randomPitchOffset, randomPitchOffset);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}