using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleEffect : MonoBehaviour
{
    bool isOn = false;

    private void Start()
    {
        isOn = false;
    }

    public void ToggleButton()
    {
        isOn = !isOn;

        if (!isOn)
        {
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
    }

    public bool GetIsOn()
    {
        return isOn;
    }

    public void TurnOff()
    {
        isOn = false;
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }
}
