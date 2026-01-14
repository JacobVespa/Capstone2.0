using UnityEngine;

public class AttackZone : MonoBehaviour
{
    [SerializeField] private AttackQueueManager manager;
    [SerializeField] private Transform rigRoot;

    private void Awake()
    {
        if (manager == null) manager = GetComponentInParent<AttackQueueManager>();
        if (rigRoot == null) rigRoot = manager != null ? manager.transform : transform.root;
    }

    private void OnTriggerEnter(Collider other)
    {
        AttackQueueAgent agent = other.GetComponentInParent<AttackQueueAgent>();
        if (agent != null && manager != null)
        {
            agent.OnEnterAttackZone(manager, rigRoot);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AttackQueueAgent agent = other.GetComponentInParent<AttackQueueAgent>();
        if (agent != null)
        {
            agent.OnExitAttackZone();
        }
    }
}
