using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyStepClimber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsule;

    [Header("Ground and obstacle layers")]
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Header("Step settings")]
    [SerializeField] private float maxStepHeight = 0.35f;
    [SerializeField] private float stepCheckDistance = 0.35f;
    [SerializeField] private float stepSmooth = 4f;
    [SerializeField] private float minMoveSpeed = 0.1f;

    [Header("Ground check")]
    [SerializeField] private float groundCheckDistance = 0.12f;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        if (rb == null || capsule == null)
            return;

        if (!IsGrounded())
            return;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        if (horizontalVelocity.magnitude < minMoveSpeed)
            return;

        Vector3 moveDir = horizontalVelocity.normalized;

        TryStepUp(moveDir);
    }

    private bool IsGrounded()
    {
        Vector3 center = transform.TransformPoint(capsule.center);

        float radius = Mathf.Max(0.01f, capsule.radius * 0.95f);
        float halfHeight = Mathf.Max(capsule.height * 0.5f - radius, 0.01f);

        Vector3 bottom = center + Vector3.down * halfHeight;

        return Physics.SphereCast(
            bottom + Vector3.up * 0.05f,
            radius,
            Vector3.down,
            out _,
            groundCheckDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void TryStepUp(Vector3 moveDir)
    {
        Vector3 center = transform.TransformPoint(capsule.center);

        float radius = capsule.radius * 0.95f;
        float bottomY = center.y - (capsule.height * 0.5f) + radius;

        Vector3 lowerOrigin = new Vector3(center.x, bottomY + 0.05f, center.z);
        Vector3 upperOrigin = lowerOrigin + Vector3.up * maxStepHeight;

        bool lowerHit = Physics.Raycast(
            lowerOrigin,
            moveDir,
            out RaycastHit lowerInfo,
            stepCheckDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        if (!lowerHit)
            return;

        bool upperHit = Physics.Raycast(
            upperOrigin,
            moveDir,
            stepCheckDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        if (upperHit)
            return;

        Vector3 stepTarget = rb.position + Vector3.up * (stepSmooth * Time.fixedDeltaTime);
        rb.MovePosition(stepTarget);
    }

    private void OnDrawGizmosSelected()
    {
        if (capsule == null)
            capsule = GetComponent<CapsuleCollider>();

        if (capsule == null)
            return;

        Vector3 center = transform.TransformPoint(capsule.center);
        float radius = capsule.radius * 0.95f;
        float bottomY = center.y - (capsule.height * 0.5f) + radius;

        Vector3 forward = transform.forward;

        Vector3 lowerOrigin = new Vector3(center.x, bottomY + 0.05f, center.z);
        Vector3 upperOrigin = lowerOrigin + Vector3.up * maxStepHeight;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(lowerOrigin, lowerOrigin + forward * stepCheckDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(upperOrigin, upperOrigin + forward * stepCheckDistance);
    }
}