using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {

    static Potion[] selection = {Potion.Blast, Potion.Quick, Potion.Spine, Potion.Venom};
    public Potion potionType;

	// Use this for initialization
	void Start () {
	}

    void Awake() {
        potionType = selection[Random.Range(0, selection.Length)];
        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.GetColor(potionType);
    }
	
    public void SetPotionType(Potion pot) {
        potionType = pot;
        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.GetColor(pot);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            var pc = other.gameObject.GetComponent<PlayerController>();
            bool wasfull = pc.ReceivePotion(potionType);
            if (!wasfull) {
                Destroy(gameObject);
            }
        }
    }
}
