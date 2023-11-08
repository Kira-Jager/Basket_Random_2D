using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasketScore : MonoBehaviour
{
    private int score;
    public GameObject playerScore;
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
            score++;
            playerScore.GetComponent<TextMeshProUGUI>().text = score.ToString();
        }
    }
}
