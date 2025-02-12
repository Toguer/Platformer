using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody RB;
    private float walkSpeed;

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

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
    public void Move(Vector3 d)
    {
        //Movimineto de las plataformas (ascensores) 
        d.Normalize();
        transform.position += d * speed * Time.deltaTime;
    }


}
