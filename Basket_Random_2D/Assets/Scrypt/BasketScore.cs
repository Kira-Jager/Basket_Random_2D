using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasketScore : MonoBehaviour
{
    private int score;
    public GameObject playerScore;

    public delegate void onePlayerScoreAction(bool state);
    public static event onePlayerScoreAction onePlayerScore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            Debug.Log("Collided with");
            // Event When Score
            onePlayerScore?.Invoke(true);
            score++;

            //disable collider to avoid multiple scoring
            playerScore.GetComponent<TextMeshProUGUI>().text = score.ToString();
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        StartCoroutine(scoreEvent(other));

    }

    private IEnumerator scoreEvent(Collider other)
    {

        yield return new WaitForSeconds(2f);

        other.gameObject.GetComponent<Ball>().resetBall();

        //activate collider after scoring
        this.gameObject.GetComponent<BoxCollider>().enabled = true;

        // Event When Score
        onePlayerScore?.Invoke(false);

    }

    public int getScore()
    {
        return score;
    }
}
