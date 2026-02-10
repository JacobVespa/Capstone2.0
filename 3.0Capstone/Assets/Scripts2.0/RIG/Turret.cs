using System.Collections;
using UnityEngine;
using TMPro;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject crosshair;
    [SerializeField] private GameObject reloadNotif;
    [SerializeField] public GameObject buttonPromptXB;
    [SerializeField] public TextMeshPro ammoCountText;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSpawnLocation;

    [Header("VFX")]
    public GameObject muzzleFlash;
    [SerializeField] private ParticleSystem comicShot;

    [Header("Settings")]
    [SerializeField] public int maxAmmo = 10;
    [SerializeField] private float shootingCD = 1f;
    [SerializeField] float aimSpeed = 10.0f;
    [SerializeField] bool autoTarget = true;
    [SerializeField] private float bulletSpeed = 50f;

    // Boundary Caching
    private Rect camRect;
    private float lastScreenWidth;
    private float lastScreenHeight;
    private float objectWidth;
    private float objectHeight;

    private GameObject player;
    private PlayerControls currentControls;
    public int currentAmmo;
    public bool needsReload = false;
    private bool playerMounted = false;
    private bool canShoot = true;

    Vector2 aimPos;
    private LineRenderer lineRenderer;
    private DamageSource currentDamage;

    private void Start()
    {
        // Calculate initial object size from SpriteRenderer
        objectWidth = crosshair.transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        objectHeight = crosshair.transform.GetComponent<SpriteRenderer>().bounds.extents.y;

        UpdateCameraBounds();

        reloadNotif.SetActive(false);
        buttonPromptXB.SetActive(false);
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
        // Check if resolution changed mid-game
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateCameraBounds();
        }

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
            lineRenderer.enabled = false;
        }
    }

    private void UpdateCameraBounds()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        Camera cam = Camera.main;
        // Use nearClipPlane to get accurate world points relative to the camera view
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        camRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    private void ManualTarget()
    {
        if (currentControls == null) return;

        // Apply movement
        aimPos += currentControls.controlEvent.LookDirection * Time.deltaTime * aimSpeed;

        // Clamp to cached camera boundaries
        aimPos.x = Mathf.Clamp(aimPos.x, camRect.xMin + objectWidth, camRect.xMax - objectWidth);
        aimPos.y = Mathf.Clamp(aimPos.y, camRect.yMin + objectHeight, camRect.yMax - objectHeight);

        // Update visual
        crosshair.transform.position = aimPos;
    }

    public void Mount(GameObject p)
    {
        player = p;
        currentControls = player.GetComponent<PlayerControls>();
        crosshair.SetActive(true);
        buttonPromptXB.SetActive(false);
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

        if (currentControls.controlEvent.IsAttacking)
        {
            if (currentAmmo > 0 && canShoot)
            {
                ShootBullet();
                currentAmmo--;
                ammoCountText.text = currentAmmo.ToString();

                if (currentAmmo == 0)
                {
                    reloadNotif.SetActive(true);
                    needsReload = true;
                }

                StartCoroutine(ShootingVFX());
                StartCoroutine(CoolDown());
            }
        }
    }

    private void ShootBullet()
    {
        GameObject spawnedBullet = Instantiate(bullet, bulletSpawnLocation.transform.position, bulletSpawnLocation.transform.rotation);
        Rigidbody2D rb = spawnedBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-spawnedBullet.transform.right * bulletSpeed, ForceMode2D.Impulse);
    }

    private void Aim()
    {
        if (player != null && currentControls != null)
        {
            transform.LookAt(transform.position + Vector3.forward, (Vector3)aimPos - transform.position);
            transform.Rotate(new Vector3(0, 0, -90));
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, aimPos);
        }
    }

    private void DetermineTarget()
    {
        Vector2 currentPos = transform.position;
        Vector2 closestPos = currentPos;
        float closestDistanceSqr = Mathf.Infinity;

        var attackers = AttackQueueManager.instance.ActiveAttackers;
        if (attackers == null) return;

        foreach (var enemy in attackers)
        {
            if (enemy == null || enemy.gameObject.layer != 6) continue;

            Vector2 offset = (Vector2)enemy.transform.position - currentPos;
            float currentDistanceSqr = offset.sqrMagnitude;

            if (currentDistanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = currentDistanceSqr;
                closestPos = enemy.transform.position;
            }
        }

        if (closestDistanceSqr < Mathf.Infinity)
        {
            aimPos = closestPos;
            crosshair.transform.position = aimPos;
        }
    }

    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
        reloadNotif.SetActive(false);
        ammoCountText.text = maxAmmo.ToString();
    }

    // Helper Collision Methods
    public void HitEnemy(Collider2D col)
    {
        var body = col.GetComponent<EnemyBody>();
        if (body != null) { StartCoroutine(HitMarker(Color.red)); body.Attacked(currentDamage); }
    }

    public void HitGem(Collider2D col)
    {
        var gem = col.GetComponent<Resource>();
        if (gem != null) { StartCoroutine(HitMarker(Color.blue)); gem.Damage(); }
    }

    public void HitProjectile(Collider2D col)
    {
        var proj = col.GetComponent<Projectile>();
        if (proj != null) { StartCoroutine(HitMarker(Color.yellow)); proj.Attacked(currentDamage); }
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

    IEnumerator HitMarker(Color hitMarkerColor)
    {
        var crosshairSprite = crosshair.GetComponent<SpriteRenderer>();
        crosshairSprite.color = hitMarkerColor;
        yield return new WaitForSeconds(0.1f);
        crosshairSprite.color = Color.black;
    }
}