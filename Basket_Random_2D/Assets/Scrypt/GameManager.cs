using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    public float GameTimer = 180f;
    public float speed = 0.11f;
    public float jumpForce = 5f;
    public float aiTimer = 3f;

    public float distancePlayerTarget = 3f;
    public float distanceThreshold = 5f;
    public float targetMinHeightValue = 6f;
    public float targetMaxHeightValue = 8.5f;


    public GameObject GameObjects = null;
    public GameObject[] limitBounds = null;
    public GameObject Timer = null;
    public GameObject AiControlerPlayer = null;


    public Image soundButtonImage;
    public Sprite soundOn = null;
    public Sprite soundOff = null;


    public AudioClip drible_audio = null;
    public AudioClip gameMusic = null;
    public AudioClip ballCollisionSound = null;
    public AudioClip onStartSound = null;

    public delegate void endGameEvent(String Winner);
    public static event endGameEvent onEndGame;
    
    public delegate void soundDisableAction(bool isDisable);
    public static event soundDisableAction soundDisable;

    
    private AudioSource audioSource = null;
    
    private string _timer = null;
    private bool hasGameEnded = false;


    //this is a singleton
    public static GameManager instance = null;



    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //stopGame();
        GameObjects.SetActive(false);

        audioSource = GetComponent<AudioSource>();

        _timer = Timer.GetComponent<TextMeshProUGUI>().text;

        playAudio(gameMusic, true);
    }

    // Update is called once per frame
    void Update()
    {
        timer();
    }


    private void OnEnable()
    {
        UiManager.selectOponnent += opponentSelect;
    }

    private void OnDisable()
    {
        UiManager.selectOponnent -= opponentSelect;
    }



    private void opponentSelect(bool boolean)
    {
        if (!boolean)
        {
            AiControlerPlayer.GetComponent<AiController>().enabled = false;
            AiControlerPlayer.GetComponent <InputController>().enabled = true;
        }
        else
        {
            AiControlerPlayer.GetComponent<AiController>().enabled = true;
            AiControlerPlayer.GetComponent<InputController>().enabled = false;
        }

        GameObjects.SetActive(true);
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

            GameObjects.SetActive(false);
            //stopGame();

            // flag to true to indicate that the game has ended
            hasGameEnded = true;
        }
    }

    public void playAudio(AudioClip audioClip, bool loop = false)
    {
        if (audioClip != null && audioSource.enabled)
        {
            if (loop)
            {
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                audioSource.loop = false;

                audioSource.PlayOneShot(audioClip);
            }
        }
    }

    public bool IsPlayingAudio()
    {
        return audioSource.isPlaying;
    }

    public void stopAudio()
    {
         audioSource.loop = false;
         audioSource.Stop();
    }

    public void DisableAllAudio()
    {
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
      
        for(int i = 0; i < audios.Length; i++)
        {
            audios[i].enabled = !audios[i].enabled;
        }
        soundButtonImage.sprite = !audios[audios.Length - 1].enabled ? soundOff : soundOn ;

        soundDisable?.Invoke(!audios[audios.Length - 1].enabled);
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

    public void quitApplication()
    {
        Application.Quit();
    }
}
