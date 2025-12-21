using UnityEngine;
using System.Linq; // Cần cái này để xử lý mảng

public class U_TerrainAutoPainter : MonoBehaviour
{
    public Terrain terrain;

    [Header("Cấu hình Layer (Thứ tự Index trong Terrain)")]
    public int sandLayerIndex = 0;
    public int rockLayerIndex = 1;
    public int snowLayerIndex = 2;
    public int lavaLayerIndex = 3;

    [Header("Cấu hình Độ Dốc (Rock)")]
    [Range(0, 90)] public float rockSlopeStart = 30f; // Bắt đầu có đá từ độ dốc này
    [Range(0, 90)] public float rockSlopeMax = 45f;   // Dốc hơn mức này là full đá

    [Header("Cấu hình Độ Cao (Snow & Lava)")]
    public float snowHeightStart = 300f; // Độ cao bắt đầu có tuyết
    public float lavaHeightEnd = 50f;    // Độ cao kết thúc của vùng lava (dưới mức này là lava)

    [Header("Độ mượt (Blending)")]
    public float blendRange = 10f; // Khoảng chuyển màu cho mềm (đỡ bị cắt bụp)

    [ContextMenu("Tô Màu Tự Động (Auto Paint)")]
    public void AutoPaint()
    {
        if (terrain == null) terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;

        // Lấy kích thước splatmap (bản đồ texture)
        int mapWidth = data.alphamapWidth;
        int mapHeight = data.alphamapHeight;

        // Mảng 3 chiều lưu thông tin texture: [x, y, layer]
        float[,,] splatmapData = new float[mapWidth, mapHeight, data.terrainLayers.Length];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // 1. Chuẩn hóa tọa độ (0.0 đến 1.0)
                float normX = (float)x / (mapWidth - 1);
                float normY = (float)y / (mapHeight - 1);

                // 2. Lấy độ dốc (Steepness) và độ cao (Height) tại điểm này
                float steepness = data.GetSteepness(normX, normY); // Đã sửa lại thứ tự X, Y
                float height = data.GetInterpolatedHeight(normX, normY); // Dùng InterpolatedHeight để chính xác với mọi độ phân giải

                // --- TÍNH TOÁN TRỌNG SỐ (WEIGHT) CHO TỪNG LAYER ---
                
                // Mặc định là Cát
                float sandWeight = 1f;
                float rockWeight = 0f;
                float snowWeight = 0f;
                float lavaWeight = 0f;

                // A. Xử lý Đá (Dựa trên độ dốc)
                if (steepness > rockSlopeStart)
                {
                    // Nội suy tuyến tính (Lerp) để đá hiện dần ra
                    rockWeight = Mathf.InverseLerp(rockSlopeStart, rockSlopeMax, steepness);
                    sandWeight -= rockWeight; // Giảm cát đi
                }

                // B. Xử lý Tuyết (Dựa trên độ cao)
                if (height > snowHeightStart)
                {
                    // Tuyết đè lên tất cả (kể cả đá)
                    float snowFactor = Mathf.InverseLerp(snowHeightStart, snowHeightStart + blendRange, height);
                    snowWeight = snowFactor;
                    
                    // Giảm các layer khác để nhường chỗ cho tuyết
                    sandWeight *= (1 - snowFactor);
                    rockWeight *= (1 - snowFactor);
                }

                // C. Xử lý Lava (Ở vùng trũng thấp)
                if (height < lavaHeightEnd)
                {
                    // Càng thấp càng nhiều lava
                    float lavaFactor = Mathf.InverseLerp(lavaHeightEnd, lavaHeightEnd - blendRange, height);
                    lavaWeight = lavaFactor;

                    // Giảm các layer khác
                    sandWeight *= (1 - lavaFactor);
                    rockWeight *= (1 - lavaFactor);
                    snowWeight *= (1 - lavaFactor); // Tuyết chắc ko rơi xuống lava đâu nhỉ :v
                }

                // --- GÁN VÀO MẢNG DỮ LIỆU ---
                // Đảm bảo tổng weight = 1 (Normalizing)
                float totalWeight = sandWeight + rockWeight + snowWeight + lavaWeight;
                
                // Gán giá trị vào đúng index layer
                splatmapData[x, y, sandLayerIndex] = sandWeight;
                splatmapData[x, y, rockLayerIndex] = rockWeight;
                splatmapData[x, y, snowLayerIndex] = snowWeight;
                splatmapData[x, y, lavaLayerIndex] = lavaWeight;
            }
        }

        // Apply dữ liệu texture vào Terrain
        data.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("Đã tô màu xong!");
    }
}