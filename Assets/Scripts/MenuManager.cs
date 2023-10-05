using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;
    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip disabledSound;
    [SerializeField] GameObject[] menuButtons;
    [SerializeField] bool isSuccessScreen = false;
    [SerializeField] GameObject successText;
    [SerializeField] float successFadeDuration = 3f;
    Transform successTransform;


    void Start()
    {
        if (isSuccessScreen)
        {
            FindObjectOfType<GameSession>().gameObject.SetActive(false);
            successTransform = successText.transform;
            InitialiseText();
            StartCoroutine(FadeIn(successFadeDuration));
            StartCoroutine(EnableButtons());
        }
    }

    void InitialiseText()
    {
        for (int i = 0; i < successTransform.childCount; i++)
        {
            TMP_Text childText = successTransform.GetChild(i).GetComponent<TMP_Text>();
            childText.overrideColorTags = true;
            Color childTextColour = childText.color;
            childTextColour.a = 0f;
            childText.color = childTextColour;
            childText.gameObject.SetActive(true);
        }
    }

    IEnumerator FadeIn(float duration)
    {
        float transparency;

        for (int i = 0; i < successTransform.childCount; i++)
        {
            transparency = 0f;
            TMP_Text text = successTransform.GetChild(i).GetComponent<TMP_Text>();
            text.overrideColorTags = true;
            Color textColour = text.color;

            while (transparency < 1.0f)
            {
                textColour.a = transparency;
                text.color = textColour;
                transparency += Time.deltaTime / duration;
                yield return null;
            }
        }
    }

    IEnumerator EnableButtons()
    {
        yield return new WaitForSecondsRealtime(successFadeDuration * (successText.transform.childCount + 0.5f));
        
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].SetActive(true);
        }
    }

    public void StartNewGame()
    {
        StartCoroutine(LoadFirstLevel());
    }

    IEnumerator LoadFirstLevel()
    {
        yield return new WaitForSeconds(loadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void QuitGame()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(0);
    }

    public void PlayButtonSound(int index)
    {
        Button button = menuButtons[index].GetComponent<Button>();
        AudioSource audioSource = GetComponent<AudioSource>();
        
        if (button.interactable)
        {
            audioSource.PlayOneShot(selectSound);
        }
        else
        {
            audioSource.PlayOneShot(disabledSound);
        }
    }
}
