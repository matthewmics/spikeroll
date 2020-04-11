using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHost : MonoBehaviour
{


    private const int MAX_CONNECTION = 1;
    private int port = 5701;

    private int hostId;
    private int webHostId;
    private int reliableChannel;
    private int unreliableChannel;
    private int stateUpdateChannel;

    private int clientId = -1;

    private bool isStarted = false;
    private bool isConnected = false;

    private byte error;


    public Text messageText;
    public GameObject ConnectionPanel;
    public GameObject NetworkObjects;
    public GameObject CountrySelectionPanel;
    public NetworkClientController NCC;

    public MultiplayerUIScript MultiplayerUIScript;

    public static MultiplayerUIScript NetUI;

    [HideInInspector]
    public MultiplayerGameSession GameSession;

    public static MultiplayerPrefs MultiplayerSession { get; private set; }
    public static MultiplayerGeneralControl GeneralControl;
    public static PlayerHost Server;

    // Start is called before the first frame update
    void Start()
    {
        NetUI = MultiplayerUIScript;
        Server = this;
        GameSession = new MultiplayerGameSession();
        GeneralControl = NetworkObjects.GetComponent<MultiplayerGeneralControl>();
        MultiplayerSession = GetComponent<MultiplayerPrefs>();

        NetworkObjects.SetActive(false);
        ConnectionPanel.SetActive(true);

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.ReliableSequenced);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        stateUpdateChannel = cc.AddChannel(QosType.StateUpdate);
        
        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, port, null); 

        messageText.text = "WAITING FOR OTHER PLAYER. YOUR IP IS " + SepakNetworkUtil.GetLocalIPAddress();


        isStarted = true;

    }

    public void ReturnToMenu()
    {
        Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    private void Disconnect()
    {
        NetworkTransport.RemoveHost(hostId);
    }

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
                //Debug.Log("try");   
                break;  
            case NetworkEventType.ConnectEvent:

                if(clientId == -1)
                {
                    isConnected = true;
                    clientId = connectionId;
                    Debug.Log("Player " + connectionId + " has connected");
                    CountrySelectionPanel.SetActive(true);
                    ConnectionPanel.SetActive(false);

                    string tosend = $"{GameConstants.HostAction}DELAY%{DelayReducer.DELAY_VALUE}";
                    Send(tosend, true);
                }
                //OnConnect(connectionId);
                break;
            case NetworkEventType.DataEvent:

                if (isConnected)
                {
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    //Debug.Log("Receiving from : " + connectionId + " : " + msg);
                    string[] splitData = msg.Split('|');

                    switch (splitData[0])
                    {
                        case "MOVEREQUEST":
                            OnMoveRequest(splitData);
                            break;
                        case "P2SELECTION":
                            var hcs = GameObject.FindObjectOfType<HostCountrySelect>();
                            hcs.SelectPlayer2(splitData[1]);
                        break;

                        case "CLIENTACTION":
                            OnClientAction(splitData[1].Split('%'));
                        break;
                    }
                }


                break;
            case NetworkEventType.DisconnectEvent:

                Debug.Log("Player " + connectionId + " has disconnected");
                //OnDisconnection(connectionId);
                break;

            case NetworkEventType.BroadcastEvent:

                break;
        }

    }


    private void OnClientAction(string[] data)
    {
        switch (data[0])
        {
            case "SELECTCOUNTRY":
                MultiplayerSession.P2Country = data[1];
            break;
            case "KICK":
                NCC.DoKick();
            break;
            case "PASS":
                NCC.DoPass();
            break;
            case "SPECIAL":
                NCC.DoSpecial();
            break;
            case "READYTOPLAY":
                NetworkObjects.SetActive(true);
                MultiplayerUIScript.CloseModal();
                Invoke("StartRound",1f);
            break;
            case "DELAY":
                DelayReducer.REQUESTED_DELAY = float.Parse(data[1]);
                Debug.Log("requested delay is set to " + data[1] + "ms");
            break;
        }
    }

    private void StartRound()
    {

        GeneralControl.StartRound();
    }

    private void OnMoveRequest(string[] data)
    {
      //  Debug.Log(data[2]);
      //  Debug.Log(data[1]);
       // NetworkClientController ncc = NetworkObjects.transform.Find(data[1]).GetComponent<NetworkClientController>();
        string[] movements = data[2].Split('%');
        NCC.movement.x = float.Parse(movements[0]);
        NCC.movement.y = float.Parse(movements[1]);

        //ncc.GetComponent<Animator>()?.SetBool("IsMoving", bool.Parse(data[3]));
    }

    public void Send(string message,bool isReliable = false)
    {
        int channel = (isReliable) ? reliableChannel : stateUpdateChannel;
        byte[] msg = Encoding.Unicode.GetBytes(message);

        NetworkTransport.Send(hostId, clientId, channel, msg,message.Length * sizeof(char), out error);
    }

    public void SendPlayer1Selection(string sel)
    {
        string tosend = "P1SELECTION|"+sel;
        Send(tosend,true);
    }

    //public void SendPosition(string name, Transform trans, bool isMoving)
    //{

    //    string tosend = "POSITIONUPDATE|" + name 
    //        + "|" + // TRANSFORM
    //        trans.position.x * -1 + "%" + 
    //        trans.position.y * -1 + "%" + 
    //        trans.position.z 
    //        + "|" +// IS MOVING ANIMATION
    //        isMoving
    //        ;
    //    Send(tosend);


    //    //Debug.Log(tosend);
    //}
}
