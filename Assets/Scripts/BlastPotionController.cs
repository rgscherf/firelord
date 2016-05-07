using UnityEngine;
using System.Collections;

public class BlastPotionController : MonoBehaviour {

    int damage = 4;
    int forceamount = 500;

    float damageradius = 1f;
    float pushradius = 2f;

    bool isTweening = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        isTweening = LeanTween.isTweening(gameObject);
        if(!isTweening) {
            Explode();
        }
	}

    public void Init(Vector2 end) {
        GetComponent<SpriteRenderer>().color = PotionColors.Blast;
        LeanTween.move(gameObject, end, Vector2.Distance(transform.position, end) * 0.03f);
    }

    void Explode() {
        Debug.Log("I exploded@@");
        var damagedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, damageradius) ;
        foreach (var d in damagedUnits) {
            GameObject go = d.gameObject;
            if (go.tag == "MovingEntity") {
                HealthController hc = go.GetComponent<HealthController>();
                if (hc != null) {
                    hc.ReceiveDamage(damage);
                }
            }
        }

        var pushedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, pushradius);
        foreach (var p in pushedUnits) {
            GameObject go = p.gameObject;
            if (go.tag == "MovingEntity") {
                // Vector2 dir = -1 * (gameObject.transform.position - go.transform.position);
                // go.GetComponent<Rigidbody2D>().AddForce(dir * forceamount);
                go.GetComponent<Rigidbody2D>().AddForce(getOutwardExplosionVector(gameObject.transform.position, go.transform.position, forceamount));
            }

            MistController mi = go.GetComponent<MistController>();
            if (mi != null) {
                mi.ClearMist( getOutwardExplosionVector(gameObject.transform.position, go.transform.position, forceamount / 2) );
            }
        }
        Object.Destroy(gameObject);
    }

    private Vector2 getOutwardExplosionVector(Vector2 exploder, Vector2 explodee, float forceamount) {
        return -1 * forceamount * (exploder - explodee);
    }
}
