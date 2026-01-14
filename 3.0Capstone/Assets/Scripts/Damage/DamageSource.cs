using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [Header("Damage Settings")]
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

    private void Start()
    {
        if(damageTarget == 0)
        {
            Debug.LogError("DamageTarget was set to none");
        }
        if(damageVal == 0)
        {
            Debug.LogError("DamageVal was not set or set to 0");
        }

    }

}
