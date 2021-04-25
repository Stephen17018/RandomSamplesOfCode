using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

    [SerializeField]
    private bool Hacking;

    [SerializeField]
    private bool Activating;

    [SerializeField]
    private Text ActionMessage, ActionPercentage;
    float ActionPercentageValue = 0;
    float delayBetweenTextUpdate = 0.25f, originaldelayBetweenTextUpdate;
    int actionUpdateCount;
    // Start is called before the first frame update
    void Start()
    {
        originaldelayBetweenTextUpdate = delayBetweenTextUpdate;
    }

    // Update is called once per frame
    void Update()
    {

        if (Hacking && !Activating)
        {

            delayBetweenTextUpdate = delayBetweenTextUpdate - Time.deltaTime;

            if (delayBetweenTextUpdate < 0)
            {

                actionUpdateCount++;

                delayBetweenTextUpdate = originaldelayBetweenTextUpdate;
            }

            switch (actionUpdateCount)
            {
                case 0:
                    ActionMessage.text = "Hacking";
                    break;
                case 1:
                    ActionMessage.text = "Hacking.";
                    break;
                case 2:
                    ActionMessage.text = "Hacking..";
                    break;
                case 3:
                    ActionMessage.text = "Hacking...";
                    break;
            }
            if (actionUpdateCount > 3)
            {
                actionUpdateCount = 0;
            }

            ActionPercentageValue = ActionPercentageValue + Time.deltaTime * 15;
            ActionPercentage.text = (int)ActionPercentageValue + "%";
            if (ActionPercentageValue > 100)
            {
                ActionPercentageValue = 0;
                Hacking = false;
            }
        }
        else if (!Hacking && Activating)
        {

            delayBetweenTextUpdate = delayBetweenTextUpdate - Time.deltaTime;

            if (delayBetweenTextUpdate < 0)
            {

                actionUpdateCount++;

                delayBetweenTextUpdate = originaldelayBetweenTextUpdate;
            }

            switch (actionUpdateCount)
            {
                case 0:
                    ActionMessage.text = "Activating";
                    break;
                case 1:
                    ActionMessage.text = "Activating.";
                    break;
                case 2:
                    ActionMessage.text = "Activating..";
                    break;
                case 3:
                    ActionMessage.text = "Activating...";
                    break;
            }
            if (actionUpdateCount > 3)
            {
                actionUpdateCount = 0;
            }

            ActionPercentageValue = ActionPercentageValue + Time.deltaTime * 15;
            ActionPercentage.text = (int)ActionPercentageValue + "%";
            if (ActionPercentageValue > 100)
            {
                ActionPercentageValue = 0;
                Activating = false;
            }
        }
        else
        {
            actionUpdateCount = 0;
            ActionMessage.text = "";
            ActionPercentage.text = "";
            ActionPercentageValue = 0;
        }
    }
}
