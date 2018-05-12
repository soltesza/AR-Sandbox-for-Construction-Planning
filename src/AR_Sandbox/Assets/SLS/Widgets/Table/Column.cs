using UnityEngine;
using System;

namespace SLS.Widgets.Table {
  public class Column {

    private int _idx;

    public int idx { get { return this._idx; } }

    Color? _headerTextColorOverride = null;

    public Color headerTextColorOverride {
      get { return _headerTextColorOverride.HasValue ? _headerTextColorOverride.Value : this.table.headerTextColor; }
      set { _headerTextColorOverride = value; }
    }

    public enum ColumnType {
      TEXT,
      IMAGE
    }

    public enum HorAlignment {
      LEFT,
      CENTER,
      RIGHT
    }

    public void ClearMeasure() {
      this.rawWidth = null;
      this.measuredMinWidth = null;
      this.measuredMaxWidth = null;
    }

    public float safeWidth {
      get {
        float r;
        if(this.rawWidth.HasValue)
          r = this.rawWidth.Value;
        else
          r = 0;

        if(this.measuredMinWidth.HasValue && this.measuredMinWidth.Value > 0)
          r = Mathf.Max(this.measuredMinWidth.Value, r);

        if(this.minWidth > 0)
          r = Mathf.Max(this.minWidth, r);

        if(this.maxWidth > 0)
          r = Mathf.Min(this.maxWidth, r);

        if(this.measuredMaxWidth.HasValue && this.measuredMaxWidth.Value > 0)
          r = Mathf.Min(r, this.measuredMaxWidth.Value);

        /*
           if (this.measuredMinWidth.HasValue)
           Debug.Log("returning safeWidth for " + this.idx + " as " + r + " MMinW: " + this.measuredMinWidth.Value + " MMaxW: " + this.maxWidth);
           else
           Debug.Log("returning safeWidth for " + this.idx + " as " + r);
         */
        return r;
      }
    }

    public float? rawWidth;

    public float CheckWidth(float w) {
      if(!this.rawWidth.HasValue || w > this.rawWidth.Value)
        this.rawWidth = w;
      return this.safeWidth;
    }

    public HorAlignment horAlignment;
    protected ColumnType _columnType;

    public ColumnType columnType { get { return this._columnType; } }

    protected float _minWidth;

    public float minWidth { get { return this._minWidth; } }

    protected float _maxWidth;

    public float maxWidth { get { return this._maxWidth; } }

    public float? measuredMinWidth;
    public float? measuredMaxWidth;
    public int headerFontSize;
    public int footerFontSize;
    private float _imageWidth;

    public float imageWidth { get { return this._imageWidth; } }

    private float _imageHeight;

    public float imageHeight { get { return this._imageHeight; } }

    private bool _isInput;

    public bool isInput { get { return this._isInput; } }

    public Action<Datum, Column, string, string> inputChangeCallback;
    private Datum headerDatum;
    private Datum footerDatum;

    //private string _headerText;
    public string headerValue {
      get {
        // < 0 for extraText column
        if(this.idx < 0)
          return null;
        return this.headerDatum.elements[this.idx].value;
      }
      set {
        // < 0 for extraText column
        if(this.idx < 0)
          return;
        this.headerDatum.elements[this.idx].value = value;
      }
    }

    private string _headerIcon;

    public string headerIcon {
      get { return this._headerIcon; }
      set {
        this._headerIcon = value;
        this.headerDatum.isDirty = true;
      }
    }

    private Color? _headerIconColor;

    public Color? headerIconColor {
      get { return this._headerIconColor; }
      set {
        if(!this.table.hasHeaderIcons)
          return;
        this._headerIconColor = value;
        this.headerDatum.isDirty = true;
      }
    }

    //private string _footerText;
    public string footerValue {
      get {
        // < 0 for extraText column
        if(this.idx < 0)
          return null;
        return this.footerDatum.elements[this.idx].value;
      }
      set {
        // < 0 for extraText column
        if(this.idx < 0)
          return;
        this.footerDatum.elements[this.idx].value = value;
      }
    }

    private Table table;

    public static Column ImageColumn(Table table, int idx,
                                     float imageWidth, float imageHeight,
                                     Datum headerDatum, Datum footerDatum) {
      Column c = new Column(table, idx, headerDatum, footerDatum);
      c._columnType = ColumnType.IMAGE;
      c._imageWidth = imageWidth;
      c._imageHeight = imageHeight;
      return c;
    }

    public static Column TextColumn(Table table, int idx,
                                    float minWidth, float maxWidth,
                                    Datum headerDatum, Datum footerDatum, bool isInput) {
      Column c = new Column(table, idx, headerDatum, footerDatum);
      c._columnType = ColumnType.TEXT;
      c._minWidth = minWidth;
      c._maxWidth = maxWidth;
      c._isInput = isInput;
      return c;
    }

    protected Column(Table table, int idx,
                     Datum headerDatum, Datum footerDatum) {
      this.table = table;
      this._idx = idx;
      this.headerDatum = headerDatum;
      this.footerDatum = footerDatum;
      this.horAlignment = HorAlignment.LEFT;
      this.headerFontSize = this.table.defaultFontSize;
      this.footerFontSize = this.table.defaultFontSize;
      this.ClearMeasure();
    }

    public int CalcFont(bool isHeader, bool isFooter) {
      if(!isHeader && !isFooter)
        return this.table.defaultFontSize;
      else if(isHeader)
        return this.headerFontSize;
      else
        return this.footerFontSize;
    }

    public int CalcFont(Datum d) {
      return this.CalcFont(d.isHeader, d.isFooter);
    }

  }

}