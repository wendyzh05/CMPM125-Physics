using UnityEngine;
using UnityEngine.InputSystem;

public class marbleController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float torqueMultiplier = 5f;
    [SerializeField] private float maxAngularVelocity = 25f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float linearDamping = 0.1f;

    private Rigidbody _rb;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("marbleController requires a Rigidbody on the same GameObject.");
            enabled = false;
            return;
        }

        _rb.maxAngularVelocity = maxAngularVelocity;
        _rb.linearDamping = linearDamping;
        _rb.useGravity = true;

        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        Vector3 torque = new Vector3(_moveInput.x, 0f, _moveInput.y) * torqueMultiplier;
        _rb.AddTorque(torque, ForceMode.Force);
        
        if (_rb.linearVelocity.magnitude > maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
    }

    private void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }
}
