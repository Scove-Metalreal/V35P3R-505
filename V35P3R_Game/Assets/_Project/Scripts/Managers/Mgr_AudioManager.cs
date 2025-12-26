using _Project.Scripts.Model.Data;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class Mgr_AudioManager : MonoBehaviour
    {
        public static Mgr_AudioManager Instance { get; private set; }

        [Header("--- CONFIG ---")]
        [SerializeField] private Data_AudioConfig _config;

        [Header("--- SOURCES ---")]
        [SerializeField] private AudioSource _musicSource; // Nhạc nền (Loop)
        [SerializeField] private AudioSource _sfxSource2D; // Âm thanh UI (Không gian 2D)
    
        // Pool cho âm thanh 3D (Tạo sẵn 1 đống AudioSource để dùng dần)
        [SerializeField] private GameObject _sfxPrefab3D; 

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        private void Start()
        {
            // Tự động chơi nhạc nền u ám
            PlayMusic(_config.horrorAmbience);
        }

        // --- 1. CHƠI NHẠC NỀN ---
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;
            _musicSource.clip = clip;
            _musicSource.loop = true;
            _musicSource.Play();
        }

        // --- 2. CHƠI SFX 2D (UI, Nhặt đồ vào túi) ---
        public void PlaySFX_2D(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource2D.PlayOneShot(clip, volume);
        }

        // --- 3. CHƠI SFX 3D (Tiếng bước chân, Quái gầm) ---
        // Hàm này tạo ra một object tạm thời tại vị trí pos, phát xong tự hủy
        public void PlaySFX_3D(AudioClip clip, Vector3 pos, float volume = 1f)
        {
            if (clip == null) return;
        
            // Tạo object tạm để phát tiếng (AudioClip.PlayClipAtPoint của Unity không chỉnh được volume/pitch tốt)
            // Nên ta dùng cách thủ công này để kiểm soát tốt hơn
            GameObject sfxObj = new GameObject("Temp_SFX");
            sfxObj.transform.position = pos;
        
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1f; // 3D Sound
            source.minDistance = 2f;
            source.maxDistance = 20f; // Xa quá 20m là không nghe thấy
        
            source.Play();
        
            // Tự hủy sau khi phát xong
            Destroy(sfxObj, clip.length + 0.1f);
        }

        // --- GETTERS ĐỂ NGƯỜI KHÁC LẤY DATA ---
        public Data_AudioConfig Config => _config;
    }
}