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
    public GameObject startButton = null;
    
    public GameObject endPanel = null;

    //to be deleted
    public GameObject restartButton = null;

    private string _timer = null;

    //public float BounceForce = 1f;

    // Start is called before the first frame update
    void Start()
    {
        stopGame();
        _timer = Timer.GetComponent<TextMeshProUGUI>().text;
    }

    // Update is called once per frame
    void Update()
    {
        timer();

        if(GameTimer <= 0)
        {
            Debug.Log("Game End");
            stopGame();

        }
    }

    private void timer()
    {
        GameTimer -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(GameTimer);
        _timer = timeSpan.ToString(@"mm\:ss");

        Timer.GetComponent<TextMeshProUGUI>().text = _timer;
    }

    public void startGame()
    {
        Time.timeScale = 1;
        Debug.Log("Game Start");
        restartButton.active = false;
    }
    public void restartGame()
    {
        Time.timeScale = 1;
        Debug.Log("Game Start");
        restartButton.active = false;
    }

    public void stopGame()
    {
        Time.timeScale = 0;
    }

}
