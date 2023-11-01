using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;

    private bool ballCathed = false;
    private bool playerJumping = false;

    private bool ballKinematic = false;
    private bool ballThrowing = false;
    private bool isFacingTarget = false;
    private Transform target;

    private Rigidbody rb;

    public GameObject player1Object;
    public GameObject player2Object;

    private Player player1;
    private Player player2;

    public float throwingForce = 10f;

    /* public delegate void OnBallCatched();
     public event OnBallCatched ballCatched;*/


    /*private void Awake()
    {
        player1 = player1Object.GetComponent<Player>();
        player2 = player2Object.GetComponent<Player>();

        
        //Debug.Log("Pllayer declared");
    }*/
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCathed && !playerJumping)
        {
            setAnimation("drible", true);
        }

    }

    private void throwBallProjectile()
    {
        rb.isKinematic = false;

        //Unparrent the ball
        transform.SetParent(null);


        //EditorApplication.isPaused = true;
        // Calculate the direction to the target

        Vector3 direction = (target.position - transform.position).normalized;

        // Calculate the velocity needed to reach the target
        float initialVelocity = Mathf.Sqrt(throwingForce * Physics.gravity.magnitude / Mathf.Sin(2 * Mathf.Deg2Rad * Vector3.Angle(direction, Vector3.up)));

        // Set the velocity to achieve the calculated initial velocity
        rb.velocity = direction * initialVelocity ;

        // enable the animator to allow dribling
        animator.enabled = true;
    }

    public void throwBall(Transform targeted)
    {
        //if (ballCathed && controlThrowingDirection())
        if (ballCathed )
        {
            target = targeted;

            setAnimation("drible", false);

            ballThrowing = true;

            ballCathed = false;

            throwBallProjectile();

        }
            playerJumping = false;
        Debug.Log("ball throw");

    }


    //change to use it player
/*   private void OnEnable()
    {
        player1.onJumpKeyPressed += ballJump;
        player1.onThrowKey += throwBall;
        player1.playerCatchball += ballCatch;

        player2.onJumpKeyPressed += ballJump;
        player2.onThrowKey += throwBall;
        player2.playerCatchball += ballCatch;
    }

    private void OnDisable()
    {
        player1.onJumpKeyPressed -= ballJump;
        player1.onThrowKey -= throwBall;
        player1.playerCatchball -= ballCatch;
        
        player2.onJumpKeyPressed -= ballJump;
        player2.onThrowKey -= throwBall;
        player2.playerCatchball -= ballCatch;


    }*/

    public void ballCatch()
    {
        ballCathed = true;
        rb.isKinematic = true;
    }

    public void ballJump()
    {
        Debug.Log("Inside ball Jump");

        playerJumping = true;

        if (ballCathed)
        {
            animator.enabled = false;
        }

    }

    private bool controlThrowingDirection()
    {
        Vector3 playerDirection = transform.forward;
        Vector3 targetDirection = (target.position - transform.position).normalized;

        float dotProduct = Vector3.Dot(playerDirection, targetDirection);

        if (dotProduct > 0)
        {
            isFacingTarget = true;
        }
        else
        {
            isFacingTarget = false;
        }

        return isFacingTarget;
    }

    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }



}
