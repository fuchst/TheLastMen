using UnityEngine;

public class MainMenu : MonoBehaviour {

    [SerializeField] GameObject Intro;
    [SerializeField] GameObject Instructions;
    [SerializeField] GameObject Main;
    [SerializeField] GameObject Credits;

    void Start () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Main.SetActive(true);
        Intro.SetActive(false);
        Instructions.SetActive(false);
        Credits.SetActive(false);
    }

    public void ShowIntro()
    {
        Main.SetActive(false);
        Intro.SetActive(true);
    }
    
    public void StartGame()
    {
        Application.LoadLevel(1);
    }

    public void EndGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void ShowInstructions()
    {
        Main.SetActive(false);
        Instructions.SetActive(true);
    }

    public void ShowCredits()
    {
        Main.SetActive(false);
        Credits.SetActive(true);
    }

    public void BackToMain()
    {
        Instructions.SetActive(false);
        Credits.SetActive(false);
        Intro.SetActive(false);
        Main.SetActive(true);
    }
}
