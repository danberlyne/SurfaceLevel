using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;
    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip disabledSound;
    [SerializeField] GameObject[] menuButtons;

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
