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

    Color flickerColor;
    Color[] flickerColorPalette;

    Color constantColor;

	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
    }
	
	void Update () {

        if (flickerColor != Color.clear && constantColor != Color.clear) {
            throw new System.NotImplementedException("Particle has both flicker and constant color. Instantiated by " + instantiator);
        }

        if (flickerColor != Color.clear) {
            spr.color = flickerColorPalette[Random.Range(0, flickerColorPalette.Length)];
        }

        if (constantColor != Color.clear ) {
            spr.color = constantColor;
        }
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

    public void SetFlickerColor(GameObject ins, Color col, float timer, int size) {
        instantiator = ins;
        MountSprite(size);
        flickerColor = col;
        flickerColorPalette = new Color[] {col, col * new Color(1,1,1,0.60f), col * new Color(1,1,1,0.85f)};
        CallDeathTimer(timer);
    }

    public void SetConstantColor(GameObject ins, Color col, float timer, int size) {
        instantiator = ins;
        MountSprite(size);
        constantColor = col;
        CallDeathTimer(timer);
    }
}
