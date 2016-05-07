using UnityEngine;
using System.Collections;

public class Entities : MonoBehaviour {

    public GameObject player;

    public GameObject wall01;
    public GameObject wall02;
    public GameObject wall03;
    public GameObject[] walls;

    public GameObject door;
    public GameObject lastRoomDoor;

    public GameObject mist;

    public GameObject empty;
    public GameObject newline;
    public GameObject particle;

    public GameObject ooze;

    public GameObject thrownBlast;

    
    void Awake() {
        walls = new GameObject[] {wall01, wall02, wall03};
    }
}
