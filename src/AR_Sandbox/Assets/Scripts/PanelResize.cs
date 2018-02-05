using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelResize : MonoBehaviour {
    private Vector2 initialMousePos, initialDeltaSize;

    public RectTransform rectTransform;

    void Start() {
        if (!rectTransform)
            Debug.LogError("PanelResize: no RectTransform selected!");
    }

    public void BeginDrag() {
        initialMousePos = Input.mousePosition;
        initialDeltaSize = rectTransform.sizeDelta;
    }

    public void OnDrag() {
        Vector2 mouseDelta = (Vector2)initialMousePos - (Vector2)Input.mousePosition;
        mouseDelta = new Vector2(-mouseDelta.x, mouseDelta.y);
        mouseDelta += initialDeltaSize;
        rectTransform.sizeDelta = mouseDelta;
    }
}
