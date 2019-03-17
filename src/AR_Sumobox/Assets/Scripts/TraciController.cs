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
    public Traci.TraCIClient Client = new Traci.TraCIClient();
    public String HostName;
    public int Port;
    public String ConfigFile;
    void Start()
    {
        Cars_GO = GameObject.Find("Cars");
        Client.VehicleSubscription += OnVehicleUpdate;
    }
    
    void ConnectToSumo()
    {
        List<byte> carInfo = new List<byte> { Traci.TraCIConstants.POSITION_3D };
        try
        {
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "bin/sumo.exe --remote-port " + Port.ToString() + " --configuration-file " + ConfigFile
            };
            p.StartInfo = si;
            p.Start();
            //Connect to sumo running on specified port
            Client.Connect(HostName, Port);

            //Get all the car ids we need to keep track of. 
            Traci.TraCIResponse<List<String>> CarIds = Client.Vehicle.GetIdList();

            // Subscribe to all cars from 0 to 2147483647, and get their 3d position data
            CarIds.Content.ForEach(car => Client.Vehicle.Subscribe(car, 0, 2147483647, carInfo));
            
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    public void OnVehicleUpdate(object sender, Traci.Types.SubscriptionEventArgs e)
    {
        GameObject Car_GO = GameObject.Find(e.ObjecId);
        if (Car_GO != null)
        {
            Cars_GO.transform.position = (Vector3)e.Responses.ToArray()[0];
        }
        else
        {
            SphereCollider car = Cars_GO.AddComponent(typeof(SphereCollider)) as SphereCollider;
            car.tag = e.ObjecId;
            Traci.Types.Position3D pos = (Traci.Types.Position3D)e.Responses.ToArray()[0];
            car.transform.position = new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
