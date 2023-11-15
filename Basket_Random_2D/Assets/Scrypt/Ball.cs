using System.Collections;
using System.Collections.Generic;
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
        if (currentPlayer != null && collision.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            //Debug.Log("Previous player name " + previousPlayer.gameObject.name);
            //Debug.Log("Current player name " + currentPlayer.gameObject.name);
            if (previousPlayer != currentPlayer)
            {
                previousPlayer.anotherPlayerGetBall();

                previousPlayer = currentPlayer;
            }

        }
    }

    public void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

    public void ResetBallPosition(Vector3 resetValue)
    {
        rb.velocity = Vector3.zero;

        transform.position = resetValue;
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

}
