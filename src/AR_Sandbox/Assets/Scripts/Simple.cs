using UnityEngine;
// Note this, it's important
using SLS.Widgets.Table;

public class Simple : MonoBehaviour
{

    private Table table;

    void Start()
    {

        this.table = this.GetComponent<Table>();

        this.table.reset();

        this.table.addTextColumn();
        this.table.addTextColumn();
        this.table.addTextColumn();

        // Initialize Your Table
        this.table.initialize(this.onTableSelected);

        // Populate Your Rows (obviously this would be real data here)
        for (int i = 0; i < 100; i++)
        {
            Datum d = Datum.Body(i.ToString());
            d.elements.Add("Col1:Row" + i.ToString());
            d.elements.Add("Col2:Row" + i.ToString());
            d.elements.Add("Col3:Row" + i.ToString());
            this.table.data.Add(d);
        }

        // Draw Your Table
        this.table.startRenderEngine();

    }

    // Handle the row selection however you wish
    private void onTableSelected(Datum datum)
    {
        print("You Clicked: " + datum.uid);
    }

}