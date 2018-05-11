using UnityEngine;
// Note this, it's important
using SLS.Widgets.Table;

public class Simple : MonoBehaviour
{

    private Table table;

    void Start()
    {

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

        // Populate Your Rows (obviously this would be real data here)
        for (int i = 0; i < 20; i++)
        {
            Datum d = Datum.Body(i.ToString());
            d.elements.Add("Col1:Row" + i.ToString());
            d.elements.Add("Col2:Row" + i.ToString());
            d.elements.Add("Col3:Row" + i.ToString());
            d.elements.Add("Col4:Row" + i.ToString());
            d.elements.Add("Col5:Row" + i.ToString());
            d.elements.Add("Col6:Row" + i.ToString());
            d.elements.Add("Col7:Row" + i.ToString());
            d.elements.Add("Col8:Row" + i.ToString());
            d.elements.Add("Col9:Row" + i.ToString());
            d.elements.Add("Col10:Row" + i.ToString());
            d.elements.Add("Col11:Row" + i.ToString());
            this.table.data.Add(d);
        }

        // Draw Your Table
        this.table.StartRenderEngine();

    }

    // Handle the row selection however you wish
    private void onTableSelected(Datum datum)
    {
        print("You Clicked: " + datum.uid);
    }

}