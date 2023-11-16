using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ballObject;
    public GameObject AiBuffer;

    private Player player;

    private Ball ball = null;

    private float BufferRelativeMinX = 0f;
    private float BufferRelativeMaxX = 0f;

    void Start()
    {
        player = GetComponent<Player>();
        ball = ballObject.GetComponent<Ball>();
        MeshRenderer meshRenderer = AiBuffer.GetComponent<MeshRenderer>();

         BufferRelativeMinX = meshRenderer.bounds.min.x - player.transform.position.x;
         BufferRelativeMaxX = meshRenderer.bounds.max.x - player.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            if (!player.getBallCath() && ball.getBallOnGround() == true)
            {
                Invoke("findBall",.8f);
                //ballCatch = player.getBallCath();
            }

            else
            {
                player.stopRunning();
            }

            if(player.getBallCath())
            {
                Invoke("throwBall",.6f);
            }

           /* if(player.distancePlayerTarget <= threshold)
            {
                movePlayerAround();
            }*/
        }

    }


    private void movePlayerAround()
    {
        if (player.getBallCath())
        {

        }
    }

    private void OnEnable()
    {
        Player.playerCatchball += onePlayerHaveBall;
        Player.onePlayerThrow += actionOnthrow;
    }

    private void OnDisable()
    {
        Player.playerCatchball -= onePlayerHaveBall;
        Player.onePlayerThrow -= actionOnthrow;
    }



    private void onePlayerHaveBall()
    {
        //ballCatch = player.getBallCath();
    }
    
    private void actionOnthrow()
    {
        //ballCatch = player.getBallCath();
    }

    private void throwBall()
    {
        player.jump();
        player.playerThrowBall();

    }


    private bool findBallDirection()
    {
        float dotProduct = 0;
        Vector3 ballPosition = ballObject.transform.position;
        Vector3 playerDirection = transform.forward;

        Vector3 targetDirection = (transform.position - ballPosition).normalized;

        if(player.controlThrowingDirection() == true)
        {
            dotProduct = Vector3.Dot(playerDirection , targetDirection);
        }
        else
        {
            dotProduct = Vector3.Dot(- playerDirection , targetDirection);
        }


        if (dotProduct > 0)
        {
            /*Debug.Log("AI going right");
            Debug.Log("dot product = " + 1);*/
            return true;

        }

        /*Debug.Log("AI going left");
        Debug.Log("dot product = " + -1);*/
        return false;
    }

    private void findBall()
    {
        player.move(findBallDirection());
    }

}
