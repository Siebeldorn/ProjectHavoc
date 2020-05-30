using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Public

    [Header("Movement")]
    public float speedMs = 10.0f;
    public float maxYawAngle = 30;
    public float maxRollAngle = 45;
    public float rotationSpeed = 10f;

    [Space]

    [Header("Debug")]
    public bool disableMovement = false;
    public bool disableRotation = false;

    #endregion Public
    #region Private

    private Rigidbody rigidBody;

    // calculated in Update(), applied in UpdateFixed()
    private Vector3 movementDelta = Vector3.zero;
    private float yawAngle = 0;
    private float pitchAngle = 0;
    private float rollAngle = 0;

    #endregion Private

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var horz = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");

        if ( !disableMovement )
        {
            movementDelta.Set(horz, vert, 0);
        }

        if ( !disableRotation )
        {
            yawAngle = Mathf.LerpAngle(yawAngle, -vert * maxYawAngle, rotationSpeed * Time.deltaTime);
            pitchAngle = Mathf.LerpAngle(pitchAngle, horz * maxRollAngle, rotationSpeed * Time.deltaTime);
            rollAngle = pitchAngle * -0.5f;
        }
    }

    void FixedUpdate()
    {
        rigidBody.MovePosition(transform.position + movementDelta * speedMs * Time.deltaTime);
        rigidBody.MoveRotation(Quaternion.Euler(yawAngle, pitchAngle, rollAngle));
    }
}
