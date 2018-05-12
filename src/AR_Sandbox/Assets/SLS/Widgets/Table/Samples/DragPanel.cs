using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


namespace SLS.Widgets.Table {
  public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler {

    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private RectTransform panelRectTransform;
    private RectTransform parentRectTransform;

    void Start() {
      panelRectTransform = transform.parent as RectTransform;
      parentRectTransform = panelRectTransform.parent as RectTransform;
    }

    public void OnPointerDown(PointerEventData data) {
      originalPanelLocalPosition = panelRectTransform.anchoredPosition;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera,
                                                              out originalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData data) {
      if(panelRectTransform == null || parentRectTransform == null)
        return;

      Vector2 localPointerPosition;
      if(RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera,
                                                                 out localPointerPosition)) {
        Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
        panelRectTransform.anchoredPosition = originalPanelLocalPosition + offsetToOriginal;
      }

      ClampToWindow();
    }

    // Clamp panel to area of parent
    void ClampToWindow() {
      Vector3 pos = panelRectTransform.anchoredPosition;

      Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
      Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

      pos.x = Mathf.Clamp(panelRectTransform.anchoredPosition.x, minPosition.x, maxPosition.x);
      pos.y = Mathf.Clamp(panelRectTransform.anchoredPosition.y, minPosition.y, maxPosition.y);

      panelRectTransform.anchoredPosition = pos;
    }
  }
}