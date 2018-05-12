using UnityEngine;

// Include this when using outside our namespace:
// using SLS.Widgets.Table;

namespace SLS.Widgets.Table {
  public class KeyboardInput : MonoBehaviour {

    private Table table;

    void Start() {

      MakeDefaults.Set();
      this.table = this.GetComponent<Table>();

      this.table.ResetTable();

      this.table.AddInputColumn(this.OnInputFieldChange);
      this.table.AddInputColumn(this.OnInputFieldChange);
      this.table.AddInputColumn(this.OnInputFieldChange);
      this.table.AddInputColumn(this.OnInputFieldChange);
      this.table.AddInputColumn(this.OnInputFieldChange);

      // Initialize Your Table
      this.table.Initialize(this.OnTableSelected);

      // Populate Your Rows (obviously this would be real data here)
      for(int i = 0; i < 100; i++) {
        Datum d = Datum.Body(i.ToString());
        d.elements.Add(i.ToString());
        d.elements.Add("Col2:Row" + i.ToString());
        d.elements.Add("Col3:Row" + i.ToString());
        d.elements.Add("Col4:Row" + i.ToString());
        d.elements.Add("Col5:Row" + i.ToString());
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
      print("You Clicked: " + datum.uid + " Column: " + cidx);
    }

    // handle field input
    private void OnInputFieldChange(Datum d, Column c, string oldVal, string newVal) {
      print("Change from " + oldVal + " to " + newVal);
    }

    public void Update() {
      if (Input.GetKeyDown(KeyCode.Tab)) {
        this.table.MoveSelectionRight(false);
      }
    }

  }
}