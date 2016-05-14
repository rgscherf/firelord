using UnityEngine;

public class Room : MonoBehaviour {

    const int xlen = 31;
    const int ylen = 16;
    const int localOffsetX = 15;
    const int localoffsetY = 5;

    string mapDefinition;

    Entities entities;
    Transform geoparent;

    // rooms are implemented as strings in the dimensions of a room
    // maps start at -15, 5 and go to 15, -10
	void Awake () {
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        geoparent = GameObject.Find("GeometryParent").transform;
	}

	public Room Init(int level, int roomPositionInLevel) {
        mapDefinition = def(level, roomPositionInLevel);
        instantiateRoom(roomPositionInLevel, mapDefinition);
        return this;
    }

    void instantiateRoom(int roomPositionInLevel, string mapdef) {
        int roomOffsetX, roomOffsetY;
        switch (roomPositionInLevel) {
            case 2:
                roomOffsetX = xlen;
                roomOffsetY = 0;
                break;
            case 3:
                roomOffsetX = xlen;
                roomOffsetY = ylen;
                break;
            case 4:
                roomOffsetX = 0;
                roomOffsetY = ylen;
                break;
            default:
                roomOffsetX = 0;
                roomOffsetY = 0;
                break;
        }

        int curx = 0;
        int cury = 0;

        foreach (var ch in mapdef) {
            if (ch == '\n') {
                cury--;
                curx = 0;
                continue;
            }
            var loc = new Vector2(curx - localOffsetX + roomOffsetX, cury + localoffsetY + roomOffsetY);
            GameObject cur = getTile(roomPositionInLevel, curx, cury, ch);

            var newobj = (GameObject) Instantiate(cur, loc, Quaternion.identity);

            // put object under geometry game object
            newobj.transform.parent = geoparent;

            // set the room of this object
            MapObject roomsetter = newobj.GetComponent<MapObject>();
            if (roomsetter) {
                roomsetter.room = roomPositionInLevel;
            }

            // increment X and let's keep going!
            curx++;
        }
    }

    GameObject getTile(int roomPositionInLevel, int curx, int cury, char ch) {
        switch (ch) {
            case '.':
                return entities.empty;
            case 'X':
                return entities.walls[Random.Range(0, entities.walls.Length)];
            // case 'P':
            //     return roomPositionInLevel == 1 ? entities.player : entities.empty;
            case 'M':
                return entities.mist;
            case 'o':
                return entities.ooze;
            case 'd':
                return entities.bigdog;
            case 'f':
                return entities.firespirit;
            case 'C':
                return entities.chest;
            case 'G':
                return entities.shrinegoat;
            case 'H':
                return entities.shrineHeart;
            case 'w':
                return entities.wizard;
            case 'D':
                bool isDoor = false;
                bool isLastRoomDoor = false;
                switch (roomPositionInLevel) {
                    case 1:
                        // right, up
                        isDoor = (curx == 30 && (cury == -6 || cury == -7 || cury == -8 || cury == -9)) || (cury == 0 && (curx == 13 || curx == 14 || curx == 15 || curx == 16));
                        isLastRoomDoor = (cury == 0 && (curx == 13 || curx == 14 || curx == 15 || curx == 16));
                        break;
                    case 2:
                        // left, up
                        isDoor = (curx == 0 && (cury == -6 || cury == -7 || cury == -8 || cury == -9)) || (cury == 0 && (curx == 13 || curx == 14 || curx == 15 || curx == 16));
                        break;
                    case 3:
                        // left, down
                        isDoor = (curx == 0 && (cury == -6 || cury == -7 || cury == -8 || cury == -9)) || (cury == -15 && (curx == 13 || curx == 14 || curx == 15 || curx == 16));
                        break;
                    case 4:
                        // left, down, right
                        isDoor = (curx == 0 && (cury == -6 || cury == -7 || cury == -8 || cury == -9)) || (cury == -15 && (curx == 13 || curx == 14 || curx == 15 || curx == 16)) || (curx == 30 && (cury == -6 || cury == -7 || cury == -8 || cury == -9));
                        break;
                }
                if (isDoor) {
                    return isLastRoomDoor ? entities.lastRoomDoor : entities.door;
                } 
                return entities.walls[Random.Range(0, entities.walls.Length)];
            default:
                return entities.newline;
        }
    }

    string def(int level, int roomPositionInLevel) {
        if (level == 0) {
            // special rooms for tutorial / intro will go here.
            switch (roomPositionInLevel) {
                case 1:
                    return tut1;
                    break;
                case 2:
                    return tut2;
                    break;
                case 3:
                    return tut3;
                    break;
                case 4:
                    return baseRoom;
                    break;
            }
        }
        if (roomPositionInLevel == 1) {
            // special encounter rooms
            return special[Random.Range(0, special.Length)];

        }
        switch (level) {
            case 1:
                return level1[Random.Range(0, level1.Length)];
            case 2:
                return level2[Random.Range(0, level2.Length)];
            default:
                return level2[Random.Range(0, level2.Length)];
                // return level2[Random.Range(0, level2.Length)];
        }
        return "";
    }

    readonly string[] level2 = {
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.....XXX.................o...X
X......XXX....................X
X......XXX...ooo..............X
D......XXX...........XXX......D
D......XXX...........XXX......D
D......XXX....f......XXX......D
D......XXX...........XXX......D
X............ooo.....XXX......X
X....................XXX......X
X.....................XXX.....X
X..o..........................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X........................d....X
X.............................X
X.............................X
X.............................X
D.............................D
D.............d...............D
D.............................D
D.............................D
X........XX...................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X......X...............X......X
D......X...............X......D
D......X......d........X......D
D......X...............X......D
D......X..........f....X......D
X......X...............X......X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X..........XXXXXXXX...........X
D.............................D
D.............d...............D
D.............................D
D.............................D
X.............................X
X.............................X
X.......w..............w......X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
D.............................D
D..............o..............D
D.............................D
D.............................D
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.......................w.....X
X.............................X
X........w....................X
X.............................X
D.............................D
D..............o..............D
D.............................D
D.............................D
X.............................X
X........w....................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X...oooooo........XXXXXXXXX...X
X..................oooo.......X
X.............................X
D..........XXXXXXX............D
D.............................D
D................ff...........D
D.............................D
X..XXXXXXXX...................X
X....ooooo.......XXXXXXXXX....X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X...XXXXXXXXX....X......oo....X
X..oo..XXXXX.....X............X
X.......XXXX....XX............X
X....XXXX.....................X
X.............................X
D.............................D
D.......X.....................D
D.......X.........XXXXXXXXX...D
D.......X.........X.......X...D
X.......X.........X.......X...X
X.o.....X.........X..ooo..X...X
X...XXXXX............XXXXXX.o.X
X...ooo.......................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.....................f.......X
X......X......XXXXXXXXX.......X
X......X.w....................X
D......X......................D
D......X......................D
D.............................D
D..........XXXXXXXXXXXX.......D
X.............................X
X.............................X
X.......w.....................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X............XX...............X
X.........oo..................X
D.........oo..................D
D.......X......d.......X......D
D.......X..............X......D
D..................oo.........D
X..................oo.........X
X..............XX.............X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X........d............d.......X
X.............................X
D.............................D
D..............X..............D
D.............................D
D.............................D
X...................d.........X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",


    };




    readonly string[] level1 = {
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.....XXX.....................X
X......XXX....................X
X......XXX...ooo..............X
D....................XXX......D
D....................XXX......D
D......XXX....f...............D
D......XXX....................D
X............ooo.....XXX......X
X....................XXX......X
X.....................XXX.....X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X........................d....X
X.............................X
X......XXXXX.......XXXXX......X
X......XXXXX.......XXXXX......X
D.............................D
D.............d...............D
D.............................D
D.............................D
X......XXXXX.......XXXXX......X
X......XXXXX.......XXXXX......X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",


@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X...................oooo......X
X..................oooo.......X
X........XXXXXXXXXXXXXX.......X
D.......XX............XX......D
D.............................D
D................w............D
D.......XX............XX......D
X........XXXXXXXXXXXXXX.......X
X.............................X
X...ooo.......................X
X...ooo.......................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.....XXXXX.........XXXXX.....X
X.......XXX........XXXX.......X
X.............................X
D............oooo.............D
D..........ooooo..............D
D...........oooo..............D
D............XXX..............D
X............XXX..............X
X.......XXXXXXXXXXX...........X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.....XX.................X....X
X............XX..........X....X
X.......oo.........XX.........X
D..............w..............D
D.......XX...........X........D
D....................XX.......D
D.............XXX.............D
X..........ooo................X
X....................XX.......X
X......XXX.....XX.............X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.......w.....................X
X.............................X
X.................X.....XXXXXXX
XXXXXXXXX.........X...........X
D.................X...........D
D........oo.......X...........D
D.............................D
D.............................D
X.....XXXXXXXXXXXXXXXXXXX.....X
X.................oooo........X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
D.............................D
D..............d..............D
D.............................D
D.............................D
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",

@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X........XXXXXXXXX............X
X........X..f.................X
X....................X........X
D.............................D
D........X.....f..............D
D........X....................D
D....................X........D
X.............................X
X.............................X
X.........o...................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
D.............................D
D..............w..............D
D.............................D
D.............................D
X.............................X
X............d................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX"

};

   

    readonly string[] special = {
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.....XXXXXXXX...XXXXXXXX.....X
X.....X.....XX...XX.....X.....X
D.....X..H...........G..X.....D
D.............................D
D.............................D
D............XXXXX............D
X.............................X
X..............C..............X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.......H............G........X
X......XXX..........XXX.......X
D........X..........X.........D
D........XX........XX.........D
D.............................D
D.............................D
X........XXXX...XXXXX.........X
X.............................X
X.............C...............X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX",
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X..........X.......X..........X
X......H...X...C...X...G......X
D..........X.......X..........D
D..........X.......X..........D
D.............................D
D.............................D
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX"

};


    const string tut1 =
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
D.............................D
D.............................D
D.............................D
D.............................D
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX";

    const string tut2 =
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
D.............................D
D.............................D
D.............................D
D.............................D
X........................o....X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX";

    const string tut3 =
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X........o.........o..........X
X................o............X
X.............................X
D.............................D
D.............................D
D.............................D
D.............................D
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX";


    const string baseRoom =
@"XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
D.............................D
D.............................D
D.............................D
D.............................D
X.............................X
X.............................X
X.............................X
X.............................X
X.............................X
XXXXXXXXXXXXXDDDDXXXXXXXXXXXXXX";
}





