using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TMP_PRESENT
using TMPro;
#endif

namespace SLS.Widgets.Table {
  public class Defaults {

    protected static Defaults _instance = null;

    // Singleton pattern implementation
    public static Defaults Instance {
      get {
        if (Defaults._instance == null) {
          Defaults._instance = new Defaults();
        }
        return Defaults._instance;
      }
    }

  #if TMP_PRESENT
    public TMP_FontAsset font;
    public FontStyles fontStyle;
  #else
    public Font font;
    public FontStyle fontStyle;
  #endif

  }
}