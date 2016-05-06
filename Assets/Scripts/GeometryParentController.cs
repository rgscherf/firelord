using UnityEngine;

public class GeometryParentController : MonoBehaviour {

    // GameController game;

    // void Start () {
    //     game = GameObject.Find("GameManager").GetComponent<GameController>();
    // }

    public void SwitchRooms(int newRoom, GameObject playerObject) {
        playerObject.GetComponent<MapObject>().room = newRoom;
        foreach (Transform child in transform) {
            MapObject mo = child.gameObject.GetComponent<MapObject>();
            if(mo != null) {
                if (mo.room != newRoom) {
                    mo.gameObject.SetActive(false);
                } else {
                    mo.gameObject.SetActive(true);
                }
            } else {
                Debug.Log(mo);
            }
        }
    }
}
