using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if TMP_PRESENT
using TMPro;
#endif

namespace SLS.Widgets.Table {
  public class InputCell : MonoBehaviour {

    public Table table { private set; get; }

  #if TMP_PRESENT
    public TMP_InputField inputField { private set; get; }
  #else
    public InputField inputField { private set; get; }
  #endif

    public RectTransform rectTransform { private set; get; }

    private bool isTryingDisable;

  #if TMP_PRESENT
    public bool Initialize(Table table, RectTransform rt, TMP_InputField inputField) {
  #else
    public bool Initialize(Table table, RectTransform rt, InputField inputField) {
  #endif
      this.table = table;
      this.rectTransform = rt;
      this.inputField = inputField;
      return true;
    }

    private Cell cell;

    void Start() {
      if(this.inputField != null) {
        this.inputField.onEndEdit.AddListener(this.OnEndEdit);
      }
    }

    void OnEnable() {
      if(this.inputField != null) {
        this.inputField.onEndEdit.AddListener(this.OnEndEdit);
      }
    }

    void OnDisable() {
      // handle case where table is repopulated during current frame before we
      //    fully deactivate our input cell object
      if (this.isTryingDisable) {
        this.gameObject.SetActive(false);
        this.isTryingDisable = false;
      }
      if(this.inputField != null) {
        this.inputField.onEndEdit.RemoveListener(this.OnEndEdit);
      }
    }

    private void OnEndEdit(string s) {
      this.RemoveFocus();
    }

    public void SetFocus(Cell c) {

      this.gameObject.SetActive(true);
      this.isTryingDisable = false;
      this.RemoveFocus(true);

      this.cell = c;

      this.rectTransform.SetParent(this.cell.transform.parent);
      this.rectTransform.pivot = this.cell.rt.pivot;
      this.rectTransform.anchorMin = this.cell.rt.anchorMin;
      this.rectTransform.anchorMax = this.cell.rt.anchorMax;
      this.rectTransform.offsetMin = this.cell.rt.offsetMin;
      this.rectTransform.offsetMax = this.cell.rt.offsetMax;
      this.inputField.image.color = this.table.cellSelectColor;
      this.inputField.textComponent.color = this.table.rowTextColor;
      this.inputField.text = this.cell.text.text;
      this.inputField.textComponent.font = this.cell.text.font;
      this.inputField.textComponent.fontStyle = this.cell.text.fontStyle;
      this.inputField.textComponent.fontSize = this.cell.text.fontSize;

    #if !TMP_PRESENT
      this.inputField.textComponent.rectTransform.anchoredPosition = new Vector2(3, -3);
      this.inputField.textComponent.rectTransform.sizeDelta =
        new Vector2(this.table.inputCell.rectTransform.sizeDelta.x - 6,
                    this.table.inputCell.rectTransform.sizeDelta.y - 6);
    #endif

      this.cell.text.gameObject.SetActive(false);
      this.inputField.interactable = true;

    #if TMP_PRESENT
      this.StartCoroutine(this.SelectLater());
    #else
      this.inputField.MoveTextStart(false);
      this.inputField.MoveTextEnd(true);
      this.inputField.Select();
    #endif
    }

    public void RemoveFocus(bool withRefocus=false) {
      if(this.cell == null)
        return;
      if(this.cell.element.value != this.inputField.text) {
        this.cell.column.inputChangeCallback(this.cell.row.datum, this.cell.column,
                                             this.cell.element.value, this.inputField.text);
        this.cell.element.value = this.inputField.text;
      }
      this.cell.text.gameObject.SetActive(true);
      if(!withRefocus && this.gameObject.activeInHierarchy) {
        this.isTryingDisable = true;
        StartCoroutine(this.DeactivateLater());
      }
      this.cell = null;
    }

    IEnumerator SelectLater() {
      yield return Table.WaitForEndOfFrame;
      this.inputField.MoveTextStart(false);
      this.inputField.MoveTextEnd(true);
      this.inputField.Select();
    }

    IEnumerator DeactivateLater() {
      yield return Table.WaitForEndOfFrame;
      this.gameObject.SetActive(false);
    }

  }
}