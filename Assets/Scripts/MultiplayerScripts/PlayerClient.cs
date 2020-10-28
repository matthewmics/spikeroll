using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerClient : MonoBehaviour
{


    private const int MAX_CONNECTION = 1;
    private int port = 5701;

    private int hostId;
    private int webHostId;

    private int myConnectionId = -1;

    private bool isConnected = false;

    private int reliableChannel;
    private int unreliableChannel;
    private int stateUpdateChannel;
    private int ourClientId;

    private byte error;
    private bool isStarted = false;
    private float connectionTime;

    public Text TextIPInput;
    string ipInput = "";

    public GameObject ConnectionPanel;
    public GameObject CountrySelectionPanel;
    public GameObject NetworkObjects;
    public static MultiplayerUIScript NetUI;
    public MultiplayerUIScript NetUIInstance;

    public CountryModel P1Country = null;
    public CountryModel P2Country = null;

    public RectTransform P2SpecialThreshold;
    public GameObject P2SpecialButton;
    public GameObject BrightFire;

    public void Start()
    {

        NetUI = NetUIInstance;
        ConnectionPanel.SetActive(true);
        NetworkObjects.SetActive(false);

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.ReliableSequenced);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        stateUpdateChannel = cc.AddChannel(QosType.StateUpdate);

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, 0, null);
    }


    public void ReturnToMenu()
    {
        Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
    private void Disconnect()
    {
        //NetworkTransport.Disconnect(hostId, myConnectionId);
        NetworkTransport.RemoveHost(hostId);
    }



    public void Connect()
    {

        ipInput = TextIPInput.text;

        try
        {
             NetworkTransport.Connect(hostId, ipInput, port, 0, out error);
            Debug.Log(myConnectionId);
        }catch(Exception err)
        {
            Debug.Log(err);
            Debug.Log(error);
            return;
        }
        //NetworkTransport.dis

        connectionTime = Time.time;

        isStarted = true;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ConnectionPanel.SetActive(false);
            NetworkObjects.SetActive(true);
        }



        if (!isStarted)
        {
            return;
        }

        
        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.Nothing:
                //Debug.Log("gg");
                break;
            case NetworkEventType.ConnectEvent:

                Debug.Log("connect event");
                if(myConnectionId == -1)
                {

                    isConnected = true;
                    this.myConnectionId = connectionId;
                    ConnectionPanel.SetActive(false);
                    CountrySelectionPanel.SetActive(true);
                    Debug.Log("Player " + connectionId + " has connected");

                    string tosend = $"{GameConstants.ClientAction}DELAY%{DelayReducer.DELAY_VALUE}";
                    Send(tosend,true);

                }
                //OnConnect(connectionId);
                break;
            case NetworkEventType.DataEvent:

                if (isConnected)
                {

                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    //Debug.Log("Receiving from : " + connectionId + " : " + msg);
                    //Debug.Log(msg);
                    string[] splitData = msg.Split('|');

                    //Debug.Log(splitData[1]);

                    switch (splitData[0])
                    {

                        case "P1SELECTION":
                            var hcs = GameObject.FindObjectOfType<ClientCountrySelection>();
                            hcs.SelectPlayer1(splitData[1]);
                            break;

                        case "P1LOCKIN":
                            ClientCountrySelection.Instance.HideCountry(splitData[1]);
                            break;
                        case "P2LOCKIN":
                            ClientCountrySelection.Instance.HideCountry(splitData[1], true);
                            break;


                        case "UPDATESTATE":
                            //Debug.Log(msg);
                            //Debug.Log(splitData[1]);
                            OnStateUpdate(splitData);
                            break;

                        case "HOSTACTION":
                            OnHostAction(splitData[1].Split('%'));
                        break;

                        case "ANIMATIONTRIGGER":
                            //ClientBall.RotateToDest();
                            string[] triggerData = splitData[1].Split('%');
                            NetworkObjects.transform.Find(triggerData[0]).GetComponent<Animator>().
                                SetTrigger(triggerData[1]);
                        break;

                        case "UPDATESCOREBOARD":
                            ScoreboardUI.Scoreboard.UpdateClient(splitData[1].Split('%'));
                        break;
                    }
                }

                break;
            case NetworkEventType.DisconnectEvent:

                Debug.Log("Player " + connectionId + " has disconnected");
                if(connectionId == myConnectionId)
                {
                    NetUI.CloseModal();
                }
                //OnDisconnection(connectionId);
                break;

            case NetworkEventType.BroadcastEvent:
                break;
        }
    }

    
    private void OnHostAction(string[] data)
    {
        switch (data[0])
        {
            case "PICKINGSDONE":

                GameObject.Find("Canvas").transform.Find("CountrySelection").gameObject.SetActive(false);
                P1Country = CountryModel.GetCountryByShortName(data[1]);
                P2Country = CountryModel.GetCountryByShortName(data[2]);

                RuntimeAnimatorController player1Controller = P1Country.ControllerTop2;
                RuntimeAnimatorController player2Controller = P2Country.ControllerBottom2;

                //  NetworkObjects.transform.Find("player1").GetComponent<Animator>().runtimeAnimatorController = player1Controller;
                // NetworkObjects.transform.Find("player2").GetComponent<Animator>().runtimeAnimatorController = player2Controller;

                int count = NetworkObjects.transform.childCount;
                for(int i = 0; i < count; i++)
                {
                    Transform t = NetworkObjects.transform.GetChild(i);

                    Animator animator = t.GetComponent<Animator>();

                    if (t.name.Contains("player1"))
                    {
                        animator.runtimeAnimatorController = player1Controller;
                    }else if (t.name.Contains("player2"))
                    {

                        animator.runtimeAnimatorController = player2Controller;
                    }
                }


                ScoreboardUI.Scoreboard.SetPlayer1Name("Player 1" + "(" + data[1] + ")");
                ScoreboardUI.Scoreboard.SetPlayer2Name("Player 2" + "(" + data[2] + ")");

                NetworkObjects.SetActive(true);
                NetUI.ShowModal("Loading...");

                Invoke("ReadyToPlay", 3f);
                break;
            case "SHOWMODAL":
                NetUI.ShowModal(data[1]);
                break;
            case "CLOSEMODAL":
                NetUI.CloseModal();
                break;
            case "YOUWON":
                NetUI.OpenModalWin();
                break;
            case "YOULOSE":
                NetUI.OpenModalLose();
                break;
            case "DELAY":
                DelayReducer.REQUESTED_DELAY = float.Parse(data[1]);

                if (int.Parse(data[2]) == 2)
                {
                    Destroy(NetworkObjects.transform.Find("player1ai1").gameObject);
                    Destroy(NetworkObjects.transform.Find("player2ai1").gameObject);
                }

                Debug.Log("requested delay is set to " + data[1] + "ms");
                break;
        }
    }

    private void ReadyToPlay()
    {
        NetUI.CloseModal();
        Send(GameConstants.ClientAction+"READYTOPLAY",true);
    }

    private void OnStateUpdate(string[] data)
    {
        int count = data.Length;
        for(int i = 1; i < count; i++)
        {
            string[] objdata = data[i].Split('%');

            if (objdata[0].Equals("p2spc"))
            {
                float spc = float.Parse(objdata[1]);
                P2SpecialThreshold.sizeDelta = new Vector2(P2SpecialThreshold.rect.width, spc);
                if (spc == 100f)
                {
                    P2SpecialButton.SetActive(true);
                }
                else
                {
                    P2SpecialButton.SetActive(false);
                }
                continue;
            }else if (objdata[0].Equals("bf"))
            {
                BrightFire.SetActive((objdata[1].Equals("1")));
                continue;
            }



            Transform toupdate = NetworkObjects.transform.Find(objdata[0]);




            Vector3 newpos = new Vector3(float.Parse(objdata[1]) * -1, float.Parse(objdata[2]) * -1);
            toupdate.position = newpos;

            Animator nimator = toupdate.GetComponent<Animator>();
            if(nimator != null)
            {
                nimator.SetBool("IsMoving", (objdata[3]).Equals("1"));
            }

            if (toupdate.name.Equals("ball"))
            {
                
                float scale = float.Parse(objdata[4]);
                toupdate.localScale = new Vector3(scale, scale);
            }else if (toupdate.name.Equals("destination"))
            {
                toupdate.GetComponent<SpriteRenderer>().sortingOrder = int.Parse(objdata[5]);
            }
        }

    }

    public void Send(string message, bool isReliable = false)
    {


        int channel = (isReliable) ? reliableChannel : stateUpdateChannel;
        byte[] msg = Encoding.Unicode.GetBytes(message);

        NetworkTransport.Send(hostId, myConnectionId, channel,msg, message.Length * sizeof(char), out error);
    }

    public void SendPlayer2Selection(string sel)
    {
        string tosend = "P2SELECTION|" + sel;

        Send(tosend, true);
    }

    public void SendMoveRequest(string name, float x, float y)
    {

        string msg = "MOVEREQUEST|" + name + "|" +
            x * -1 + "%" +
            y * -1;
        Send(msg);
        // Debug.Log(msg);
    }

    //private void OnPositionUpdate(string[] data)
    //{
    //    Transform objTrans = NetworkObjects.transform.Find(data[1]);

    //    string[] posVals = data[2].Split('%');
        
    //    objTrans.position = new Vector3(float.Parse(posVals[0]),float.Parse(posVals[1]),float.Parse(posVals[2]));

    //    objTrans.GetComponent<Animator>()?.SetBool("IsMoving", bool.Parse(data[3]));
    //}
}
