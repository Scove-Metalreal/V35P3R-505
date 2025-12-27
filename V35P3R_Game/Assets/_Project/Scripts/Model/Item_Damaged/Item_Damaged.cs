using System.Collections.Generic;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Model;
using _Project.Scripts.Utilities;
using UnityEngine;
using _Project.Scripts.Utilities;

[System.Serializable]
public struct RepairRequirement
{
    public ScrapType type;
    public int amount;
}

namespace _Project.Scripts.Model
{
    public class Item_Damaged : MonoBehaviour, IInteractable
    {
        [Header("--- REPAIR STATE ---")]
        [Range(0f, 100f)]
        [SerializeField] private float _repairProgress;
        [SerializeField] private float _repairPerUse = 5f;

        [Header("--- REQUIREMENTS ---")]
        [SerializeField] private List<RepairRequirement> _requirements = new();

        private void Awake()
        {
            GenerateRandomRequirements();
        }

        public string GetInteractionPrompt()
        {
            if (_repairProgress >= 100f)
                return "Repaired";

            return "Press E to Repair";
        }

        public bool IsHoldable() => false;

        public void OnInteract(M_Player player)
        {
            if (_repairProgress >= 100f) return;

            Item_Scrap heldItem = player.GetCurrentHeldItem();
            if (heldItem == null) return;

            ScrapType heldType = heldItem.GetScrapType();

            for (int i = 0; i < _requirements.Count; i++)
            {
                if (_requirements[i].type == heldType && _requirements[i].amount > 0)
                {
                    ConsumeRequirement(i);
                    player.ConsumeCurrentItem();
                    AddRepairProgress();
                    return;
                }
            }
        }

        private void ConsumeRequirement(int index)
        {
            RepairRequirement req = _requirements[index];
            req.amount--;
            _requirements[index] = req;
        }

        private void AddRepairProgress()
        {
            _repairProgress += _repairPerUse;
            _repairProgress = Mathf.Clamp(_repairProgress, 0f, 100f);

            if (_repairProgress >= 100f)
            {
                OnRepairComplete();
            }
        }

        private void OnRepairComplete()
        {
            Debug.Log("REPAIR COMPLETE");
        }

        private void GenerateRandomRequirements()
        {
            _requirements.Clear();

            _requirements.Add(new RepairRequirement
            {
                type = ScrapType.Generic,
                amount = Random.Range(1, 3)
            });

            _requirements.Add(new RepairRequirement
            {
                type = ScrapType.Electronic,
                amount = 1
            });
        }
    }
}
