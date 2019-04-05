using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Traci = CodingConnected.TraCI.NET;

public class TraciController : MonoBehaviour
{

    public GameObject Cars_GO;
    public float speed = 2.0f;
    public Traci.TraCIClient Client;
    public String HostName;
    public int Port;
    public String ConfigFile;
    private float Elapsedtime;
    private Edge edge;

    /// <summary>
    /// Called when the scene is first rendered
    /// </summary>
    void Start()
    {
        Cars_GO = GameObject.Find("Cars");
    }
    
    /// <summary>
    /// This connects to sumo asynchronously
    /// </summary>
    /// <returns>A task to await</returns>
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
            UnityEngine.Debug.LogWarning(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Removes the construction zone attribute for a defined lane in the given road, and updates the simulation in SUMO.
    /// </summary>
    /// <param name="roadObject">The gameobject to whom we will update the specified lane</param>
    /// <param name="laneId">The lane Id as specified in the SUMO network file</param>
    public void RemoveWorkZoneOnLane(GameObject roadObject, String laneId)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadObject.name);
        int laneIndex = road.Lanes.FindIndex(l => l.Id == laneId);
        Lane lane = road.Lanes[laneIndex];

        if (lane.ConstructionZone)
        {
            road.Lanes[laneIndex] = new Lane()
            {
                Id = lane.Id,
                Index = lane.Index,
                Speed = lane.DefaultSpeed,
                Length = lane.Length,
                Width = lane.Width,
                Allow = lane.Allow,
                Disallow = lane.Disallow,
                Shape = lane.Shape,
                Built = lane.Built,
                DefaultSpeed = lane.DefaultSpeed,
                ConstructionZone = false
            };

            Client.Edge.SetMaxSpeed(road.Id, (double)Int32.Parse(lane.DefaultSpeed));
        }
        else
        {
            UnityEngine.Debug.LogWarning("Lane: " + laneId + " Is not a construction zone");
        }
    }

    /// <summary>
    /// Removes the construction zone attribute from every lane in the road, and updates the simulation accordingly in SUMO.
    /// </summary>
    /// <param name="roadObject">The Road GameObject with an Edge component of roads to update </param>
    public void RemoveWorkZoneEntireRoad(GameObject roadObject)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadObject.name);

        for (int i = 0; i < road.Lanes.Count; i++)
        {
            Lane lane = road.Lanes[i];

            if (lane.ConstructionZone)
            {
                road.Lanes[i] = new Lane()
                {
                    Id = lane.Id,
                    Index = lane.Index,
                    Speed = lane.DefaultSpeed,
                    Length = lane.Length,
                    Width = lane.Width,
                    Allow = lane.Allow,
                    Disallow = lane.Disallow,
                    Shape = lane.Shape,
                    Built = lane.Built,
                    DefaultSpeed = lane.DefaultSpeed,
                    ConstructionZone = false
                };

                Client.Edge.SetMaxSpeed(road.Id, (double)Int32.Parse(lane.DefaultSpeed));
            }
        }
    }

    /// <summary>
    /// Sets the construction zone attribute for every lane in the road, and updates the simulation accordingly in SUMO.
    /// </summary>
    /// <param name="roadObject">The Road GameObject to update the road</param>
    public void SetWorkZoneEntireRoad(GameObject roadObject)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadObject.name);

        for (int i = 0; i < road.Lanes.Count; i++)
        {
            Lane lane = road.Lanes[i];

            if (!lane.ConstructionZone)
            {
                double newSpeed = double.Parse(road.Lanes[i].Speed);
                //Gets the smallest, 0.75 * the speed, or 45 mph
                newSpeed = (newSpeed * 3f / 4f) > 45f ? (newSpeed * 3f / 4f) : 45f;

                road.Lanes[i] = new Lane()
                {
                    Id = lane.Id,
                    Index = lane.Index,
                    Speed = newSpeed.ToString(),
                    Length = lane.Length,
                    Width = lane.Width,
                    Allow = lane.Allow,
                    Disallow = lane.Disallow,
                    Shape = lane.Shape,
                    Built = lane.Built,
                    DefaultSpeed = lane.DefaultSpeed,
                    ConstructionZone = true
                };

                Client.Edge.SetMaxSpeed(road.Id, (double)newSpeed);
            }
        }
    }

    /// <summary>
    /// Sets the construction zone attribute for a defined lane in the given road, and updates the simulation in SUMO.
    /// </summary>
    /// <param name="roadObject">The gameobject to whom we will update the specified lane</param>
    /// <param name="laneId">The lane Id as specified in the SUMO network file</param>
    public void SetWorkZoneOneLane(GameObject roadObject, String laneId)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadObject.name);
        int laneIndex = road.Lanes.FindIndex(l => l.Id == laneId);
        Lane lane = road.Lanes[laneIndex];
        
        if (!lane.ConstructionZone)
        {
            int newSpeed = Int32.Parse(lane.Speed);
            //Gets the smallest, 0.75 * the speed, or 45 mph
            newSpeed = (newSpeed * 3 / 4) > 45 ? (newSpeed * 3 / 4) : 45;

            road.Lanes[laneIndex] = new Lane()
            {
                Id = lane.Id,
                Index = lane.Index,
                Speed = newSpeed.ToString(),
                Length = lane.Length,
                Width = lane.Width,
                Allow = lane.Allow,
                Disallow = lane.Disallow,
                Shape = lane.Shape,
                Built = lane.Built,
                DefaultSpeed = lane.DefaultSpeed,
                ConstructionZone = true
            };

            Client.Edge.SetMaxSpeed(road.Id, (double)newSpeed);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Lane: " + laneId + " Is already a construction zone");
        }
    }

    /// <summary>
    /// Subscribes to all vehicles in the simulation
    /// </summary>
    public void Subscribe()
    {
        List<byte> carInfo = new List<byte> { Traci.TraCIConstants.POSITION_3D };

        // Get all the car ids we need to keep track of. 
        Traci.TraCIResponse<List<String>> CarIds = Client.Vehicle.GetIdList();

        // Subscribe to all cars from 0 to 2147483647, and get their 3d position data
        CarIds.Content.ForEach(car => Client.Vehicle.Subscribe(car, 0, 2147483647, carInfo));
    }

    /// <summary>
    /// Event handler to handle a car update event
    /// </summary>
    /// <param name="sender">The client</param>
    /// <param name="e">The event args</param>
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

    /// <summary>
    /// Update is called once per frame
    /// If the client is defined, it will attempt to get a list of vehicles who are currently active and set their positions/create them accordingly. 
    /// </summary>
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
