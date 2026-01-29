using UnityEngine;


public interface IDamageReceiver
{
    public GameObject gameObject { get; }
    public void Attacked(DamageSource d);
}
