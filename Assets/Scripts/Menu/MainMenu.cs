using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string mainSceneName;
    [SerializeField] private RectTransform titleText;
    private float initialPos;
    [SerializeField] private SceneTransition transition;

    private void Awake()
    {
        initialPos = this.titleText.anchoredPosition.y;
        Debug.Log(initialPos);
        this.moveTitleUp();
    }

    private void moveTitleUp()
    {
        LeanTween.moveY(titleText, initialPos + 5, 3)
            .setOnComplete(() => { moveTitleDown(); });
    }

    private void moveTitleDown()
    {
        LeanTween.moveY(titleText, initialPos - 5, 3)
            .setOnComplete(() => { moveTitleUp(); });
    }

    public void PlayButtonClicked()
    {
        transition.FadeOut("Main");
    }
}
