using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientCountrySelection : MonoBehaviour
{

    PlayerClient server;

    [SerializeField]
    private Button SelectCountryButton;

    [HideInInspector]
    public ButtonClientSelection SelectedPlayer1 = null;
    [HideInInspector]
    public ButtonClientSelection SelectedPlayer2 = null;
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerClient>();


        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var gObj = transform.GetChild(i).GetComponent<ButtonClientSelection>();
            var gBtn = transform.GetChild(i).GetComponent<Button>();
            gObj.DeselectPlayer1();
            gObj.DeselectPlayer2();
            gBtn.onClick.AddListener(() =>
            {
                if (!SelectCountryButton.interactable)
                    return;


                if (SelectedPlayer2 != null)
                {
                    SelectedPlayer2.DeselectPlayer2();
                }
                SelectedPlayer2 = gObj;
                gObj.SelectPlayer2();
                server.SendPlayer2Selection(gObj.name);
               /// server.SendPlayer1Selection(gObj.name);
                //Debug.Log(gObj.name);
            });
        }

        //SelectPlayer1("PH");
        SelectCountryButton.onClick.AddListener(()=>
        {

            if (SelectedPlayer2 != null)
            {
                string msg = "CLIENTACTION|SELECTCOUNTRY%" + SelectedPlayer2.name;
                server.Send(msg, true);
                Debug.Log("country selected");
                SelectCountryButton.interactable = false;
                SelectCountryButton.transform.GetChild(0).GetComponent<Text>().text = "WAITER FOR OTHER PLAYER";
            }
        });

    }

    public void SelectPlayer1(string sel)
    {
        var obj = this.transform.Find(sel).GetComponent<ButtonClientSelection>();

        if(SelectedPlayer1 != null)
        {
            SelectedPlayer1.DeselectPlayer1();
        }
        SelectedPlayer1 = obj;
        obj.SelectPlayer1();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
