using UnityEngine;
using UnityEngine.UI;

public class DoneButton : MonoBehaviour
{
    [SerializeField] private Button doneButton;

    protected void OnEnable()
    {
        EventManager.OnHexesSelected += EnableButton;
    }

    private void EnableButton(int hexesSelected)
    {
        doneButton.interactable = hexesSelected == 0;
    }

    protected void OnDisable()
    {
        EventManager.OnHexesSelected -= EnableButton;
    }
}
