using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public void ChangeRoom(int signal) {
        var newpos = CameraTarget(signal);
        transform.position = newpos;
    }

    Vector3 CameraTarget(int position) {
        switch (position) {
            case 1:
                return new Vector3(0,1,-10);
            case 2:
                return new Vector3(31,1,-10);
            case 3:
                return new Vector3(31,17,-10);
            case 4:
                return new Vector3(0,17,-10);
            default:
                return new Vector3(0,1,-10);
        }
    }
}
