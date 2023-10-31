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

    private Rigidbody rb;

    public Transform target;
    public float throwingForce = 10f;

    public delegate void OnBallCatched();
    public static event OnBallCatched ballCatched;



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

            //setAnimation("drible", true);
        }

        if (playerJumping)
        {
            

            // setAnimation("drible", false);
            //setAnimation("ballJump", true);
            playerJumping = false;

            if(controlThrowingDirection() == false)
            {

            }
        }




        if (ballThrowing)
        {
            throwProjectile();

        }

        if (ballCathed == false && ballKinematic == true)
        {
            disableKinematic();
            //Debug.Log("ball catch == false");

        }

    }

    private void throwProjectile()
    {
        rb.isKinematic = false;

        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.SetParent(null);


        //EditorApplication.isPaused = true;
        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Calculate the velocity needed to reach the target
        float initialVelocity = Mathf.Sqrt(throwingForce * Physics.gravity.magnitude / Mathf.Sin(2 * Mathf.Deg2Rad * Vector3.Angle(direction, Vector3.up)));

        // Set the velocity to achieve the calculated initial velocity
        rb.velocity = direction * initialVelocity;

        // Reset the flag to stop updating the velocity
        ballThrowing = false;
    }

    private void throwBall()
    {
        if (ballCathed && controlThrowingDirection())
        {
            setAnimation("drible", false);


            ballThrowing = true;
            ballCathed = false;

            animator.enabled = true;
        }
            playerJumping = false;
        //Debug.Log("ball throw");

    }


    //change to use it player
    private void OnEnable()
    {
        InputController.onJump += ballJump;
        InputController.onThrow += throwBall;
    }

    private void OnDisable()
    {
        InputController.onJump -= ballJump;
        InputController.onThrow -= throwBall;

    }

    private void ballJump()
    {
        playerJumping = true;

        if (ballCathed)
        {
            animator.enabled = false;
        }

        disableKinematic();
    }


    private void disableKinematic()
    {
        if (ballKinematic == true)
        {
            rb.isKinematic = false;
            ballKinematic = false;


            //setAnimation("ballJump", true);
            //Debug.Log("Kinematic disable");
        }
    }

    private void activateKinematic()
    {
        if (!ballKinematic)
        {
            rb.isKinematic = true;
            ballKinematic = true;
            //Debug.Log("Kinematic activate");

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !ballCathed)
        {
            Debug.Log("Contact with player");
            ballCatched?.Invoke();

            transform.SetParent(collision.gameObject.transform, true);

            ballCathed = true;

            //reset ball rotation
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("ground") && !ballThrowing)
        {
            activateKinematic();

        }
    }

}
