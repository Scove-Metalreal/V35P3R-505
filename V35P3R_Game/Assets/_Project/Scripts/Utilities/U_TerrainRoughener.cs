using UnityEngine;

public class U_TerrainRoughener : MonoBehaviour
{
    public Terrain terrain;
    
    [Header("Độ gồ ghề")]
    public float roughnessScale = 200f; // Độ nhỏ của hạt (càng to hạt càng mịn)
    public float roughnessStrength = 0.002f; // Độ lồi lõm (đừng chỉnh to quá kẻo nát map)

    [ContextMenu("Làm Gồ Ghề Map")]
    public void RoughenTerrain()
    {
        if (terrain == null) terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;
        
        int w = data.heightmapResolution;
        int h = data.heightmapResolution;
        
        // Lấy dữ liệu độ cao hiện tại (mảng 2 chiều)
        float[,] heights = data.GetHeights(0, 0, w, h);
        
        // Duyệt qua từng pixel của terrain
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                // Tạo nhiễu ngẫu nhiên dựa trên tọa độ
                float noise = Mathf.PerlinNoise(x / roughnessScale, y / roughnessScale);
                
                // Áp dụng nhiễu vào độ cao hiện tại
                // Chỉ làm sần sùi thêm chứ không phá dáng núi cũ
                heights[x, y] += (noise * roughnessStrength) - (roughnessStrength / 2f);
            }
        }
        
        // Gán lại vào map
        data.SetHeights(0, 0, heights);
        Debug.Log("Đã cà nhám xong! Nhìn đỡ nhựa chưa?");
    }
}