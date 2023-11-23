using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class AiController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ballObject;
    public GameObject AiBuffer;

    private Player player;
    private GameManager gameManager;

    private Ball ball = null;

    private bool ballDirection = false;

    private float timer;

    void Start()
    {
        player = GetComponent<Player>();
        ball = ballObject.GetComponent<Ball>();
        gameManager = FindObjectOfType<GameManager>();

        timer = gameManager.aiTimer;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {

            //Debug.Log("Ball on ground" + ball.getBallOnGround());
            if (!player.getBallCath() && ball.getBallOnGround())
            {
                findBall();

            }
            else
            {
                player.stopRunning();

                findToCatchBall();

            }

            // throw ball if it get the ball
            if (player.getBallCath())
            {
                //this is just another measure to stop the dribling
                //player.getOtherPlayer().anotherPlayerGetBall();

                if (timer > 0 && player.distancePlayerPlayer() < 1.3)
                {
                    player.move(!ballDirection);
                    timer -= Time.deltaTime;
                }

                Debug.Log("Player face Player = " + playerFacePlayer());
                if (player.distancePlayerPlayer() < 1 && playerFacePlayer() == false)
                {
                    Invoke("throwBall", .6f);
                }
                else if (player.distancePlayerPlayer() > 1.3)
                {
                    Invoke("throwBall", .6f);
                }

                timer = gameManager.aiTimer;
            }

        }

    }


    private void findToCatchBall()
    {

        timer -= Time.deltaTime;
        if (player.distancePlayerPlayer() > 1.1f)
        {
            if (timer < 0)
            {
                findBall();
            }
        }
        Debug.Log("Player face Player = " + playerFacePlayer());

        if (!playerFacePlayer())
        {
            //this is in case of the player is behind the opponent then it have to catch the ball
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                findBall();
            }
        }

    }

    private void OnEnable()
    {
        Player.onePlayerThrow += actionOnthrow;
        BasketScore.onePlayerScore += onScoreEvent;
    }

    private void OnDisable()
    {
        Player.onePlayerThrow -= actionOnthrow;
        BasketScore.onePlayerScore -= onScoreEvent;
    }

    private void onScoreEvent(bool boolean)
    {
        ball.setBallOnGround();
    }

    private void actionOnthrow()
    {
        // action to defend ball
        if (ball.getCurrentPlayer() != player && playerFacePlayer())
        {
            player.jump();
        }
    }

    private void throwBall()
    {
        player.jump();
        player.playerThrowBall();

    }
    private void findBallDirection()
    {
        float dotProduct = 0;
        Vector3 ballPosition = ballObject.transform.position;
        Vector3 playerDirection = transform.forward;


        Vector3 targetDirection = (transform.position - ballPosition).normalized;



        if (player.getThrowingDirection() == true)
        {
            dotProduct = Vector3.Dot(playerDirection, targetDirection);
        }
        else
        {
            dotProduct = Vector3.Dot(-playerDirection, targetDirection);
        }

        if (dotProduct > 0.6f)
        {
            /*Debug.Log("AI going right");
            Debug.Log("dot product = " + 1);*/
            ballDirection = true;
        }
        else if (dotProduct < -0.6f)
        {
            ballDirection = false;
        }
    }

    private bool playerFacePlayer()
    {
        float dotProduct = 0;

        Vector3 myDirection = transform.forward;

        Vector3 opponentDirection = player.getOtherPlayer().gameObject.transform.forward;

        dotProduct = Vector3.Dot(myDirection, opponentDirection);
        //Debug.Log("dot product value = " + dotProduct);

        if (dotProduct < 0)
        {
            //means player face player
            return true;
        }

        return false;
    }

    private void findBall()
    {

        findBallDirection();
        player.move(ballDirection);

    }

}
