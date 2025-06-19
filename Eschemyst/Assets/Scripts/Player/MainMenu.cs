using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    [SerializeField] private GameObject VolumeUI;
    [SerializeField] private GameObject MenuUI;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.anyKeyDown &&
            !Input.GetKeyDown(KeyCode.Escape) &&
            !Input.GetKeyDown(KeyCode.Mouse0) &&
            !Input.GetKeyDown(KeyCode.Mouse1) &&
            !Input.GetKeyDown(KeyCode.Mouse2))
        {
            Play();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }


    public void Play()
    {
        StartCoroutine(FadeOutAndLoadScene("Mein"));
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void ToggleVolumeMenu(bool showVolume)
    {
        VolumeUI.SetActive(showVolume);
        MenuUI.SetActive(!showVolume);
    }
}
