using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour {

    public Sprite openSprite;

    bool opened;
    int numPotions = 15;

    SpriteRenderer spr;
    Entities entities;


	// Use this for initialization
	void Start () {
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        spr = GetComponent<SpriteRenderer>();
	}
	
    public void OpenChest() {
        opened = true;
        int num = (int)Mathf.Ceil(Random.Range(numPotions * 0.75f, numPotions * 1.25f));
        for (int i = 0; i < num; i ++ ) {
            var pos = (Vector2) gameObject.transform.position + new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            var newpot = (GameObject) Instantiate(entities.ammoPickup, pos, Quaternion.identity);
            newpot.GetComponent<Rigidbody2D>().AddForce(entities.getOutwardExplosionVector(gameObject.transform.position, pos, 40f));
        }

        spr.sprite = openSprite;
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Player" && !opened) {
            OpenChest();
        }
    }
}
