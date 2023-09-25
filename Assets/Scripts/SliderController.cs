using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SliderController : MonoBehaviour
{
    [SerializeField] GameObject energyAmount;
    [SerializeField] Slider slider;
    [SerializeField] GameObject fill;
    [SerializeField] float colourThreshold = 75f;
    int remainingEnergy;

    void Awake()
    {
        remainingEnergy = 100;
    }
    public void OnSliderChanged(float value)
    {
        TextMeshProUGUI energyText = energyAmount.GetComponent<TextMeshProUGUI>();
        energyText.text = value.ToString() + "%";

        if (value < 75f)
        {
            fill.GetComponent<Image>().color = new Color(1 - value / colourThreshold, value / colourThreshold, 0);
        }
    }

    public void UpdateEnergy(int energyUsed)
    {
        remainingEnergy -= energyUsed;
        slider.value = Mathf.Max(remainingEnergy, 0);
    }

    public int GetRemainingEnergy()
    {
        return remainingEnergy;
    }

    public void ResetEnergy()
    {
        remainingEnergy = 100;
        slider.value = remainingEnergy;
        fill.GetComponent<Image>().color = new Color(0, 1, 0);
    }
}
