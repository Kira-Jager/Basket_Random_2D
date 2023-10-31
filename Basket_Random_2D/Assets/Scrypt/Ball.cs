using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;

    private bool ballCathed = false;
    private bool ballKinematic = false;
    private bool ballThrowing = false;

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
        //kinematicOff is the same as isJumping == true in Player;
        //It here to avoid the ball movement in animation being affected by the gravity 
        if (ballCathed)
        {
            setAnimation("drible", true);
        }

        if (ballThrowing)
        {
            rb.isKinematic = false;
            //disableKinematic();

            //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            transform.SetParent(null);

            // Calculate the direction to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Calculate the velocity needed to reach the target
            float initialVelocity = Mathf.Sqrt(throwingForce * Physics.gravity.magnitude / Mathf.Sin(2 * Mathf.Deg2Rad * Vector3.Angle(direction, Vector3.up)));

            // Set the velocity to achieve the calculated initial velocity
            rb.velocity = direction * initialVelocity;

            // Reset the flag to stop updating the velocity
            ballThrowing = false;


        }

        if (ballCathed == false && ballKinematic == true)
        {
            disableKinematic();
            //Debug.Log("ball catch == false");

        }

    }


    private void throwBall()
    {
        setAnimation("drible", false);

        ballThrowing = true;
        ballCathed = false;

        //Debug.Log("ball throw");

    }

    private void OnEnable()
    {
        InputController.onJump += disableKinematic;
        InputController.onThrow += throwBall;
    }

    private void OnDisable()
    {
        InputController.onJump -= disableKinematic;
        InputController.onThrow -= throwBall;

    }


    private void disableKinematic()
    {
        if (ballKinematic == true)
        {
            rb.isKinematic = false;
            ballKinematic = false;
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


    private void setAnimation(string animationName, bool animationState)
    {
        animator.SetBool(animationName, animationState);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !ballCathed)
        {
            Debug.Log("Contact with player");

            transform.SetParent(collision.gameObject.transform, true);

            ballCathed = true;
            ballCatched?.Invoke();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("ground") && !ballThrowing)
        {
            activateKinematic();

        }
    }

      /* private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (ballThrowing)
                {
                    ballCathed = false;
                }
                    Debug.Log("collision exit");

            }
        }*/
}
