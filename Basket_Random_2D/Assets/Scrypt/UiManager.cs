using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject ScorePanel = null;
    public GameObject endPanel = null;

    public GameObject startButton = null;

    // Start is called before the first frame update
    void Start()
    {
        stopGame();
        startButton.SetActive(true);
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
    }

    private void OnEnable()
    {
        GameManager.onEndGame += endGamePanel;
    }

    private void OnDisable()
    {
        GameManager.onEndGame -= endGamePanel;
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
