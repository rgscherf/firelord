using UnityEngine;

public class ParticleController : MonoBehaviour {

    SpriteRenderer spr;
    Color baseColor;
    Color[] baseColorPalette;

    float deathTimer = 1.5f;


	// Use this for initialization
	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        Object.Destroy(gameObject, deathTimer);
	}
	
	// Update is called once per frame
	void Update () {
        if (baseColor != null) {
            spr.color = baseColorPalette[Random.Range(0, baseColorPalette.Length)];
        }
        
	}

    public void BaseColor(Color col) {
        baseColor = col;
        baseColorPalette = new Color[] {col, col * new Color(1,1,1,0.60f), col * new Color(1,1,1,0.85f)};
    }
}
