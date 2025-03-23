using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("Player", transform.position);
    }
}
