using UnityEditor;
using UnityEngine;

namespace Editor
{
    // Class này tự động chạy mỗi khi có file mới được Import vào Project
    public class AssetImportEnforcer : AssetPostprocessor
    {
        // 1. TỰ ĐỘNG XỬ LÝ TEXTURE (CHO STYLE PS1)
        void OnPreprocessTexture()
        {
            // Chỉ áp dụng cho file nằm trong folder _Project/Art
            if (!assetPath.Contains("_Project/Art/Textures")) return;

            TextureImporter importer = (TextureImporter)assetImporter;

            // Nếu setting chưa đúng thì mới sửa (để tránh re-import liên tục)
            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point; // Pixel art sắc nét
                importer.textureCompression = TextureImporterCompression.Uncompressed; // Tránh vỡ hình
                importer.mipmapEnabled = false; // PS1 thường không dùng Mipmap hoặc dùng rất ít
                importer.maxTextureSize = 512; // Giới hạn size để tối ưu

                Debug.Log($"<color=cyan>[AUTO-FIX]</color> Đã chỉnh Texture về chuẩn PS1: {assetPath}");
            }
        }

        // 2. TỰ ĐỘNG XỬ LÝ MODEL (FBX)
        void OnPreprocessModel()
        {
            if (!assetPath.Contains("_Project/Art/Models")) return;

            ModelImporter importer = (ModelImporter)assetImporter;

            // Tắt chức năng tự tạo Material (Để tránh rác project)
            if (importer.materialImportMode != ModelImporterMaterialImportMode.None)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                
                // Các setting tối ưu cho Low-poly
                importer.importCameras = false;
                importer.importLights = false;
                importer.isReadable = false; // Tắt đi cho nhẹ, trừ khi cần thao tác mesh bằng code
                importer.meshCompression = ModelImporterMeshCompression.Medium;

                Debug.Log($"<color=yellow>[AUTO-FIX]</color> Đã chỉnh Model về chuẩn sạch: {assetPath}");
            }
        }

        // 3. TỰ ĐỘNG XỬ LÝ AUDIO
        void OnPreprocessAudio()
        {
            if (!assetPath.Contains("_Project/Art/Audio")) return;

            AudioImporter importer = (AudioImporter)assetImporter;
            
            // Ép về Mono để giả lập âm thanh cũ hoặc tiết kiệm bộ nhớ (tùy chọn)
            // importer.forceToMono = true; 
            
            // Tối ưu hóa load
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.CompressedInMemory;
            importer.defaultSampleSettings = settings;
        }
    }
}