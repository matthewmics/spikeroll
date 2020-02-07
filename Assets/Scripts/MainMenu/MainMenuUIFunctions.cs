using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIFunctions : MonoBehaviour
{
    private AudioSource audioSource;
    public GameObject VsPlayerMenu;
    private GameObject menuCamera;
    public GameObject HowToPlay;
    public GameObject SettingsPanel;
    public GameObject MainMenuObject;
    public GameObject VSAllCountriesPanel;
    public GameObject VSObject;
    public Text VSTitle;

    public Toggle BgmToggle;
    public Toggle SfxToggle;

    public PointsDisplay PointsDisplay;

    public void Add25Points()
    {
        PointsUtil.AddPoints(25);
        PointsDisplay.UpdatePoints();
    }
    public void Reduce25Points()
    {
        PointsUtil.AddPoints(-25);
        PointsDisplay.UpdatePoints();
    }

    // Start is called before the first frame update
    void Start()
    {
      //  PointsUtil.AddPoints(1000);
        menuCamera = GameObject.FindGameObjectWithTag("MainCamera");
        audioSource = menuCamera.GetComponent<AudioSource>();

        audioSource.volume = AudioUtil.GetBgm();

        BgmToggle.isOn = AudioUtil.GetBgm() == 1f;
        SfxToggle.isOn = AudioUtil.GetSfx() == 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }

    public void OpenVSAI()
    {
        VSTitle.text = "vs A.I.";
        SelectionSingleton.instance.VersusType = VersusType.AI;
        OpenVS();
    }
    public void OpenVSPlayer()
    {
        MainMenuObject.SetActive(false);
        VsPlayerMenu.SetActive(true);
    }

    private void OpenVS()
    {
        SelectionSingleton.instance.IsPractice = false;
        SelectionSingleton.instance.IsMinigame = false;
        MainMenuObject.SetActive(false);
        VSObject.SetActive(true);
    }

    private void VersusSelected()
    {

        SelectionSingleton i = SelectionSingleton.instance;
        i.IsMinigame = false;
        switch (i.VersusType)
        {
            case VersusType.AI:
                SceneManager.LoadScene("VSAITeamSelection");
                break;
            case VersusType.PLAYER:
                /// VS PLAYER LOGIC HERE
                break;
        }
    }

    public void Select2v2()
    {
        SelectionSingleton i = SelectionSingleton.instance;
        i.VersusType = VersusType.AI;
        i.IsPractice = false;
        i.NumberOfPlayers = 2;
        VersusSelected();
    }

    public void Select3v3()
    {
        SelectionSingleton i = SelectionSingleton.instance;
        i.VersusType = VersusType.AI;
        i.IsPractice = false;
        i.NumberOfPlayers = 3;
        VersusSelected();
    }

    public void SelectPractice()
    {

        SelectionSingleton i = SelectionSingleton.instance;
        i.IsPractice = true;
        i.IsMinigame = false;
        i.NumberOfPlayers = 3;
        VersusSelected();
    }

    public void ReturnToMenu()
    {
        VsPlayerMenu.SetActive(false);
        VSAllCountriesPanel.SetActive(false);
        MainMenuObject.SetActive(true);
        VSObject.SetActive(false);
    }

    public void OpenVSAllCOuntries()
    {

        VSAllCountriesPanel.SetActive(true);
        MainMenuObject.SetActive(false);
    }

    public void BgmToggled()
    {
        AudioUtil.SetBgm((BgmToggle.isOn) ? 1f : 0f);
        audioSource.volume = AudioUtil.GetBgm();
    }
    public void SfxToggled()
    {
        AudioUtil.SetSfx((SfxToggle.isOn) ? 1f : 0f);
    }

    public void OpenHowTopPlay()
    {
        HowToPlay.SetActive(true);
    }
    public void CloseHowTopPlay()
    {
        HowToPlay.SetActive(false);
    }
}
