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
    public int level = -1;
    int highestRoomCompleted;
    int mobsInRoom;
    List<GameObject> doorsInRoom;
    List<GameObject> finalDoorsInRoom;

    public float thisLevelTime;
    public int thisLevelDamage;
    public int thisLevelThrown;

    public float lastLevelTime;
    public int lastLevelDamage;
    public int lastLevelThrown;

    UIController uiController;
    PlayerController playerController;

    bool endoflevel;
    float panlength = 2f;

    void Awake() {
        entities = GetComponent<Entities>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        uiController = GameObject.Find("UI Manager").GetComponent<UIController>();
    }

	void Start () {
        geometryParentController = GameObject.Find("GeometryParent").GetComponent<GeometryParentController>();
        GameStart();
	}

    void GameStart() {
        level = -1;
        GenerateLevel();
        playerController.currentPotion = Potion.Blast;
    }

    public void Restart() {
        uiController.ChangePlayerDeathState(false);
        GameStart();
    }

    void TreardownLevel() {
        foreach (Transform g in geometryParentController.transform) {
            Object.Destroy(g.gameObject);
        }

    }

    void TutorialTextCheck() {
        if(level == 0) {
            uiController.SetRoom(currentRoom);
        } else {
            uiController.ClearTutoral();
        }
    }
    
    void UpdateTutorial() {
        if (level != 0) {
            return;
        } 
        uiController.SetRoom(currentRoom);
    }

    void GenerateLevel () {
        var leftoverpotions = GameObject.FindGameObjectsWithTag("Pickup");
        
        foreach (var p in leftoverpotions) {
            Object.Destroy(p);
        }

        level += 1;
        endoflevel = false;
        SwapLevelStats();
        TreardownLevel();
        highestRoomCompleted = 0;
        currentRoom = 1;
        allGeometry = null;
        TutorialTextCheck();

        doorsInRoom = new List<GameObject>();
        finalDoorsInRoom = new List<GameObject>();

        rooms = new List<Room>();
        rooms = MakeRooms();
        GetGlobalReferences();
        AstarPath.active.Scan();
        player.transform.position = new Vector2(-13, -3);

        SetupRoom(currentRoom, player);
    }




    void SwapLevelStats() {
        lastLevelTime = thisLevelTime;
        lastLevelDamage = thisLevelDamage;
        lastLevelThrown = thisLevelThrown;

        thisLevelTime = 0f;
        thisLevelDamage = 0;
        thisLevelThrown = 0;
    }

    void SetupRoom(int room, GameObject player) {

        UpdateTutorial();
        uiController.ShowStats(room);

        gameCamera.ChangeRoom(currentRoom);
        mobsInRoom = 0;
        doorsInRoom = new List<GameObject>();
        finalDoorsInRoom = new List<GameObject>();
        player.GetComponent<MapObject>().room = currentRoom;

        foreach (Transform t in geometryParentController.gameObject.transform) {
            var go = t.gameObject;
            if (go.GetComponent<MapObject>().room == currentRoom) {
                go.SetActive(true);
                if (go.GetComponent<IsDoor>() != null) {
                    doorsInRoom.Add(go);
                } else if (go.GetComponent<IsFinalDoor>() != null) {
                    finalDoorsInRoom.Add(go);
                } else if (go.tag == "MovingEntity") {
                    mobsInRoom++;
                }
            } else {
                go.SetActive(false);
            }
        }

        uiController.UpdateRoom();

        foreach(var d in doorsInRoom) {
            d.GetComponent<Collider2D>().isTrigger = false;
            d.GetComponent<SpriteRenderer>().enabled = true;
        }
        foreach(var f in finalDoorsInRoom) {
            f.GetComponent<Collider2D>().isTrigger = false;
            f.GetComponent<SpriteRenderer>().enabled = true;
        }
        if(gameCamera.signal != currentRoom) {
            gameCamera.ChangeRoom(currentRoom);
        }
    }

    void Update() {

        if (endoflevel) {
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        } else {
            thisLevelTime += Time.deltaTime;
        }



        bool complete = CheckRoomCompletion();
        if (doorsInRoom != null && finalDoorsInRoom != null) {
            if (complete) {
                highestRoomCompleted = highestRoomCompleted > currentRoom ? highestRoomCompleted : currentRoom;
                foreach (var d in doorsInRoom) {
                    if (d != null) {
                        d.GetComponent<Collider2D>().isTrigger = true;
                        d.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
                if (highestRoomCompleted == 4 || level == 0) {
                    foreach (var f in finalDoorsInRoom) {
                        if (f != null) {
                            f.GetComponent<Collider2D>().isTrigger = true;
                            f.GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }
                }
            }
        }
    }

    bool CheckRoomCompletion() {
        mobsInRoom = GameObject.FindGameObjectsWithTag("MovingEntity").Where(e => e.active).Count();
        return mobsInRoom == 0;
    }


    List<Room> MakeRooms() {
        // eventually this will have to be moved into a level class.
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
        uiController.PotionSwap(dispatch, playerController.AmmoCount(dispatch));
    }

    void EndOfLevelPan() {
        endoflevel = true;
        Invoke("GenerateLevel", panlength);
        LeanTween.move(Camera.main.gameObject, new Vector3(-51, 17, -10), panlength);
    }

    public void RoomChangeSignal(int signal) {
        if (mobsInRoom == 0) {
            if (signal == 5 ) {
                uiController.ClearTutoral();
                EndOfLevelPan();
            } else if (signal != currentRoom) {
                currentRoom = signal;
                SetupRoom(currentRoom, player);
            }
        }
    }



}
