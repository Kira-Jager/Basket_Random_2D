using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float speed = 5f;
    public float jumpForce = 5f;

    public float distance = 3f;
    public float distanceValue = 3f;
    public float minValue = 4f;
    public float maxValue = 9f;
    public float GameTimer = 180f;


    public GameObject[] limitBounds = null;
    public GameObject Timer = null;

    private string _timer = null;
    private bool hasGameEnded = false;

    public delegate void endGameEvent(String Winner);
    public static event endGameEvent onEndGame;

    // Start is called before the first frame update
    void Start()
    {
        //stopGame();
        _timer = Timer.GetComponent<TextMeshProUGUI>().text;
    }

    // Update is called once per frame
    void Update()
    {
        timer();
    }

    private void timer()
    {
        GameTimer -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(GameTimer);
        _timer = timeSpan.ToString(@"mm\:ss");

        Timer.GetComponent<TextMeshProUGUI>().text = _timer;

        if (GameTimer <= 0 && !hasGameEnded)
        {
            Debug.Log("Game End");
            onEndGame?.Invoke(gameWinner());
            //stopGame();

            // flag to true to indicate that the game has ended
            hasGameEnded = true;
        }
    }

   

    private string gameWinner()
    {
        BasketScore[] playerScores = FindObjectsOfType<BasketScore>();
        int i = 0;

        //Debug.Log($"Player Name = {playerScores[i].playerScore.name}" +
        //  $"\nScore = {playerScores[i].getScore()}");
        if (playerScores[i].getScore() > playerScores[i + 1].getScore())
        {
            //Debug.Log($"Winner is {playerScores[i].playerScore.name} ");
            return playerScores[i].playerScore.name;
        }
        else if (playerScores[i + 1].getScore() > playerScores[i].getScore())
        {
            //Debug.Log($"Winner is {playerScores[i + 1].playerScore.name} ");
            return playerScores[i+1].playerScore.name;
        }
        else
        {
            //Debug.Log("Draw");
            return "Draw";
        }
    }


}
