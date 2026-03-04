using UnityEngine;

[RequireComponent(typeof(EnemyBody))]
public class AttackQueueAgent : MonoBehaviour
{
    [SerializeField] private EnemyBody body;

    private AttackQueueManager manager;
    private Transform rig;

    private bool inAttackZone = false;
    private bool canDealDamage = false;

    public float TimeEnteredQueue { get; private set; }
    public float DistanceToRig => rig == null ? float.MaxValue : Vector3.Distance(transform.position, rig.position);

    public bool IsEligible
    {
        get
        {
            // ActiveSelf check works with your current "Death() => SetActive(false)"
            if (!gameObject.activeSelf) return false;
            if (!inAttackZone) return false;
            if (rig == null) return false;
            return true;
        }
    }

    private void Awake()
    {
        if (body == null) body = GetComponent<EnemyBody>();
    }

    public void Initialize(AttackQueueManager mgr, Transform rigTransform)
    {
        manager = mgr;
        rig = rigTransform;
    }

    public void OnEnterAttackZone(AttackQueueManager mgr, Transform rigTransform)
    {
        manager = mgr;
        rig = rigTransform;
        inAttackZone = true;
        TimeEnteredQueue = Time.time;

        manager.Register(this);
    }

    public void OnExitAttackZone()
    {
        inAttackZone = false;

        if (manager != null)
            manager.Unregister(this);

        manager = null;
        rig = null;

        SetCanDealDamage(false);
    }

    public void SetCanDealDamage(bool allowed)
    {
        canDealDamage = allowed;

        // Hook player readability
        // show exclamation mark / outline / etc.
        // Pseudo: body.SetThreatIndicator(allowed);


        // Visual debug: tint sprite
        /*
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = allowed ? Color.red : Color.white;
        */
    }

    public bool CanDealDamage => canDealDamage;
}
