using UnityEngine;
using Unity.Cinemachine;

public class CameraFollowPlayer : MonoBehaviour
{
    CinemachineCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
        vcam.enabled = false; // matikan dulu saat awal

        CharacterManager.Instance.OnCharacterSpawned += OnCharacterSpawned;
    }

    void OnDestroy()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSpawned -= OnCharacterSpawned;
    }

    void OnCharacterSpawned(GameObject character)
    {
        Transform playerChild = character.transform.GetChild(0);
        vcam.Follow = playerChild;
        vcam.LookAt = playerChild;

        // Teleport camera langsung ke posisi target, skip lerp
        vcam.enabled = true;
        
    }
}