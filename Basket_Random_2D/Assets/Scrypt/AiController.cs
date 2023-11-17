using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float threshold = 2f;
    private float BufferRelativeMinX = 0f;
    private float BufferRelativeMaxX = 0f;

    private bool opponentHaveBall = false;
    void Start()
    {
        player = GetComponent<Player>();
        ball = ballObject.GetComponent<Ball>();
        gameManager = FindObjectOfType<GameManager>();

        timer = gameManager.aiTimer;
        if (AiBuffer != null)
        {
            MeshRenderer meshRenderer = AiBuffer.GetComponent<MeshRenderer>();

            BufferRelativeMinX = meshRenderer.bounds.min.x - player.transform.position.x;
            BufferRelativeMaxX = meshRenderer.bounds.max.x - player.transform.position.x;
        }
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

                //Invoke("findBall", .6f);

            }
            else
            {
                player.stopRunning();

                //Invoke("findToCatchBall", .3f);
                findToCatchBall();

            }

            // throw ball if it get the ball
            if (player.getBallCath())
            {
                //player.move(player.controlThrowingDirection());

                Invoke("throwBall", .6f);
                timer = gameManager.aiTimer;
            }

        }

    }


    private float distancePlayerPlayer()
    {
        float distance = Mathf.Abs(ball.getCurrentPlayer().transform.position.x - ball.getPreviousPlayer().transform.position.x);

        return distance;
    }

    private void findToCatchBall()
    {
        if (ball.getCurrentPlayer() != null && ball.getCurrentPlayer() != player)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                findBall();
            }

        }
    }

    private void movePlayerAround()
    {
        if (player.getBallCath())
        {
            while (timer > 0)
            {
                player.move(player.getThrowingDirection());
                timer -= Time.deltaTime;
            }
            timer = gameManager.aiTimer;
        }
    }


    private void OnEnable()
    {
        //Player.playerCatchball += onePlayerHaveBall;
        Player.onePlayerThrow += actionOnthrow;
        BasketScore.onePlayerScore += onScoreEvent;
    }

    private void OnDisable()
    {
        //Player.playerCatchball -= onePlayerHaveBall;
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
        if (ball.getCurrentPlayer() != null && ball.getCurrentPlayer() != player)
        {
            player.jump();
        }
        //ballCatch = player.getBallCath();
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

        if (dotProduct > 0.5f)
        {
            /*Debug.Log("AI going right");
            Debug.Log("dot product = " + 1);*/
            ballDirection = true;
        }
        else if (dotProduct < -0.5f)
        {
            ballDirection = false;
        }
    }

    private void findBall()
    {

        findBallDirection();
        player.move(ballDirection);

    }

}
