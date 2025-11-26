using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestDeck : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Player.Enable();
        }
    }

    private void OnEnable()
    {
        _inputActions.Player.Interact.canceled += OnGainXp;
    }

    private void OnDisable()
    {
        _inputActions.Player.Interact.canceled -= OnGainXp;
    }

    private void OnGainXp(InputAction.CallbackContext context)
    {
        LevelUpManager.Instance.GainXP(100);
    }
}
