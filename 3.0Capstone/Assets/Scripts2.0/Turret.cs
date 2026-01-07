using System.Collections;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{

    [SerializeField] GameObject crosshair;
    private GameObject player;
    private PlayerControls currentControls;

    private bool playerMounted = false;

    Vector2 aimPos;
    RaycastHit2D hit;

    [SerializeField] float aimSpeed = 20.0f;

    //reference to the muzzle flash vfx
    public GameObject muzzleFlash;

    private void Start()
    {
        aimPos = transform.position;
    }

    private void Update()
    {
        if (playerMounted)
        {
            Aim();
            Shoot();
        }
    }

    public void Mount(GameObject p)
    {
        player = p;
        currentControls = player.GetComponent<PlayerControls>();
        crosshair.SetActive(true);
        playerMounted = true;
    }

    public void Dismount()
    {
        player = null;
        currentControls = null;
        crosshair.SetActive(false);
        playerMounted = false;
    }

    private void Shoot()
    {
        if (player != null && currentControls != null)
        {
            if (currentControls.controlEvent.HasAttacked)
            {
                StartCoroutine(ShootingVFX());
                hit = Physics2D.Raycast(transform.position, aimPos);

                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy hit!");
                    
                }

            }
        }
    }

    private void Aim()
    {
        if (player != null && currentControls != null)
        {
            aimPos += currentControls.controlEvent.LookDirection * Time.deltaTime * aimSpeed;
            crosshair.transform.position = aimPos;
        }
    }

    IEnumerator ShootingVFX()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForEndOfFrame();
        muzzleFlash.SetActive(false);
    }

}
