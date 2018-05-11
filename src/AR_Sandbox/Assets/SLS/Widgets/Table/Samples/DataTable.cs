using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace SLS.Widgets.Table {
  public class DataTable : MonoBehaviour {

    public Table table;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    public Sprite sprite5;
    public Sprite iconUp;
    public Sprite iconDown;
    private Dictionary<string, Sprite> spriteDict;
    private List<string> spriteNames;
    private List<DataTableData.Population> poplist;

    void Start() {
      MakeDefaults.Set();
      this.DrawTable();
    }

    public void DrawTable() {

      // build our sprite cross-reference
      this.spriteDict = new Dictionary<string, Sprite>();
      this.spriteDict.Add("1", this.sprite1);
      this.spriteDict.Add("2", this.sprite2);
      this.spriteDict.Add("3", this.sprite3);
      this.spriteDict.Add("4", this.sprite4);
      this.spriteDict.Add("5", this.sprite5);
      this.spriteNames = new List<string>(this.spriteDict.Keys);

      this.spriteDict.Add("UP", this.iconUp);
      this.spriteDict.Add("DOWN", this.iconDown);

      // get our data list, this would be data retrieved from any source you wish
      this.poplist = DataTableData.Generate();

      int popTotal = 0;
      for(int i = 0; i < this.poplist.Count; i++) {
        popTotal += this.poplist[i].population;
      }

      this.table.ResetTable();

      // Define our columns
      Column c;
      c = this.table.AddTextColumn("Rank", null, -1, 50);
      c.horAlignment = Column.HorAlignment.CENTER;
      c.headerIcon = "UP";
      c = this.table.AddInputColumn(this.OnInputFieldChange, "City");
      c = this.table.AddTextColumn("Country");
      c = this.table.AddTextColumn("Population", popTotal.ToString("#,##0"));
      c.horAlignment = Column.HorAlignment.RIGHT;
      c = this.table.AddImageColumn("Icon");
      c.horAlignment = Column.HorAlignment.CENTER;
      c = this.table.AddTextColumn("Density (/km²)");
      c.horAlignment = Column.HorAlignment.RIGHT;
      c = this.table.AddTextColumn("Area (km²)");
      c.horAlignment = Column.HorAlignment.RIGHT;

      // Initialize Your Table
      this.table.Initialize(this.OnTableSelectedWithCol, this.spriteDict, true, this.OnHeaderClick);

      // Just activate our default sort
      this.OnHeaderClick(this.table.columns[0], null);

      // Draw Your Table
      this.table.StartRenderEngine();

    }

    private void OnInputFieldChange(Datum d, Column c, string oldVal, string newVal) {
      print("Change from " + oldVal + " to " + newVal);
    }

    private void OnTableSelectedWithCol(Datum datum, Column column) {
      if (datum == null) return;
      string cidx = "N/A";
      if(column != null)
        cidx = column.idx.ToString();
      print("You Clicked: " + datum.uid + " Column: " + cidx);
    }

    public void MoveSelection() {
      Element e = table.GetSelectedElement();
      if (e == null) return; // no selected cell
      table.MoveSelectionDown(false);
    }

    private void OnHeaderClick(Column column, PointerEventData e) {

      bool isAscending = false;
      // Reset current sort UI
      for(int i = 0; i < this.table.columns.Count; i++) {
        if(column == this.table.columns[i]) {
          if(column.headerIcon == "UP") {
            isAscending = true;
            column.headerIcon = "DOWN";
          }
          else {
            isAscending = false;
            column.headerIcon = "UP";
          }
        }
        else
          this.table.columns[i].headerIcon = null;
      }

      // do the sort
      this.poplist.Sort(
        delegate(DataTableData.Population p1, DataTableData.Population p2) {
        // RANK
        if(column.idx == 0) {
          if(isAscending)
            return p1.rank.CompareTo(p2.rank);
          else
            return p2.rank.CompareTo(p1.rank);
        }
        // CITY
        if(column.idx == 1) {
          if(isAscending)
            return p1.city.CompareTo(p2.city);
          else
            return p2.city.CompareTo(p1.city);
        }
        // COUNTRY
        if(column.idx == 2) {
          if(isAscending)
            return p1.country.CompareTo(p2.country);
          else
            return p2.country.CompareTo(p1.country);
        }
        // POPULATION
        if(column.idx == 3) {
          if(isAscending)
            return p1.population.CompareTo(p2.population);
          else
            return p2.population.CompareTo(p1.population);
        }
        // ICON
        if(column.idx == 4) {
          if(isAscending)
            return p1.iconIndex.CompareTo(p2.iconIndex);
          else
            return p2.iconIndex.CompareTo(p1.iconIndex);
        }
        // DENSITY
        if(column.idx == 5) {
          if(isAscending)
            return p1.density.CompareTo(p2.density);
          else
            return p2.density.CompareTo(p1.density);
        }
        // SIZE
        if(column.idx == 6) {
          if(isAscending)
            return p1.sqkm.CompareTo(p2.sqkm);
          else
            return p2.sqkm.CompareTo(p1.sqkm);
        }
        return p1.rank.CompareTo(p2.rank);
      }
        );

      this.table.data.Clear();
      for(int i = 0; i < this.poplist.Count; i++) {
        DataTableData.Population p = this.poplist[i];
        Datum d = Datum.Body(i.ToString());
        d.elements.Add(p.rank.ToString());
        d.elements.Add(p.city);
        d.elements.Add(p.country);
        d.elements.Add(p.population.ToString("#,##0"));
        d.elements.Add(this.spriteNames[p.iconIndex]);
        d.elements.Add(p.density.ToString("#,##0"));
        d.elements.Add(p.sqkm.ToString("#,##0.0"));
        if(!string.IsNullOrEmpty(p.extraText)) {
          d.extraText = new Element(d, p.extraText);
        }
        this.table.data.Add(d);
      }

    }

  }

}