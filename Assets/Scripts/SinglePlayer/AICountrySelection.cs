using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AICountrySelection : MonoBehaviour
{
    public GameObject playButton;
    [System.Serializable]
    public class SelectionProps
    {
        public RawImage FlagImage;
        public Text IntText;
        public Text PowerText;
        public Text SpeedText;
        public Text Label;
        public GameObject LockedPanel;

        public string User;
    }

    public Text UnlockText;
    public PointsDisplay PointsObject;

    public SelectionProps PlayerObjects;
    public SelectionProps OpponentObjects;

    private List<CountryModel> countries;

    int PlayerSelectedIndex = 0;
    int OpponentSelectedIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        PlayerObjects.User = "Player";
        OpponentObjects.User = "Enemy";
        countries = CountryModel.GetCountries();
        SetSelectedCountry(0, PlayerObjects);
        SetSelectedCountry(0, OpponentObjects);
    }

    public void PlayerPrev()
    {
        if (PlayerSelectedIndex > 0)
            PlayerSelectedIndex--;
        else
            PlayerSelectedIndex = countries.Count - 1;


        SetSelectedCountry(PlayerSelectedIndex, PlayerObjects);
    }
    public void OpponentPrev()
    {
        if (OpponentSelectedIndex > 0)
            OpponentSelectedIndex--;
        else
            OpponentSelectedIndex = countries.Count - 1;


        SetSelectedCountry(OpponentSelectedIndex, OpponentObjects);
    }
    public void PlayerNext()
    {
        if (PlayerSelectedIndex < countries.Count - 1)
            PlayerSelectedIndex++;
        else
            PlayerSelectedIndex = 0;


        SetSelectedCountry(PlayerSelectedIndex, PlayerObjects);
    }
    public void OpponentNext()
    {
        if (OpponentSelectedIndex < countries.Count - 1)
            OpponentSelectedIndex++;
        else
            OpponentSelectedIndex = 0;


        SetSelectedCountry(OpponentSelectedIndex, OpponentObjects);
    }

    void SetSelectedCountry(int index, SelectionProps user)
    {
        CountryModel m = countries[index];
        user.FlagImage.texture = m.Flag;
        user.Label.text = m.Name;
        user.IntText.text = m.Intelligence.ToString();
        user.PowerText.text = m.Power.ToString();
        user.SpeedText.text = m.Speed.ToString();
        user.LockedPanel.SetActive(m.IsLocked);

        if(user.User.Equals("Player"))
        UnlockText.text = $"Unlock for {m.UnlockCost}";

        if(countries[PlayerSelectedIndex].IsLocked ||
            countries[OpponentSelectedIndex].IsLocked)
        {
            playButton.SetActive(false);
        }
        else
        {
            playButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SelectionSingleton.instance.PlayerCountry = countries[PlayerSelectedIndex]; 
        SelectionSingleton.instance.OpponentCountry = countries[OpponentSelectedIndex]; 
        SceneManager.LoadScene("VSAI");
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BuyCharacter()
    {
        int currentPoints = PointsUtil.GetCurrentPoints();
        var country = countries[PlayerSelectedIndex];

        if(currentPoints >= country.UnlockCost)
        {
            Debug.Log("COUNTRY BOUGHT");
            country.IsLocked = false;
            PointsUtil.AddPoints(-country.UnlockCost);

            PointsObject.UpdatePoints();
            RefreshSelection();
            CountryModel.OverwriteCountries(countries);
            
        }
        else
        {
            Debug.Log("NOT ENOUGH POINTS");
        }
    }

    private void RefreshSelection()
    {
        SetSelectedCountry(PlayerSelectedIndex, PlayerObjects);
        SetSelectedCountry(OpponentSelectedIndex, OpponentObjects);
    }


}
