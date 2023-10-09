using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingSliderController : MonoBehaviour
{
    [SerializeField] GameObject sliderAmount;
    public void OnSliderChanged(float value)
    {
        TextMeshProUGUI sliderText = sliderAmount.GetComponent<TextMeshProUGUI>();
        sliderText.text = value.ToString();
    }
}
