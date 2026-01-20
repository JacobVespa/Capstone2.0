using System.Collections;
using UnityEngine;
using TMPro;

public class Turret : MonoBehaviour
{
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject crosshair;
    private GameObject player;
    private PlayerControls currentControls;

    [SerializeField] public int maxAmmo = 10;
    public int currentAmmo;
    [SerializeField] private GameObject reloadNotif;
    public bool needsReload = false;
    [SerializeField] public TextMeshPro ammoCountText;

    private bool playerMounted = false;

    Vector2 aimPos;
    RaycastHit2D hit;
    DamageSource currentDamage;

    [SerializeField] float aimSpeed = 20.0f;

    //cooldown between shots
    //[SerializeField] float fireRate = 10f;
    //float shotCooldown;

    //reference to the muzzle flash vfx
    public GameObject muzzleFlash;
    [SerializeField] private ParticleSystem comicShot;

    private void Start()
    {
        reloadNotif.SetActive(false);
        currentAmmo = maxAmmo;

        aimPos = transform.position;
        audioSource.clip = shootClip;
        currentDamage = GetComponent<DamageSource>();
        ammoCountText.text = maxAmmo.ToString();
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
        if (player == null || currentControls == null) return;
        if (!currentControls.controlEvent.HasAttacked) return;
        if (currentAmmo > 0)
        {
            currentAmmo--;
            ammoCountText.text = currentAmmo.ToString();
            if (currentAmmo ==0)
            {
                reloadNotif.SetActive(true);
                needsReload = true; //test
            }

            StartCoroutine(ShootingVFX());

            Vector3 origin = transform.position;
            Vector3 direction = (aimPos - (Vector2)origin).normalized;

            hit = Physics2D.Raycast(origin, direction, 100f, 64);
            
            if (hit) //layer 6 is enemy layer
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.CompareTag("Enemy"))
                {
                    var body = hit.collider.GetComponent<EnemyBody>();
                    if (body != null)
                    {
                        StartCoroutine(HitMarker(Color.red));
                        body.Attacked(currentDamage);
                    }
                }

                if (hit.collider.CompareTag("Gem"))
                {
                    var gem = hit.collider.GetComponent<Resource>();
                    if (gem != null)
                    {
                        StartCoroutine(HitMarker(Color.blue));
                        gem.Damage();
                    }
                }

                if (hit.collider.CompareTag("Projectile")){
                    var proj = hit.collider.GetComponent<Projectile>();
                    if(proj != null)
                    {
                        StartCoroutine(HitMarker(Color.yellow));
                        proj.Attacked(currentDamage);
                    }
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
            transform.LookAt(transform.position + Vector3.forward, crosshair.transform.position - transform.position); //maybe?
            transform.Rotate(new Vector3(0, 0, -90));
        }
    }

    IEnumerator ShootingVFX()
    {
        muzzleFlash.SetActive(true);
        comicShot.Play();
        audioSource.Play();
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        reloadNotif.SetActive(false);
        ammoCountText.text = maxAmmo.ToString();
    }

    IEnumerator HitMarker(Color hitMarkerColor)
    {
        var crosshairSprite = crosshair.GetComponent<SpriteRenderer>();
        crosshairSprite.color = hitMarkerColor;
        yield return new WaitForSeconds(0.1f);
        crosshairSprite.color = Color.black;
    }

}
