using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TurretController : MonoBehaviour
{
    #region public editor fields

    [Space]

    [Header("Movement")]

    [Tooltip("The name of the Tranform representing the turret dome")]
    public string domeName;

    [Tooltip("The amount of degrees to rotate per seconds")]
    public float rotationSpeed = 42f;

    [Space]

    [Header("Attack")]

    [Tooltip("The tag of the GameObject to aim at")]
    public string targetTag = "Player";

    [Tooltip("The seconds to wait after a scan when no target were found before doing the next scan")]
    public float scanInterval = 1f;

    [Tooltip("The maximum radius around the turret to scan for a target")]
    public float attackRange = 20f;

    [Tooltip("The number of shots to fire per attack phase")]
    public int shotsPerSalvo = 3;

    [Tooltip("The type of ammonition to use per shoot")]
    public GameObject bullet;

    [Tooltip("The amount of seconds to wait after a complete salvo")]
    public float reloadTime = 1f;

    [Tooltip("The begin of the identifier for gameobjects representing muzzle used to spawn bullets at theír position")]
    public string muzzlePrefix = "muzzle_";

    #endregion public editor fields
    #region init

    void Start()
    {
        Debug.Assert(bullet != null, "Bullet prefab not set");
        // find and validate components

        animator = GetComponentInChildren<Animator>();
        Debug.Assert(animator != null, "Cannot find Animator component for TurretController");

        dome = transform.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == domeName);
        Debug.Assert(dome != null, "Cannot find turret dome with name " + domeName);

        muzzles = transform.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith(muzzlePrefix)).ToList();
        if ( muzzles.Count == 0 )
            Debug.LogWarning("No transforms for muzzles found, no bullets will spawn!");

        // init fields

        shotsRemaining = shotsPerSalvo;
        rangeSqr = attackRange * attackRange;
    }

    #endregion init
    #region logic

    void Update()
    {
        UpdateFlow();
    }

    private void UpdateFlow()
    {
        // skip if we are not idle, i.e. shooting
        if ( animator == null || !animator.GetCurrentAnimatorStateInfo(0).IsName("idle") )
            return;

        switch ( flow )
        {
            case FlowState.Scan:
                ScanForTarget();
                break;
            case FlowState.Aim:
                AimAtTarget();
                break;
            case FlowState.Shoot:
                ShootAtTarget();
                break;
            case FlowState.Reload:
                Reload();
                break;
        }
    }

    private void ScanForTarget()
    {
        scanTimer -= Time.deltaTime;
        if ( scanTimer <= 0 )
        {
            targetPosition = GameObject.FindWithTag(targetTag)?.transform?.position ?? Vector3.zero;
            if ( targetPosition != Vector3.zero && (targetPosition - transform.position).sqrMagnitude <= rangeSqr )
                flow = FlowState.Aim;
            else
                scanTimer = scanInterval; // scan failed, wait for rescan to not spam GameObject.FindWithTag()
        }
    }

    private void AimAtTarget()
    {
        // rotation to target, but keep z axis, use RotateTowards for constant movement
        // Todo: lookAt rotation does not change between target scans (turrent is fix, target position not updated), so that could be calculated once before aiming
        var lockAt = Quaternion.LookRotation(targetPosition - dome.position);
        var lookAt_xy = Quaternion.Euler(lockAt.eulerAngles.x, lockAt.eulerAngles.y, 0);
        dome.localRotation = Quaternion.RotateTowards(dome.localRotation, lookAt_xy, rotationSpeed * Time.deltaTime);

        // check if rotation is done and continue with next state
        if ( Mathf.Abs(Quaternion.Dot(dome.localRotation, lookAt_xy)) >= 0.9999f )
            flow = FlowState.Shoot;
    }

    private void ShootAtTarget()
    {
        // start shoot animation for recoil
        animator.Play("shoot");

        // spawn a bullet for each muzzle
        foreach ( var muzzle in muzzles )
            Instantiate(bullet, muzzle.position, dome.rotation);

        // "consume" 1 attack of salvo and check for continue
        shotsRemaining--;
        if ( shotsRemaining <= 0 )
        {
            shotsRemaining = shotsPerSalvo;
            reloadTimer = reloadTime;
            flow = FlowState.Reload;
        }
    }

    private void Reload()
    {
        reloadTimer -= Time.deltaTime;
        if ( reloadTimer <= 0f )
            flow = FlowState.Scan;
    }

    #endregion logic

    #region private fields

    private enum FlowState
    {
        Scan = 0,   // find desired gameobject, save position and test whether it's in range
        Aim,        // rotate towards target with constant speed
        Shoot,      // fire salvo towards target position
        Reload      // rest for a short moment to reload, then back to scan
    }

    private FlowState flow = FlowState.Scan;

    private Vector3 targetPosition;

    private Transform dome;

    private Animator animator;

    private float reloadTimer = 0;

    private float scanTimer = 0;

    private float rangeSqr;

    private int shotsRemaining;

    private List<Transform> muzzles = new List<Transform>();

    #endregion private fields
}
