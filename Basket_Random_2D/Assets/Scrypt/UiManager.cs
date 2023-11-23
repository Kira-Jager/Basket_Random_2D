using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{
    public GameObject ScorePanel = null;
    public GameObject endPanel = null;

    public GameObject startButton = null;

    public GameObject blackBg = null;

    public GameObject selectOpponentPanel = null;

    public GameObject settingsPanel = null;

    public Image winnerImg = null;

    public Sprite player1Img = null;
    public Sprite player2Img = null;

    public delegate void selectOponentAction(bool onePlayerSelected);
    public static event selectOponentAction selectOponnent;

    // Start is called before the first frame update
    void Start()
    {
        stopGame();
        selectOpponentPanel.SetActive(true);
        //startButton.SetActive(true);
        blackBg.SetActive(false);
        ScorePanel.SetActive(false);
        settingsPanel.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startGame()
    {
        Time.timeScale = 1;
        Debug.Log("Game Start");
        //startButton.SetActive(false);
        selectOpponentPanel.SetActive(false);
        ScorePanel.SetActive(true);

        GameManager.instance.stopAudio();
        GameManager.instance.playAudio(GameManager.instance.onStartSound, loop: false);

    }

    public void restartGame()
    {
        Time.timeScale = 1;
        Debug.Log("Game Start");
        endPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable()
    {
        GameManager.onEndGame += endGamePanel;
        BasketScore.onePlayerScore += actionOnScore;
    }

    private void OnDisable()
    {
        GameManager.onEndGame -= endGamePanel;
        BasketScore.onePlayerScore -= actionOnScore;
    }

    private void actionOnScore(bool boolean)
    {
        if (!boolean)
        {
            blackBg.SetActive(true);
            ScorePanel.SetActive(false );
            Invoke("disableBlackScrenn", .25f);
        }
    }
    private void disableBlackScrenn()
    {
        blackBg.SetActive(false);
        ScorePanel.SetActive(true);
    }
    private void endGamePanel( string winnerName)
    {
        endPanel.SetActive(true);
        GameObject displayTextObject = endPanel.gameObject.transform.GetChild(0).gameObject;
        blackBg.SetActive(false);

        if (winnerName == "Draw")
        {
            displayTextObject.GetComponent<TextMeshProUGUI>().text = winnerName;
            winnerImg.enabled = false;
        }
        else if(winnerName == "Player 1")
        {
            displayTextObject.GetComponent<TextMeshProUGUI>().text = $"Winner is {winnerName}" ;
            winnerImg.sprite = player1Img;
        }
        else
        {
            displayTextObject.GetComponent<TextMeshProUGUI>().text = $"Winner is {winnerName}";
            winnerImg.sprite = player2Img;
        }

        stopGame();

    }

    public void stopGame()
    {
        Time.timeScale = 0;
    }

    public void onePlayerSelected()
    {
        //true means one player is selected 
        selectOponnent?.Invoke(true);
        selectOpponentPanel.GetComponent<Animation>().Play();
        startGame();
    }

    public void twoPlayerSelected()
    {
        //false means two player is selected 
        selectOponnent?.Invoke(false);
        selectOpponentPanel.GetComponent<Animation>().Play();
        startGame();
    }

    public void displaySettings()
    {
        stopGame();

        settingsPanel.SetActive(true);

        GameManager.instance.playAudio(GameManager.instance.gameMusic, loop: true);
    }

    public void removeSettings()
    {
        startGame();

        settingsPanel.SetActive(false);
    }


}
