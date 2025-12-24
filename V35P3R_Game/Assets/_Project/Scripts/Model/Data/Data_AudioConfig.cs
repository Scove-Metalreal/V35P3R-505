using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Model.Data
{
    [CreateAssetMenu(fileName = "Data_AudioConfig", menuName = "Vesper/Audio Config")]
    public class Data_AudioConfig : ScriptableObject
    {
        [Header("--- PLAYER SFX ---")]
        public List<AudioClip> stepsMetal;
        public List<AudioClip> stepsConcrete;
        public List<AudioClip> jump;
        public List<AudioClip> land;

        [Header("--- INTERACTION SFX ---")]
        public List<AudioClip> pickUp;
        public List<AudioClip> drop;
        public List<AudioClip> repairSuccess;
        public List<AudioClip> repairFail;

        [Header("--- MUSIC / AMBIENCE ---")]
        public AudioClip mainMenuMusic;
        public AudioClip horrorAmbience;

        // Hàm Helper: Lấy ngẫu nhiên 1 clip từ list (để âm thanh đỡ nhàm)
        public AudioClip GetRandomClip(List<AudioClip> clips)
        {
            if (clips == null || clips.Count == 0) return null;
            return clips[Random.Range(0, clips.Count)];
        }
    }
}