using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardUI : MonoBehaviour
{

    private GameObject _player1;
    private GameObject _player2;

    public static ScoreboardUI Scoreboard;

    // Start is called before the first frame update
    void Start()
    {
        Scoreboard = this;
        _player1 = this.GetComponent<RectTransform>().Find("player1").gameObject;
        _player2 = this.GetComponent<RectTransform>().Find("player2").gameObject;
    }

    private void SetUIText(GameObject player,string uitext,string name)
    {

        player.transform.Find(uitext).GetComponent<Text>().text = name;
    }


    public void SetPlayer1Name(string name)
    {
        SetUIText(_player1,"name",name);
    }
    public void SetPlayer1Score(string name)
    {
        SetUIText(_player1, "score", name);
    }
    public void SetPlayer1Series(string name)
    {
        SetUIText(_player1, "series", name);
    }



    public void SetPlayer2Name(string name)
    {
        SetUIText(_player2,"name", name);
    }
    public void SetPlayer2Score(string name)
    {
        SetUIText(_player2, "score", name);
    }
    public void SetPlayer2Series(string name)
    {
        SetUIText(_player2, "series", name);
    }


    public void UpdateScoreboard()
    {
        MultiplayerGameSession g = PlayerHost.Server.GameSession;

        SetPlayer1Score(g.Player1Score+"");
        SetPlayer2Score(g.Player2Score+"");
        SetPlayer1Series(g.Player1Set+"");
        SetPlayer2Series(g.Player2Set+"");

        PlayerHost.Server.Send($"UPDATESCOREBOARD|{g.Player1Score}%{g.Player1Set}%{g.Player2Score}%{g.Player2Set}"
            ,true);

    }

    public void UpdateClient(string[] data)
    {

        SetPlayer1Score(data[0]);
        SetPlayer1Series(data[1]);

        SetPlayer2Score(data[2]);
        SetPlayer2Series(data[3]);
    }

}
