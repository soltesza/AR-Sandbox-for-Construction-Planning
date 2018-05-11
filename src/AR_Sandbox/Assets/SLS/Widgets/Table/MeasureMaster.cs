using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT
using TMPro;
#endif

namespace SLS.Widgets.Table {
  public class MeasureMaster : MonoBehaviour {

    public Table table;
    public Control control;
  #if TMP_PRESENT
    private TextMeshProUGUI text;
  #else
    private Text text;
  #endif

  #if TMP_PRESENT
    public void Initialize(Table table, TextMeshProUGUI text, Control control) {
  #else
    public void Initialize(Table table, Text text, Control control) {
  #endif
      this.table = table;
      this.text = text;
      this.control = control;
    }

    public float datumRevision;

    private float SumColumnWidths() {
      float wsum = 0;
      for(int i = 0; i < this.table.columns.Count; i++) {
        wsum += this.table.columns[i].safeWidth;
      }
      return wsum;
    }

    public void DoMeasure(Row r) {

      Datum d = r.datum;
      this.datumRevision = d.revision;
      d.ClearMeasure();
      for(int i = 0; i < d.elements.Count; i++) {
        this.MeasureCell(d, d.elements[i], this.table.columns[i]);
      }
      this.MeasureCell(d, d.extraText, this.table.extraTextColumn);

      d.isDirty = false;

      if(!d.isHeader && !d.isFooter && (this.table.min100PercentWidth || this.table.max100PercentWidth)) {

        float bodyw = this.table.root.rect.width -
                      this.table.leftMargin - this.table.rightMargin -
                      ((this.table.columns.Count + 1) *
                       this.table.horizontalSpacing);

        float wsum = this.SumColumnWidths();


        bool hasNullMaxWidth = false;

        for(int i = 0; i < this.table.columns.Count; i++) {
          Column c = this.table.columns[i];
          if(!c.measuredMaxWidth.HasValue) {
            hasNullMaxWidth = true;
            break;
          }
        }

        // we are too wide... set us so we try to fit
        // (+1 here cause sometimes wierd rounding on prev loop)
        if(this.table.max100PercentWidth && (hasNullMaxWidth || wsum > bodyw + 1f)) {

          int tries = 0;
          bool hadFudge = true;
          int nonImageCols = 0;

          for(int i = 0; i < this.table.columns.Count; i++) {
            Column c = this.table.columns[i];
            // clear our previously set min (if any)
            c.measuredMinWidth = null;
            if(c.columnType != Column.ColumnType.IMAGE)
              nonImageCols += 1;
          }

          while(hadFudge && tries < 100) {

            //Debug.Log("COL RESIZE: " + tries);

            tries += 1;

            // yeah, i get it... this will be run for the first
            //  iteration and it doesn't need to be.  want to fight about it?
            wsum = this.SumColumnWidths();
            hadFudge = false;

            for(int i = 0; i < this.table.columns.Count; i++) {
              Column c = this.table.columns[i];

              if(nonImageCols == 0 || c.columnType != Column.ColumnType.IMAGE) {
                float usew = (c.safeWidth / wsum) * bodyw;
                if(usew < c.minWidth) {
                  c.measuredMaxWidth = c.minWidth;
                  r.foundMax = true;
                  hadFudge = true;
                  break;
                }
                else {
                  c.measuredMaxWidth = usew;
                  r.foundMax = true;
                }
              }
              else {
                c.measuredMaxWidth = c.safeWidth;
                r.foundMax = true;
              }

            }
          } // while fudge

          return;

        } // if max100

        // we are too narrow... set us so we try to fit
        // (-1 here cause sometimes wierd rounding on prev loop)
        if(this.table.min100PercentWidth && wsum < bodyw - 1f) {

          //Debug.Log(d.uid + " DOING MIN!: " + wsum + " vs" + bodyw);

          int nonImageCols = 0;
          float minSum = 0;

          for(int i = 0; i < this.table.columns.Count; i++) {
            Column c = this.table.columns[i];
            // clear our previously set max (if any)
            c.measuredMaxWidth = null;
            if(c.minWidth > 0f)
              minSum += Mathf.Max(c.minWidth);
            if(c.columnType != Column.ColumnType.IMAGE)
              nonImageCols += 1;
          }

          for(int i = 0; i < this.table.columns.Count; i++) {
            Column c = this.table.columns[i];
            float usew = 0;
            if(nonImageCols > 0 && c.columnType == Column.ColumnType.IMAGE)
              usew = c.minWidth;
            else if(minSum > 0)
              usew = (c.minWidth / minSum) * (bodyw - wsum);
            else {
              if(nonImageCols > 0)
                usew = (1f / nonImageCols) * (bodyw - wsum);
              else
                usew = (1f / this.table.columns.Count) * (bodyw - wsum);
            }

            c.measuredMinWidth = c.rawWidth + usew;
            //Debug.Log(d.uid + " Seting measured MW!: " + c.measuredMinWidth.Value + ": " + c.idx);
          }

        } // if min100

      } // if !isHeader and !isFooter

    } // doMeasure

    public void MeasureCell(Datum d, Element e, Column c) {

      if(c.columnType == Column.ColumnType.TEXT || d.isHeader || d.isFooter) {
        if(e == null) {
          this.MeasureCellDone(c, d, e);
          return;
        }
        if(string.IsNullOrEmpty(e.value)) {
          e.measuredWidth = 0;
          e.measuredHeight = 0;
          this.MeasureCellDone(c, d, e);
          return;
        }

      #if !TMP_PRESENT
        TextGenerationSettings settings;
        float pixelsPerUnit;
      #endif

        float useW;
        float mw;
        float mh;
        Vector2 size;
        this.text.fontSize = c.CalcFont(d);

        this.text.text = e.value;

        if(c.measuredMaxWidth.HasValue) {
        #if TMP_PRESENT
          this.text.enableWordWrapping = true;
        #else
          this.text.horizontalOverflow = HorizontalWrapMode.Wrap;
        #endif
          size = new Vector2(c.measuredMaxWidth.Value, 0);
        }
        else {
        #if TMP_PRESENT
          this.text.enableWordWrapping = false;
          size = new Vector2(this.table.root.rect.width, 0);
        #else
          this.text.horizontalOverflow = HorizontalWrapMode.Overflow;
          size = Vector2.zero;
        #endif
        }

      #if TMP_PRESENT
        this.text.rectTransform.offsetMin = new Vector2(size.x * -0.5f, 0);
        this.text.rectTransform.offsetMax = new Vector2(size.x * 0.5f, 0);
        this.text.ForceMeshUpdate();
        mw = this.text.preferredWidth;
        mh = this.text.preferredHeight;
      #else
        settings = this.text.GetGenerationSettings(size);
        pixelsPerUnit = this.text.pixelsPerUnit;
        mw = this.text.cachedTextGeneratorForLayout.GetPreferredWidth
               (e.value, settings) / pixelsPerUnit;
        mh = this.text.cachedTextGeneratorForLayout.GetPreferredHeight
               (e.value, settings) / pixelsPerUnit;
      #endif

        //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

        useW = c.maxWidth;
        if(c.measuredMaxWidth.HasValue &&
           (c.measuredMaxWidth.Value < useW || useW <= 0))
          useW = c.measuredMaxWidth.Value;

        if(useW > 0 && mw > useW) {

          //Debug.Log("Doing Max Width on " + c.idx + " Row: " + d.uid + " Width: " + useW);
        #if TMP_PRESENT
          this.text.enableWordWrapping = true;
        #else
          this.text.horizontalOverflow = HorizontalWrapMode.Wrap;
        #endif

          size = new Vector2(useW, 0);

        #if TMP_PRESENT
          this.text.rectTransform.offsetMin = new Vector2(size.x * -0.5f, 0);
          this.text.rectTransform.offsetMax = new Vector2(size.x * 0.5f, 0);
          this.text.ForceMeshUpdate();
          mw = this.text.preferredWidth;
          mh = this.text.preferredHeight;
        #else
          settings = this.text.GetGenerationSettings(size);
          pixelsPerUnit = this.text.pixelsPerUnit;
          mw = this.text.cachedTextGeneratorForLayout.GetPreferredWidth
                 (e.value, settings) / pixelsPerUnit;
          mh = this.text.cachedTextGeneratorForLayout.GetPreferredHeight
                 (e.value, settings) / pixelsPerUnit;
        #endif
        }

        //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

        mw = Mathf.Max(c.minWidth, mw);

        //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

        if(c.measuredMinWidth.HasValue && c.measuredMinWidth.Value > mw)
          mw = c.measuredMinWidth.Value;

        //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

        e.measuredWidth = mw;
        e.measuredHeight = mh;
        this.MeasureCellDone(c, d, e);

      } // if TEXT
      else {
        e.measuredWidth = c.imageWidth;
        e.measuredHeight = c.imageHeight;
        this.MeasureCellDone(c, d, e);
      }
    }

    private void MeasureCellDone(Column c, Datum d, Element e) {
      if(c != null && d != null && e != null)
        //if (c.idx == 0) Debug.Log("DONE MEASURE: " + d.uid + ": " + c.idx + ": " + e.value + " h: " + e.measuredHeight + " w: " + e.measuredWidth + " fzise: " + this.text.fontSize + " PPU: " + this.text.pixelsPerUnit);
        if(e != null && c != null &&
           (d != null && d.revision == this.datumRevision)) {
          if(d != null && d.isHeader && d.table.hasHeaderIcons)
            c.CheckWidth(e.measuredWidth.Value +
                         d.table.rowVerticalSpacing * 0.5f +
                         d.table.headerIconWidth);
          else
            c.CheckWidth(e.measuredWidth.Value);
        }
    } // measureCallback

  } // MeasureMaster
}