using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ButtonRemapper : MonoBehaviour
{
    [SerializeField] InputActionReference inputAction;
    // [SerializeField] PlayerInput playerInput;
    [SerializeField] GameObject keyBindingText;
    [SerializeField] GameObject waitingText;
    [SerializeField] int bindingNumber;

    InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebinding()
    {
        // Deselects the currently selected UI button to prevent the rebinding operation from being repeatedly triggered.
        EventSystem.current.SetSelectedGameObject(null);

        inputAction.action.Disable();

        keyBindingText.SetActive(false);
        waitingText.SetActive(true);

        rebindingOperation = inputAction.action.PerformInteractiveRebinding(bindingNumber)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => OnRebindComplete())
            .Start();
    }

    void OnRebindComplete()
    {
        rebindingOperation.Dispose();

        UpdateBindingText();

        waitingText.SetActive(false);
        keyBindingText.SetActive(true);

        inputAction.action.Enable();
    }

    void UpdateBindingText()
    {
        TextMeshProUGUI bindingText = keyBindingText.GetComponent<TextMeshProUGUI>();
        string bindingPath = inputAction.action.bindings[bindingNumber].effectivePath;

        if (bindingPath == null || bindingPath == "")
        {
            bindingPath = "None";
        }
        else
        {
            bindingPath = InputControlPath.ToHumanReadableString(bindingPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        bindingText.text = bindingPath;
    }
}
