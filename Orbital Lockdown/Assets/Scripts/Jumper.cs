using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class SimpleRigidbodyJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsule;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundCheckDistance = 0.15f;

    private bool jumpRequested = false;

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

    private void Update()
    {
        if (Input.GetKeyDown(jumpKey) && IsGrounded())
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        if (!jumpRequested) return;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpRequested = false;
    }

    private bool IsGrounded()
    {
        Vector3 center = transform.TransformPoint(capsule.center);
        float radius = capsule.radius * 0.9f;
        float halfHeight = (capsule.height * 0.5f) - radius;

        Vector3 origin = center + Vector3.down * halfHeight + Vector3.up * 0.05f;

        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }
}