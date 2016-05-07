using UnityEngine;
using System.Collections;

public class RollOverlayController : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "MovingEntity") {
            HealthController hc = other.GetComponent<HealthController>();
            hc.ReceiveDamage(1);
        }
        if (other.gameObject.tag == "Mist") {
            MistController mc = other.gameObject.GetComponent<MistController>();
            if (mc != null) {
                mc.ClearMist(new Vector2(0,0));
            }
        }
    }
}
