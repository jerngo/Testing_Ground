using System.Collections;
using UnityEngine;

public class PlayerBuffHandler : MonoBehaviour
{
    private PlayerController controller;
    private Coroutine speedBuffCoroutine;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        if (speedBuffCoroutine != null)
            StopCoroutine(speedBuffCoroutine);

        speedBuffCoroutine = StartCoroutine(SpeedBuffRoutine(multiplier, duration));
    }

    IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        float originalWalk = controller.walkSpeed;
        float originalRun = controller.runSpeed;

        controller.walkSpeed *= multiplier;
        controller.runSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        controller.walkSpeed = originalWalk;
        controller.runSpeed = originalRun;

        speedBuffCoroutine = null;
    }
}