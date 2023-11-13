using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool ballCathed = false;
    private bool playerJumping = false;

    private Animator animator;
    private Transform target;

    private Rigidbody rb;

    private GameManager manager;

    private Player currentPlayer = null;
    private Player previousPlayer = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void startBallDrible()
    {
        if (ballCathed && playerJumping == false )
        {
            setAnimation("drible", true);
        }
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

        ballCathed = false;
        rb.velocity = Vector3.zero;

        throwBallProjectile();

        playerJumping = false;
        //Debug.Log("ball throw");
    }

    public void ballCatch(Player player)
    {
        animator.enabled = true;
        ballCathed = true;
        rb.isKinematic = true;


        currentPlayer = player;
        if (previousPlayer == null)
        {
            previousPlayer = currentPlayer;
        }

        startBallDrible();
    }

    public void ballJump()
    {
        //Debug.Log("Inside ball Jump");

        playerJumping = true;

        if (ballCathed)
        {
            setAnimation("drible", false);

            animator.enabled = false;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentPlayer != null && collision.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            //Debug.Log("Previous player name " + previousPlayer.gameObject.name);
            //Debug.Log("Current player name " + currentPlayer.gameObject.name);
            if (previousPlayer != currentPlayer)
            {
                //Debug.Log("It is another player");
                previousPlayer.anotherPlayerGetBall();

                previousPlayer = currentPlayer;
            }
        }
    }

    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

    public void ResetBallPosition(Vector3 resetValue)
    {
        rb.velocity = Vector3.zero;

        transform.position = resetValue;
    }

}
