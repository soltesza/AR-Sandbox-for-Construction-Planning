using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Traci = CodingConnected.TraCI.NET;

public class TraciController : MonoBehaviour
{

    public GameObject Cars_GO;
    public float speed = 2.0f;
    public Traci.TraCIClient Client;
    private Traci.TraCISubscriptionResponse Subscriptions = new Traci.TraCISubscriptionResponse();

    void Start()
    {
        Cars_GO = GameObject.Find("Cars");
    }
    
    void ConnectToSumo(string Hostname, int Port)
    {
        List<byte> carInfo = new List<byte> { Traci.TraCIConstants.POSITION_3D };
        try
        {
            //Connect to sumo running on specified port
            Client = new Traci.TraCIClient();
            Client.Connect(Hostname, Port);

            //Get all the car ids we need to keep track of. 
            Traci.TraCIResponse<List<String>> CarIds = Client.Vehicle.GetIdList();
            
            // Subscribe to all cars
            CarIds.Content.ForEach(car => Client.Vehicle.Subscribe(car, 0, 2147483647, carInfo));
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    Traci.Types.SubscriptionEventArgs HandleVehicleUpdate()
    {
        //Traci.Types.SubscriptionEventArgs Event = 
        return new Traci.Types.SubscriptionEventArgs();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
