using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


public class Ball : MonoBehaviour
{
    private Animator animator;
    private Transform target;

    private Rigidbody rb;

    private Player currentPlayer = null;
    private Player previousPlayer = null;
    private Player lanceur = null;

    private bool ballOnGround = false;



    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void throwBallProjectile()
    {
        rb.isKinematic = false;

        // Unparent the ball
        transform.SetParent(null);

        // Calculate the direction to the target
        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        // Calculate the relative position to the target, including Y-axis
        Vector3 direction = (targetPosition - currentPosition);

        float angle = Mathf.Atan2(direction.y, direction.x);
        float g = 10f; //Physics.gravity.magnitude ; // acceleration due to gravity
        float R = direction.magnitude; // distance to the target
        float initialVelocity = Mathf.Sqrt((R * g) / Mathf.Abs(Mathf.Sin(2 * angle)));

        float initialVelocityX = initialVelocity * Mathf.Cos(angle);
        float initialVelocityY = initialVelocity * Mathf.Sin(angle);

        rb.AddForce(new Vector3(initialVelocityX, initialVelocityY, 0), ForceMode.VelocityChange);

    }



    public void throwBall(Transform targeted)
    {
        target = targeted;

        rb.velocity = Vector3.zero;

        lanceur = currentPlayer;

        ballOnGround = false;

        throwBallProjectile();

        //Debug.Log("ball throw");
    }

    public void ballCatch(Player player)
    {
        animator.enabled = true;
        rb.isKinematic = true;

        currentPlayer = player;

        if (previousPlayer == null)
        {
            previousPlayer = currentPlayer;
        }

        transform.SetParent(currentPlayer.playerHand);

        ResetBallPosition(currentPlayer.playerHand.position);

        setAnimation("drible", true);

    }

    public void ballJump()
    {

        Vector3 updateHandPosition = new Vector3(currentPlayer.playerHand.position.x +.2f, currentPlayer.playerHand.position.y + 1f, currentPlayer.playerHand.position.z);

        ResetBallPosition(updateHandPosition);

        setAnimation("drible", false);

        animator.enabled = false;


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("ground") || collision.gameObject.layer != LayerMask.NameToLayer("player"))
        {
            if (!GameManager.instance.IsPlayingAudio())
            {
                GameManager.instance.playAudio(GameManager.instance.ballCollisionSound, loop: false);
            }
        }

        if (currentPlayer != null && collision.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            //Debug.Log("Previous player name " + previousPlayer.gameObject.name);
            //Debug.Log("Current player name " + currentPlayer.gameObject.name);
            if (previousPlayer != currentPlayer)
            {
                ballOnGround = false;

                previousPlayer.anotherPlayerGetBall();

                previousPlayer = currentPlayer;

            }

        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            // this is for the AI to know that ball is on ground 
            ballOnGround = true;
            GameManager.instance.playAudio(GameManager.instance.drible_audio, false);

        }
    }


    public bool getBallOnGround()
    {
        //Debug.Log("Ball on ground = " + ballOnGround);

        return ballOnGround;
    }
    public void setBallOnGround()
    {
        //Debug.Log("Ball on ground = " + ballOnGround);

        ballOnGround = true;
    }

    public void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

    public void ResetBallPosition(Vector3 resetValue)
    {
        rb.velocity = Vector3.zero;

        transform.position = resetValue;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        ballOnGround = false;

    }

    public void resetBall()
    {
        setAnimation("drible", false);
        ResetBallPosition(new Vector3(0,3,0));
        transform.SetParent(null);
        rb.isKinematic = false;
    }

    public Player getPlayerWhoScore()
    {
        //the player who strow the ball is the scorer if case of score
        return lanceur;
    }

    public Player getCurrentPlayer()
    {
        //this is to help the AI controller to defend
        return currentPlayer;
    }
    
    public Player getPreviousPlayer()
    {
        //this is to help the AI controller to defend
        return previousPlayer;
    }



}
