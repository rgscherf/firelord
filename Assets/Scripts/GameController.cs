using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    List<Room> rooms;
    CameraController gameCamera;
    public GeometryParentController geometryParentController;
    public Entities entities;
    public GameObject player;
    GameObject[] allGeometry;
    GameObject[] allDoors;
    GameObject[] allMovingEntities;

    public int currentRoom;
    int highestRoomCompleted;
    int level = 0;
    int mobsInRoom;
    List<GameObject> doorsInRoom;
    List<GameObject> finalDoorsInRoom;

    void Awake() {
        entities = GetComponent<Entities>();
        player = GameObject.Find("Player");
    }

	void Start () {
        geometryParentController = GameObject.Find("GeometryParent").GetComponent<GeometryParentController>();

        GenerateLevel();

        ButtonDispatch(Potion.Blast);
	}

    void GenerateLevel () {
        level += 1;
        currentRoom = 1;
        allGeometry = null;

        doorsInRoom = new List<GameObject>();
        finalDoorsInRoom = new List<GameObject>();

        rooms = new List<Room>();
        rooms = MakeRooms();
        GetGlobalReferences();
        AstarPath.active.Scan();
        SetupRoom(1, player);
    }

    void SetupRoom(int room, GameObject player) {
        mobsInRoom = 0;
        doorsInRoom.Clear();
        finalDoorsInRoom.Clear();
        geometryParentController.SwitchRooms(room, player);
        foreach( Transform t in geometryParentController.transform ) {
            GameObject go = t.gameObject;
            if(go.GetComponent<MapObject>().room == room) {
                if (go.GetComponent<IsDoor>() != null) {
                    doorsInRoom.Add(go);
                } else if (go.GetComponent<IsFinalDoor>() != null) {
                    finalDoorsInRoom.Add(go);
                } else if (go.tag == "MovingEntity") {
                    mobsInRoom++;
                }
            }
        }
        foreach(var d in doorsInRoom) {
            d.GetComponent<Collider2D>().isTrigger = false;
            d.GetComponent<SpriteRenderer>().enabled = true;
        }
        foreach(var f in finalDoorsInRoom) {
            f.GetComponent<Collider2D>().isTrigger = false;
            f.GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    void Update() {
        bool complete = CheckRoomCompletion();
        if (complete) {
            highestRoomCompleted = highestRoomCompleted > currentRoom ? highestRoomCompleted : currentRoom;
            foreach (var d in doorsInRoom) {
                d.GetComponent<Collider2D>().isTrigger = true;
                d.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (highestRoomCompleted == 4) {
                foreach (var f in finalDoorsInRoom) {
                    f.GetComponent<Collider2D>().isTrigger = true;
                    f.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }

    bool CheckRoomCompletion() {
        int allActiveEntities = GameObject.FindGameObjectsWithTag("MovingEntity").Where(e => e.active).Count();
        return allActiveEntities == 0;
    }


    List<Room> MakeRooms() {
        // eventually this will have to be moved into a Level class.
        var room1 = gameObject.AddComponent<Room>().Init(level, 1);
        var room2 = gameObject.AddComponent<Room>().Init(level, 2);
        var room3 = gameObject.AddComponent<Room>().Init(level, 3);
        var room4 = gameObject.AddComponent<Room>().Init(level, 4);

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
        gameCamera = Camera.main.GetComponent<CameraController>();
    }

    public void ButtonDispatch(Potion dispatch) {

        foreach (var g in allGeometry) {
            var geo = g.GetComponent<GeometryController>();
            geo.ColorSwap(dispatch);
        }

    }

    public void RoomChangeSignal(int signal) {
        if (signal == 5 ) {
            GenerateLevel();
        }
        if (signal != currentRoom) {
            currentRoom = signal;
            gameCamera.ChangeRoom(currentRoom);
        }
        SetupRoom(currentRoom, player);
    }



}
