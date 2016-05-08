using UnityEngine;
using System.Collections;

public class WizorbController : MonoBehaviour {

    void Start() {
        Destroy(gameObject, 6f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            HealthController hc = other.gameObject.GetComponent<HealthController>();
            if (hc != null) {
                hc.ReceiveDamage(1);
            }
        }
    }
}
