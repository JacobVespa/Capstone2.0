using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject crosshair;
    [SerializeField] GameObject pivot;
    [SerializeField] public GameObject buttonPromptXB;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSpawnLocation;

    [Header("VFX")]
    public GameObject muzzleFlash;
    [SerializeField] private ParticleSystem comicShot;

    [Header("Settings")]
    public float shootingCD;
    [SerializeField] float aimSpeed = 10.0f;
    [SerializeField] bool autoTarget = true;
    [SerializeField] bool assistAim = true;
    [SerializeField] float assistAngle = 5f;
    [SerializeField] private float bulletSpeed = 50f;

    [Header("Bullet Sprites")]
    [SerializeField] private Sprite[] projectiles;

    private Camera cam;
    private float camHalfWidth;
    private float camHalfHeight;
    private float lastScreenWidth;
    private float lastScreenHeight;

    // Crosshair size (world units)
    private float objectWidth;
    private float objectHeight;

    private GameObject player;
    private PlayerControls currentControls;
    public int currentAmmo;
    public bool needsReload = false;
    private bool playerMounted = false;
    private bool canShoot = true;

    Vector2 aimPos;
    Vector3 originalPos;
    private LineRenderer lineRenderer;
    private DamageSource currentDamage;

    //turret scaling
    private Vector3 dismountedScale;
    private Vector3 mountedScale;

    private void Start()
    {
        cam = Camera.main;

        if (!cam.orthographic)
        {
            Debug.LogError("Turret requires an Orthographic camera.");
        }

        RecalculateCameraExtents();

        // Calculate crosshair size properly in world space
        SpriteRenderer sr = crosshair.GetComponent<SpriteRenderer>();
        objectWidth = sr.sprite.bounds.extents.x * crosshair.transform.lossyScale.x;
        objectHeight = sr.sprite.bounds.extents.y * crosshair.transform.lossyScale.y;

        buttonPromptXB.SetActive(false);

        aimPos = transform.position;
        originalPos = pivot.transform.localPosition;
        audioSource.clip = shootClip;
        currentDamage = GetComponent<DamageSource>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        dismountedScale = new Vector3(0.85f, 0.85f, 0.85f);
        mountedScale = Vector3.one;
    }

    private void Update()
    {
        // Recalculate if resolution/aspect changes
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            RecalculateCameraExtents();
        }

        if (playerMounted)
        {
            if (autoTarget)
                DetermineTarget();
           
            else
                ManualTarget();

            Aim();
            if (assistAim) { AdujstAim(); }
            Shoot();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void RecalculateCameraExtents()
    {
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void ClampAimToCamera()
    {
        Vector3 camPos = cam.transform.position;

        float minX = camPos.x - camHalfWidth + objectWidth;
        float maxX = camPos.x + camHalfWidth - objectWidth;
        float minY = camPos.y - camHalfHeight + objectHeight;
        float maxY = camPos.y + camHalfHeight - objectHeight;

        aimPos.x = Mathf.Clamp(aimPos.x, minX, maxX);
        aimPos.y = Mathf.Clamp(aimPos.y, minY, maxY);
    }

    private void ManualTarget()
    {
        if (currentControls == null) return;

        aimPos += currentControls.controlEvent.LookDirection * Time.deltaTime * aimSpeed;

        ClampAimToCamera();
        crosshair.transform.position = aimPos;
    }

    public void Mount(GameObject p)
    {
        player = p;
        currentControls = player.GetComponent<PlayerControls>();
        crosshair.SetActive(true);
        buttonPromptXB.SetActive(false);
        playerMounted = true;

        //testing size increase
        gameObject.transform.localScale = Vector3.Lerp(dismountedScale, mountedScale, 10f);

        aimPos = transform.position;
    }

    public void Dismount()
    {
        player = null;
        currentControls = null;
        crosshair.SetActive(false);
        playerMounted = false;

        //testing size decrease
        gameObject.transform.localScale = Vector3.Lerp(mountedScale, dismountedScale, 10f);
    }

    private void Shoot()
    {
        if (player == null || currentControls == null) return;

        if (currentControls.controlEvent.IsAttacking && canShoot)
        {
            Debug.Log($"SPEED SHOOWING CD IS {shootingCD}");
            ShootBullet();
            StartCoroutine(ShootingVFX());
            StartCoroutine(Recoil());
            StartCoroutine(CoolDown());
        }
    }

    private void ShootBullet()
    {
        int randomNum = UnityEngine.Random.Range(0, projectiles.Length);
        bullet.GetComponentInChildren<SpriteRenderer>().sprite = projectiles[randomNum];

        GameObject spawnedBullet = Instantiate(
            bullet,
            bulletSpawnLocation.transform.position,
            bulletSpawnLocation.transform.rotation);

        Rigidbody2D rb = spawnedBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(-spawnedBullet.transform.right * bulletSpeed, ForceMode2D.Impulse);
    }

    private Vector3 recoilCurrentOffset = Vector3.zero;

    private void Aim()
    {
        if (player != null && currentControls != null)
        {
            Vector2 direction = (Vector3)aimPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            pivot.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
            pivot.transform.localPosition = originalPos + recoilCurrentOffset;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, bulletSpawnLocation.transform.position);
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
            ClampAimToCamera();
            crosshair.transform.position = aimPos;
        }
    }

    private void AdujstAim()
    {
        ManualTarget();

        Vector2 dir = -pivot.transform.up;

        int enemies = LayerMask.GetMask("Enemy");
        int flyEnemies = LayerMask.GetMask("Flying Enemy");

        RaycastHit2D closestRay = Physics2D.Raycast(pivot.transform.position, dir * 10, Mathf.Infinity, enemies | flyEnemies);
        Vector2 closestDir = Vector2.zero;
        if(closestRay.collider != null)
        {
            closestDir = dir;
        }

        float angleAdjuster = -1;

        for (float x = 0; x <= assistAngle; x += angleAdjuster)
        {
            
            Vector2 rayDir = Quaternion.AngleAxis(x, Vector3.forward) * dir;
            RaycastHit2D r = Physics2D.Raycast(pivot.transform.position, rayDir * 10, Mathf.Infinity, enemies | flyEnemies);

            Debug.DrawLine(pivot.transform.position, rayDir * 100, Color.white);

            if (r.collider != null)
            {

                //if (r.distance > closestRay.distance)
                if (closestDir == Vector2.zero) 
                {
                    //Debug.Log(r.collider.name);
                    closestRay = r;
                    closestDir = rayDir;
                }
            }

            if (x <= -assistAngle)
            {
                angleAdjuster = 1;
            }
        }

        if (closestDir != Vector2.zero)
        {
            

            float angle = Mathf.Atan2(closestDir.y, closestDir.x) * Mathf.Rad2Deg;
            
            pivot.transform.rotation = Quaternion.Euler(0,0,angle + 90);
            

        }

    }

    

    // Helper Collision Methods (unchanged)
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

    public void HitGrass(Collider2D col)
    {
        Debug.LogWarning("GRASS HERE!!!");
        var proj = col.GetComponent<Grass>();
        if (proj != null) { StartCoroutine(HitMarker(Color.green)); proj.grassClipped(); }
    }

    IEnumerator ShootingVFX()
    {
        muzzleFlash.SetActive(true);
        comicShot.Play();
        audioSource.Play();
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator Recoil()
    {
        Vector3 startPos = originalPos;
        // Push back along the pivot's current facing direction in local space
        Vector3 recoilDir = pivot.transform.parent.InverseTransformDirection(pivot.transform.up);
        Vector3 recoilOffset = startPos + recoilDir * 0.2f;

        float recoilTime = 0.05f;
        float returnTime = 0.25f;
        float elapsed = 0f;

        while (elapsed < recoilTime)
        {
            pivot.transform.localPosition = Vector3.Lerp(startPos, recoilOffset, elapsed / recoilTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < returnTime)
        {
            pivot.transform.localPosition = Vector3.Lerp(recoilOffset, startPos, elapsed / returnTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pivot.transform.localPosition = originalPos;
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