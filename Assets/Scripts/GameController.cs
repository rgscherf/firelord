using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    List<Room> rooms;
    UIController ui;
    CameraController gameCamera;
    public GeometryParentController geometryParentController;
    public Entities entities;
    GameObject player;
    GameObject[] allGeometry;
    GameObject[] allDoors;
    GameObject[] allMovingEntities;

    public int currentRoom;
    int level;

	void Start () {
        // this is the level of the game.
        // 4 rooms per level.
        // level 0 is the start screen/tutorial.
        level = 1;
        currentRoom = 1;

        allGeometry = null;
        ui = GameObject.Find("UI Manager").GetComponent<UIController>();
        gameCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        geometryParentController = GameObject.Find("GeometryParent").GetComponent<GeometryParentController>();
        entities = GetComponent<Entities>();
        rooms = new List<Room>();
        rooms = MakeRooms();
        GetGlobalReferences();
        geometryParentController.SwitchRooms(1, player);
	}
	
    List<Room> MakeRooms() {
        // eventually this will have to be moved into a Level class.
        var room1 = gameObject.AddComponent<Room>().Init(1,1);
        var room2 = gameObject.AddComponent<Room>().Init(1,2);
        var room3 = gameObject.AddComponent<Room>().Init(1,3);
        var room4 = gameObject.AddComponent<Room>().Init(1,4);

        rooms.Add(room1);
        rooms.Add(room2);
        rooms.Add(room3);
        rooms.Add(room4);

        return rooms;
    }

    void GetGlobalReferences() {
        allGeometry = GameObject.FindGameObjectsWithTag("Geometry");
        allDoors = allGeometry.Where(c => c.name.Contains("Door")).ToArray();
        allMovingEntities = GameObject.FindGameObjectsWithTag("MovingEntity");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ButtonDispatch(Potion dispatch) {

        foreach (var g in allGeometry) {
            var geo = g.GetComponent<GeometryController>();
            geo.ColorSwap(dispatch);
        }

    }

    public void RoomChangeSignal(int signal) {
        if (signal != currentRoom) {
            currentRoom = signal;
            gameCamera.ChangeRoom(currentRoom);
        }
        geometryParentController.SwitchRooms(currentRoom, player);
    }

}
