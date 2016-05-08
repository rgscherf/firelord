using UnityEngine;
using System.Linq;

public enum ParticleType { venom, ooze, quick, spine, effects}

public class ParticleController : MonoBehaviour {

    public ParticleType instantiator;

    SpriteRenderer spr;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite4;
    public Sprite sprite8;
    public Sprite sprite16;
    public Sprite sprite32;


    bool spineShrapnel;
    bool flicker;

    Color color;
    Color[] flickerColorPalette;

    SpinePotionController spineController;
    public QuickPotionController quickController;

    Color constantColor;
    Entities entities;

	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
    }
	
	void Update () {
        spr.color = flicker ? flickerColorPalette[Random.Range(0, flickerColorPalette.Length)] : color;

        if (spineShrapnel) {
            var coll = Physics2D.OverlapPointAll(gameObject.transform.position);
            foreach (var c in coll) {
                if (c.gameObject.tag == "MovingEntity") {
                    c.GetComponent<HealthController>().ReceiveDamage(2);
                }
            }
        }
	}

    public void ExplodeVenom() {
        Invoke("PrivateExplodeVenom", 0.2f);
    }

    void PrivateExplodeVenom() {
        float blastRadius = 2f;
        int damage = 1;
        float blastForce = 600f;
        int found = 0;

        var damagedUnits = Physics2D.OverlapCircleAll(gameObject.transform.position, blastRadius) ;
        damagedUnits = damagedUnits.OrderBy( v => Vector2.Distance(gameObject.transform.position, v.gameObject.transform.position)).ToArray();
        foreach(var d in damagedUnits) {
            GameObject go = d.gameObject;
            if (go.tag == "MovingEntity") {
                HealthController hc = go.GetComponent<HealthController>();
                if (hc != null) {
                    hc.ReceiveDamage(damage);
                }
            }
            if (go.tag == "Particle") {
                ParticleController pc = go.GetComponent<ParticleController>();
                if (pc != null && pc.instantiator == ParticleType.venom) {
                    pc.ExplodeVenom();
                    found++;
                    if (found == 1) {
                        break;
                    }
                }
            }
        }

        const int numexplosionparticles = 20;
        
        for (var i = 0; i < numexplosionparticles; i++) {
            Vector2 pos = (Vector2) gameObject.transform.position + (Random.insideUnitCircle * blastRadius);
            var exp = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            Color c = PotionColors.Blast;
            exp.GetComponent<BoxCollider2D>().enabled = false;
            exp.GetComponent<ParticleController>().Init(ParticleType.effects, true, c, 0.5f, 8);
            exp.GetComponent<Rigidbody2D>().AddForce(entities.getOutwardExplosionVector(exp.transform.position, gameObject.transform.position, blastForce / 60));
        }

        Destroy(gameObject);
    }

    void CallDeathTimer(float t) {
        Object.Destroy(gameObject, t);
    }

    void MountSprite(int s) {
        if (spr == null) {
            spr = gameObject.GetComponent<SpriteRenderer>();
        }

        Sprite newsprite;
        switch (s) {
            case 1:
                newsprite = sprite1;
                break;
            case 2:
                newsprite = sprite2;
                break;
            case 4:
                newsprite = sprite4;
                break;
            case 8:
                newsprite = sprite8;
                break;
            case 16:
                newsprite = sprite16;
                break;
            case 32:
                newsprite = sprite32;
                break;
            default:
                throw new System.NotImplementedException("Particle given invalid size param. Instantiated by " + instantiator);
        }
        spr.sprite = newsprite;
        float collidersize = s / 16f;
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(collidersize, collidersize);
    }

    public void Init(ParticleType whoInstantiated, bool doesFlicker, Color col, float timer, int size) {
        instantiator = whoInstantiated;
        MountSprite(size);
        color = col;
        flicker = doesFlicker;
        if(flicker) {
            flickerColorPalette = new Color[] {col, col * new Color(1,1,1,0.60f), col * new Color(1,1,1,0.85f)};
        }
        CallDeathTimer(timer);
    }

    public void ApplyForce(Vector2 forceDirection) {
        gameObject.GetComponent<Collider2D>().isTrigger = false;
        gameObject.GetComponent<Rigidbody2D>().AddForce(forceDirection / 2);
        Object.Destroy(gameObject, 1f);
        spineShrapnel = true;
    }

    void OnTriggerEnter2D(Collider2D other) {
        GameObject go = other.gameObject;
        switch (instantiator) {

            case ParticleType.venom:
                if(go.tag == "Mist") {
                    MistController mi = go.GetComponent<MistController>();
                    if (mi != null ) {
                        mi.ClearMist(new Vector2(0f,0f));
                    }
                }
                if(go.tag == "MovingEntity") {
                    MapObject mo = go.GetComponent<MapObject>();
                    if (mo != null) {
                        mo.Poison();
                    }
                }
                break;

            case ParticleType.ooze:
                if (go.tag == "Player") {
                    int dmgamt = EnemyOoze.damage;
                    go.GetComponent<HealthController>().ReceiveDamage(dmgamt);
                }
                break;

            case ParticleType.quick:
                if (go.tag == "MovingEntity") {
                    // combo with quick potion
                    if(go.GetComponent<MapObject>().poisoned == true) {
                        VenomPuke vp = go.GetComponent<VenomPuke>();
                        if (vp == null) {
                            go.AddComponent<VenomPuke>();
                        }
                    }
                    quickController.ChainFrom(other.gameObject, gameObject.GetComponent<Rigidbody2D>().velocity);
                }
                break;

            case ParticleType.spine:
                if(go.tag == "Mist") {
                    MistController mi = go.GetComponent<MistController>();
                    if (mi != null) {
                        mi.ClearMist(new Vector2(0f,0f));
                    }
                }
                if(go.tag == "MovingEntity") {
                    var entrigid = go.GetComponent<Rigidbody2D>();
                    if (entrigid != null) {
                        entrigid.velocity *= 0.75f;
                    }
                    if (Random.value < 0.75 * Time.deltaTime) {
                        var hc = go.GetComponent<HealthController>();
                        if (hc != null) {
                            hc.ReceiveDamage(1);
                        }
                    }
                }
                break;

            case ParticleType.effects:
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                break;

            default:
                break;
        }
    }
}
