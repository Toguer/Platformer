using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    [Header("Lever")]
    [SerializeField] private GameObject lever;

    [Header("List")]
    [SerializeField] private List<GameObject> stepsList = new List<GameObject>();

    [Header("Bool")]
    [SerializeField] private bool isButton;
    private bool isButtonPressed;

    [Header("Cool Down")]
    [SerializeField] private float timeBetweenLadder;
    [SerializeField] private float timeBetweenPlatfrom;
    private float coolDown = 0;
    private bool isCooldown;

    private int i;
    private float aux;
    private bool onetime;

    public void SetIsButtonPressed(bool isButtonPressed)
    {
        this.isButtonPressed = isButtonPressed;
    }
    void Start()
    {
        isCooldown = false;
        i = 0;
        aux = timeBetweenPlatfrom + 2;
        if (isButton)
        {
            lever.GetComponent<Animator>().SetTrigger("leverUp");
        }
    }

    void Update()
    {
        // este script es de una escalera, los escalones van saliendo de uno en uno en un tiempo x y luego se vuelven a meter
        //hay dos opciones, las escalones que aaparecen cuando salen pulsan el boton, y otro en tiempo normal
        if (isButton)
        {
            if (isButtonPressed)
            {
                coolDown -= Time.deltaTime;
                if (i < stepsList.Count)
                {
                    pressButton();
                }
                aux -= Time.deltaTime;
                if (aux <= 0)
                {
                    isButtonPressed = false;
                    aux = timeBetweenPlatfrom + 2;
                    i = 0;
                }
            }
        }
        else
        {
            if (!onetime)
            {
                coolDown -= Time.deltaTime;
                if (i < stepsList.Count)
                {
                    normalLadder();
                }
                else
                {
                    onetime = true;
                }
            }
        }
    }

    private void pressButton()
    {
        if (!isCooldown)
        {
            if (coolDown <= 0)
            {
                GameObject step = stepsList[i];
                step.SetActive(true);
                step.GetComponentInChildren<PlatfromController>().resetPlatform();
                step.GetComponentInChildren<PlatfromController>().SetTimeBetween(timeBetweenPlatfrom);
                step.GetComponentInChildren<PlatfromController>().SetIsLadder(true);

                i++;

                isCooldown = true;
                coolDown = timeBetweenLadder;
            }
        }
        else
        {
            isCooldown = false;
        }
    }

    private void normalLadder()
    {
        if (!isCooldown)
        {
            if (coolDown <= 0)
            {
                GameObject step = stepsList[i];
                step.SetActive(true);

                i++;

                isCooldown = true;
                coolDown = timeBetweenLadder;
            }
        }
        else
        {
            isCooldown = false;
        }
    }
}
