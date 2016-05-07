using UnityEngine;

public class BlastPotionController : MonoBehaviour {

    const int damage = 4;
    const int blastForce = 600;
    const float damageradius = 1f;
    const float pushradius = 2f;
    const float rotationSpeed = -600f;

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

        transform.Rotate(0,0,rotationSpeed * Time.deltaTime);
	}

    public void Init(Vector2 end) {
        GetComponent<SpriteRenderer>().color = PotionColors.Blast;
        LeanTween.move(gameObject, end, Vector2.Distance(transform.position, end) * 0.03f);
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
                    bool didDie = hc.ReceiveDamage(damage);
                    if (didDie) {
                        Kill(gopos);
                    }
                }
            }
        }

        // PUSH UNITS
        var pushedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, pushradius);
        foreach (var p in pushedUnits) {
            GameObject go = p.gameObject;
            if (go.tag == "MovingEntity") {
                go.GetComponent<Rigidbody2D>().AddForce(getOutwardExplosionVector(gameObject.transform.position, go.transform.position, blastForce));
            }

            // clear mist
            MistController mi = go.GetComponent<MistController>();
            if (mi != null) {
                mi.ClearMist( getOutwardExplosionVector(gameObject.transform.position, go.transform.position, blastForce / 2) );
            }
        }

        // spawn explosion projectiles

        const int numexplosionparticles = 70;
        var expsize = new int[] {1, 1, 1, 2};
        for (var i = 0; i < numexplosionparticles; i++) {
            Vector2 pos = (Vector2) gameObject.transform.position + (Random.insideUnitCircle * pushradius);
            var exp = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            Color c = Vector2.Distance(pos, gameObject.transform.position) < damageradius ? PotionColors.Blast : PotionColors.White;
            exp.GetComponent<ParticleController>().SetConstantColor(gameObject, c, 0.5f, expsize[Random.Range(0, expsize.Length)]);
            exp.GetComponent<Rigidbody2D>().AddForce(getOutwardExplosionVector(exp.transform.position, gameObject.transform.position, blastForce / 60));
        }

        // finally, kill this game object
        Object.Destroy(gameObject);
    }

    private Vector2 getOutwardExplosionVector(Vector2 exploder, Vector2 explodee, float blastForce) {
        return -1 * blastForce * (exploder - explodee);
    }

    void Kill(Vector2 killed) {

        const int numparticles = 40;
        var particles = new GameObject[numparticles];
        for (int i = 0; i < numparticles; i++) {
            var pos = (Vector2) killed + new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            particles[i] = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
        }
        foreach (var p in particles) {
            p.GetComponent<Rigidbody2D>().AddForce(getOutwardExplosionVector(killed, p.transform.position, blastForce));
            p.GetComponent<ParticleController>().SetFlickerColor(gameObject, PotionColors.Blast, 1.5f, 4);
        }
    }
}
