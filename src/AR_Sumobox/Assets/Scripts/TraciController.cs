using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Traci = CodingConnected.TraCI.NET;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

public class TraciController : MonoBehaviour
{

    public GameObject Cars_GO;
    public float speed = 2.0f;
    public Traci.TraCIClient Client;
    public String HostName;
    public int Port;
    public String ConfigFile;
    private float Elapsedtime;
    void Start()
    {
        Cars_GO = GameObject.Find("Cars");
    }
    
    async Task <Traci.TraCIClient> ConnectToSumo()
    {
        try
        {
            Client = new Traci.TraCIClient();
            Client.VehicleSubscription += OnVehicleUpdate;
            string tmp = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\Sumo\\bin\\"));
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo()
            {
                WorkingDirectory = "C:\\Sumo\\bin\\",
                FileName = "sumo.exe",
                Arguments = " --remote-port " + Port.ToString() + " --configuration-file " + ConfigFile,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            p.StartInfo = si;
            p.Start();
            Thread.Sleep(400);
            //Connect to sumo running on specified port
            await Client.ConnectAsync(HostName, Port);
            Subscribe();
            Client.Control.SimStep();
            return Client;
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            return null;
        }
    }

    public void Subscribe()
    {
        List<byte> carInfo = new List<byte> { Traci.TraCIConstants.POSITION_3D };

        // Get all the car ids we need to keep track of. 
        Traci.TraCIResponse<List<String>> CarIds = Client.Vehicle.GetIdList();

        // Subscribe to all cars from 0 to 2147483647, and get their 3d position data
        CarIds.Content.ForEach(car => Client.Vehicle.Subscribe(car, 0, 2147483647, carInfo));
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
            car.transform.position = new Vector3((float)pos.X, 0, (float)pos.Y);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // Get all the car ids we need to keep track of. 
        if (Client != null)
        {
            Traci.TraCIResponse<List<String>> CarIds = Client.Vehicle.GetIdList();

            CarIds.Content.ForEach(carId => {
                Traci.Types.Position3D pos = Client.Vehicle.GetPosition3D(carId).Content;
                Transform CarTransform = Cars_GO.transform.Find(carId);
                if (CarTransform != null)
                {
                    CarTransform.position = new Vector3((float)pos.X, 0, (float)pos.Y);
                }
                else
                {
                    GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    car.name = carId;
                    car.transform.parent = Cars_GO.transform;
                    car.transform.position = new Vector3((float)pos.X, 1, (float)pos.Y);
                }
            });
            Elapsedtime += Time.deltaTime;
            if(Elapsedtime > 1)
            {
                Client.Control.SimStep();
                Elapsedtime = 0;
            }
        }
        
    }
}
