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


    private const int MAX_CONNECTION = 100;
    private int port = 5701;

    private int hostId;
    private int webHostId;

    private int myConnectionId;

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
    public GameObject NetworkObjects;


    public CountryModel P1Country = null;
    public CountryModel P2Country = null;

    public void Start()
    {


        //QualitySettings.vSyncCount = 0;  // VSync must be disabled
        //Application.targetFrameRate = 45;

        ConnectionPanel.SetActive(true);

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
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
                isConnected = true;
                this.myConnectionId = connectionId;
                ConnectionPanel.SetActive(false);
                Debug.Log("Player " + connectionId + " has connected");
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


                        case "UPDATESTATE":
                            //Debug.Log(msg);
                            OnStateUpdate(splitData);
                            break;

                        case "HOSTACTION":
                            OnHostAction(splitData[1].Split('%'));
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

    
    private void OnHostAction(string[] data)
    {
        switch (data[0])
        {
            case "PICKINGSDONE":

                GameObject.Find("Canvas").transform.Find("CountrySelection").gameObject.SetActive(false);
                P1Country = CountryModel.GetCountryByShortName(data[1]);
                P2Country = CountryModel.GetCountryByShortName(data[2]);
                NetworkObjects.transform.Find("player1").GetComponent<Animator>().runtimeAnimatorController =
                    P1Country.ControllerTop2;
                NetworkObjects.transform.Find("player2").GetComponent<Animator>().runtimeAnimatorController =
                    P2Country.ControllerBottom2;

                NetworkObjects.SetActive(true);

                break;
        }
    }

    private void OnStateUpdate(string[] data)
    {
        int count = data.Length;
        for(int i = 1; i < count; i++)
        {
            string[] objdata = data[i].Split('%');

            Transform toupdate = NetworkObjects.transform.Find(objdata[0]);
            Vector3 newpos = new Vector3(float.Parse(objdata[1]) * -1, float.Parse(objdata[2]) * -1);
            toupdate.position = newpos;

            Animator nimator = toupdate.GetComponent<Animator>();
            if(nimator != null)
            {
                nimator.SetBool("IsMoving", (objdata[3]).Equals("1"));
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
