using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLS.Widgets.Table; //Need this for calling table pro
using UnityEngine.UI; //Need this for calling UI scripts



public class CutAndFillManager : MonoBehaviour {

    // max points for the road
    const int MAXPOINTS = 20;

    // setup arrays for cut/fill and mass haul data
    float[] station       = new float[MAXPOINTS];
    float[] existGrade    = new float[MAXPOINTS];
    float[] propGrade     = new float[MAXPOINTS];
    float[] roadWidth     = new float[MAXPOINTS];
    float[] cutArea       = new float[MAXPOINTS];
    float[] fillArea      = new float[MAXPOINTS];
    float[] cutVolume     = new float[MAXPOINTS];
    float[] fillVolume    = new float[MAXPOINTS];
    float[] adjFillVolume = new float[MAXPOINTS];
    float[] algebraicSum  = new float[MAXPOINTS];
    float[] massOrdinate  = new float[MAXPOINTS];

    [SerializeField]
    Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

    public GameObject terrain;
    private TerrainGenerator terrainHeight;

    public GameObject road;
    private Road roadPoint;

    float timer = 0f;
    float waitingTime = 10f;

    private Table table;

    void Start()
    {   
        // get terrain object
        terrain = GameObject.Find("Terrain");
        // get height function from terrain generator
        terrainHeight = terrain.GetComponent<TerrainGenerator>();

        // get road object
        road = GameObject.Find("Road");
        // get point from road
        roadPoint = road.GetComponent<Road>();

        this.table = this.GetComponent<Table>();

        this.table.ResetTable();

        this.table.AddTextColumn("Station");
        this.table.AddTextColumn("Existing Gr");
        this.table.AddTextColumn("Proposed Gr");
        this.table.AddTextColumn("Roadway Width");
        this.table.AddTextColumn("Cut Area");
        this.table.AddTextColumn("Fill Area");
        this.table.AddTextColumn("Cut Volumes");
        this.table.AddTextColumn("Fill Volumes");
        this.table.AddTextColumn("Adj. Fill Volumes");
        this.table.AddTextColumn("Algebraic Sum");
        this.table.AddTextColumn("Mass Ordinate");

        // Initialize Your Table
        this.table.Initialize(this.onTableSelected);

        // setup values
        updateStation();
        updateExistGrade();
        updatePropGrade();
        updateRoadWidth();
        updateCutArea();
        updateFillArea();
        updateCutVolume();
        updateFillVolume();
        updateAdjFillVolume();
        updateAlgebraicSum();
        updateMassOrdinate();

        // Populate Your Rows
        for (int i = 0; i < MAXPOINTS; i++)
        {
            Datum d = Datum.Body(i.ToString());
            d.elements.Add(station[i].ToString());
            d.elements.Add(existGrade[i].ToString());
            d.elements.Add(propGrade[i].ToString());
            d.elements.Add(roadWidth[i].ToString());
            d.elements.Add(cutArea[i].ToString());
            d.elements.Add(fillArea[i].ToString());
            d.elements.Add(cutVolume[i].ToString());
            d.elements.Add(fillVolume[i].ToString());
            d.elements.Add(adjFillVolume[i].ToString());
            d.elements.Add(algebraicSum[i].ToString());
            d.elements.Add(massOrdinate[i].ToString());
            this.table.data.Add(d);
        }

        // Draw Your Table
        this.table.StartRenderEngine();

        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
    }

    // Handle the row selection however you wish
    private void onTableSelected(Datum datum)
    {
        print("You Clicked: " + datum.uid);
    }


    public void Pause()
    {
        UIPanel.gameObject.SetActive(true); //turn on the pause menu
    }

    public void UnPause()
    {
        UIPanel.gameObject.SetActive(false); //turn off pause menu
    }

    void updateStation()
    {
        station[0] = 0;
        
        for (int i = 1; i < MAXPOINTS; i++)
        {
            station[i] = station[i - 1] + 100;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in station)
        {
            Debug.Log("STATION #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateExistGrade()
    {
        Vector3[] positions = roadPoint.GetRoadPoints();

        int i = 0;
        foreach (Vector3 p in positions)
        {
            existGrade[i] = 10f * terrainHeight.GetHeightAtWorldPosition(p);
            i += 1;
            if (i >= MAXPOINTS)
                break;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in existGrade)
        {
            Debug.Log("EXISTING GRADE #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updatePropGrade()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            propGrade[i] = 0;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in propGrade)
        {
            Debug.Log("PROPOSED GRADE #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateRoadWidth()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            roadWidth[i] = 120;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in roadWidth)
        {
            Debug.Log("ROAD WIDTH #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateCutArea()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            if (existGrade[i] > 0)
            {
                cutArea[i] = (existGrade[i] * existGrade[i]) + (roadWidth[i] * existGrade[i]);
            }
            else
            {
                cutArea[i] = 0;
            }
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in cutArea)
        {
            Debug.Log("CUT AREA #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateFillArea()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            if (existGrade[i] <= 0)
            {
                fillArea[i] = -1 * ((existGrade[i] * existGrade[i]) + Mathf.Abs(roadWidth[i] * existGrade[i]));
            }
            else
            {
                fillArea[i] = 0;
            }
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in fillArea)
        {
            Debug.Log("FILL AREA #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateCutVolume()
    {
        cutVolume[0] = 0;

        for (int i = 1; i < MAXPOINTS; i++)
        {
            cutVolume[i] = (cutArea[i] + cutArea[i - 1]) / 2 * (station[i] - station[i - 1]) / 27;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in cutVolume)
        {
            Debug.Log("CUT VOLUME #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateFillVolume()
    {
        fillVolume[0] = 0;

        for (int i = 1; i < MAXPOINTS; i++)
        {
            fillVolume[i] = (fillArea[i] + fillArea[i - 1]) / 2 * (station[i] - station[i - 1]) / 27;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in fillVolume)
        {
            Debug.Log("FILL VOLUME #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateAdjFillVolume()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            adjFillVolume[i] = fillVolume[i] / 0.9f;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in adjFillVolume)
        {
            Debug.Log("ADJUSTED FILL VOLUME #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateAlgebraicSum()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            algebraicSum[i] = cutVolume[i] + adjFillVolume[i];
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in algebraicSum)
        {
            Debug.Log("ALGEBRAIC SUM #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateMassOrdinate()
    {
        massOrdinate[0] = algebraicSum[0];

        for (int i = 1; i < MAXPOINTS; i++)
        {
            massOrdinate[i] = massOrdinate[i - 1] + algebraicSum[i];
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in massOrdinate)
        {
            Debug.Log("MASS ORDINATE #" + count + " value " + s);
            count += 1;
        }
        */
    }
}
