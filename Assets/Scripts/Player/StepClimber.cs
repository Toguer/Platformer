using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StepClimber : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Altura máxima que se puede subir como escalón")]
    [SerializeField] private float stepHeight = 0.3f;

    [Tooltip("Distancia máxima para detectar un escalón")]
    [SerializeField] private float stepCheckDistance = 0.5f;

    [Tooltip("Offset vertical desde el suelo para el raycast bajo")]
    [SerializeField] private float lowerRayStartHeight = 0.1f;

    [Tooltip("Capa de obstáculos escalables")]
    [SerializeField] private LayerMask stepLayerMask;

    [Tooltip("Velocidad al subir el escalón (1 = instantáneo)")]
    [SerializeField] private float stepClimbSpeed = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        TryClimbStep();
    }

    private void TryClimbStep()
    {
        Vector3[] directions =
        {
            transform.forward, // delante
            (transform.forward + transform.right).normalized, // diagonal derecha
            (transform.forward - transform.right).normalized  // diagonal izquierda
        };

        foreach (Vector3 dir in directions)
        {
            Vector3 lowerOrigin = transform.position + Vector3.up * lowerRayStartHeight;
            Vector3 upperOrigin = transform.position + Vector3.up * (lowerRayStartHeight + stepHeight);

            if (Physics.Raycast(lowerOrigin, dir, out RaycastHit lowerHit, stepCheckDistance, stepLayerMask))
            {
                if (!Physics.Raycast(upperOrigin, dir, stepCheckDistance, stepLayerMask))
                {
                    // Escalón detectado: subimos suavemente
                    Vector3 targetPosition = transform.position + Vector3.up * stepHeight;
                    rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * stepClimbSpeed));
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector3 lowerOrigin = transform.position + Vector3.up * lowerRayStartHeight;
        Vector3 upperOrigin = transform.position + Vector3.up * (lowerRayStartHeight + stepHeight);
        Vector3 forward = transform.forward * stepCheckDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(lowerOrigin, lowerOrigin + forward);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(upperOrigin, upperOrigin + forward);
    }
}
