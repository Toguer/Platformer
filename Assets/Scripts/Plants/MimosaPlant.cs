using UnityEngine;

public class MimosaPlant : MonoBehaviour
{
    [SerializeField] private float durationBox;
    [SerializeField] private float finalTargerZ;

    private BoxCollider boxCollider;
    private float initialSizeZ;
    private float targetSizeZ;
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
        initialSizeZ = boxCollider.size.x;
    }

    void Update()
    {
        if (isReducing)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Interpolación del tamaño en Z
            float newSizeZ = Mathf.Lerp(initialSizeZ, targetSizeZ, t);

            Vector3 newSize = boxCollider.size;
            newSize.x = newSizeZ;

            Vector3 newCenter = boxCollider.center;
            newCenter.x -= 0;

            boxCollider.size = newSize;
            boxCollider.center = newCenter;
            if (t >= 1f) isReducing = false;
        }
    }

    public void StartReducing(float newTargetSizeZ, float newDuration)
    {
        targetSizeZ = Mathf.Max(0, newTargetSizeZ);
        duration = Mathf.Max(0.1f, newDuration);
        initialSizeZ = boxCollider.size.x;
        elapsedTime = 0f;
        isReducing = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // aqui haria la animacion de la flor recogiendose
            StartReducing(durationBox, finalTargerZ);
        }
    }
}
