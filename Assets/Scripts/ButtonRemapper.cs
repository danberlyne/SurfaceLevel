using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class ButtonRemapper : MonoBehaviour
{
    [SerializeField] GameObject keyBinding;
    [SerializeField] int bindingNumber;
    [SerializeField] InputActionAsset playerInput;
    [SerializeField] string actionName;

    public void StartRebinding()
    {
        InputAction actionToRebind = playerInput[actionName];
        actionToRebind.Disable();
        var rebindOperation = actionToRebind.PerformInteractiveRebinding(bindingNumber)
                    // To avoid accidental input from mouse motion
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.2f)
                    .Start().OnComplete(op =>
                    {
                        UpdateBindingText();
                        actionToRebind.Enable();
                    });
    }

    void UpdateBindingText()
    {
        TextMeshProUGUI bindingText = keyBinding.GetComponent<TextMeshProUGUI>();
        string bindingPath = playerInput[actionName].bindings[bindingNumber].path;

        if (bindingPath == null || bindingPath == "")
        {
            bindingPath = "None";
        }
        else
        {
            bindingPath = bindingPath.Substring(bindingPath.IndexOf("/") + 1);
        }

        bindingText.text = bindingPath;
    }
}
