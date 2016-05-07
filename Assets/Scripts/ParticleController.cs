using UnityEngine;

public class ParticleController : MonoBehaviour {

    public float deathTimer = 1.5f;

    GameObject instantiator;

    SpriteRenderer spr;

    Color flickerColor;
    Color[] flickerColorPalette;

    Color constantColor;

	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        Object.Destroy(gameObject, deathTimer);
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

    public void SetFlickerColor(GameObject ins, Color col) {
        instantiator = ins;
        flickerColor = col;
        flickerColorPalette = new Color[] {col, col * new Color(1,1,1,0.60f), col * new Color(1,1,1,0.85f)};
    }

    public void SetConstantColor(GameObject ins, Color col) {
        instantiator = ins;
        constantColor = col;
    }
}
