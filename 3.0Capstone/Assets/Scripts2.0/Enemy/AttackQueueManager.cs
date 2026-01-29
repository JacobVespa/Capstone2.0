using System.Collections.Generic;
using UnityEngine;

public class AttackQueueManager : MonoBehaviour
{
    public static AttackQueueManager instance {  get; private set; }

    

    [Header("Runtime (Read Only)")]
    [SerializeField] private List<string> activeNames = new List<string>();
    [SerializeField] private List<string> waitingNames = new List<string>();


    [Header("Attack Slots")]
    [SerializeField] private int maxAttackers = 2;

    // Active attackers currently allowed to deal damage
    private readonly List<AttackQueueAgent> active = new List<AttackQueueAgent>();
    public List<AttackQueueAgent> ActiveAttackers { get { return active; }}

    // Candidates in range who want a slot
    private readonly List<AttackQueueAgent> waiting = new List<AttackQueueAgent>();

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this); return; }
        instance = this;
    }

    private void Update()
    {
        CleanupLists();
        FillSlots();
        RefreshDebugLists();
    }

    private void RefreshDebugLists()
    {
        activeNames.Clear();
        waitingNames.Clear();

        foreach (var a in active)
            if (a != null) activeNames.Add(a.name);

        foreach (var w in waiting)
            if (w != null) waitingNames.Add(w.name);
    }

    public void Register(AttackQueueAgent agent)
    {
        if (agent == null) return;
        if (!waiting.Contains(agent) && !active.Contains(agent))
        {
            waiting.Add(agent);
        }
            
        
    }

    public void Unregister(AttackQueueAgent agent)
    {
        if (agent == null) return;

        bool wasActive = active.Remove(agent);
        if (wasActive)
        {
            agent.SetCanDealDamage(false);
            FillSlots();
        }

        
    }

    private void FillSlots()
    {
        // If we already have max attackers, nothing to do
        if (active.Count >= maxAttackers) return;

        // Sort waiting by priority: closest first, then longest waiting

        waiting.Sort((a, b) =>
        {
            /*
            if (a == null || b == null) return 0;
            float da = a.DistanceToRig;
            float db = b.DistanceToRig;

            int distCompare = da.CompareTo(db);
            if (distCompare != 0) return distCompare;
            */

            // Older waiting gets priority if distances are equal-ish
            return a.TimeEnteredQueue.CompareTo(b.TimeEnteredQueue);
        });

        while (active.Count < maxAttackers && waiting.Count > 0)
        {
            AttackQueueAgent next = waiting[0];
            waiting.RemoveAt(0);

            if (next == null || !next.IsEligible) continue;

            active.Add(next);
            next.SetCanDealDamage(true);
            
        }
    }

    private void CleanupLists()
    {
        // Remove dead/null/ineligible agents from waiting and active
        for (int i = waiting.Count - 1; i >= 0; i--)
        {
            if (waiting[i] == null || !waiting[i].IsEligible)
                waiting.RemoveAt(i);
        }

        for (int i = active.Count - 1; i >= 0; i--)
        {
            if (active[i] == null || !active[i].IsEligible)
            {
                if (active[i] != null) active[i].SetCanDealDamage(false);
                active.RemoveAt(i);
                

            }
        }
    }




}
