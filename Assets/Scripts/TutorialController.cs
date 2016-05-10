using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {

	GameObject room1;
	GameObject room2;
	GameObject room3;
	GameObject room4;

	GameObject[] rooms;

    void Awake() {
    	room1 = transform.Find("room1").gameObject;
    	room2 = transform.Find("room2").gameObject;
    	room3 = transform.Find("room3").gameObject;
    	room4 = transform.Find("room4").gameObject;

    	rooms = new[] {room1, room2, room3, room4};
    	foreach (var r in rooms) {
    		r.SetActive(false);
    	}
    }

	public void SetRoom(int room) {
		foreach (var r in rooms) {
			r.SetActive(false);
		}
		rooms[room - 1].SetActive(true);
	}
}
