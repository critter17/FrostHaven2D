using UnityEngine;
using UnityEngine.UI;

public class ConfirmAction : MonoBehaviour
{
    [SerializeField] private Button confirmButton;

    protected void OnEnable()
    {
        EventManager.OnHexesSelected += EnableButton;
    }

    private void EnableButton(int hexesSelected)
    {
        confirmButton.interactable = hexesSelected > 0;
    }

    protected void OnDisable()
    {
        EventManager.OnHexesSelected -= EnableButton;
    }
}
