using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private RectTransform fadeImage;
    [SerializeField] private float canvasSize;
    [SerializeField] private float transitionTime;

    public void FadeIn()
    {
        fadeImage.GetComponent<Image>().enabled = true;
        fadeImage.anchorMin = new Vector2(0, 0);
        fadeImage.anchorMax = new Vector2(1, 1);

        LeanTween.value(fadeImage.gameObject, (float value) =>
        {
            fadeImage.anchorMin = new Vector2(value / canvasSize, 0);
        }, 0, canvasSize, transitionTime)
        
        .setOnComplete(() => {
            fadeImage.GetComponent<Image>().enabled = false;
        });
    }

    public void FadeOut(string nextScene)
    {
        fadeImage.GetComponent<Image>().enabled = true;
        fadeImage.anchorMin = new Vector2(0, 0);
        fadeImage.anchorMax = new Vector2(0, 1);

        LeanTween.value(fadeImage.gameObject, (float value) => {
            fadeImage.anchorMax = new Vector2(value / canvasSize, 1);
        }, 0, canvasSize, transitionTime)
            
        .setOnComplete(() => {
            SceneManager.LoadScene(nextScene);
        });
    }

    private void Awake()
    {
        FadeIn();
    }
}
