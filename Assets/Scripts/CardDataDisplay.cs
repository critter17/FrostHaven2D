using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDataDisplay : MonoBehaviour
{
    [SerializeField] CardData deadlyShot;
    [SerializeField] Button doneButton;
    [SerializeField] Button undoButton;
    ScenarioManager scenarioManager;

    protected void Awake()
    {
        scenarioManager = FindObjectOfType<ScenarioManager>();
    }

    public void OnTopActionClicked()
    {
        scenarioManager.AddAbilities(deadlyShot.topAction);
        doneButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnBottomActionClicked()
    {
        scenarioManager.AddAbilities(deadlyShot.bottomAction);
        doneButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnBasicAttackClicked()
    {
        scenarioManager.AddAbilities(new List<Ability> { deadlyShot.basicAttack });
        doneButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnBasicMoveClicked()
    {
        scenarioManager.AddAbilities(new List<Ability> { deadlyShot.basicMove });
        doneButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
