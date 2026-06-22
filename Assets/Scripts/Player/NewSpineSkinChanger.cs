using Spine;
using Spine.Unity;
using UnityEngine;

public class NewSpineSkinChanger : MonoBehaviour
{
    [SerializeField] SkeletonRenderer skeletonRenderer;

    private void Start()
    {
        SetMorningstar();
    }

    public void SetSword()
    {
        SetWeapon("weapon/sword");
    }

    public void SetMorningstar()
    {
        SetWeapon("weapon/morningstar");
    }

    void SetWeapon(string weaponSkinName)
    {
        SkeletonData data = skeletonRenderer.Skeleton.Data;

        Skin customSkin = new Skin("custom");
        //customSkin.AddSkin(data.FindSkin("default"));
        customSkin.AddSkin(data.FindSkin(weaponSkinName));

        skeletonRenderer.Skeleton.SetSkin(customSkin);
        skeletonRenderer.Skeleton.SetupPoseSlots();

        skeletonRenderer.LateUpdate();
    }
}
