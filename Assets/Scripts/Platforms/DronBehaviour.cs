using UnityEngine;

public class DronBehaviour : MonoBehaviour
{
    [SerializeField] private LightingPathManager lightingPathManager;
    [Header("Points")]
    [SerializeField] private int startPoint;
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform firstPosition;

    private Vector3 direction;
    private int i;
    private bool ifIsLight;

    private MovementBehaviour MB;
    void Start()
    {
        MB = GetComponent<MovementBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        ifIsLight = lightingPathManager.GetisLight();
        if (!ifIsLight)
        {
            if (Vector2.Distance(transform.position, points[i].position) < 0.01f)
            {
                i++;
                if (i == points.Length)
                {
                    i = 0;
                }
            }
            direction = points[i].position - transform.position;
            MB.Move(direction);
        }
        else
        {
            direction = firstPosition.position - transform.position;
            MB.Move(direction);
            if(Vector2.Distance(firstPosition.position, points[i].position) < 0.01f){
                MB.SetSpeed(0);

            }
        }
    }
}
