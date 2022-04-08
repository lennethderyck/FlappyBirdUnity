using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using CodeMonkey;
using UnityEngine.SceneManagement;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;


    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
    }

    public void retryGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Start()
    {
        CMDebug.TextPopupMouse("BEGINNEN");
        Bird.getInstance().onDied += bird_onDied;
        Hide();
    }

    private void bird_onDied(object sender, System.EventArgs e)
    {
        scoreText.text = Level.getInstance().getWallsPassed().ToString();
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
