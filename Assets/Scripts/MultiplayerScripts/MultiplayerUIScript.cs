using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIScript : MonoBehaviour
{

    private Transform modal;
    private Transform modalPause;

    private Transform modalWin;
    private Transform modalLose;



    private Button buttonPause;
    private Text modalMessage;

    [SerializeField]
    private Button buttonBackToGame;

    // Start is called before the first frame update
    void Start()
    {
        modal = transform.Find("Modal");
        modalPause = transform.Find("PausePanel");
        buttonPause = transform.Find("PauseButton").GetComponent<Button>();
        modalWin = transform.Find("EndPanelWinner");
        modalLose = transform.Find("EndPanelLoser");
        modalMessage = modal.Find("Container").GetComponentInChildren<Text>();

        MyInit();
    }

    private void MyInit()
    {
        buttonPause.onClick.AddListener(() =>
        {
            modalPause.gameObject.SetActive(true);
        });

        buttonBackToGame.onClick.AddListener(() =>
        {
            modalPause.gameObject.SetActive(false);
        });
    }
    
    public void ShowModal(string message)
    {
        modalMessage.text = message;
        modal.gameObject.SetActive(true);
    }

    public void CloseModal()
    {
        modal.gameObject.SetActive(false);
    }

    public void ShowModalSend(string message)
    {
        ShowModal(message);
        PlayerHost.Server.Send(GameConstants.HostAction+"SHOWMODAL%"+message,true);
    }

    public void CloseModalSend()
    {
        CloseModal();
        PlayerHost.Server.Send(GameConstants.HostAction + "CLOSEMODAL", true);
    }

    public void ShowClientLost()
    {
        OpenModalWin();
        PlayerHost.Server.Send(GameConstants.HostAction + "YOULOSE", true);
    }


    public void ShowClientWon()
    {
        OpenModalLose();
        PlayerHost.Server.Send(GameConstants.HostAction + "YOUWON", true);
    }

    public void OpenModalWin()
    {
        modalWin.gameObject.SetActive(true);    
    }

    public void OpenModalLose()
    {
        modalLose.gameObject.SetActive(true);
    }


}
