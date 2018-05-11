using UnityEngine;
using UnityEditor;
#if TMP_PRESENT
using TMPro;
#endif

namespace SLS.Widgets.Table {
  [CustomEditor (typeof(Table))]
  public class TableEditor : Editor {

    protected static bool showGeneral = false;
    protected static bool showHeader = false;
    protected static bool showFooter = false;
    protected static bool showRow = false;
    protected static bool showExtraText = false;
    protected static bool showScrollbar = false;
    SerializedProperty font;
    SerializedProperty fontStyle;
    SerializedProperty use2DMask;
    SerializedProperty fillerSprite;

    // GENERAL SETTINGS
    SerializedProperty defaultFontSize;
    SerializedProperty scrollSensitivity;
    SerializedProperty leftMargin;
    SerializedProperty rightMargin;
    SerializedProperty horizontalSpacing;
    SerializedProperty bodyBackgroundColor;
    SerializedProperty columnLineColor;
    SerializedProperty columnLineWidth;
    SerializedProperty min100PercentWidth;
    SerializedProperty max100PercentWidth;
    SerializedProperty spinnerSprite;
    SerializedProperty spinnerColor;
    SerializedProperty rowAnimationDuration;
    SerializedProperty selectionMode;
    SerializedProperty multiSelectKey;
    SerializedProperty showHoverColors;
    SerializedProperty drawGizmos;
    SerializedProperty gizmoColor;

    // HEADER SETTINGS
    SerializedProperty minHeaderHeight;
    SerializedProperty headerTopMargin;
    SerializedProperty headerBottomMargin;
    SerializedProperty headerNormalColor;
    SerializedProperty headerHoverColor;
    SerializedProperty headerDownColor;
    SerializedProperty headerBorderColor;
    SerializedProperty headerTextColor;
    SerializedProperty headerIconWidth;
    SerializedProperty headerIconHeight;

    // FOOTER SETTINGS
    SerializedProperty minFooterHeight;
    SerializedProperty footerTopMargin;
    SerializedProperty footerBottomMargin;
    SerializedProperty footerBackgroundColor;
    SerializedProperty footerBorderColor;
    SerializedProperty footerTextColor;

    // ROW SETTINGS
    SerializedProperty minRowHeight;
    SerializedProperty rowVerticalSpacing;
    SerializedProperty rowLineColor;
    SerializedProperty rowLineHeight;
    SerializedProperty rowNormalColor;
    SerializedProperty rowAltColor;
    SerializedProperty rowHoverColor;
    SerializedProperty rowDownColor;
    SerializedProperty rowSelectColor;
    SerializedProperty rowTextColor;
    SerializedProperty cellHoverColor;
    SerializedProperty cellDownColor;
    SerializedProperty cellSelectColor;

    // EXTRA TEXT
    SerializedProperty extraTextWidthRatio;
    SerializedProperty extraTextBoxColor;
    SerializedProperty extraTextColor;

    // SCROLLBAR
    SerializedProperty scrollBarSize;
    SerializedProperty scrollBarBackround;
    SerializedProperty scrollBarForeground;

    virtual protected void OnEnable() {

      font = serializedObject.FindProperty
               ("font");
      fontStyle = serializedObject.FindProperty
                    ("fontStyle");
      use2DMask = serializedObject.FindProperty
                    ("use2DMask");
      fillerSprite = serializedObject.FindProperty
                       ("fillerSprite");

      // GENERAL SETTINGS
      defaultFontSize = serializedObject.FindProperty
                          ("defaultFontSize");
      scrollSensitivity = serializedObject.FindProperty
                            ("scrollSensitivity");
      leftMargin = serializedObject.FindProperty
                     ("leftMargin");
      rightMargin = serializedObject.FindProperty
                      ("rightMargin");
      horizontalSpacing = serializedObject.FindProperty
                            ("horizontalSpacing");
      bodyBackgroundColor = serializedObject.FindProperty
                              ("bodyBackgroundColor");
      columnLineColor = serializedObject.FindProperty
                          ("columnLineColor");
      columnLineWidth = serializedObject.FindProperty
                          ("columnLineWidth");
      min100PercentWidth = serializedObject.FindProperty
                             ("min100PercentWidth");
      max100PercentWidth = serializedObject.FindProperty
                             ("max100PercentWidth");
      spinnerSprite = serializedObject.FindProperty
                        ("spinnerSprite");
      spinnerColor = serializedObject.FindProperty
                       ("spinnerColor");
      rowAnimationDuration = serializedObject.FindProperty
                               ("rowAnimationDuration");
      selectionMode = serializedObject.FindProperty
                        ("selectionMode");
      multiSelectKey = serializedObject.FindProperty
                        ("multiSelectKey");
      showHoverColors = serializedObject.FindProperty
                          ("showHoverColors");
      drawGizmos = serializedObject.FindProperty
                     ("drawGizmos");
      gizmoColor = serializedObject.FindProperty
                     ("gizmoColor");
      // HEADER SETTINGS
      minHeaderHeight = serializedObject.FindProperty
                          ("minHeaderHeight");
      headerTopMargin = serializedObject.FindProperty
                          ("headerTopMargin");
      headerBottomMargin = serializedObject.FindProperty
                             ("headerBottomMargin");
      headerNormalColor = serializedObject.FindProperty
                            ("headerNormalColor");
      headerHoverColor = serializedObject.FindProperty
                           ("headerHoverColor");
      headerDownColor = serializedObject.FindProperty
                          ("headerDownColor");
      headerBorderColor = serializedObject.FindProperty
                            ("headerBorderColor");
      headerTextColor = serializedObject.FindProperty
                          ("headerTextColor");
      headerIconHeight = serializedObject.FindProperty
                           ("headerIconHeight");
      headerIconWidth = serializedObject.FindProperty
                          ("headerIconWidth");
      // FOOTER SETTINGS
      minFooterHeight = serializedObject.FindProperty
                          ("minFooterHeight");
      footerTopMargin = serializedObject.FindProperty
                          ("footerTopMargin");
      footerBottomMargin = serializedObject.FindProperty
                             ("footerBottomMargin");
      footerBackgroundColor = serializedObject.FindProperty
                                ("footerBackgroundColor");
      footerBorderColor = serializedObject.FindProperty
                            ("footerBorderColor");
      footerTextColor = serializedObject.FindProperty
                          ("footerTextColor");
      // ROW SETTINGS
      minRowHeight = serializedObject.FindProperty
                       ("minRowHeight");
      rowVerticalSpacing = serializedObject.FindProperty
                             ("rowVerticalSpacing");
      rowLineColor = serializedObject.FindProperty
                       ("rowLineColor");
      rowLineHeight = serializedObject.FindProperty
                        ("rowLineHeight");
      rowNormalColor = serializedObject.FindProperty
                         ("rowNormalColor");
      rowAltColor = serializedObject.FindProperty
                      ("rowAltColor");
      rowHoverColor = serializedObject.FindProperty
                        ("rowHoverColor");
      rowDownColor = serializedObject.FindProperty
                       ("rowDownColor");
      rowSelectColor = serializedObject.FindProperty
                         ("rowSelectColor");
      rowTextColor = serializedObject.FindProperty
                       ("rowTextColor");
      cellHoverColor = serializedObject.FindProperty
                         ("cellHoverColor");
      cellDownColor = serializedObject.FindProperty
                        ("cellDownColor");
      cellSelectColor = serializedObject.FindProperty
                          ("cellSelectColor");
      // EXTRA TEXT
      extraTextWidthRatio = serializedObject.FindProperty
                              ("extraTextWidthRatio");
      extraTextBoxColor = serializedObject.FindProperty
                            ("extraTextBoxColor");
      extraTextColor = serializedObject.FindProperty
                         ("extraTextColor");
      // SCROLLBAR
      scrollBarSize = serializedObject.FindProperty
                        ("scrollBarSize");
      scrollBarBackround = serializedObject.FindProperty
                             ("scrollBarBackround");
      scrollBarForeground = serializedObject.FindProperty
                              ("scrollBarForeground");
    }

    public override void OnInspectorGUI() {

      if(Application.isPlaying) {
        EditorGUILayout.HelpBox("Edit disabled in Play mode.",
                                MessageType.Info, true);
        return;
      }

      serializedObject.Update();

      //Table table = (Table)target;

      EditorGUILayout.Space();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Load Preset");
      if(GUILayout.Button("Light") &&
         EditorUtility.DisplayDialog("Load Light Preset?",
                                     "All current table settings will be overwritten!",
                                     "Continue", "Cancel")) {
        Debug.Log("Applying 'Light' Table Preset");
        this.applyLightTheme();
      }
      if(GUILayout.Button("Dark") &&
         EditorUtility.DisplayDialog("Load Dark Preset?",
                                     "All current table settings will be overwritten!",
                                     "Continue", "Cancel")) {
        Debug.Log("Applying 'Dark' Table Preset");
        this.applyDarkTheme();
      }
      GUILayout.EndHorizontal();

      EditorGUILayout.Space();

      EditorGUILayout.PropertyField
        (font, new GUIContent("Font"));
      EditorGUILayout.PropertyField
        (fontStyle, new GUIContent("Font Style"));
      EditorGUILayout.PropertyField
        (use2DMask, new GUIContent("Use 2D Mask"));
      EditorGUILayout.PropertyField
        (fillerSprite, new GUIContent("Filler Sprite"));

      EditorGUILayout.Space();

      showGeneral = EditorGUILayout.Foldout
                      (showGeneral, "General Settings");

      if(showGeneral) {
        EditorGUILayout.PropertyField
          (defaultFontSize, new GUIContent("Font Size"));
        EditorGUILayout.PropertyField
          (scrollSensitivity, new GUIContent("Scroll Sensitivity"));
        EditorGUILayout.PropertyField
          (leftMargin, new GUIContent("Left Margin"));
        EditorGUILayout.PropertyField
          (rightMargin, new GUIContent("Right Margin"));
        EditorGUILayout.PropertyField
          (horizontalSpacing, new GUIContent("Horizontal Spacing"));
        EditorGUILayout.PropertyField
          (bodyBackgroundColor, new GUIContent("Background"));
        EditorGUILayout.PropertyField
          (columnLineColor, new GUIContent("Column Line Color"));
        EditorGUILayout.PropertyField
          (columnLineWidth, new GUIContent("Column Line Width"));
        EditorGUILayout.PropertyField
          (min100PercentWidth, new GUIContent("Force 100% Width Min"));
        EditorGUILayout.PropertyField
          (max100PercentWidth, new GUIContent("Restrict 100% Width Max"));
        EditorGUILayout.PropertyField
          (spinnerSprite, new GUIContent("Spinner Sprite"));
        EditorGUILayout.PropertyField
          (spinnerColor, new GUIContent("Spinner Sprite Color"));
        EditorGUILayout.PropertyField
          (rowAnimationDuration, new GUIContent("Row Animation Duration"));
        EditorGUILayout.PropertyField
          (selectionMode, new GUIContent("Table Selection UI Mode"));
        if (selectionMode.enumValueIndex == 2 ||
            selectionMode.enumValueIndex == 3)
          EditorGUILayout.PropertyField
            (multiSelectKey, new GUIContent("Multi Select Key"));
        EditorGUILayout.PropertyField
          (showHoverColors, new GUIContent("Show Hover Colors"));
        EditorGUILayout.PropertyField
          (drawGizmos, new GUIContent("Draw Editor Gizmo"));
        EditorGUILayout.PropertyField
          (gizmoColor, new GUIContent("Gizmo Color"));
      }

      showHeader = EditorGUILayout.Foldout
                     (showHeader, "Header Settings");

      if(showHeader) {
        EditorGUILayout.PropertyField
          (minHeaderHeight, new GUIContent("Minimum Height"));
        EditorGUILayout.PropertyField
          (headerTopMargin, new GUIContent("Top Margin"));
        EditorGUILayout.PropertyField
          (headerBottomMargin, new GUIContent("Bottom Margin"));
        EditorGUILayout.PropertyField
          (headerNormalColor, new GUIContent("Normal Background"));
        EditorGUILayout.PropertyField
          (headerHoverColor, new GUIContent("Hover Background"));
        EditorGUILayout.PropertyField
          (headerDownColor, new GUIContent("Down Background"));
        EditorGUILayout.PropertyField
          (headerBorderColor, new GUIContent("Border Line"));
        EditorGUILayout.PropertyField
          (headerTextColor, new GUIContent("Text"));
        EditorGUILayout.PropertyField
          (headerIconHeight, new GUIContent("Icon Height"));
        EditorGUILayout.PropertyField
          (headerIconWidth, new GUIContent("Icon Width"));
      }

      showFooter = EditorGUILayout.Foldout
                     (showFooter, "Footer Settings");

      if(showFooter) {
        EditorGUILayout.PropertyField
          (minFooterHeight, new GUIContent("Minimum Height"));
        EditorGUILayout.PropertyField
          (footerTopMargin, new GUIContent("Top Margin"));
        EditorGUILayout.PropertyField
          (footerBottomMargin, new GUIContent("Bottom Margin"));
        EditorGUILayout.PropertyField
          (footerBackgroundColor, new GUIContent("Background"));
        EditorGUILayout.PropertyField
          (footerBorderColor, new GUIContent("Border Line"));
        EditorGUILayout.PropertyField
          (footerTextColor, new GUIContent("Text"));
      }

      showRow = EditorGUILayout.Foldout
                  (showRow, "Data Row Settings");

      if(showRow) {
        EditorGUILayout.PropertyField
          (minRowHeight, new GUIContent("Minimum Height"));
        EditorGUILayout.PropertyField
          (rowVerticalSpacing, new GUIContent("Vertical Spacing"));
        EditorGUILayout.PropertyField
          (rowLineColor, new GUIContent("Line Color"));
        EditorGUILayout.PropertyField
          (rowLineHeight, new GUIContent("Line Height"));
        EditorGUILayout.PropertyField
          (rowNormalColor, new GUIContent("Normal Background"));
        EditorGUILayout.PropertyField
          (rowAltColor, new GUIContent("Alt Normal Background"));
        EditorGUILayout.PropertyField
          (rowHoverColor, new GUIContent("Hover Background"));
        EditorGUILayout.PropertyField
          (rowDownColor, new GUIContent("Down Background"));
        EditorGUILayout.PropertyField
          (rowSelectColor, new GUIContent("Select Background"));
        EditorGUILayout.PropertyField
          (rowTextColor, new GUIContent("Text"));
        EditorGUILayout.PropertyField
          (cellHoverColor, new GUIContent("Cell Hover Background"));
        EditorGUILayout.PropertyField
          (cellDownColor, new GUIContent("Cell Down Background"));
        EditorGUILayout.PropertyField
          (cellSelectColor, new GUIContent("Cell Select Background"));
      }

      showExtraText = EditorGUILayout.Foldout
                        (showExtraText, "Extra Text Settings");

      if(showExtraText) {
        EditorGUILayout.HelpBox("Datum objects contains an optional" +
                                " extraText attribute.  If assigned, this data will display" +
                                " outside the normal column layout.", MessageType.Info, true);
        EditorGUILayout.HelpBox("The 'Width Ratio' should be a value" +
                                " between 0 and 1 and corresponds to the normalized percent" +
                                " width we should consume.", MessageType.Info, true);
        EditorGUILayout.PropertyField
          (extraTextWidthRatio, new GUIContent("Width Ratio"));
        EditorGUILayout.PropertyField
          (extraTextBoxColor, new GUIContent("Background"));
        EditorGUILayout.PropertyField
          (extraTextColor, new GUIContent("Text"));
      }

      showScrollbar = EditorGUILayout.Foldout
                        (showScrollbar, "Scrollbar Settings");

      if(showScrollbar) {
        EditorGUILayout.PropertyField
          (scrollBarSize, new GUIContent("Bar Size"));
        EditorGUILayout.PropertyField
          (scrollBarForeground, new GUIContent("Foreground"));
        EditorGUILayout.PropertyField
          (scrollBarBackround, new GUIContent("Background"));
      }

/*
    table.defaultFontSize =
      EditorGUILayout.IntField("Default Font Size",
        table.defaultFontSize);
    EditorGUILayout.LabelField("Something", table.font.ToString());


    GUILayout.BeginHorizontal();
    GUILayout.Label("Scroll Sensitivity");
    table.scrollSensitivity =
      EditorGUILayout.IntField
        (table.scrollSensitivity, GUILayout.Width(50));
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Label("Text Color");
    table.rowTextColor =
      EditorGUILayout.ColorField
        (table.rowTextColor, GUILayout.Width(50));
    GUILayout.EndHorizontal();

    if (Application.isPlaying) {
      if (GUILayout.Button("Apply to Running Table")) {
        Debug.Log("something");
      }
    }
 */

      serializedObject.ApplyModifiedProperties();

    } // OnInspectorGUI


    private void applyLightTheme() {

      Table table = (Table)target;

    #if TMP_PRESENT
      table.fontStyle = FontStyles.Bold;
    #else
      table.fontStyle = FontStyle.Bold;
    #endif

      // GENERAL SETTINGS
      table.defaultFontSize = 12;
      table.scrollSensitivity = 5;
      table.leftMargin = 8;
      table.rightMargin = 8;
      table.horizontalSpacing = 8;
      table.bodyBackgroundColor = new Color(1, 1, 1, 1);
      table.columnLineColor = new Color(0, 0, 0, .5f);
      table.columnLineWidth = 2;
      table.spinnerColor = new Color(0.8f, 0.8f, 0.8f, 1f);
      table.min100PercentWidth = true;
      table.max100PercentWidth = false;
      table.rowAnimationDuration = 0.5f;
      table.selectionMode = Table.SelectionMode.CELL;
      table.multiSelectKey = Table.MultiSelectKey.SHIFT;
      table.showHoverColors = true;
      table.drawGizmos = false;
      table.gizmoColor = new Color(0.4f, 0.4f, 0f, 0.6f);

      // HEADER SETTINGS
      table.minHeaderHeight = 30;
      table.headerTopMargin = 10;
      table.headerBottomMargin = 10;
      table.headerNormalColor = new Color(.8f, .8f, .8f, 1f);
      table.headerHoverColor = new Color(.9f, .9f, .9f, 1f);
      table.headerDownColor = new Color(.8f, .8f, .8f, 1f);
      table.headerBorderColor = new Color(0, 0, 0, .5f);
      table.headerTextColor = new Color(0, 0, 0, 1f);
      table.headerIconHeight = 16;
      table.headerIconWidth = 8;

      // FOOTER SETTINGS
      table.minFooterHeight = 20;
      table.footerTopMargin = 8;
      table.footerBottomMargin = 8;
      table.footerBackgroundColor = new Color(.9f, .9f, .9f, .9f);
      table.footerBorderColor = new Color(0, 0, 0, .5f);
      table.footerTextColor = new Color(0, 0, 0, 1f);

      // ROW SETTINGS
      table.minRowHeight = 20;
      table.rowVerticalSpacing = 10;
      table.rowLineColor = new Color(0, 0, 0, .5f);
      table.rowLineHeight = 2;
      table.rowNormalColor = new Color(1, 1, 1, 1f);
      table.rowAltColor = new Color(0.90f, 0.90f, 0.90f, 1f);
      table.rowHoverColor = new Color(.8f, .8f, .8f, 1f);
      table.rowDownColor = new Color(1f, 1f, 1f, 1f);
      table.rowSelectColor = new Color(.7f, .7f, .7f, 1f);
      table.rowTextColor = new Color(0, 0, 0, 1f);
      table.cellHoverColor = new Color(.95f, .95f, .95f, 1f);
      table.cellDownColor = new Color(1f, 1f, 1f, 9f);
      table.cellSelectColor = new Color(.95f, .95f, .95f, 1f);

      // EXTRA TEXT
      table.extraTextWidthRatio = 0.9f;
      table.extraTextBoxColor = new Color(1, 1, 1, 1f);
      table.extraTextColor = new Color(0, 0, 0, 1f);

      // SCROLLBAR
      table.scrollBarSize = 10;
      table.scrollBarBackround = new Color(.5f, .5f, .5f, .1f);
      table.scrollBarForeground = new Color(.5f, .5f, .5f, .5f);

      serializedObject.Update();
    }

    private void applyDarkTheme() {

      Table table = (Table)target;

    #if TMP_PRESENT
      table.fontStyle = FontStyles.Bold;
    #else
      table.fontStyle = FontStyle.Bold;
    #endif

      // GENERAL SETTINGS
      table.defaultFontSize = 12;
      table.scrollSensitivity = 5;
      table.leftMargin = 8;
      table.rightMargin = 8;
      table.horizontalSpacing = 8;
      table.bodyBackgroundColor = new Color(0, 0, 0, 1);
      table.columnLineColor = new Color(1, 1, 1, .2f);
      table.columnLineWidth = 2;
      table.spinnerColor = new Color(0.15f, 0.15f, 0.15f, 1f);
      table.min100PercentWidth = true;
      table.max100PercentWidth = false;
      table.rowAnimationDuration = 0.5f;
      table.selectionMode = Table.SelectionMode.CELL;
      table.multiSelectKey = Table.MultiSelectKey.SHIFT;
      table.showHoverColors = true;
      table.drawGizmos = false;
      table.gizmoColor = new Color(0.4f, 0.4f, 0f, 0.6f);

      // HEADER SETTINGS
      table.minHeaderHeight = 30;
      table.headerTopMargin = 10;
      table.headerBottomMargin = 10;
      table.headerNormalColor = new Color(.15f, .15f, .15f, 1f);
      table.headerHoverColor = new Color(.25f, .25f, .25f, 1f);
      table.headerDownColor = new Color(.10f, .10f, .10f, 1f);
      table.headerBorderColor = new Color(1, 1, 1, .5f);
      table.headerTextColor = new Color(1, 1, 1, 1f);
      table.headerIconHeight = 16;
      table.headerIconWidth = 8;

      // FOOTER SETTINGS
      table.minFooterHeight = 20;
      table.footerTopMargin = 8;
      table.footerBottomMargin = 8;
      table.footerBackgroundColor = new Color(.15f, .15f, .15f, 1f);
      table.footerBorderColor = new Color(1, 1, 1, .5f);
      table.footerTextColor = new Color(1, 1, 1, 1f);

      // ROW SETTINGS
      table.minRowHeight = 20;
      table.rowVerticalSpacing = 10;
      table.rowLineColor = new Color(1, 1, 1, .2f);
      table.rowLineHeight = 2;
      table.rowNormalColor = new Color(0.05f, 0.05f, 0.05f, 1f);
      table.rowAltColor = new Color(0.1f, 0.1f, 0.1f, 1f);
      table.rowHoverColor = new Color(.2f, .2f, .2f, 1f);
      table.rowDownColor = new Color(.0f, .0f, .0f, 1f);
      table.rowSelectColor = new Color(.3f, .3f, .3f, 1f);
      table.rowTextColor = new Color(1, 1, 1, 1f);
      table.cellHoverColor = new Color(.3f, .3f, .3f, 1f);
      table.cellDownColor = new Color(.1f, .1f, .1f, 1f);
      table.cellSelectColor = new Color(.5f, .5f, .5f, 1f);

      // EXTRA TEXT
      table.extraTextWidthRatio = 0.9f;
      table.extraTextBoxColor = new Color(0, 0, 0, 1f);
      table.extraTextColor = new Color(1, 1, 1, 1f);

      // SCROLLBAR
      table.scrollBarSize = 10;
      table.scrollBarBackround = new Color(.5f, .5f, .5f, .1f);
      table.scrollBarForeground = new Color(.5f, .5f, .5f, .5f);

      serializedObject.Update();
    }

  }
}