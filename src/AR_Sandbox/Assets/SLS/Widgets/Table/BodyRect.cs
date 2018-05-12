using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace SLS.Widgets.Table {
  public class BodyRect : UIBehaviour {

    private Table _table;
    private RectTransform _rt;
    public bool isMeasured;
    public float lastWidth;

    public Table table {
      get {
        return this._table;
      }
    }

    public RectTransform rt {
      get {
        return this._rt;
      }
    }

    public void Init(Table t, RectTransform rt) {
      this._table = t;
      this._rt = rt;
    }

    override protected void OnRectTransformDimensionsChange() {
      if(!isMeasured || this.lastWidth != this.rt.rect.width) {
        this.lastWidth = this.rt.rect.width;
        base.OnRectTransformDimensionsChange();
        this.table.DirtyLater();
      }
      this.isMeasured = true;
    }

  }
}