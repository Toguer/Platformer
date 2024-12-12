using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State entryState; // el primer estado default(en la mayoria de casos es Idle, este estado tiene que ser un scriptableObject
    [SerializeField] private State currentState; //el estado que va cambiando para controlar en que estado esta el player, este estado tiene que ser un scriptableObject
    [SerializeField] private State anyState; // estado general la cual puede hacer transicion con cualquier estado, este estado tiene que ser un scriptableObject
    float currentStateTime = 0;
    void Start()
    {
        anyState = Instantiate(anyState); // instanciamos una copia
        anyState.SetStateController(this); //añadimos al estado la state Machine
        if(entryState != null)
        {
            ChangeState(entryState); // añadimos el estado entrante a currentState
        }
    }
    private void ChangeState(State newState)
    { 
        //esta funcion se ara siempre que se cambie de estado para que se poga como esatdo actual
        if (currentState != null)
        {
            currentState.OnExitState();
            Destroy(currentState);
        }
        currentState = Instantiate(newState);
        currentState.SetStateController(this);
        currentState.OnEnterState();// forzamos un start para los scriptable objects
        currentStateTime = 0; // cuenta el tiemppo en la que se esta actuando el estado

    }

    void Update()
    {
        currentState.UpdateState(); //forzamos un update a los scriptable object
        currentStateTime += Time.deltaTime;  //sumamos el tiempo
        State newState = null;
        if (anyState != null)
        {
            newState = anyState.CheckTransitions();
        }

        if (newState == null)
        {
            //añadimos el siguiente estado
            newState = currentState.CheckTransitions();
        }
        if (newState != null)
        {
            ChangeState(newState);
        }
    }

    public float GetCurrentStateTime()
    {
        return currentStateTime;
    }

    public bool CurrentStateIsDone()
    {
        return currentState.isDone;
    }

    public State GetCurrentState()
    {
        return currentState;
    }
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
        //llamara a la funcion de las clases hijas
    }
}
