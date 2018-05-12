Thanks for using Table Pro!

Release Notes 1.24 (TBD)
  BUG: TablePro now works with Unity >= 2017.3!
  INFO: Minimum supported Unity rev increased from 5.5.0 to 5.5.5p2

Release Notes 1.23 (2018.01.04)
  ENHANCEMENT: 'long-tap' event performance improvement [using invoke instead of Update()]
  ENHANCEMENT: Now support cell deselect callbacks [Table.SetDeselected()]
  ENHANCEMENT: Now support configurable multi-select key (control/shift)
  BUG: Fixed rare issue with TMPRo Input Cell positioning
  BUG: Changed Row click handler to trigger Cell-level clicks in all cases

Release Notes 1.22 (2017.10.25):
  ENHANCEMENT: Now support a 'long-tap' event on cells.
  ENHANCEMENT: Now support selection modes: MULTICELL and MULTIROW
  ENHANCEMENT: Globally applied uncrustify code style
  BUG: Fixed rare issue related to alpha colors and header cells
  BUG: Input cells now reset propery when table is redrawn in change handler
  BUG: Input cells now position properly in 2017.1

Release Notes 1.21 (2017.07.05):
  ENHANCEMENT: added ability to disable row hover colors
  BUG: Input cells + programmatically animated scrolling now work together!

Release Notes 1.20 (2017.04.20):
  ENHANCEMENT: now supports TextMeshPro text! (http://u3d.as/L38 or Pro version >= 1.0.55.56.0b8)
  ENHANCEMENT: added UID based indexof method on datum object
  ENHANCEMENT: can now defined scrollbar 'thumb' size
  ENHANCEMENT: new can define default font and style for all tables in singleton
  BUG: Patched vertical row spacing issue
  BUG: TablePro will now throw an WARNING when StartRenderEngine called on inactive table
  BUG: Fixes for row selection hover issues
  BUG: Fixed rare error when rendering header cells

Release Notes 1.19 (2017.02.10):
  BUG: Correcting rare issues around multi-line data in single-row tables
  BUG: Correcting issues with using keyboard nav with input field heavy tables

Release Notes 1.18 (2017.01.13):
  ENHANCEMENT: now support adding integer elements directly (no stringing needed)

Release Notes 1.17 (2016.12.23):
  BUG: Fixed issue with scrollbar visibility in Unity 5.5+

Release Notes 1.16 (2016.12.19):
  BUG: Correct gizmo display for ScreenSpaceOverlay/Camera
  BUG: Setting transform parent properly on input widget
  ENHANCEMENT: Rows now have a definable "Normal Alt Color"

Release Notes 1.15 (2016.10.19):
  BUG: Will now color rows properly on mouse-over on touch devices
  BUG: Will handle resize when set to force 100% width

Release Notes 1.14 (2016.10.19):
  BUG: Fixed issue with table redrawing in draggable parent rect
  BUG: Fixed issue applying table formatting after redraw
  ENHANCEMENT: Code Formatting Updates

Release Notes 1.13 (2016.10.13):
  ENHANCEMENT: Multiple bug fixes and enhancements per awesome user feedback!

Release Notes 1.12 (2016.09.19):
  BUG: Corrected cell coloring issue when deactivating and activating a table
  ENHANCEMENT: Will now auto-scroll when using the MoveSelectionUp/MoveSelectionDown methods
  ENHANCEMENT: Added new "General Settings" called "Table Selection UI Mode"
               this can be used to disable cell-level UI highlighting on interaction

Release Notes 1.11 (2016.08.18):
  Refactored all methods to make them UpperCase.  Tried to wrap the core methods with an
  "obsolete" warning message but could potentially throw errors if more obscure methods
  were previously being used in external implementations.  Simply change the method call
  to UpperCase and it'll work.


Simple instructions to get started:

  1. Create an 'empty' UI element and anchor it however you wish.
  2. Add our Table script to your empty UI element.
  3. In one of your scripts, just access and work with the Table
     component something like the simple example below.

Please see our other samples in the included Samples directory or visit
  http://semi-legitimate.com/tablepro/ for additional documentation!

Sample Usage:

using UnityEngine;
using SLS.Widgets.Table;

public class Simple : MonoBehaviour {

  private Table table;

  void Start () {

    this.table = this.GetComponent<Table>();

    this.table.ResetTable();

    this.table.AddTextColumn();
    this.table.AddTextColumn();
    this.table.AddTextColumn();

    // Initialize Your Table
    this.table.Initialize(this.OnTableSelected);

    // Populate Your Rows (obviously this would be real data here)
    for (int i = 0; i < 100; i++) {
      Datum d = Datum.Body(i.ToString());
      d.elements.Add("Col1:Row" + i.ToString());
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
    if (column != null) cidx = column.idx.ToString();
    print("You Clicked: " + datum.uid + " Column: " + cidx);
  }

}
