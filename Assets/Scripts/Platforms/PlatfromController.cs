using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatfromController : MonoBehaviour
{
    [Header("Bool")]
    [SerializeField] private bool waitATime;
    [SerializeField] private bool isLadderButton;

    [Header("Points")]
    [SerializeField] private int startPoint;
    [SerializeField] private Transform[] points;
    private Vector3 direction;
    private int i;

    [Header("Cool Down")]
    [SerializeField] private float timeBetween;
    private float coolDown = 0;
    private bool isCooldown;

    private MovementBehaviour MB;

    public void SetTimeBetween(float timeBetween)
    {
        this.timeBetween = timeBetween;
    }
    public void SetIsLadder(bool isLadderButton)
    {
        this.isLadderButton = isLadderButton;
    }

    void Start()
    {
        isCooldown = false;
        MB = GetComponent<MovementBehaviour>();
    }

    void Update()
    {
        // movimiento de plataformas a distintos puntos
        if (!waitATime)
        {
            //este es como un ascensor que sube y baja depenciendo que llegue a al siguente punto, tambien se peude utilizar en movimiento lateral
            if (Vector2.Distance(transform.position, points[i].position) < 0.01f)
            {
                i++;
                if (i == points.Length)
                {
                    i = 0;
                }
            }
        }
        else
        {
            // este se utliliza para que la platafroma acceda al siguiente punto no de manera instantanea, sino que haya un periodo de tiempo, es decir, llega a un punto, se detiene x tiempo y pasa al siguiente
            if (Vector2.Distance(transform.position, points[i].position) < 0.01f)
            {
                MB.SetSpeed(0);
            }
            else
            {
                MB.SetSpeed(3);
            }
            coolDown -= Time.deltaTime;
            movementPlatfrom();
        }
        direction = points[i].position - transform.position;
        MB.Move(direction);
    }

    private void movementPlatfrom()
    {
        if (!isCooldown)
        {
            if (coolDown <= 0)
            {
                i++;
                isCooldown = true;
                coolDown = timeBetween;
                if (i >= points.Length)
                {
                    i = 0;
                    if (isLadderButton)
                    {
                        StartCoroutine(_OnDesactive());
                    }
                }
            }
        }
        else
        {
            isCooldown = false;
        }
    }

    IEnumerator _OnDesactive()
    {
        yield return new WaitForSeconds(2.0f);
        transform.parent.gameObject.SetActive(false);

    }

    public void resetPlatform()
    {
        isCooldown = false;
        coolDown = 0;
        i = 0;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(this.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
        }
    }
}