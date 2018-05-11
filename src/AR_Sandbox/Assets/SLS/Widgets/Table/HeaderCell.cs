using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace SLS.Widgets.Table {
  public class HeaderCell : Cell {

    public Image icon;
    private Action<Column> clickCallback;
    private Action<Column, PointerEventData> clickCallbackWithData;

    public void Initialize(Column column, Action<Column> clickCallback) {
      this.column = column;
      this.clickCallback = clickCallback;
      this.clickCallbackWithData = null;
    }

    public void Initialize(Column column, Action<Column, PointerEventData> clickCallbackWithData) {
      this.column = column;
      this.clickCallback = null;
      this.clickCallbackWithData = clickCallbackWithData;
    }

    public void UpdateDatum() {
      this.SetColor();
      if(this.table.hasHeaderIcons) {
        if(!string.IsNullOrEmpty(this.column.headerIcon) &&
           this.table.sprites.ContainsKey(this.column.headerIcon)) {
          this.icon.sprite = this.table.sprites[this.column.headerIcon];
          if(this.column.headerIconColor.HasValue)
            this.icon.color = this.column.headerIconColor.Value;
          else
            this.icon.color = Color.white;
        }
        else {
          this.icon.sprite = null;
          this.icon.color = Color.clear;
        }
      }
    }

    override public void HandleClick(PointerEventData data) {
      if(this.clickCallback != null)
        this.clickCallback(this.column);
      if(this.clickCallbackWithData != null)
        this.clickCallbackWithData(this.column, data);
    }

    override public void SetColor() {
      if(this.clickCallback == null && this.clickCallbackWithData == null) {
        this.background.color = this.table.headerNormalColor;
        return;
      }
      if(this.table.bodyScrollWatcher.isDragging) {
        this.background.color = this.table.headerNormalColor;
      }
      else if(this.table.IsPointerOver(this)) {
        if(this.isDown)
          this.background.color = this.table.headerDownColor;
        else
          this.background.color = this.table.headerHoverColor;
      }
      else
        this.background.color = this.table.headerNormalColor;
    }

  }

}