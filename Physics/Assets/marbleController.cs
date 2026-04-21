using UnityEngine;
using UnityEngine.InputSystem;

public class marbleController : MonoBehaviour
{
    float desired_acceleration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddRelativeForce(desired_acceleration*10, 0, 0);
    }
    void OnMove(InputValue action)
    {
        var movement = action.Get<Vector2>();
        desired_acceleration = movement.y;
    }

}
