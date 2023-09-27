using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectButtons : MonoBehaviour
{
    [SerializeField] GameObject[] effectButtons;
    [SerializeField] int energyCost = 4;
    SliderController sliderController;

    public void OnEffectSelected(int index)
    {
        RecalculateEnergy(index);
        InvertButtonStates(index);
        GreyOut();
    }

    void InvertButtonStates(int index)
    {
        sliderController = FindObjectOfType<SliderController>();

        for (int i = 0; i < effectButtons.Length; i++)
        {
            Button button = effectButtons[i].GetComponent<Button>();
            ToggleEffect buttonToggle = effectButtons[i].GetComponent<ToggleEffect>();

            if (i != index && sliderController.GetRemainingEnergy() >= energyCost)
            {
                button.interactable = !button.interactable;
            }
            else if (i != index)
            {
                button.interactable = false;
            }
            else if (sliderController.GetRemainingEnergy() < energyCost && buttonToggle.GetIsOn())
            {
                button.interactable = false;
            }
        }
    }

    void GreyOut()
    {
        for (int i = 0; i < effectButtons.Length; i++)
        {
            Button button = effectButtons[i].GetComponent<Button>();
            TMP_Text text = effectButtons[i].GetComponentInChildren<TMP_Text>();
            text.overrideColorTags = true;
            Color textColour = text.color;

            if (button.interactable == false)
            {
                textColour.a = 0.25f;
                text.color = textColour;
            }
            else
            {
                textColour.a = 1.0f;
                text.color = textColour;
            }

        }
    }

    void RecalculateEnergy(int index)
    {
        ToggleEffect buttonToggle = effectButtons[index].GetComponent<ToggleEffect>();
        sliderController = FindObjectOfType<SliderController>();

        if (!buttonToggle.GetIsOn())
        {
            sliderController.UpdateEnergy(energyCost);
        }
        else
        {
            sliderController.UpdateEnergy(-energyCost);
        }
    }

    public void DisableEffectButtons()
    {
        for (int i = 0; i < effectButtons.Length; i++)
        {
            effectButtons[i].GetComponent<Button>().interactable = false;
        }
        GreyOut();
    }

    public void EnableEffectButtons()
    {
        for (int i = 0; i < effectButtons.Length; i++)
        {
            effectButtons[i].GetComponent<Button>().interactable = true;
        }
        GreyOut();
    }

    public void ToggleAllOff()
    {
        for (int i = 0; i < effectButtons.Length; i++)
        {
            effectButtons[i].GetComponent<ToggleEffect>().TurnOff();
        }
    }
}
