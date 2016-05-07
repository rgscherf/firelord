using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MistController : MapObject {

    const float cloneTime = 3f;
    const float speed = 0.5f;
    const float deathTimer = 0.75f;

    GameController game;

    float currentCloneTime;
    Vector2 direction;
    bool isDying;

    readonly Color[] mistColors = {PotionColors.Mist, PotionColors.MistFade, PotionColors.MistFadeTwo};

    void Start () {
        isDying = false;
        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.Mist;
        game = GameObject.Find("GameManager").GetComponent<GameController>();
        currentCloneTime = 0f;
    }



    void Update () {
        if (!isDying) {
            gameObject.GetComponent<SpriteRenderer>().color = mistColors[Random.Range(0, mistColors.Length)];
            currentCloneTime = currentCloneTime + (Time.deltaTime * Random.Range(0.5f, 1f));

            if (currentCloneTime > cloneTime) {
                SpawnNewMist();
                currentCloneTime = 0f;
            }
        } else {
            gameObject.GetComponent<SpriteRenderer>().color = mistColors[Random.Range(0, mistColors.Length)] * new Color(1,1,1,0.5f);
        }
    }

    void SpawnNewMist() {
        var possibleSpawnPositions = new List<Vector2>();
        var left = new Vector2(-1f, 0f);
        var right = new Vector2(1f, 0f);
        var up = new Vector2(0f, 1f);
        var down = new Vector2(0f, -1f);
        Vector2[] allPositions = {left, right, up, down};

        var mistPos = (Vector2) transform.position;

        foreach (var p in allPositions) {
            Vector2 candidatePos = mistPos + p;
            Collider2D pointCollider = Physics2D.OverlapPoint(candidatePos);
            if(pointCollider != null) {
                if (pointCollider.gameObject.tag == "Geometry" || pointCollider.gameObject.layer == 1 << LayerMask.NameToLayer("Mist")) {
                    continue;
                }
            } else {
                possibleSpawnPositions.Add(candidatePos);
            }
        }

        if(possibleSpawnPositions.Count > 0) {
            var newMistPos = possibleSpawnPositions[Random.Range(0, possibleSpawnPositions.Count)];
            var newMist = (GameObject) Instantiate(game.entities.mist, (Vector3) newMistPos, Quaternion.identity);
            newMist.transform.parent = GameObject.Find("GeometryParent").transform;

            var mistmo = newMist.GetComponent<MapObject>();
            if (mistmo != null) {
                mistmo.room = room;
            }
        }
    }

    public void ClearMist( Vector2 pushVector) {
        gameObject.GetComponent<Rigidbody2D>().AddForce(pushVector);
        isDying = true;
        Object.Destroy(gameObject, deathTimer);
    }
}



















