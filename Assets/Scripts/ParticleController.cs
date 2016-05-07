using UnityEngine;

public class ParticleController : MonoBehaviour {

    GameObject instantiator;

    SpriteRenderer spr;

    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite4;
    public Sprite sprite8;
    public Sprite sprite16;
    public Sprite sprite32;

    bool flicker;
    Color color;
    Color[] flickerColorPalette;

    Color constantColor;

	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        spr.color = flicker ? flickerColorPalette[Random.Range(0, flickerColorPalette.Length)] : color;
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

    public void Init(GameObject whoInstantiated, bool doesFlicker, Color col, float timer, int size) {
        instantiator = whoInstantiated;
        MountSprite(size);
        color = col;
        flicker = doesFlicker;
        if(flicker) {
            flickerColorPalette = new Color[] {col, col * new Color(1,1,1,0.60f), col * new Color(1,1,1,0.85f)};
        }
        CallDeathTimer(timer);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (instantiator != null) {
            if (instantiator.GetComponent<EnemyOoze>() != null) {
                GameObject go = other.gameObject;
                if (go.tag == "Player") {
                    int dmgamt = EnemyOoze.damage;
                    go.GetComponent<HealthController>().ReceiveDamage(dmgamt);
                }
            }
            if (instantiator.GetComponent<QuickPotionController>() != null) {
                GameObject go = other.gameObject;
                QuickPotionController qp = instantiator.GetComponent<QuickPotionController>();
                if (go.tag == "MovingEntity") {
                    qp.ChainFrom(other.gameObject, gameObject.GetComponent<Rigidbody2D>().velocity);
                }
            }
        }
    }
}
