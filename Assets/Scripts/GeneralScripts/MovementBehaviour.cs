using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody RB;
    private float walkSpeed;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        walkSpeed = speed;
    }

    public void MoveIsometric(Vector3 input)
    {
        input.y = 0;
        Vector3 velocityXZ = input.normalized * speed;
        RB.linearVelocity = new Vector3(velocityXZ.x, RB.linearVelocity.y, velocityXZ.z);
    }
}
