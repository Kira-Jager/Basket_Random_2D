using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public GameObject ScorePanel = null;
    public GameObject endPanel = null;

    public GameObject startButton = null;

    public GameObject blackBg = null;

    // Start is called before the first frame update
    void Start()
    {
        stopGame();
        startButton.SetActive(true);
        blackBg.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startGame()
    {
        Time.timeScale = 1;
        Debug.Log("Game Start");
        startButton.SetActive(false);
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

        if(winnerName == "Draw")
        {
            displayTextObject.GetComponent<TextMeshProUGUI>().text = winnerName;
        }
        else
        {
            displayTextObject.GetComponent<TextMeshProUGUI>().text = $"Winner is {winnerName}" ;
        }

        stopGame();

    }

    public void stopGame()
    {
        Time.timeScale = 0;
    }


}
