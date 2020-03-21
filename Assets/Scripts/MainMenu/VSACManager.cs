using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VSACManager : MonoBehaviour
{
    public Transform VersusPanel;
    VSAllCountriesModel vsacm;
    List<CountryModel> cm;

    public Text AllocationPointsText;

    public Text AllocationInt;
    public Text AllocationPow;
    public Text AllocationSpd;

    private int _points;
    private int _int;
    private int _spd;
    private int _pow;

    public GameObject AllocationPanel;

    // Start is called before the first frame update
    void Start()
    {
        vsacm = VSAllCountriesModel.GetCurrentGame();
        cm = CountryModel.GetCountries();

        SetPointsText(vsacm.RemainingPoints);
        SetIntText(vsacm.AllocatedInt);
        SetPowText(vsacm.AllocatedPower);
        SetSpdText(vsacm.AllocatedSpeed);

        Initialize();
    }


    public void Add5Allocation()
    {
        SetPointsText(5);
    }

    public void SaveAllocation()
    {
        AllocationInt.color = Color.white;
        AllocationSpd.color = Color.white;
        AllocationPow.color = Color.white;

        vsacm.AllocatedInt = _int;
        vsacm.AllocatedPower = _pow;
        vsacm.AllocatedSpeed = _spd;
        vsacm.RemainingPoints = _points;

        VSAllCountriesModel.SaveInstance(vsacm);

    }

    private void SetPointsText(int point)
    {
        _points += point;
        AllocationPointsText.text = "Current allocation points : "+_points;
    }

    private void SetIntText(int point)
    {
        _int += point;
        AllocationInt.text = "" + _int;
        AllocationInt.color = Color.white;
        if (_int != vsacm.AllocatedInt)
        {
            AllocationInt.color = Color.yellow;
        }
    }
    private void SetPowText(int point)
    {
        _pow += point;
        AllocationPow.text = "" + _pow;
        AllocationPow.color = Color.white;
        if (_pow != vsacm.AllocatedPower)
        {
            AllocationPow.color = Color.yellow;
        }
    }
    private void SetSpdText(int point)
    {
        _spd += point;
        AllocationSpd.text = "" + _spd;
        AllocationSpd.color = Color.white;
        if (_spd != vsacm.AllocatedSpeed)
        {
            AllocationSpd.color = Color.yellow;
        }
    }

    public void MinusInt()
    {
        if(_int > vsacm.AllocatedInt)
        {
            SetPointsText(1);
            SetIntText(-1);
        }
    }

    public void MinusPow()
    {
        if (_pow > vsacm.AllocatedPower)
        {
            SetPointsText(1);
            SetPowText(-1);
        }
    }
    public void MinusSpd()
    {
        if (_spd > vsacm.AllocatedSpeed)
        {
            SetPointsText(1);
            SetSpdText(-1);
        }
    }

    public void AddInt()
    {
        if (_int < 100)
        {
            if(_points > 0)
            {
                SetPointsText(-1);
                SetIntText(1);
            }
        }
    }
    public void AddPow()
    {
        if (_pow < 100)
        {
            if (_points > 0)
            {
                SetPointsText(-1);
                SetPowText(1);
            }
        }
    }
    public void AddSpd()
    {
        if (_spd < 100)
        {
            if (_points > 0)
            {
                SetPointsText(-1);
                SetSpdText(1);
            }
        }
    }

    private void Initialize()
    {
        VersusPanel.GetChild(vsacm.CurrentLevel).GetChild(0).GetChild(0).gameObject.SetActive(true);


        for(int i = 0; i < VersusPanel.childCount; i++)
        {
            var obj = VersusPanel.GetChild(i).GetChild(2).GetComponentInChildren<RawImage>();

            obj.texture = GetOpponentCountry(i).Flag;
            //foreach(var j in cm)
            //{
            //    if (vsacm.OrderOfCountries[i].Equals(j.ShortName))
            //    {

            //        obj.texture = j.Flag;
            //        break;
            //    }
            //}
        }
    }

    public CountryModel GetOpponentCountry(int index)
    {
        CountryModel result = null;

        foreach (var j in cm)
        {
            if (vsacm.OrderOfCountries[index].Equals(j.ShortName))
            {

                result = j;
                break;
            }
        }

        return result;
    }


    public void PlayNextMatch()
    {
        SelectionSingleton.instance.IsPractice = false;
        SelectionSingleton.instance.IsMinigame = true;


        SelectionSingleton.instance.OpponentCountry = GetOpponentCountry(vsacm.CurrentLevel);
        SelectionSingleton.instance.PlayerCountry = new CountryModel
        {
            Intelligence = vsacm.AllocatedInt,
            Power = vsacm.AllocatedPower,
            Speed = vsacm.AllocatedSpeed,
            ShortName = "YOU", Name = "YOU",
            AnimatorLocation = "Controllers/MY/my"
        };
        SelectionSingleton.instance.NumberOfPlayers = 3;

        SceneManager.LoadScene("VSAI");
    }

    public void ShowAllocationPanel()
    {
        AllocationPanel.SetActive(true);
    }



    public void HideAllocationPanel()
    {

        AllocationPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
