using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CryptidHunter.Cryptids;

namespace CryptidHunter.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }
        
        [Header("Game Settings")]
        [SerializeField] private int maxCryptidsInWorld = 5;
        [SerializeField] private float cryptidSpawnInterval = 120f;
        [SerializeField] private float spawnDistanceFromPlayer = 50f;
        [SerializeField] private float despawnDistance = 200f;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        
        [Header("Cryptid Prefabs")]
        [SerializeField] private GameObject[] cryptidPrefabs;
        
        [Header("Game State")]
        [SerializeField] private int playerScore = 0;
        [SerializeField] private int cryptidsEncountered = 0;
        [SerializeField] private int cryptidsCaptured = 0;
        
        private List<CryptidBase> activeCryptids = new List<CryptidBase>();
        private Transform player;
        private float spawnTimer;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            spawnTimer = cryptidSpawnInterval;
            
            if (spawnPoints.Length == 0)
            {
                CreateDefaultSpawnPoints();
            }
        }
        
        private void Update()
        {
            HandleCryptidSpawning();
            CheckCryptidDistances();
        }
        
        private void HandleCryptidSpawning()
        {
            spawnTimer -= Time.deltaTime;
            
            if (spawnTimer <= 0 && activeCryptids.Count < maxCryptidsInWorld)
            {
                SpawnRandomCryptid();
                spawnTimer = cryptidSpawnInterval;
            }
        }
        
        private void SpawnRandomCryptid()
        {
            if (cryptidPrefabs.Length == 0 || player == null) return;
            
            GameObject cryptidPrefab = cryptidPrefabs[Random.Range(0, cryptidPrefabs.Length)];
            Transform spawnPoint = GetRandomSpawnPoint();
            
            if (spawnPoint != null && Vector3.Distance(spawnPoint.position, player.position) > spawnDistanceFromPlayer)
            {
                GameObject cryptidObj = Instantiate(cryptidPrefab, spawnPoint.position, spawnPoint.rotation);
                CryptidBase cryptid = cryptidObj.GetComponent<CryptidBase>();
                
                if (cryptid != null)
                {
                    activeCryptids.Add(cryptid);
                    cryptidsEncountered++;
                }
            }
        }
        
        private Transform GetRandomSpawnPoint()
        {
            if (spawnPoints.Length == 0) return null;
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        
        private void CheckCryptidDistances()
        {
            if (player == null) return;
            
            for (int i = activeCryptids.Count - 1; i >= 0; i--)
            {
                if (activeCryptids[i] == null)
                {
                    activeCryptids.RemoveAt(i);
                    continue;
                }
                
                float distance = Vector3.Distance(activeCryptids[i].transform.position, player.position);
                if (distance > despawnDistance)
                {
                    Destroy(activeCryptids[i].gameObject);
                    activeCryptids.RemoveAt(i);
                }
            }
        }
        
        private void CreateDefaultSpawnPoints()
        {
            List<Transform> defaultSpawns = new List<Transform>();
            
            for (int i = 0; i < 8; i++)
            {
                GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
                spawnPoint.transform.parent = transform;
                
                float angle = i * 45f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * 100f;
                float z = Mathf.Sin(angle) * 100f;
                
                spawnPoint.transform.position = new Vector3(x, 0, z);
                defaultSpawns.Add(spawnPoint.transform);
            }
            
            spawnPoints = defaultSpawns.ToArray();
        }
        
        public void RegisterCryptidCapture(CryptidBase cryptid)
        {
            if (cryptid != null)
            {
                cryptidsCaptured++;
                playerScore += cryptid.GetDangerLevel() * 100;
                
                if (activeCryptids.Contains(cryptid))
                {
                    activeCryptids.Remove(cryptid);
                }
            }
        }
        
        public void RegisterCryptidSighting(CryptidBase cryptid)
        {
            if (cryptid != null)
            {
                playerScore += cryptid.GetDangerLevel() * 10;
            }
        }
        
        public int GetScore() => playerScore;
        public int GetCryptidsEncountered() => cryptidsEncountered;
        public int GetCryptidsCaptured() => cryptidsCaptured;
        public List<CryptidBase> GetActiveCryptids() => new List<CryptidBase>(activeCryptids);
    }
}