using UnityEngine;

public class RepairPatch : MonoBehaviour
{
    public bool isPatched = false;
    [SerializeField] private SpriteRenderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (renderer == null) Debug.Log("Patch has no renderer");

        renderer.enabled = false;
    }

    public void ActivatePatch()
    {
        renderer.enabled = true;
        isPatched = true;
    }

    public void DeactivatePatch()
    {
        renderer.enabled = false;
        isPatched = false;
    }

    
}
