using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private float damageVal;
    public float DamageVal { get { return damageVal; } set { damageVal = value; } }

    [SerializeField] private DamageType damageTarget = DamageType.None;
    public DamageType DamageTarget { get { return damageTarget; }  set { damageTarget = value; } }

    public enum DamageType
    {
        None = 0,
        Enemy = 1,
        Player = 2,
    }
       
    

}
