using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIFunctions : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject camera;
    public GameObject SettingsPanel;
    public GameObject MainMenuObject;
    public GameObject VSObject;
    public Text VSTitle;

    public Toggle BgmToggle;
    public Toggle SfxToggle;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        audioSource = camera.GetComponent<AudioSource>();

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
        VSTitle.text = "vs Player";
        SelectionSingleton.instance.VersusType = VersusType.PLAYER;
        OpenVS();
    }

    private void OpenVS()
    {

        MainMenuObject.SetActive(false);
        VSObject.SetActive(true);
    }

    private void VersusSelected()
    {

        SelectionSingleton i = SelectionSingleton.instance;
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
        i.NumberOfPlayers = 2;
        VersusSelected();
    }

    public void Select3v3()
    {
        SelectionSingleton i = SelectionSingleton.instance;
        i.NumberOfPlayers = 3;
        VersusSelected();
    }

    public void ReturnToMenu()
    {

        MainMenuObject.SetActive(true);
        VSObject.SetActive(false);
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
}
