using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 5f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 right = transform.right;

        float movement = Mathf.Sin(Time.time * speed) * moveDistance;

        transform.position = startPos + right * movement;
    }
}