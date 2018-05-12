using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TMP_PRESENT
using TMPro;
#endif
using SLS.Widgets.Table;

namespace SLS.Widgets.Table {
  public static class MakeDefaults {
    public static void Set() {
      Defaults d = Defaults.Instance;
    #if TMP_PRESENT
      d.font = Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
      d.fontStyle = FontStyles.Bold;
    #else
      d.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
      d.fontStyle = FontStyle.Bold;
    #endif
    }
  }
}