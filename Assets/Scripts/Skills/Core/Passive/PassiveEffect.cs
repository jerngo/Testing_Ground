using UnityEngine;

public enum PassiveTrigger
{
    Always,
    OnLowHP,
    OnKillEnemy,
    OnTakeDamage
}

public abstract class PassiveEffect : ScriptableObject
{
    public PassiveTrigger trigger;
    public abstract void OnActivate(GameObject owner);
    public virtual void OnDeactivate(GameObject owner) { }
}