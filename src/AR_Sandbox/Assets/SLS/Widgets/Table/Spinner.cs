using UnityEngine;

namespace SLS.Widgets.Table {
  public class Spinner : MonoBehaviour {
    public float speed = 80f;
    void Update() {
      transform.Rotate(Vector3.back, speed * Time.smoothDeltaTime);
    }
  }
}