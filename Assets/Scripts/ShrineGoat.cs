using UnityEngine;
using System.Collections.Generic;

public class ShrineGoat : MapObject {

    Entities entities;
    PlayerController player;
    SpriteRenderer spr;
    Color[] colors;
    const float actionCooldown = 1f;
    float actionCooldownCurrent;
    bool spawned;

    PlayerController GetPlayer() {
        return GameObject.Find("Player").GetComponent<PlayerController>();
    }

	void Start () {
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        player = GetPlayer();
        spr = gameObject.GetComponent<SpriteRenderer>();
        colors = new[] {PotionColors.Blast, PotionColors.Quick, PotionColors.Spine, PotionColors.Venom};
	}
	
	// Update is called once per frame
	void Update () {
        if (player == null) {
            player = GetPlayer();
        }

        actionCooldownCurrent += Time.deltaTime;

        spr.color = spawned ? PotionColors.Gray : spr.color = colors[Random.Range(0, colors.Length)];
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Particle") {
            var pc = other.gameObject.GetComponent<ParticleController>();
            if (pc != null && !spawned) {
                spawned = true;
                Potion pot = ParticleController.ParticleTypeToPotion(pc.instantiator);
                SpawnPotions(pot);
            }
            Object.Destroy(other.gameObject);
            actionCooldownCurrent = 0f;
        }
    }

    void AddPotionToSpawnList(Potion pot, int num, List<Potion> ls) {
        for (int i = 0; i < num; i++) {
            ls.Add(pot);
        }
    }

    void SpawnPotions(Potion pot) {
        int toDistribute = player.AmmoCount(pot) + 1;
        int guaranteed = (int) Mathf.Floor(toDistribute / 3.0f);
        int remainder = toDistribute % 3;
        var spawnList = new List<Potion>();

        if (pot != Potion.Blast) { AddPotionToSpawnList(Potion.Blast, guaranteed, spawnList); }
        if (pot != Potion.Quick) { AddPotionToSpawnList(Potion.Quick, guaranteed, spawnList); }
        if (pot != Potion.Spine) { AddPotionToSpawnList(Potion.Spine, guaranteed, spawnList); }
        if (pot != Potion.Venom) { AddPotionToSpawnList(Potion.Venom, guaranteed, spawnList); }
        int remdist = 0;
        var allPotions = new[] { Potion.Blast, Potion.Quick, Potion.Spine, Potion.Venom };
        while (remdist != remainder) {
            var candidate = allPotions[Random.Range(0, allPotions.Length)];
            if (candidate != pot) {
                spawnList.Add(candidate);
                remdist += 1;
            }
        }
        foreach (var p in spawnList) {
            var pos = (Vector2) gameObject.transform.position + new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            var newpot = (GameObject) Instantiate(entities.ammoPickup, pos, Quaternion.identity);
            newpot.GetComponent<PickupController>().SetPotionType(p);
            newpot.GetComponent<Rigidbody2D>().AddForce(entities.getOutwardExplosionVector(gameObject.transform.position, pos, 40f));

        }
        player.ZeroOutAmmo(pot);
    }

}
