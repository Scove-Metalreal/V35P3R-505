using UnityEngine;
using System.Collections.Generic;

public class U_MapObjectSpawner : MonoBehaviour
{
    // Class con để cấu hình cho từng loại đá
    [System.Serializable]
    public class RockType
    {
        public string name;             // Đặt tên cho dễ nhớ (ví dụ: Đá Tảng, Đá Dăm)
        public GameObject prefab;       // Model đá
        [Range(1, 100)] public int spawnWeight = 10; // Trọng số (càng cao càng dễ ra)
        
        [Header("Cấu hình xoay & Kích thước")]
        public bool randomRotationY = true;   // Chỉ xoay quanh trục Y (thẳng đứng)
        public bool randomRotationAll = false; // Xoay lung tung mọi trục (dùng cho đá nhỏ)
        public Vector2 scaleRange = new Vector2(0.8f, 1.2f); // Random kích thước từ 0.8 đến 1.2
    }

    [Header("Cài đặt chung")]
    public Terrain terrain;
    public int totalRocksToSpawn = 500;
    public Transform parentFolder;
    
    // Thay vì mảng GameObject, ta dùng List các class RockType đã định nghĩa ở trên
    public List<RockType> rockTypes; 

    [Header("Quy tắc chung")]
    public float yOffset = -0.1f; // Chìm xuống đất xíu cho thật

    [ContextMenu("Rải Đá Theo Tỉ Lệ & Xoay")]
    public void SpawnRocks()
    {
        // 1. Dọn dẹp đá cũ
        if (parentFolder != null)
        {
            // Dùng vòng lặp ngược để xóa an toàn trong Editor
            for (int i = parentFolder.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parentFolder.GetChild(i).gameObject);
            }
        }

        // 2. Tính tổng trọng số (Total Weight) để làm mốc random
        int totalWeight = 0;
        foreach (var type in rockTypes)
        {
            totalWeight += type.spawnWeight;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        for (int i = 0; i < totalRocksToSpawn; i++)
        {
            // --- A. Chọn vị trí (như cũ) ---
            float randomX = Random.Range(0, terrainData.size.x);
            float randomZ = Random.Range(0, terrainData.size.z);
            float worldX = terrainPos.x + randomX;
            float worldZ = terrainPos.z + randomZ;
            float y = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = terrainPos.y + y;
            Vector3 spawnPos = new Vector3(worldX, worldY + yOffset, worldZ);

            // --- B. Chọn loại đá dựa trên Tỉ lệ (Weighted Random) ---
            RockType selectedRock = GetRandomRockByType(totalWeight);
            if (selectedRock == null || selectedRock.prefab == null) continue;

            // --- C. Xử lý Xoay (Rotation) theo cấu hình từng loại ---
            Quaternion spawnRot = Quaternion.identity;
            
            if (selectedRock.randomRotationAll)
            {
                // Xoay lung tung cả 3 trục (thích hợp đá dăm nhỏ)
                spawnRot = Random.rotation; 
            }
            else if (selectedRock.randomRotationY)
            {
                // Chỉ xoay quanh trục Y, giữ nguyên độ thăng bằng
                spawnRot = Quaternion.Euler(0, Random.Range(0, 360f), 0); 
            }
            else
            {
                // Không xoay gì cả (theo prefab gốc)
                spawnRot = selectedRock.prefab.transform.rotation;
            }

            // --- D. Instantiate & Random Scale ---
            GameObject instance = Instantiate(selectedRock.prefab, spawnPos, spawnRot);
            
            // Random kích thước to nhỏ
            float randomScale = Random.Range(selectedRock.scaleRange.x, selectedRock.scaleRange.y);
            instance.transform.localScale *= randomScale;

            // Gom vào folder
            if (parentFolder != null) instance.transform.parent = parentFolder;
        }
        
        Debug.Log($"Đã rải {totalRocksToSpawn} đá dựa trên tỉ lệ.");
    }

    // Hàm thuật toán chọn đá dựa trên trọng số
    RockType GetRandomRockByType(int totalWeight)
    {
        int randomValue = Random.Range(0, totalWeight); // Quay xổ số từ 0 -> Tổng
        int currentWeightCheck = 0;

        foreach (var type in rockTypes)
        {
            currentWeightCheck += type.spawnWeight;
            
            // Nếu con số random rơi vào khoảng của loại này -> Chọn nó
            if (randomValue < currentWeightCheck)
            {
                return type;
            }
        }
        return rockTypes[0]; // Fallback (hiếm khi xảy ra)
    }
}