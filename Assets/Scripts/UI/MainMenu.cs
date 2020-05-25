using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuItems;
    public GameObject howToPlayItems;
    public GameObject creditItems;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("FinalWorldBackup");
    }

    public void HowToPlay()
    {
        mainMenuItems.SetActive(false);
        howToPlayItems.SetActive(true);
    }

    public void BackToMenu()
    {
        howToPlayItems.SetActive(false);
        creditItems.SetActive(false);
        mainMenuItems.SetActive(true);
    }

    public void Credits()
    {
        mainMenuItems.SetActive(false);
        creditItems.SetActive(true);
    }
}
