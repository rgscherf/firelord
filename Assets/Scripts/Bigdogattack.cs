using UnityEngine;

public class Bigdogattack : MonoBehaviour {

    Transform dogtransform;

	void Update () {
        if (dogtransform != null) {
            Vector2 dpos = dogtransform.position;
            transform.position = new Vector3(dpos.x, dpos.y, 1);
        }
	}

    public void SetTransform(Transform dog) {
        dogtransform = dog;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "MovingEntity") {
            var hc = other.gameObject.GetComponent<HealthController>();
            if (hc != null) {
                hc.ReceiveDamage(1);
            }
        }
    }
}
