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
            //Start Event When Score
            onePlayerScore?.Invoke(true);
            score++;

            //disable collider to avoid multiple scoring
            playerScore.GetComponent<TextMeshProUGUI>().text = score.ToString();
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        StartCoroutine(ResetBallPosition(other));

    }

    private IEnumerator ResetBallPosition(Collider other)
    {

        yield return new WaitForSeconds(2f);

        Vector3 resetValue = new Vector3(0, 5, 0);
        other.gameObject.GetComponent<Ball>().ResetBallPosition(resetValue);

        //activate collider after scoring
        this.gameObject.GetComponent<BoxCollider>().enabled = true;

       //Start Event When Score
        onePlayerScore?.Invoke(false);

    }

    public int getScore()
    {
        return score;
    }
}
