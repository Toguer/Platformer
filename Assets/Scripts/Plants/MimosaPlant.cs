using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class MimosaPlant : MonoBehaviour
{
    [SerializeField] private List<GameObject> boxList = new List<GameObject>();

    private BoxCollider boxCollider;
    public bool isReducing = false;
    public bool isAllBox;

    [Header("CoolDown")]
    [SerializeField] private float timeBetweenCollider;
    private int i;
    private float coolDown = 0;
    private bool isCooldown;
   

    [Header("Restart")]
    [SerializeField] private float timeToRestartColliders;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("No hay un BoxCollider en este objeto.");
            return;
        }

        coolDown = timeBetweenCollider;
        i = 0;
        //AdjustGameObjects();
    }

    void Update()
    {
        if (isReducing && !isAllBox)
        {
            coolDown -= Time.deltaTime;
            if (i < boxList.Count)
            {
                reduceListCollider();
            }
            else
            {
                isReducing = false;
                isAllBox = true;
                StartCoroutine(_RestartColiiders());
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Iniciar la reducción del BoxCollider
            isReducing = true;
            Debug.Log("enter");
        }
    }
    private void reduceListCollider()
    {
        if (!isCooldown)
        {
            if (coolDown <= 0)
            {
                boxList[i].SetActive(false);
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
   /* private void AdjustGameObjects()
    {
        if (boxList.Count == 0) return;

        Bounds bounds = GetComponent<Renderer>().bounds;
        Vector3 size = bounds.size;
        Vector3 startPos = bounds.min;

        for (int i = 0; i < boxList.Count; i++)
        {
            // Ajusta el tamaño dependiendo de la cantidad de colisionadores
            boxList[i].size = new Vector3(size.x / boxList.Count, size.y, size.z);

            // Ajusta la posición de cada colisionador
            float offsetX = (size.x / boxList.Count) * i + (boxList[i].size.x / 2);
            boxList[i].center = new Vector3(startPos.x + offsetX - transform.position.x, 0, 0);
        }
    }*/
    private IEnumerator _RestartColiiders()
    {
        yield return new WaitForSeconds(timeToRestartColliders);

        for (int i = 0; i < boxList.Count; i++)
        {
            boxList[i].SetActive(true);
        }
        isAllBox = false;
        i = 0;
    }
}
