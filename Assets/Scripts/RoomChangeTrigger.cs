using UnityEngine;
using System.Collections;

public class RoomChangeTrigger : MonoBehaviour {

    public int myRoom;
    GameController game;

	void Start () {
        game = GameObject.Find("GameManager").GetComponent<GameController>();
	}
	
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            game.RoomChangeSignal(myRoom);
        }
    }
}
