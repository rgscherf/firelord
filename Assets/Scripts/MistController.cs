using UnityEngine;

public class MistController : MapObject {

    const float cloneTime = 3f;
    const float speed = 0.5f;

    Entities entities;

    float currentCloneTime;
    Vector2 direction;

    readonly Color[] mistColors = {PotionColors.Mist, PotionColors.MistFade, PotionColors.MistFadeTwo};

    void Start () {
        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.Mist;
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        currentCloneTime = 0f;
    }


    void FixedUpdate() {
    }


    void Update () {
        currentCloneTime += Time.deltaTime;
        gameObject.GetComponent<SpriteRenderer>().color = mistColors[Random.Range(0, mistColors.Length)];

        // todo: mist spreading
    }

}
