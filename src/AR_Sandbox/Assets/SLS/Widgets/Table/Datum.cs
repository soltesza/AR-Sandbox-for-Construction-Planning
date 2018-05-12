using UnityEngine;
using System;
using System.Collections.Generic;

namespace SLS.Widgets.Table {
  public class Element {

    public Datum datum { private set; get; }

    public int idx { private set; get; }

    public string tooltip { set; get; }

    private string _value;

    public string value {
      get { return this._value; }
      set {
        this._value = value;
        this.datum.animationStartTime = Time.realtimeSinceStartup;
        this.datum.isDirty = true;
      }
    }

    private Color? _color;

    public Color? color {
      get { return this._color; }
      set {
        this._color = value;
        this.datum.isDirty = true;
      }
    }

    private Color? _backgroundColor;

    public Color? backgroundColor {
      get { return this._backgroundColor; }
      set {
        this._backgroundColor = value;
        this.datum.isDirty = true;
      }
    }

    public float? measuredWidth;
    public float? measuredHeight;

    public Element(Datum d, string value, int idx=-1) {
      this.datum = d;
      this.value = value;
      this.idx = idx;
    }

    public void ClearMeasure() {
      this.measuredWidth = null;
      this.measuredHeight = null;
    }

  }

  public class Datum {
    public string uid { set; get; }

    public object rawObject { set; get; }

    public string tooltip { set; get; }

    public DatumElementList elements { set; get; }

    public Element extraText { set; get; }

    public Color? extraTextBoxColor;
    public Color? extraTextColor;
    public Table table;
    public float animationStartTime;
    private bool _isDirty;
    private float _revision;
    public bool isEvenRow;

    public float revision {
      get {
        return this._revision;
      }
    }

    private float _lastDirty;

    public float lastDirty {
      get {
        return this._lastDirty;
      }
    }

    private float _lastClean;

    public float lastClean {
      get {
        return this._lastClean;
      }
    }

    public bool isDirty {
      get { return this._isDirty; }
      set {
        //Debug.Log(this.uid + " setting isDirty: " + value);
        this._isDirty = value;
        if(this._isDirty) {
          this._revision = Time.realtimeSinceStartup;
          this._lastDirty = this._revision;
        }
        else {
          this._lastClean = Time.realtimeSinceStartup;
        }
        if(this._isDirty && this.table != null && this.table.isRunning && this.table.data != null) {
          this.table.data.ClearSafeHeightSum();
          this.table.data.ClearMeasuredVertPos();
          Row r = this.table.GetRowForDatum(this);
          if(r != null) {
            //Debug.Log("running row refresh on datum: " + this.uid);
            r.Refresh();
          }
        }
      }
    }

    public void ClearMeasure() {
      //Debug.Log("Clearing measure on " + this.uid);
      //this.addedHeight = false;
      this.measuredVertPos = null;
      this._measuredHeight = null;
      this._measuredCellHeight = null;
      this._lastSafeHeightResult = null;
      this.table.data.ClearSafeHeightSum();
      for(int i = 0; i < this.elements.Count; i++) {
        if(this.elements[i] != null)
          this.elements[i].ClearMeasure();
      }
      if(this.extraText != null)
        this.extraText.ClearMeasure();
    }

    public float? measuredVertPos;

    public float SafeExtraTextHeight() {
      if(this.extraText != null && this.extraText.measuredHeight.HasValue) {
        //Debug.Log("ETH: " + this.extraText.measuredHeight.Value);
        return this.extraText.measuredHeight.Value;
      }
      return 0;
    }

    private float? _measuredCellHeight;

    public float SafeCellHeight() {
      if(!this._measuredCellHeight.HasValue) {
        float tmpHeight = 0;
        for(int i = 0; i < this.elements.Count; i++) {
          if(this.elements[i] != null) {
            Element element = this.elements[i];
            if(!element.measuredHeight.HasValue)
              return -1;
            if(element.measuredHeight.Value > tmpHeight) {
              //Debug.Log("SCH: " + this.uid + " incrementing height on cell " + i + " to " + element.measuredHeight.Value);
              tmpHeight = element.measuredHeight.Value;
            }
          }
        }
        this._measuredCellHeight = tmpHeight;
      }
      return this._measuredCellHeight.Value;
    }

    private float? _measuredHeight;
    private float? _lastSafeHeightResult;

    public float SafeHeight() {
      if(this.SafeCellHeight() < 0 ||
         (this.extraText != null && !this.extraText.measuredHeight.HasValue) ||
         this.table == null) {
        //Debug.Log("MH " + this.uid + ": -1");
        return -1;
      }
      if(!this._measuredHeight.HasValue) {
        // See the comment in vertical layout in Control.cs
        //  if the rowVertSpace multiples don't make sense
        float tmpHeight;
        if(this.isHeader)
          tmpHeight = this.SafeCellHeight() +
                      this.table.headerTopMargin + this.table.headerBottomMargin;
        else if(this.isFooter)
          tmpHeight = this.SafeCellHeight() +
                      this.table.footerTopMargin + this.table.footerBottomMargin;
        else
          tmpHeight = this.SafeCellHeight() +
                      this.table.rowVerticalSpacing;
        if(this.SafeExtraTextHeight() > 0) {
          tmpHeight += this.SafeExtraTextHeight() +
                       this.table.rowVerticalSpacing * 1.5f;
        }
        this._measuredHeight = tmpHeight;
      }

      if(!this._lastSafeHeightResult.HasValue ||
         this._lastSafeHeightResult.Value != this._measuredHeight.Value) {
        if(!this.table.data.doingSafeHeightSum)
          this.table.data.ClearSafeHeightSum();
        this._lastSafeHeightResult = this._measuredHeight.Value;
      }

      //Debug.Log("MH " + this.uid + ": " + this._measuredHeight.Value);
      //Thanks greay!
      //(http://forum.unity3d.com/threads/table-pro-1-3-when-your-data-isnt-a-game.347893/#post-2312395)
      if(this._measuredHeight.Value > this.table.minRowHeight)
        return this._measuredHeight.Value;
      else {
        if(this.isHeader)
          return this.table.minHeaderHeight;
        if(this.isFooter)
          return this.table.minFooterHeight;
        return this.table.minRowHeight;
      }
    }

    //public bool addedHeight;

    private bool _isHeader;
    private bool _isFooter;

    public bool isHeader { get { return this._isHeader; } }

    public bool isFooter { get { return this._isFooter; } }

    public static Datum Body(string uid) {
      Datum d = new Datum(uid);
      return d;
    }

    public static Datum Header() {
      Datum d = new Datum("Header_" + Guid.NewGuid().ToString());
      d._isHeader = true;
      return d;
    }

    public static Datum Footer() {
      Datum d = new Datum("Footer_" + Guid.NewGuid().ToString());
      d._isFooter = true;
      return d;
    }

    private Datum(string uid) {
      this.uid = uid;
      this.elements = new DatumElementList(this);
      this.isDirty = true;
    }
  }

  public class DatumElementList {

    private Datum datum;
    private Control control;
    private List<Element> list;

    public int Count {
      get { return this.list.Count; }
    }

    public DatumElementList(Datum d) {
      this.datum = d;
      this.list = new List<Element>();
    }

    public Element this[int index] {
      get {
        if(index >= 0 && index < this.list.Count)
          return this.list[index];
        return null;
      }
    }

    public Element Add(int val) {
      return this.Add(val.ToString());
    }

    public Element Add(string val) {
      Element e = new Element(this.datum, val, this.list.Count);
      this.list.Add(e);
      return e;
    }

    public Element Add() {
      Element e = new Element(this.datum, null, this.list.Count);
      this.list.Add(e);
      return e;
    }

  }

  public class TableDatumList : IEnumerable<Datum> {

    private Table table;
    private Control control;
    private List<Datum> list;
    private Dictionary<string, int> indexes;
    private int count;

    public int Count {
      get { return this.count; }
    }

    private float? _safeTempRowHeight;

    public float safeTempRowHeight {
      get {
        // this is a hack to just issue the getter for safeHeightSum if we
        //   need to assign a value for our _avgHeight
        if(!this._avgHeight.HasValue && safeHeightSum == 99.99f) {}
        ;
        return Mathf.Max(this._avgHeight.Value, this.table.minRowHeight);
      }
    }

    private bool _doingSafeHeightSum;

    public bool doingSafeHeightSum {
      get { return this._doingSafeHeightSum; }
    }

    private float? _avgHeight;
    private float? _safeHeightSum;

    public float safeHeightSum {
      get {
        if(!this._safeHeightSum.HasValue) {
          this._doingSafeHeightSum = true;
          float undefCount = 0;
          float defCount = 0;
          this._safeHeightSum = 0.0f;
          for(int i = 0; i < this.count; i++) {
            Datum d = this.list[i];
            if(d.SafeHeight() >= 0) {
              this._safeHeightSum += this.list[i].SafeHeight();
              defCount += 1;
            }
            else
              undefCount += 1;
          } // for
          if(defCount > 0)
            this._avgHeight = (this._safeHeightSum / defCount);
          else
            this._avgHeight = 0f;
          this._safeHeightSum += this._avgHeight.Value * undefCount;
          this._doingSafeHeightSum = false;
        }
        return this._safeHeightSum.Value;
      }
    }

    private bool _changing;

    public bool changing {
      get {
        return this._changing;
      }
    }

    public TableDatumList(Table table, Control control) {
      this.table = table;
      this.control = control;
      this.count = 0;
      this.ClearSafeHeightSum();
      this.list = new List<Datum>();
      this.indexes = new Dictionary<string, int>();
    }

    private void InitDatum(Datum item, bool isNew=true) {
      item.table = this.table;
      if(isNew && this.indexes.ContainsKey(item.uid)) {
        this.table.error("Datum UID collision: " + item.uid);
      }
      if(item.elements.Count != this.table.columns.Count) {
        this.table.error("Datum Column mismatch on UID: " + item.uid);
      }
    }

    public void ClearMeasured() {
      for(int i = 0; i < this.count; i++) {
        // this calls our 'clearSafeHeightSum'
        this.list[i].ClearMeasure();
      }
    }

    public void ClearSafeHeightSum() {
      this._safeHeightSum = null;
      this._avgHeight = null;
    }

    public void ClearMeasuredVertPos() {
      for(int i = 0; i < this.count; i++) {
        // this calls our 'clearSafeHeightSum'
        this.list[i].measuredVertPos = null;
      }
    }

    private void FinishDataChange() {
      this._changing = false;
      if(this.table.isRunning) {
        this.ClearMeasured();
        for(int i = 0; i < this.count; i++) {
          this.list[i].isDirty = true;
        }
        this.control.HandleDataChange();
      }
    }

    private void RebuildIndexes() {
      int i = 0;
      bool even = false;
      this.indexes.Clear();
      for(i = 0; i < this.list.Count; i++) {
        this.indexes.Add(this.list[i].uid, i);
        this.list[i].isEvenRow = even;
        even = !even;
      }
      this.count = i;
      //Debug.Log("Set DS Count to: " + this.count);
    }

    public void AddAll(List<Datum> items) {
      items.TrimExcess();
      foreach(Datum item in items) {
        this.Add(item, false);
      }
      this.FinishDataChange();
    }

    public void Add(Datum item, bool finishAfterAdd=true) {
      this._changing = true;
      this.InitDatum(item);
      this.list.Add(item);
      this.indexes.Add(item.uid, this.count);
      this.count += 1;
      item.isEvenRow = (this.count % 2 == 0);
      item.animationStartTime = Time.realtimeSinceStartup;
      if(finishAfterAdd) {
        this.FinishDataChange();
      }
    }

    public void Clear() {
      this._changing = true;
      this.count = 0;
      this.list.Clear();
      this.indexes.Clear();
      this.FinishDataChange();
    }

    public bool Contains(Datum item) {
      return this.indexes.ContainsKey(item.uid);
    }

    public Datum Get(string uid) {
      if(this.indexes.ContainsKey(uid)) {
        return this.list[this.indexes[uid]];
      }
      return null;
    }

    public bool Remove(string uid) {
      this._changing = true;
      bool res = false;
      if(this.indexes.ContainsKey(uid)) {
        res = true;
        this.list.RemoveAt(this.indexes[uid]);
        this.RebuildIndexes();
        this.FinishDataChange();
      }
      else
        this._changing = false;
      return res;
    }

    public bool Remove(Datum item) {
      return this.Remove(item.uid);
    }

    public void Insert(int index, Datum item) {
      this._changing = true;
      this.InitDatum(item);
      this.list.Insert(index, item);
      this.RebuildIndexes();
      item.animationStartTime = Time.realtimeSinceStartup;
      this.FinishDataChange();
    }

    public void RemoveAt(int index) {
      this._changing = true;
      if(this.count - 1 >= index) {
        this.list.RemoveAt(index);
        this.RebuildIndexes();
        this.FinishDataChange();
      }
      else
        this._changing = false;
    }

    public Datum this[int index] {
      get {
        if(index >= 0 && index < this.count)
          return this.list[index];
        return null;
      }
      set {
        this._changing = true;
        if(index >= 0 && index < this.count) {
          this.InitDatum(value, false);
          this.list[index] = value;
          value.animationStartTime = Time.realtimeSinceStartup;
          this.FinishDataChange();
        }
      }
    }

    public int IndexOf(Datum item) {
      if(item != null)
        return this.IndexOf(item.uid);
      return -1;
    }

    public int IndexOf(string uid) {
      if(this.indexes.ContainsKey(uid))
        return this.indexes[uid];
      return -1;
    }

    public bool Update(Datum item) {
      this._changing = true;
      int index = this.IndexOf(item);
      if(index >= 0) {
        this.InitDatum(item, false);
        this.list[index] = item;
        item.animationStartTime = Time.realtimeSinceStartup;
        this.FinishDataChange();
        return true;
      }
      return false;
    }

    public IEnumerator<Datum> GetEnumerator() {
      return this.list.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

  }

}