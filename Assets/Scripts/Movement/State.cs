using UnityEngine;

public abstract class State : ScriptableObject
{
    public abstract void Enter(GameObject owner);
    public abstract void Execute(GameObject owner);
    public abstract void Exit(GameObject owner);
}
