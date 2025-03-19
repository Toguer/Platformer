using TMPro;
using UnityEngine;

public class UpdateScore : MonoBehaviour
{
    public void UpdateCanvasInt(int h)
    {
        GetComponent<TextMeshProUGUI>().text = " " + h;
    }
}
