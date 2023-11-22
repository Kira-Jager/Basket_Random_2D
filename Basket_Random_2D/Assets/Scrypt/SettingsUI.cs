using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public GameObject questionPanel = null;
    public GameObject OptionsPanel = null;
    
    


    private bool selectedOponent = false;

    private ColorBlock originalColors;

    // Start is called before the first frame update
    void Start()
    {
        questionPanel.SetActive(false);
        OptionsPanel.SetActive(false);


    }

    private void OnEnable()
    {
        UiManager.selectOponnent += getSelectedOponent;
    }

    private void OnDisable()
    {
        UiManager.selectOponnent -= getSelectedOponent;

    }

    public void getSelectedOponent(bool state)
    {
        selectedOponent = state;
        //Debug.Log("Selected Oponent in UI Settings " + (state == true ? "One Player" : "Two player"));
    }


    public void PlayerQuestion()
    {
        // Check if questionPanel is not null
        if (questionPanel != null && !OptionsPanel.activeSelf)
        {
            // Toggle the active state of questionPanel
            questionPanel.SetActive(!questionPanel.activeSelf);
            Time.timeScale = !questionPanel.activeSelf ? 1 : 0;

            GameManager.instance.DisableAllAudio();

            // Check if transform has at least two children

            // Get the first and second child game objects
            GameObject firstChild = questionPanel.transform.GetChild(0).gameObject;
            GameObject secondChild = questionPanel.transform.GetChild(1).gameObject;

            //originalColors = firstChild.GetComponent<Button>().colors;

            // Check if selectedOponent is not null


            // If selectedOponent is false, it means two players are selected
            if (selectedOponent == false)
            {
                firstChild.SetActive(false);
                secondChild.SetActive(true);
            }
            // If selectedOponent is true, it means one player is selected
            else
            {
                firstChild.SetActive(true);
                secondChild.SetActive(false);
            }

        }
    }

    public void GameOptions()
    {
        if(!questionPanel.activeSelf)
        {
            OptionsPanel.SetActive(!OptionsPanel.activeSelf);
            Time.timeScale = !OptionsPanel.activeSelf ? 1 : 0;
        }
        

    }

}
