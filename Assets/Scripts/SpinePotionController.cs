using UnityEngine;
using System.Collections.Generic;

public class SpinePotionController : MonoBehaviour {

    const float lifetime = 5f;
    const float spinSpeed = 60f;
    const int numThorns = 150;
    const float radius = 2f;

    const float spawnTimer = 1f;
    float spawnTimerCurrent;
    bool spawned;

    Entities entities;

    List<ParticleController> thorns;

	// Use this for initialization
	void Start () {
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.Spine;
	}

    void Init() {
        Object.Destroy(gameObject, lifetime);
        thorns = new List<ParticleController>();
        MakeThorns();
    }
	
    void MakeThorns() {
        var spineSizes = new int[] {1,1,2};
        for (int i = 0; i < numThorns; i++) {
            Vector2 pos = (Random.insideUnitCircle * 2 ) + (Vector2) gameObject.transform.position;
            var t = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            ParticleController pc = t.GetComponent<ParticleController>();
            if (pc != null) {
                pc.Init(gameObject, true, PotionColors.Spine, lifetime, spineSizes[Random.Range(0,spineSizes.Length)]);
                thorns.Add(pc);
            }
        }
    }

	void Update () {
        if (!spawned) {
            spawnTimerCurrent += Time.deltaTime;
            gameObject.transform.Rotate(new Vector3(0f,0f, spinSpeed * Time.deltaTime));
            gameObject.transform.localScale *= 1 - (spawnTimerCurrent / spawnTimer);
            if (spawnTimerCurrent > spawnTimer) {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.transform.localScale = new Vector3(1f,1f,1f);
                spawned = true;
                Init();
            }
        }
	}
}
