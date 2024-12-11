using UnityEngine;

public abstract class State : ScriptableObject
{
    protected StateMachine stateMachine;

    //Entra al estado pasa un tiempo y vuelvo, este estado que utilice esta funcion sirve para entrar y salir en un tiempo determinado, como un shot
    public bool isDone = false;

    public GameObject characterGame;

    [SerializeField] Transition[] possibleTransitions;
    public abstract void OnEnterState();
    public abstract void UpdateState();
    public abstract void OnExitState();

    public void SetStateController(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public State CheckTransitions() // esta funcion devuelve el siguiente esatdo y lo hace en transicion
    {
        // tiempo exit time para dar tiempo al estado para la siguiente accion (parecido a un cooldown)
        State exitState = null;
        for (int i = 0; i < possibleTransitions.Length && exitState == null; i++)
        {
            if (possibleTransitions[i].GetExitTime() == -1 || stateMachine.GetCurrentStateTime() > possibleTransitions[i].GetExitTime())
            {
                exitState = possibleTransitions[i].GetExitState(stateMachine);
            }
        }
        return exitState;
    }
    public virtual void OnTriggerEnter(Collider other)
    {

    }
}
