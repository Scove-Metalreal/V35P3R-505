using System.Collections.Generic;
using _Project.Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
// Để reload scene

namespace _Project.Scripts.Managers
{
    public class Mgr_GameLevel : MonoBehaviour
    {
        // --- SINGLETON (Để truy cập toàn cục) ---
        public static Mgr_GameLevel Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        [Header("--- SPAWNING SETTINGS ---")]
        [SerializeField] private List<Item_Scrap> _itemPrefabs; // Danh sách các loại rác
        [SerializeField] private Transform _spawnPointsContainer; // Cha của các điểm spawn
        [SerializeField] private int _totalItemsToSpawn = 10;

        [Header("--- UI WIRING ---")]
        [SerializeField] private GameObject _gameOverPanel; // Panel Thua
        [SerializeField] private GameObject _victoryPanel;  // Panel Thắng

        private bool _isGameEnded = false;

        private void Start()
        {
            SpawnItems();
        
            // Ẩn UI kết thúc game
            if (_gameOverPanel) _gameOverPanel.SetActive(false);
            if (_victoryPanel) _victoryPanel.SetActive(false);
        }

        // --- LOGIC 1: SPAWN ITEM NGẪU NHIÊN ---
        private void SpawnItems()
        {
            // 1. Lấy tất cả điểm spawn
            U_SpawnPoint[] points = _spawnPointsContainer.GetComponentsInChildren<U_SpawnPoint>();
        
            if (points.Length == 0) return;

            // 2. Tạo list tạm để shuffle (xáo trộn) vị trí
            List<U_SpawnPoint> availablePoints = new List<U_SpawnPoint>(points);

            int spawnedCount = 0;
            while (spawnedCount < _totalItemsToSpawn && availablePoints.Count > 0)
            {
                // Chọn ngẫu nhiên 1 điểm spawn
                int randPointIndex = Random.Range(0, availablePoints.Count);
                U_SpawnPoint p = availablePoints[randPointIndex];
            
                // Chọn ngẫu nhiên 1 loại item
                Item_Scrap prefab = _itemPrefabs[Random.Range(0, _itemPrefabs.Count)];

                // Instantiate (Sinh ra)
                Instantiate(prefab, p.transform.position, Quaternion.identity);

                // Xóa điểm này khỏi danh sách để không spawn trùng
                availablePoints.RemoveAt(randPointIndex);
                spawnedCount++;
            }
        
            Debug.Log($"Đã rải {_totalItemsToSpawn} mảnh vỡ ra map.");
        }

        // --- LOGIC 2: XỬ LÝ THUA (GAME OVER) ---
        public void TriggerGameOver(string reason)
        {
            if (_isGameEnded) return;
            _isGameEnded = true;

            Debug.Log($"GAME OVER: {reason}");

            // Hiện UI Thua
            if (_gameOverPanel) _gameOverPanel.SetActive(true);

            // Khóa chuột và hiện con trỏ để bấm nút
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Dừng thời gian (Optional)
            Time.timeScale = 0f;
        }

        // --- LOGIC 3: XỬ LÝ THẮNG (VICTORY) ---
        public void TriggerVictory()
        {
            if (_isGameEnded) return;
            _isGameEnded = true;

            Debug.Log("VICTORY!");

            if (_victoryPanel) _victoryPanel.SetActive(true);
        
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }

        // --- LOGIC 4: TIỆN ÍCH UI (Gắn vào nút bấm) ---
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}