using UnityEngine;

public class BlastPotionController : MonoBehaviour {

    const int damage = 4;
    const int forceamount = 400;
    const float damageradius = 1f;
    const float pushradius = 2f;
    const float rotationSpeed = -600f;

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

        transform.Rotate(0,0,rotationSpeed * Time.deltaTime);
	}

    public void Init(Vector2 end) {
        GetComponent<SpriteRenderer>().color = PotionColors.Blast;
        LeanTween.move(gameObject, end, Vector2.Distance(transform.position, end) * 0.03f);
    }

    void Explode() {
        var damagedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, damageradius) ;
        foreach (var d in damagedUnits) {
            GameObject go = d.gameObject;
            if (go.tag == "MovingEntity") {
                HealthController hc = go.GetComponent<HealthController>();
                if (hc != null) {
                    var gopos = hc.gameObject.transform.position;
                    bool didDie = hc.ReceiveDamage(damage);
                    if (didDie) {
                        Kill(gopos);
                    }
                }
            }
        }

        var pushedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, pushradius);
        foreach (var p in pushedUnits) {
            GameObject go = p.gameObject;
            if (go.tag == "MovingEntity") {
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

    void Kill(Vector2 killed) {
        Entities entities = GameObject.Find("GameManager").GetComponent<Entities>();

        var particles = new GameObject[10];
        for (int i = 0; i < 10; i++) {
            var pos = (Vector2) killed + new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            particles[i] = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
        }
        foreach (var p in particles) {
            p.GetComponent<Rigidbody2D>().AddForce(getOutwardExplosionVector(killed, p.transform.position, 500f));
            p.GetComponent<ParticleController>().BaseColor(PotionColors.Blast);
        }
    }
}
