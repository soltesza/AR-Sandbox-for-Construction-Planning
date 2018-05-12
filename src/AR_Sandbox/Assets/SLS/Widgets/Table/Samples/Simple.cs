using UnityEngine;

// Include this when using outside our namespace:
// using SLS.Widgets.Table;

namespace SLS.Widgets.Table {
  public class Simple : MonoBehaviour {

    private Table table;
    private const int ROWS = 100;

    void Start() {

      // this instantiates default fonts for ALL tables when not explicitely defined on an individual table
      //  ProTip: You don't need to do this if you just set a font for your table in the editor
      MakeDefaults.Set();

      this.table = this.GetComponent<Table>();

      this.table.ResetTable();

      this.table.AddTextColumn("Column1");
      this.table.AddTextColumn("Column2");
      this.table.AddTextColumn("Column3");

      // Initialize Your Table
      this.table.Initialize(this.OnTableSelected);

      // Populate Your Rows (obviously this would be real data here)
      for(int i = 0; i < Simple.ROWS; i++) {
        Datum d = Datum.Body(i.ToString());
        d.elements.Add("Col1:Row" + i.ToString());
        d.elements[0].color = new Color(0, .5f, 0);
        d.elements.Add("Col2:Row" + i.ToString());
        d.elements.Add("Col3:Row" + i.ToString());
        this.table.data.Add(d);
      }

      // Draw Your Table
      this.table.StartRenderEngine();

    }

    // Handle the row selection however you wish (be careful as column can be null here
    //  if your table doesn't fill the full horizontal rect space and the user clicks an edge)
    private void OnTableSelected(Datum datum, Column column) {
      string cidx = "N/A";
      if(column != null)
        cidx = column.idx.ToString();
      if(datum != null)
        print("You Clicked: " + datum.uid + " Column: " + cidx);
      else
        print("Selection Cleared!");
    }

    public void HandleSelectRandomClick() {
      int idx = Random.Range(0, this.table.data.Count - 1);
      this.table.SetSelected(this.table.data[idx], null, true, true);
    }

    public void HandleClearSelectionClick() {
      this.table.lastSelectedDatum = null;
    }

  }
}