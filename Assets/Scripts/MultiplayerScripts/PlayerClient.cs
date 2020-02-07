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
    private int ourClientId;

    private byte error;
    private bool isStarted = false;
    private float connectionTime;

    public Text TextIPInput;
    string ipInput = "";

    public GameObject ConnectionPanel;
    public GameObject NetworkObjects;

    public void Start()
    {
        ConnectionPanel.SetActive(true);

        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();

        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);

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
                        case "POSITIONUPDATE":
                            OnPositionUpdate(splitData);
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

    public void Send(string message, bool isReliable = false)
    {


        int channel = (isReliable) ? reliableChannel : unreliableChannel;
        byte[] msg = Encoding.Unicode.GetBytes(message);

        NetworkTransport.Send(hostId, myConnectionId, channel,msg, message.Length * sizeof(char), out error);
    }

    public void SendMoveRequest(string name, float x, float y)          
    {     

        string msg = "MOVEREQUEST|" + name + "|" +
            x * -1 + "%" +
            y * -1;
        Send(msg);
       // Debug.Log(msg);
    }

    private void OnPositionUpdate(string[] data)
    {
        Transform objTrans = NetworkObjects.transform.Find(data[1]);

        string[] posVals = data[2].Split('%');
        
        objTrans.position = new Vector3(float.Parse(posVals[0]),float.Parse(posVals[1]),float.Parse(posVals[2]));

        objTrans.GetComponent<Animator>()?.SetBool("IsMoving", bool.Parse(data[3]));
    }
}
