using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Traci = CodingConnected.TraCI.NET;

/// <summary>
/// Traci Controller class manages a running simulation by communicating with a Sumo process. 
/// </summary>
public class TraciController : MonoBehaviour
{
    /// <summary>
    /// The Car main Game Object
    /// </summary>
    public GameObject Cars_GO;
    /// <summary>
    /// The simulation speed.
    /// </summary>
    public float speed = 2.0f;
    /// <summary>
    /// The Traci client.
    /// </summary>
    public Traci.TraCIClient Client;
    /// <summary>
    /// The hostname of the computer for remote connections.
    /// </summary>
    public String HostName;
    /// <summary>
    /// The post of the computer for remote connections.
    /// </summary>
    public int Port;
    /// <summary>
    /// The current simulation config file.
    /// </summary>
    public String ConfigFile;

    /// <summary>
    /// Flag to determine if the road color should be set
    /// </summary>
    public bool OccupancyVisual;
    private bool VisualsSwitched;

    private bool TrafficLightsLoaded;
    /// <summary>
    /// Flag to determine if car positions should be shown.
    /// </summary>
    public bool CarVisual;
    
    /// <summary>
    /// Simulation elapsed time.
    /// </summary>
    private float Elapsedtime;
    private Edge edge;

    /// <summary>
    /// The default traffic light program. (Used to reset stop lights to traffic lights)
    /// </summary>
    private string DefaultProgram;

    /// <summary>
    /// Called when the scene is first rendered
    /// </summary>
    void Start()
    {
        Cars_GO = GameObject.Find("Cars");
        OccupancyVisual = false;
        CarVisual = true;
        VisualsSwitched = false;
        TrafficLightsLoaded = false;
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
                Arguments = " --remote-port " + Port.ToString() + " --configuration-file " + ConfigFile
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
    /// Returns the speed that a lane should have when it is a construction zone
    /// </summary>
    /// <param name="normalSpeed">The normal speed of the lane</param>
    /// <returns>3/4ths of the normal speed of the lane, or 20 meters per second (45 mph), whichever is higher</returns>
    private double ToWorkZoneSpeed(double normalSpeed)
    {
        //Gets the smallest, 0.75 * the speed, or 45 mph
        return (normalSpeed * 3.0 / 4.0) > 20.0 ? (normalSpeed * 3.0 / 4.0) : 20.0;
    }

    /// <summary>
    /// Removes the construction zone attribute for a defined lane in the given road, and updates the simulation in SUMO.
    /// </summary>
    /// <param name="roadId">The ID of the road to which the lane belongs</param>
    /// <param name="laneId">The lane Id as specified in the SUMO network file</param>
    public void RemoveWorkZoneOnLane(string roadId, string laneId)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadId);
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

            Client.Edge.SetMaxSpeed(road.Id, double.Parse(lane.DefaultSpeed));
        }
        else
        {
            UnityEngine.Debug.LogWarning("Lane: " + laneId + " Is not a construction zone");
        }
    }

    /// <summary>
    /// Removes the construction zone attribute from every lane in the road, and updates the simulation accordingly in SUMO.
    /// </summary>
    /// <param name="roadId">The ID of the road to update </param>
    public void RemoveWorkZoneEntireRoad(string roadId)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadId);

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

                Client.Edge.SetMaxSpeed(road.Id, double.Parse(lane.DefaultSpeed));
            }
        }
    }

    /// <summary>
    /// Sets the construction zone attribute for every lane in the road, and updates the simulation accordingly in SUMO.
    /// </summary>
    /// <param name="roadId">The ID of the road to update</param>
    public void SetWorkZoneEntireRoad(string roadId)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadId);

        for (int i = 0; i < road.Lanes.Count; i++)
        {
            Lane lane = road.Lanes[i];

            if (!lane.ConstructionZone)
            {
                double newSpeed = ToWorkZoneSpeed(double.Parse(road.Lanes[i].Speed));

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
    /// <param name="roadId">The ID of the road to which the lane belongs</param>
    /// <param name="laneId">The lane Id as specified in the SUMO network file</param>
    public void SetWorkZoneOneLane(string roadId, string laneId)
    {
        if (edge == null)
            edge = FindObjectOfType<Edge>();

        Road road = edge.RoadList.Single(r => r.Id == roadId);
        int laneIndex = road.Lanes.FindIndex(l => l.Id == laneId);
        Lane lane = road.Lanes[laneIndex];
        
        if (!lane.ConstructionZone)
        {
            double newSpeed = ToWorkZoneSpeed(double.Parse(lane.Speed));

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
    /// Flips the Occupancy Visual to simulate a mesoscopic view.
    /// </summary>
    public void ToggleMesoscopic()
    {
        OccupancyVisual = !OccupancyVisual;
        CarVisual = !CarVisual;
        VisualsSwitched = true;
    }


    /// <summary>
    /// Sets a junction's stop light to the "off-blinking" phase to cause all vehicles to yeild
    /// </summary>
    /// <param name="trafficLightId">The id of the stop light/junction that will be converted</param>
    public void SetStopSignJunction(string trafficLightId)
    {
        Client.TrafficLight.SetProgram(trafficLightId, "o");
    }

    /// <summary>
    /// Sets a junction's stop light to the default phase to cause all vehicles to yeild
    /// </summary>
    /// <param name="trafficLightId">The id of the stop light/junction that will be converted</param>
    public void SetTrafficLightJunction(string trafficLightId)
    {
        Client.TrafficLight.SetProgram(trafficLightId, DefaultProgram);
    }

    /// <summary>
    /// Gets the current program for a given traffic light
    /// </summary>
    /// <param name="junctionId">The id of the stop light/junction that will be converted</param>
    private string GetProgram(string junctionId)
    {
        return Client.TrafficLight.GetCurrentProgram(junctionId).Content;
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
    }

    /// <summary>
    /// Update is called once per frame
    /// If the client is defined, it will attempt to get a list of vehicles who are currently active and set their positions/create them accordingly. 
    /// </summary>
    void Update()
    {
        if (Client != null)
        {
            if (!TrafficLightsLoaded)
            {
                GameObject.Find("TrafficLights").GetComponent<TrafficLight>().Get_Traffic_Lights();
                TrafficLightsLoaded = true;
            }
            if (OccupancyVisual)
            {
                if (VisualsSwitched)
                {
                    //GameObject.Find("Occupancy_Legend").SetActive(true);
                    foreach (Transform child in Cars_GO.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    VisualsSwitched = false;
                }

                Transform e = GameObject.Find("Edges").transform;
                if (e != null)
                {
                    foreach (Transform child in e)
                    {
                        float o = (float)Client.Lane.GetLastStepOccupancy(child.gameObject.name).Content;
                        if (o >= 0.9f)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_7", typeof(Material)) as Material;
                        }
                        else if (o >= 0.8f)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_6", typeof(Material)) as Material;
                        }
                        else if (o >= 0.5f)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_5", typeof(Material)) as Material;
                        }
                        else if (o >= 0.1f)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_4", typeof(Material)) as Material;
                        }
                        else if (o >= 0.01f)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_3", typeof(Material)) as Material;
                        }
                        else if (o >= 0.001f)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_2", typeof(Material)) as Material;
                        }
                        else
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_1", typeof(Material)) as Material;
                        }

                        //int o = Client.Lane.GetLastStepVehicleNumber(child.gameObject.name).Content;
                        //if (o >= 10)
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_7", typeof(Material)) as Material;
                        //}
                        //else if (o >= 5)
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_6", typeof(Material)) as Material;
                        //}
                        //else if (o >= 4)
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_5", typeof(Material)) as Material;
                        //}
                        //else if (o >= 3)
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_4", typeof(Material)) as Material;
                        //}
                        //else if (o >= 2)
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_3", typeof(Material)) as Material;
                        //}
                        //else if (o >= 1)
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_2", typeof(Material)) as Material;
                        //}
                        //else
                        //{
                        //    child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_1", typeof(Material)) as Material;
                        //}
                    }
                }

                    Transform j = GameObject.Find("Junctions").transform;
                    if (j != null)
                    {
                        foreach (Transform child in j)
                        { 
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Occupancy_Material_2", typeof(Material)) as Material;
                        }
                    }
            }
            if (CarVisual)
            {
                if (Client != null)
                {
                    if (VisualsSwitched)
                    {
                        //GameObject.Find("Occupancy_Legend").SetActive(false);
                        Transform e = GameObject.Find("Edges").transform;
                        foreach (Transform child in e)
                        {
                            child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Road_Material", typeof(Material)) as Material;
                        }
                        VisualsSwitched = false;
                        Transform j = GameObject.Find("Junctions").transform;
                        if (j != null)
                        {
                            foreach (Transform child in j)
                            {
                                child.gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/Road_Material", typeof(Material)) as Material;
                            }
                        }
                    }

                    Cars_GO = GameObject.Find("Cars");
                    // Get all the car ids we need to keep track of. 
                    Traci.TraCIResponse<List<String>> CarIds = Client.Vehicle.GetIdList();

                    CarIds.Content.ForEach(carId => {
                        Traci.Types.Position3D pos = Client.Vehicle.GetPosition3D(carId).Content;
                        float rot = (float)Client.Vehicle.GetAngle(carId).Content;
                        //Client.Vehicle.GetVehicleClass(carId).Content.Contains("bus");
                        Transform CarTransform = Cars_GO.transform.Find(carId);
                        if (CarTransform != null)
                        {
                            if (pos == null)
                            {
                                GameObject.Destroy(GameObject.Find(carId));
                            }
                            else
                            {
                                CarTransform.position = new Vector3((float)pos.X, 0.0f, (float)pos.Y);
                                //if (CarTransform.rotation.y != rot)
                                //{
                                //    CarTransform.Rotate(0.0f, rot, 0.0f);    
                                //}
                                CarTransform.localEulerAngles = new Vector3(0, rot - 90.0f, 0);
                            }
                            
                        }
                        else
                        {
                            GameObject car = GameObject.Instantiate(Resources.Load("Prefabs/Vehicle", typeof(GameObject)) as GameObject, new Vector3((float)pos.X, 0.0f, (float)pos.Y), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), Cars_GO.transform);
                           
                            if(carId.Contains("bus")){
                                 car.transform.Find("Bus").gameObject.SetActive(true);
                                car.transform.Find("Car").gameObject.SetActive(false);
                            }
                            else if (carId.Contains("moto"))
                            {
                                car.transform.Find("Motorcycle").gameObject.SetActive(true);
                                car.transform.Find("Car").gameObject.SetActive(false);

                            }
                            else if (carId.Contains("truck"))
                            {
                                car.transform.Find("BoxTruck").gameObject.SetActive(true);
                                car.transform.Find("Car").gameObject.SetActive(false);

                            }
                            else if (carId.Contains("bicycle"))
                            {
                                car.transform.Find("Bicycle").gameObject.SetActive(true);
                                car.transform.Find("Car").gameObject.SetActive(false);

                            }
                            else if (carId.Contains("ped"))
                            {
                                car.transform.Find("Pedestrian").gameObject.SetActive(true);
                                car.transform.Find("Car").gameObject.SetActive(false);
                            }
                            
                            car.name = carId;
                            car.transform.parent = Cars_GO.transform;
                            car.transform.position = new Vector3((float)pos.X, 0.0f, (float)pos.Y);
                        }
                    });
                }
                
            }

            Elapsedtime += Time.deltaTime;
            if(Elapsedtime > 1)
            {
                Client.Control.SimStep();
                Elapsedtime = 0;
            }
        }
    }
}
