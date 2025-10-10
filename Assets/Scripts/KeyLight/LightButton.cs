using UnityEngine;
using UnityEngine.Events;
public class LightButton : Interactable
{
    [SerializeField] private UnityEvent _onPressButton;
    public override void Interact(PlayerController player)
    {
        //este evento se cumplira cuando se pulse a la e
        _onPressButton.Invoke();
        Debug.Log("press E");
    }
}
