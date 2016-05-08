using UnityEngine;

public class MapObject : MonoBehaviour {

    public int room;

    public int GetRoom() {
        return room;
    }
    
    public virtual bool poisoned {get; set;}
    public virtual bool isFlickering {get; set;}
    public virtual void Poison() {}
    public virtual void Flicker() {}
}
