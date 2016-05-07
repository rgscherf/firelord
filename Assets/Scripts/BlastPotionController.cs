using UnityEngine;

public class BlastPotionController : MonoBehaviour {

    const int damage = 3;
    const int blastForce = 600;
    const float damageradius = 1f;
    const float pushradius = 2f;
    const float rotationSpeed = -600f;
    const float travelSpeed = 0.08f;

    Entities entities;

    bool isTweening = true;

	// Use this for initialization
	void Start () {
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
	}
	
	// Update is called once per frame
	void Update () {
        isTweening = LeanTween.isTweening(gameObject);
        if(!isTweening) {
            Explode();
        }

        var coll = Physics2D.OverlapPointAll(gameObject.transform.position);
        foreach (var c in coll) {
            if (c.gameObject.tag == "Geometry") {
                Explode();
            }
        }

        transform.Rotate(0,0,rotationSpeed * Time.deltaTime);
	}

    public void Init(Vector2 end) {
        GetComponent<SpriteRenderer>().color = PotionColors.Blast;
        LeanTween.move(gameObject, end, Vector2.Distance(transform.position, end) * travelSpeed);
    }

    void Explode() {

        // DEAL DAMAGE
        var damagedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, damageradius) ;
        foreach (var d in damagedUnits) {
            GameObject go = d.gameObject;
            if (go.tag == "MovingEntity") {
                HealthController hc = go.GetComponent<HealthController>();
                if (hc != null) {
                    var gopos = hc.gameObject.transform.position;
                    hc.ReceiveDamage(damage);
                }
            }
        }

        // PUSH UNITS
        var pushedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, pushradius);
        foreach (var p in pushedUnits) {
            GameObject go = p.gameObject;
            if (go.tag == "MovingEntity") {
                go.GetComponent<Rigidbody2D>().AddForce(entities.getOutwardExplosionVector(gameObject.transform.position, go.transform.position, blastForce));
            }

            // clear mist
            MistController mi = go.GetComponent<MistController>();
            if (mi != null) {
                mi.ClearMist( entities.getOutwardExplosionVector(gameObject.transform.position, go.transform.position, blastForce / 2) );
            }
        }

        // spawn explosion projectiles
        const int numexplosionparticles = 30;
        var expsize = new int[] {4,4};
        for (var i = 0; i < numexplosionparticles; i++) {
            Vector2 pos = (Vector2) gameObject.transform.position + (Random.insideUnitCircle * pushradius);
            var exp = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            Color c = Vector2.Distance(pos, gameObject.transform.position) < damageradius ? PotionColors.Blast : PotionColors.White;
            exp.GetComponent<ParticleController>().Init(gameObject, false, c, 0.5f, expsize[Random.Range(0, expsize.Length)]);
            exp.GetComponent<Rigidbody2D>().AddForce(entities.getOutwardExplosionVector(exp.transform.position, gameObject.transform.position, blastForce / 60));
        }

        // finally, kill this game object
        Object.Destroy(gameObject);
    }


}
