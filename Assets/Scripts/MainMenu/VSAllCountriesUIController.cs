
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VSAllCountriesUIController : MonoBehaviour
{
    public GameObject ContinueButton;
    // Start is called before the first frame update
    void Start()
    {
        if(ContinueButton != null)
        ContinueButton.GetComponent<Button>().interactable = VSAllCountriesModel.HasCurrentGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToAllCountriesGame()
    {

        SceneManager.LoadScene("VSAllCountries");
    }


    public void StartNewGame()
    {
        VSAllCountriesModel.StartNew();

        GoToAllCountriesGame();
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
