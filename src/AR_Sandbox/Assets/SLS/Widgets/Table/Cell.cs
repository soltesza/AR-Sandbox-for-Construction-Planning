using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Globalization;
#if TMP_PRESENT
using TMPro;
#endif

namespace SLS.Widgets.Table {
  public class Cell : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler {

    public Column column { protected set; get; }

    public RectTransform rt;
    public RectTransform crt;
  #if TMP_PRESENT
    public TextMeshProUGUI text;
  #else
    public Text text;
  #endif
    public Image image;
    public Image background;
    protected bool isDown;

    //public Cell preceedingCell;

    protected Table table;

    public Row row { protected set; get; }

    private Action<Datum, Column> clickCallback;
    private Action<Datum, Column, PointerEventData> clickCallbackWithData;

    public void SetContentSizeDelta(Vector2 size) {
      if(this.text != null)
        this.text.rectTransform.sizeDelta = size;
      /* we don't want to adjust the size of our image, it's set staticly in the factory
         if (this.image != null)
         this.image.rectTransform.sizeDelta = size;
       */
    }

    public void SetContentLocalPosition(float x, float y) {
      if(this.text != null)
        this.text.rectTransform.localPosition = new Vector2(x, y);
      if(this.image != null) {
        this.image.rectTransform.localPosition = new Vector2(this.image.rectTransform.localPosition.x,
                                                             (this.table.rowVerticalSpacing * -0.5f) -
                                                             (this.row.datum.SafeCellHeight() * 0.5f));
      }
    }

    // reset UI state if table is deactivated and reactivated
    private void OnEnable() {
      if(this.table == null)
        return;
      this.isDown = false;
      this.SetColor();
    }

  #if TMP_PRESENT
    public bool Initialize(Table table, Row row, Column column, int idx,
                           RectTransform rt, RectTransform guts, TextMeshProUGUI text) {
  #else
    public bool Initialize(Table table, Row row, Column column, int idx,
                           RectTransform rt, RectTransform guts, Text text) {
  #endif
      this.text = text;
      return this.FinishInit(table, row, column, idx, rt, guts);
    }

    public bool Initialize(Table table, Row row, Column column, int idx,
                           RectTransform rt, RectTransform guts, Image image) {
      this.image = image;
      return this.FinishInit(table, row, column, idx, rt, guts);
    }

    private bool FinishInit(Table table, Row row, Column column, int idx,
                            RectTransform rt, RectTransform guts) {
      this.table = table;
      this.row = row;
      this.column = column;
      this.rt = rt;
      this.crt = guts;
      if(idx >= row.cells.Count)
        row.cells.Add(this);
      else {
        row.cells[idx] = this;
      }
      // we do this here to handle initial render after a 'redraw'
      if(this._element != null)
        this.AttachElement();
      return true;
    }

    private Element _element;

    public Element element {
      set {
        this.doingDirtyLater = false;
        this._element = value;
        this.AttachElement();
      }
      get {
        return this._element;
      }
    }

    private void AttachElement() {
      if(this.column.columnType == Column.ColumnType.TEXT ||
         this.row.datum.isHeader || this.row.datum.isFooter) {
        if(this._element != null && !string.IsNullOrEmpty(this._element.value))
          this.text.text = this._element.value;
        else
          this.text.text = "";
        if(this._element != null && this._element.color.HasValue) {
          this.text.color = this._element.color.Value;
        }
        else {
          if(!this.row.datum.isHeader && !this.row.datum.isFooter) {
            this.text.color = this.table.rowTextColor;
          }
          else if(this.row.datum.isHeader) {
            this.text.color = this.table.headerTextColor;
          }
          else {
            this.text.color = this.table.footerTextColor;
          }
        }
        if(this._element != null && this._element.backgroundColor.HasValue)
          this.background.color = this._element.backgroundColor.Value;
      }
      else {
        if(this._element != null &&
           !string.IsNullOrEmpty(this._element.value) &&
           this.table.sprites.ContainsKey(this._element.value))
          this.image.sprite = this.table.sprites[this._element.value];
        else
          this.image.sprite = null;
        if(this._element != null && this._element.color.HasValue) {
          if(this.image.color != this._element.color.Value) {
            this.image.color = this._element.color.Value;
            this.DirtyLater();
          }
        }
        else {
          if(this.image.color != Color.white) {
            this.image.color = Color.white;
            this.DirtyLater();
          }
        }
      }
      // The header color can be overridden through the column's 'headerTextColorOverride' property
      if (this._element.datum.isHeader)
        this.text.color = this.column.headerTextColorOverride;

      this.SetColor();
    }

    private bool doingDirtyLater;

    private void DirtyLater() {
      //this.lastMarkForRebuild = Time.realtimeSinceStartup;
      if(!this.doingDirtyLater)
        StartCoroutine(this.DoDirtyLater());
    }

    // our icon's dont color on first draw for some reason.
    //  Use this little hack to check them later
    IEnumerator DoDirtyLater() {
      this.doingDirtyLater = true;
      yield return Table.WaitForEndOfFrame;
      this.doingDirtyLater = false;
      if(this.image != null && this._element != null) {
        if(this._element.color.HasValue) {
          if(this._element.color.Value != this.image.color)
            this.image.color = this._element.color.Value;
        }
        else {
          if(Color.white != this.image.color)
            this.image.color = Color.white;
        }
      }
    }

    virtual public void HandleClick(PointerEventData data)
    {
        if (this.table.selectionMode == Table.SelectionMode.CELL || this.table.selectionMode == Table.SelectionMode.ROW)
        {
            this.table.SetSelected(this.row.datum, this.column);
        }
        else
        {
            // add selection if shift down
            if ((this.table.multiSelectKey == Table.MultiSelectKey.SHIFT &&
                 (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) ||
                (this.table.multiSelectKey == Table.MultiSelectKey.CONTROL &&
                 (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))) {
                // remove selection if we clicked something already selected w/ shift
                if (this.table.selectedDatumColumnDict.ContainsKey(this.row.datum) && this.table.selectedDatumColumnDict[this.row.datum].Contains(this.column))
                {
                    if (this.table.selectedDatumSet.Contains(this.row.datum))
                    {
                        this.table.selectedDatumSet.Remove(this.row.datum);
                    }

                    this.table.selectedDatumColumnDict[this.row.datum].Remove(this.column);
                    this.row.SetColor();
                    this.table.SetDeselected(this.row.datum, this.column);
                }
                else
                {
                    this.table.SetSelected(this.row.datum, this.column);
                }
            }
            else
            {
                //print("HANDLE CLICK! " + this.row.datum.uid);
                // remove EXISTING selection if we clicked something w/o shift
                this.table.selectedDatumSet.Clear();
                this.table.selectedDatumColumnDict.Clear();
                // if the thing we clicked wasn't already selected, select it now
                if (!this.table.selectedDatumColumnDict.ContainsKey(this.row.datum) || !this.table.selectedDatumColumnDict[this.row.datum].Contains(this.column))
                {
                    this.table.SetSelected(this.row.datum, this.column);
                }
            }
        }
    }

    virtual public void SetColor() {
      if(this.row.datum != null && this.row.datum.isFooter) {
        this.background.color = this.table.footerBackgroundColor;
        return;
      }
      if(this.table.bodyScrollWatcher.isDragging) {
        if(this.row.datum != null && table.selectedDatumSet.Contains(this.row.datum)) {
          if(this.table.selectedDatumColumnDict[this.row.datum].Contains(this.column))
            this.background.color = this.table.cellSelectColor;
          else if(this.table.selectionMode == Table.SelectionMode.ROW || this.table.selectionMode == Table.SelectionMode.MULTIROW)
            this.background.color = this.table.rowSelectColor;
          else {
            if (this.row.datum != null && this.row.datum.isEvenRow)
              this.background.color = this.table.rowAltColor;
            else
              this.background.color = this.table.rowNormalColor;
          }
        }
        else {
          if(this._element != null && this._element.backgroundColor.HasValue)
            this.background.color = this._element.backgroundColor.Value;
          else {
            if (this.row.datum != null && this.row.datum.isEvenRow)
              this.background.color = this.table.rowAltColor;
            else
              this.background.color = this.table.rowNormalColor;
          }
        }
      }
      else if(this.table.IsPointerOver(this) && (this.table.selectionMode == Table.SelectionMode.CELL ||
                                                 this.table.selectionMode == Table.SelectionMode.MULTICELL)) {
        if(this.isDown)
          this.background.color = this.table.cellDownColor;
        else {
          if(this.row.datum != null  && table.selectedDatumSet.Contains(this.row.datum)) {
            //print(this.column.idx + ": " + (this.column == this.table.selectedColumn).ToString());
            //print(this.table.selectedColumn);
            if(this.table.selectedDatumColumnDict[this.row.datum].Contains(this.column))
              this.background.color = this.table.cellSelectColor;
            else
              this.background.color = this.table.cellHoverColor;
          }
          else
            this.background.color = this.table.cellHoverColor;
        }
      }
      else if(this.table.IsPointerOver(this.row)) {
        if(this.row.isDown)
          this.background.color = this.table.rowDownColor;
        else {
          if(this.row.datum != null  && table.selectedDatumSet.Contains(this.row.datum)) {
            //print(this.column.idx + ": " + (this.column == this.table.selectedColumn).ToString());
            //print(this.table.selectedColumn);
            if(this.table.selectionMode == Table.SelectionMode.ROW || this.table.selectionMode == Table.SelectionMode.MULTIROW) {
              this.background.color = this.table.cellSelectColor;
            }
            else {
              if(this.table.selectedDatumColumnDict[this.row.datum].Contains(this.column))
                this.background.color = this.table.cellSelectColor;
              else
                this.background.color = this.table.rowHoverColor;
            }
          }
          else
            this.background.color = this.table.rowHoverColor;
        }
      }
      else {
        if(this.row.datum != null  && table.selectedDatumSet.Contains(this.row.datum)) {
          //print(this.column.idx + ": " + (this.column == this.table.selectedColumn).ToString());
          //print(this.table.selectedColumn);
          if(this.table.selectedDatumColumnDict[this.row.datum].Contains(this.column))
            this.background.color = this.table.cellSelectColor;
          else if (this.table.selectionMode == Table.SelectionMode.ROW ||
                   this.table.selectionMode == Table.SelectionMode.MULTIROW)
            this.background.color = this.table.rowSelectColor;
          else {
            if (this.row.datum != null && this.row.datum.isEvenRow)
              this.background.color = this.table.rowAltColor;
            else
              this.background.color = this.table.rowNormalColor;
          }
        }
        else {
          if(this._element != null && this._element.backgroundColor.HasValue)
            this.background.color = this._element.backgroundColor.Value;
          else {
            if (this.row.datum != null && this.row.datum.isEvenRow)
              this.background.color = this.table.rowAltColor;
            else
              this.background.color = this.table.rowNormalColor;
          }
        }
      }
    }

    public void OnPointerEnter(PointerEventData data) {
      if(this._element != null && this._element.datum.isHeader && this.table.headerActiveCallback != null)
        if(!this.table.headerActiveCallback(this.column))
          return;
      this.table.SetPointerOverCell(this);
      if(this.table.tooltipHandler != null && this._element != null &&
         !string.IsNullOrEmpty(this._element.tooltip))
        this.table.tooltipHandler(this.rt, this._element.tooltip);
      this.row.ColorCells();
    }

    public void OnPointerExit(PointerEventData data) {
      if(this._element != null && this._element.datum.isHeader && this.table.headerActiveCallback != null)
        if(!this.table.headerActiveCallback(this.column))
          return;
      this.table.SetPointerOverCell(null);
      this.row.ColorCells();
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (this._element != null && this._element.datum.isHeader && this.table.headerActiveCallback != null)
        {
            if (!this.table.headerActiveCallback(this.column))
            {
                return;
            }
        }
        if (this.table.pointerDownHandler != null)
        {
            this.table.pointerDownHandler(data, this.element.datum);
        }
        this.isDown = true;
        this.row.isDown = true;
        this.row.ColorCells();
        Invoke("TriggerLongPressEvent", longPressWait);
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (this._element != null && this._element.datum.isHeader && this.table.headerActiveCallback != null)
        {
            if (!this.table.headerActiveCallback(this.column))
            {
                return;
            }
        }
        if (this.table.pointerUpHandler != null)
        {
            this.table.pointerUpHandler(data, this.element.datum);
        }
        CancelInvoke("TriggerLongPressEvent");
        this.isDown = false;
        this.row.isDown = false;
        this.row.ColorCells();
    }

    private float longPressWait = 1f;
    private void TriggerLongPressEvent()
    {
        if (this.table.onCellLongPress != null && this.element != null && !this.element.datum.isHeader && !this.element.datum.isFooter)
        {
            this.table.onCellLongPress(element, text != null ? text.text : "");
        }
    }

    public void OnPointerClick(PointerEventData data) {
      if(this._element != null && this._element.datum.isHeader && this.table.headerActiveCallback != null)
        if(!this.table.headerActiveCallback(this.column))
          return;
      this.HandleClick(data);
    }

  }
}