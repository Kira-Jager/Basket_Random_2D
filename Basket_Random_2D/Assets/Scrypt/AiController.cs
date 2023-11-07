using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ballObject;

    private Player player;

    private bool ballCatch;
    void Start()
    {
        player = GetComponent<Player>();
        Ball ball = ballObject.GetComponent<Ball>();
        ballCatch = player.getBallCath();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ballCatch)
        {
            findBall();
            ballCatch = player.getBallCath();
        }
    }

    private void OnEnable()
    {
        Player.playerCatchball += onePlayerHaveBall;
    }

    private void OnDisable()
    {
        Player.playerCatchball -= onePlayerHaveBall;
    }



    private void onePlayerHaveBall()
    {
        ballCatch = player.getBallCath();

        
    }

    private bool findBallDirection()
    {
        Vector3 ballPosition = ballObject.transform.position;
        Vector3 targetDirection = (ballPosition - transform.position).normalized;

        float dotProduct = Vector3.Dot(ballPosition, targetDirection);

        if (dotProduct > 0)
        {
            Debug.Log("AI going right");
            return true;

        }
        else
        {
            Debug.Log("AI going left");

            return false;
        }
    }

    private void findBall()
    {
        if(player.getBallCath() == false)
        {
            player.move(findBallDirection());
        }
    }

}
