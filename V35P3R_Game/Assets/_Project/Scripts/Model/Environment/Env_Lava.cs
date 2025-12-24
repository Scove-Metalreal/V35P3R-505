using _Project.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Model
{
    public class Env_Lava: MonoBehaviour, IDamageZone
    {
        public float damagePerSec = 10f;
        
        public float GetDamageAmount() => 0;
        public float GetDamagePerSecond() => damagePerSec;
    }
}