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

    [SerializeField] private float shootingCD = 1f;
    bool canShoot = true;

    Vector2 aimPos;
    RaycastHit2D hit;
    DamageSource currentDamage;

    [SerializeField] float aimSpeed = 10.0f;
    [SerializeField] bool autoTarget = true;

    //reference to the muzzle flash vfx
    public GameObject muzzleFlash;
    [SerializeField] private ParticleSystem comicShot;

    //Line Renderer for raycast on screen
    private LineRenderer lineRenderer;

    //BULLET STUFF
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSpawnLocation;
    private float bulletSpeed = 50f;

    private void Start()
    {
        reloadNotif.SetActive(false);
        currentAmmo = maxAmmo;

        aimPos = transform.position;
        audioSource.clip = shootClip;
        currentDamage = GetComponent<DamageSource>();
        ammoCountText.text = maxAmmo.ToString();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (playerMounted)
        {
            Aim();
            Shoot();
            if (autoTarget)
                DetermineTarget();
            else
                ManualTarget();
        }
        else
        {
            lineRenderer.enabled = false; //probably a better way to do this
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
        if (currentAmmo > 0 && canShoot)
        {
            ShootBullet();
            currentAmmo--;
            ammoCountText.text = currentAmmo.ToString();
            if (currentAmmo ==0)
            {
                reloadNotif.SetActive(true);
                needsReload = true; //test
            }

            StartCoroutine(ShootingVFX());
            StartCoroutine(CoolDown());

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

    private void ShootBullet()
    {
        GameObject spawnedBullet = Instantiate(bullet, bulletSpawnLocation.transform.position, bulletSpawnLocation.transform.rotation);
        Debug.Log("Spawned a bullet.");

        Rigidbody2D rb = spawnedBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-spawnedBullet.transform.right * bulletSpeed, ForceMode2D.Impulse);
    }

    private void Aim()
    {
        if (player != null && currentControls != null)
        {
            transform.LookAt(transform.position + Vector3.forward, crosshair.transform.position - transform.position); //maybe?
            transform.Rotate(new Vector3(0, 0, -90));
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, aimPos);
        }
    }

    private void DetermineTarget()
    {
        Vector2 currentPos = transform.position; // Cache position
        Vector2 closestPos = currentPos;
        float closestDistanceSqr = Mathf.Infinity; // Initialize to Infinity

        var attackers = AttackQueueManager.instance.ActiveAttackers;
        if (attackers == null) return;

        foreach (var enemy in attackers)
        {
            if (enemy == null || enemy.gameObject.layer != 6) continue;

            // Use subtraction + sqrMagnitude
            Vector2 offset = (Vector2)enemy.transform.position - currentPos;
            float currentDistanceSqr = offset.sqrMagnitude;

            if (currentDistanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = currentDistanceSqr;
                closestPos = enemy.transform.position;
            }
        }

        // Only update if a valid target was found
        if (closestDistanceSqr < Mathf.Infinity)
        {
            aimPos = closestPos;
            crosshair.transform.position = aimPos;
        }
    }

    private void ManualTarget()
    {
        aimPos += currentControls.controlEvent.LookDirection * Time.deltaTime * aimSpeed;
        crosshair.transform.position = aimPos;
    }

    IEnumerator ShootingVFX()
    {
        muzzleFlash.SetActive(true);
        comicShot.Play();
        audioSource.Play();
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator CoolDown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootingCD);
        canShoot = true;
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
