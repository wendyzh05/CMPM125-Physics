using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObstacle : MonoBehaviour
{
    public float moveDistance = 5f;
    public float speed = 2f;

    private Vector3 startPos;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = rb.position;

        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        Vector3 right = transform.right;

        float movement = Mathf.Sin(Time.time * speed) * moveDistance;

        Vector3 targetPos = startPos + right * movement;

        rb.MovePosition(targetPos);
    }
}