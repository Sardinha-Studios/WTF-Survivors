using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonTriggerMapper : MonoBehaviour
{
    // Variável no Inspector para arrastar a Ação
    [SerializeField]
    private InputActionReference triggerAction;

    // Variável no Inspector para arrastar o Botão da UI
    [SerializeField]
    private Button targetButton;

    private void OnEnable()
    {
        // Assina a função que será chamada quando a ação for PERFOMED
        if (triggerAction != null)
        {
            triggerAction.action.performed += OnTriggerPerformed;
            triggerAction.action.Enable(); // Garante que a ação está habilitada
        }
    }

    private void OnDisable()
    {
        // Desassina a função quando o objeto for desabilitado
        if (triggerAction != null)
        {
            triggerAction.action.performed -= OnTriggerPerformed;
            triggerAction.action.Disable(); 
        }
    }

    private void OnTriggerPerformed(InputAction.CallbackContext context)
    {
        // Chama a função OnClick do botão da UI
        if (targetButton != null && targetButton.interactable)
        {
            targetButton.onClick.Invoke();
            // Opcional: Adicionar feedback visual ou sonoro de clique aqui
        }
    }
}
