using UnityEngine;
using UnityEngine.Events;
public class LightButton : Interactable
{
    [SerializeField] private UnityEvent _onPressButton;
    public override void Interact(PlayerController player)
    {
        _onPressButton.Invoke();
    }
}
