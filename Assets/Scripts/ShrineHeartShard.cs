using UnityEngine;
using System.Collections;

public class ShrineHeartShard : MonoBehaviour {

    public bool active = true;
    public Color flickerColor;
    Color[] flicker;

    SpriteRenderer spr;

	// Use this for initialization
	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        flickerColor = PotionColors.White;
	}
	
	// Update is called once per frame
	void Update () {
        if (active) {
            flicker = new[] {flickerColor, flickerColor * new Color(1f,1f,1f,0f)};
            spr.color = flicker[Random.Range(0, flicker.Length)];
        } else {
            spr.color = PotionColors.Gray;
        }
	}

    public void SetColor(Color col) {
        flickerColor = col;
    }
}
