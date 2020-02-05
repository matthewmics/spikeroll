using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHost : MonoBehaviour
{

    private const int MAX_CONNECTION = 100;
    private int port = 5701;

    private int hostId;
    private int webHostId;
    private int reliableChannel;
    private int unreliableChannel;

    private int clientId;

    private bool isStarted = false;

    private byte error;

    private float lastUpdateMovement;
    private float updateMovementRate = 0.05f;

    public Text messageText;
    public GameObject ConnectionPanel;
    public GameObject NetworkObjects;


    // Start is called before the first frame update
    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        

        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);

        hostId = NetworkTransport.AddHost(topo, port, null);

        isStarted = true;

    }

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
                //Debug.Log("try");   
                break;
            case NetworkEventType.ConnectEvent:
                clientId = connectionId;
                Debug.Log("Player " + connectionId + " has connected");
                ConnectionPanel.SetActive(false);
                //OnConnect(connectionId);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                //Debug.Log("Receiving from : " + connectionId + " : " + msg);
                string[] splitData = msg.Split('|');

                switch (splitData[0])
                {
                    case "MOVEREQUEST":
                        OnMoveRequest(splitData);
                    break;
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

    private void OnMoveRequest(string[] data)
    {
        NetworkClientController ncc = NetworkObjects.transform.Find(data[1]).GetComponent<NetworkClientController>();
        string[] movements = data[2].Split('%');
        ncc.movement.x = float.Parse(movements[0]);
        ncc.movement.y = float.Parse(movements[1]);
    }

    public void Send(string message,bool isReliable = false)
    {
        int channel = (isReliable) ? reliableChannel : unreliableChannel;
        byte[] msg = Encoding.Unicode.GetBytes(message);

        NetworkTransport.Send(hostId, clientId, channel, msg,message.Length * sizeof(char), out error);
    }

    public void SendPosition(string name, Transform trans)
    {
        if (!isStarted) return;

        string tosend = "POSITIONUPDATE|" + name + "|" + 
            trans.position.x * -1 + "%" + 
            trans.position.y * -1 + "%" + 
            trans.position.z;
        Send(tosend);

        //Debug.Log(tosend);
    }
}
