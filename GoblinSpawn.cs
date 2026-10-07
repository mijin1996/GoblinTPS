using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class GoblinSpawn : MonoBehaviour {
    public Goblin goblinPrefab;
    public GoblinData[] goblinDatas;
    public Transform[] spawnPoints;
    private List<Goblin> goblins = new List<Goblin>();
    private int wave;

    public GameObject spawnEffect;

    public int maxWave = 10;
    public int cntPerWave = 10;
    
    private void Update() {
        if (GM.instance != null && GM.instance.isGameover) {
            return;
        }
        //if (goblins.Count <= 0) {
        if (goblins.Count <= 0 && wave < maxWave) {
            SpawnWave();
        }
        UpdateUI();
    }

    private void UpdateUI() {
        UIM.instance.UpdateWaveText(wave, goblins.Count);
    }

    private void SpawnWave() {
        wave++;
        UIM.instance.BlinkWaveText();// wave, 적이 생길때마다 깜빡 효과 📌
        UIM.instance.ShowWavePopup(wave);  //wave증가에 따른 wave증가text 보임
        //int spawnCnt = Mathf.RoundToInt(wave * 1.5f);
        //int spawnCnt = cntPerWave * wave;
        int spawnCnt = cntPerWave * (int)Mathf.Pow(2, wave - 1); // wave로 고블린이 배로 늘어나게끔 📌
        for (int i = 0; i < spawnCnt; i++) {
            CreateGoblin();
        }
    }

    private void CreateGoblin() {
        GoblinData goblinData = goblinDatas[Random.Range(0, goblinDatas.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Goblin goblin = Instantiate(goblinPrefab, spawnPoint.position, spawnPoint.rotation);

        if (spawnEffect != null) {
            Vector3 groundPos = spawnPoint.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 10f, NavMesh.AllAreas)) {
                groundPos = hit.position;
            }
            GameObject fx = Instantiate(spawnEffect, groundPos + Vector3.up * 0.05f, Quaternion.identity);
            Destroy(fx, 3f);
        }

        goblin.Setup(goblinData);
        goblins.Add(goblin);
        goblin.onDeath += () => goblins.Remove(goblin);
        goblin.onDeath += () => Destroy(goblin.gameObject, 10f);
        goblin.onDeath += () => GM.instance.AddScore(100);
    }
}
