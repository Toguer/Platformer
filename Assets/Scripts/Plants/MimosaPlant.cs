using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MimosaPlant : MonoBehaviour
{
    [SerializeField] private float durationBox;
    [SerializeField] private float finalTargerX;


    [SerializeField] private List<BoxCollider> boxList = new List<BoxCollider>();

    private BoxCollider boxCollider;
    private float initialSizeX;
    private float targetSizeX;
    private float duration;
    private float elapsedTime = 0f;
    private bool isReducing = false;

    [Header("CoolDown")]
    [SerializeField] private float timeBetweenCollider;
    private int i;
    private float coolDown = 0;
    private bool isCooldown;
    private bool isAllBox;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("No hay un BoxCollider en este objeto.");
            return;
        }
        initialSizeX = boxCollider.size.x;

        coolDown = timeBetweenCollider;
        i = 0;
        AdjustGameObjects();
    }

    void Update()
    {
        if (isReducing & !isAllBox)
        {
            //reducieCollider();
            coolDown -= Time.deltaTime;
            if (i < boxList.Count)
            {
                reduceListCollider();
            }
            else
            {
                isReducing = false;
                isAllBox = true;
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Iniciar la reducción del BoxCollider
            //StartReducing(finalTargerX, durationBox);
            isReducing = true;
        }
    }
    private void reduceListCollider()
    {
        if (!isCooldown)
        {
            if (coolDown <= 0)
            {
                boxList[i].enabled = false;
                i++;

                isCooldown = true;
                coolDown = timeBetweenCollider;
            }
        }
        else
        {
            isCooldown = false;
        }
    }
    private void AdjustGameObjects()
    {
        if (boxList.Count == 0) return;

        Bounds bounds = GetComponent<Renderer>().bounds;
        Vector3 size = bounds.size;
        Vector3 startPos = bounds.min;

        for (int i = 0; i < boxList.Count; i++)
        {
            BoxCollider collider = boxList[i];

            // Ajusta el tamaño dependiendo de la cantidad de colisionadores
            boxList[i].size = new Vector3(size.x / boxList.Count, size.y, size.z);

            // Ajusta la posición de cada colisionador
            float offsetX = (size.x / boxList.Count) * i + (boxList[i].size.x / 2);
            boxList[i].center = new Vector3(startPos.x + offsetX - transform.position.x, 0, 0);
        }
    }
    private void reducieCollider()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        // Reducir el tamaño desde la derecha hacia la izquierda
        float newSizeX = Mathf.Lerp(initialSizeX, targetSizeX, t);
        float sizeDifference = initialSizeX - newSizeX;

        Vector3 newSize = boxCollider.size;
        newSize.x = newSizeX;

        Vector3 newCenter = boxCollider.center;
        newCenter.x -= sizeDifference / 2;  // Desplazar el centro para cerrar de derecha a izquierda

        boxCollider.size = newSize;
        boxCollider.center = newCenter;

        if (t >= 1f) isReducing = false;
    }
    public void StartReducing(float newTargetSizeX, float newDuration)
    {
        targetSizeX = Mathf.Max(0, newTargetSizeX);
        duration = Mathf.Max(0.1f, newDuration);
        initialSizeX = boxCollider.size.x;
        elapsedTime = 0f;
        isReducing = true;
    }

}
