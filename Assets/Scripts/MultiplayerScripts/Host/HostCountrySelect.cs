using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostCountrySelect : MonoBehaviour
{

    public Button ButtonSelectCountry; 

    PlayerHost server;

    [HideInInspector]
    public ButtonHostCountrySelect SelectedPlayer1 = null;
    [HideInInspector]
    public ButtonHostCountrySelect SelectedPlayer2 = null;

    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerHost>();

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var gObj = transform.GetChild(i).GetComponent<ButtonHostCountrySelect>();
            var gBtn = transform.GetChild(i).GetComponent<Button>();
            gObj.DeselectPlayer1();
            gObj.DeselectPlayer2();
            gBtn.onClick.AddListener(() =>
            {
                if (!ButtonSelectCountry.interactable)
                    return;

                if(SelectedPlayer1 != null)
                {
                    SelectedPlayer1.DeselectPlayer1();                    
                }
                SelectedPlayer1 = gObj;
                gObj.SelectPlayer1();

                server.SendPlayer1Selection(gObj.name);
               // Debug.Log(gObj.name);
            });
        }

        ButtonSelectCountry.onClick.AddListener(() =>
        {
            server.MultiplayerSession.P1Country = SelectedPlayer1.name;
            ButtonSelectCountry.interactable = false;
            ButtonSelectCountry.transform.GetChild(0).GetComponent<Text>().text = "WAITER FOR OTHER PLAYER";
        });
    }


    public void SelectPlayer2(string sel)
    {
        var obj = this.transform.Find(sel).GetComponent<ButtonHostCountrySelect>();

        if (SelectedPlayer2 != null)
        {
            SelectedPlayer2.DeselectPlayer2();
        }
        SelectedPlayer2 = obj;
        obj.SelectPlayer2();

    }


    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrWhiteSpace(server.MultiplayerSession.P1Country) &&
           !string.IsNullOrWhiteSpace(server.MultiplayerSession.P2Country))
        {
            // gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
            Debug.Log("both players ready");

            server.MultiplayerSession.CountryPl = CountryModel.GetCountryByShortName(server.MultiplayerSession.P1Country);
            server.MultiplayerSession.CountryP2 = CountryModel.GetCountryByShortName(server.MultiplayerSession.P2Country);

            server.NetworkObjects.transform.Find("player1").GetComponent<Animator>().runtimeAnimatorController =
                server.MultiplayerSession.CountryPl.ControllerBottom1;
            server.NetworkObjects.transform.Find("player2").GetComponent<Animator>().runtimeAnimatorController =
                server.MultiplayerSession.CountryP2.ControllerTop1;

            server.NetworkObjects.SetActive(true);


            server.Send("HOSTACTION|PICKINGSDONE%" + server.MultiplayerSession.P1Country + "%" + server.MultiplayerSession.P2Country
                , true);
        }
    }

}
