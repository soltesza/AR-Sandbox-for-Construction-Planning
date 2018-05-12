using UnityEngine;
using System.Collections;

namespace SLS.Widgets.Table {
  public class AutoUpdating : MonoBehaviour {

    private Table table;

    void Start() {

      MakeDefaults.Set();
      this.table = this.GetComponent<Table>();

      this.table.ResetTable();

      this.table.AddTextColumn("h1", "f1");
      this.table.AddTextColumn("h2", "f2");
      this.table.AddTextColumn("h3", "f3");
      this.table.AddTextColumn("h4", "f4");
      this.table.AddTextColumn("h5", "f5");

      this.table.Initialize();

      // Populate Your Rows (obviously this would be real data here)
      for(int i = 0; i < 10; i++) {
        this.table.data.Add(this.MakeDatum("INIT"));
      }

      // Draw Your Table
      this.table.StartRenderEngine();

      StartCoroutine(this.DoRandomData());

    }

    private Datum MakeDatum(string pfx) {
      string sfx = Time.realtimeSinceStartup.ToString();
      Datum d = Datum.Body(sfx);
      d.elements.Add("Col1:" + pfx + ":" + sfx);
      d.elements.Add("Col2:" + pfx + ":" + sfx);
      d.elements.Add("Col3:" + pfx + ":" + sfx);
      d.elements.Add("Col4:" + pfx + ":" + sfx);
      d.elements.Add("Col5:" + pfx + ":" + sfx);
      return d;
    }

    IEnumerator DoRandomData() {
      yield return new WaitForSeconds(2f);
      while(true) {
        float action = Random.Range(0, 50);
        if(action < 5) {
          this.table.data.Add(this.MakeDatum("ADD"));
        }
        else if(action < 10) {
          this.table.data.Insert(0, this.MakeDatum("TOP"));
        }
        else if(action < 15) {
          int idx = Random.Range(0, this.table.data.Count);
          this.table.data.Insert(idx, this.MakeDatum("INS"));
        }
        else if(action < 20) {
          if(this.table.data.Count > 0) {
            int idx = Random.Range(0, this.table.data.Count);
            this.table.data.RemoveAt(idx);
          }
        }
        else if(action < 25) {
          int cidx = Random.Range(0, this.table.columns.Count);
          this.table.columns[cidx].headerValue =
            "UPD:" + Time.realtimeSinceStartup.ToString();
        }
        else if(action < 30) {
          int cidx = Random.Range(0, this.table.columns.Count);
          this.table.columns[cidx].footerValue =
            "UPD:" + Time.realtimeSinceStartup.ToString();
        }
        else {
          if(this.table.data.Count > 0) {
            int ridx = Random.Range(0, this.table.data.Count);
            int cidx = Random.Range(0, this.table.columns.Count);
            this.table.data[ridx].elements[cidx].value =
              "UPD:" + Time.realtimeSinceStartup.ToString();
          }
        }
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
      }
    }

  }
}