using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

// Remove Serializable before final build. For now 
// though its needed so we can see the structs in the 
// Inspector window.
[Serializable]
public struct Lane
{
    public string Id { get; set; }
    public string Index { get; set; }
    public string Speed { get; set; }
    public string Length { get; set; }
    public string Width { get; set; }
    public string Allow { get; set; }
    public string Disallow { get; set; }
    public string Shape { get; set; }
}

[Serializable]
public struct Road
{
    public string Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Name { get; set; }
    public string Shape { get; set; }
    public string Type { get; set; }
    public List<Lane> Lanes { get; set; }
}


// Edge class stores Road and Lane information and builds
// roads (Edges) for SUMO networks.
public class Edge : MonoBehaviour
{
    // Handle to Edge Parent GameObject and script.
    private GameObject Edges_GO;
    // Two lists for Roads with positive or negative ids.
    // An edge with a negative id always has a positive counterpart
    // where PosEdge.id = NegEdge.id * (-1)
    // As a note ids are saved as string type.
    public List<Road> RoadList_Neg;
    public List<Road> RoadList_Pos;

    // Start is called before the first frame update
    void Start()
    {
        Edges_GO = GameObject.Find("Edges");
        RoadList_Neg = new List<Road>();
        RoadList_Pos = new List<Road>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClearData()
    {
        RoadList_Neg.Clear();
        RoadList_Pos.Clear();
    }

    // Sumo shape sting to List of floats point order is
    // x1, y1, x2, y2, ....
    private List<float> ShapeStringToFloatList(string shape)
    {
        List<float> points = new List<float>();
        char[] find = new char[2];
        find[0] = ',';
        find[1] = ' ';
        string[] cuts = shape.Split(find);
        List<string> cutList = cuts.ToList();
        foreach (string cut in cutList)
        {
            points.Add(float.Parse(cut, CultureInfo.InvariantCulture.NumberFormat));
        }
        return points;
    }

    private void BuildRoadLine(Road r1, Road r2)
    {
        int lanecount = r1.Lanes.Count + r2.Lanes.Count;
        List<float> polyline = null;
        if (r1.Shape == null && r2.Shape != null)
        {
            polyline = ShapeStringToFloatList(r2.Shape);
        }
        else if (r1.Shape != null && r2.Shape == null)
        {
            polyline = ShapeStringToFloatList(r1.Shape);
        }
        else
        {
            if (r1.Lanes[0].Shape != null)
            {
                polyline = ShapeStringToFloatList(r1.Lanes[0].Shape);
            }
            else if (r2.Lanes[0].Shape != null)
            {
                polyline = ShapeStringToFloatList(r2.Lanes[0].Shape);
            }
            else
            {
                polyline = null;
            }

        }

        if (polyline == null)
        {
            return;
        }

        Vector3[] vertexPositions = new Vector3[polyline.Count/2];
        int counter = 0;
        for (int i = 0; i < polyline.Count; i+=2)
        {
            vertexPositions[counter] = new Vector3(polyline[i], 0.2f, polyline[i+1]);
            counter++;
        }

        GameObject newroad = new GameObject();
        if(r1.Name != null)
        {
            newroad.name = r1.Name;
        }
        else
        {
            newroad.name = r1.Id;
        }
        LineRenderer LR = newroad.AddComponent<LineRenderer>();
        LR.startWidth = lanecount * 3.7f;
        LR.endWidth = lanecount * 3.7f;
        LR.numCornerVertices = 3;
        LR.numCapVertices = 6;
        LR.textureMode = LineTextureMode.Tile;
        LR.sharedMaterial = Resources.Load("Materials/Asphault_Material") as Material;
        LR.useWorldSpace = true;
        LR.positionCount = vertexPositions.Count();
        LR.SetPositions(vertexPositions);

        newroad.transform.parent = Edges_GO.transform;
    }

    private void BuildRoadLine(Road r1)
    {
        int lanecount = r1.Lanes.Count;
        List<float> polyline;
        if (r1.Shape == null)
        {
            polyline = ShapeStringToFloatList(r1.Lanes[0].Shape);
        }
        else
        {
            polyline = ShapeStringToFloatList(r1.Shape);
        }
        
        Vector3[] vertexPositions = new Vector3[polyline.Count / 2];
        int counter = 0;
        for (int i = 0; i < polyline.Count; i += 2)
        {
            vertexPositions[counter] = new Vector3(polyline[i], 0.2f, polyline[i + 1]);
            counter++;
        }

        GameObject newroad = new GameObject();
        if (r1.Name != null)
        {
            newroad.name = r1.Name;
        }
        else
        {
            newroad.name = r1.Id;
        }
        LineRenderer LR = newroad.AddComponent<LineRenderer>();
        LR.startWidth = lanecount;
        LR.endWidth = lanecount;
        LR.numCornerVertices = 3;
        LR.numCapVertices = 6;
        LR.textureMode = LineTextureMode.Tile;
        LR.sharedMaterial = Resources.Load("Materials/Asphault_Material") as Material;
        LR.useWorldSpace = true;
        LR.positionCount = vertexPositions.Count();
        LR.SetPositions(vertexPositions);

        newroad.transform.parent = Edges_GO.transform;
    }

    // Builds Road Pieces
    public void BuildEdges()
    {
        List<Road> LongList;
        List<Road> ShortList;
        if (RoadList_Pos.Count < RoadList_Neg.Count)
        {
            LongList = RoadList_Neg;
            ShortList = RoadList_Pos;
        }
        else
        {
            LongList = RoadList_Pos;
            ShortList = RoadList_Neg;
        }

        foreach (Road road in LongList)
        {
            if (road.Type == "internal" || road.Name == null)
            {
                continue;
            }

            bool built = false;
            foreach (Road counterpart in ShortList)
            {
                if ("-" + counterpart.Id == road.Id)
                {
                    built = true;
                    BuildRoadLine(road, counterpart);
                }
                else if ("-" + road.Id == counterpart.Id)
                {
                    built = true;
                    BuildRoadLine(road, counterpart);
                }
                else
                {
                    continue;
                }
            }

            if (!built)
            {
                BuildRoadLine(road);
            }

        }
    }
}
