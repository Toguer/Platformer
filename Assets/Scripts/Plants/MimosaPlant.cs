using UnityEngine;

public class MimosaPlant : MonoBehaviour
{
    [SerializeField] private float durationBox;
    [SerializeField] private float finalTargerX;

    private BoxCollider boxCollider;
    private float initialSizeX;
    private float targetSizeX;
    private float duration;
    private float elapsedTime = 0f;
    private bool isReducing = false;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("No hay un BoxCollider en este objeto.");
            return;
        }
        initialSizeX = boxCollider.size.x;
    }

    void Update()
    {
        if (isReducing)
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
    }

    public void StartReducing(float newTargetSizeX, float newDuration)
    {
        targetSizeX = Mathf.Max(0, newTargetSizeX);
        duration = Mathf.Max(0.1f, newDuration);
        initialSizeX = boxCollider.size.x;
        elapsedTime = 0f;
        isReducing = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Iniciar la reducción del BoxCollider
            StartReducing(finalTargerX, durationBox);
        }
    }
}
