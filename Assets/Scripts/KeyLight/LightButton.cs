using UnityEngine;
using UnityEngine.Events;
public class LightButton : Interactable
{
    [SerializeField] private UnityEvent _onPressButton;
    public override void Interact()
    {
        _onPressButton.Invoke();
    }
}
