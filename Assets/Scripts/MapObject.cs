using UnityEngine;

public class MapObject : MonoBehaviour {

    public int room;
    GameController game;

    void awake() {
        game = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    public int GetRoom() {
        return room;
    }
}
