using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CharacterCustomization
{
    public class CharacterCustomizationManager : MonoBehaviour
    {
        [Header("Part Data")]
        [SerializeField] private List<CharacterPartData> allPartData;

        [Header("Current Character")]
        [SerializeField] private CharacterPresetData currentPreset;

        private Dictionary<CharacterPartType, Dictionary<string, GameObject>> partPrefabs;
        private Dictionary<CharacterPartType, GameObject> currentParts;

        public event System.Action<CharacterPartType, string> OnPartChanged;

        private void Awake()
        {
            InitializePartDictionaries();
        }

        private void InitializePartDictionaries()
        {
            partPrefabs = new Dictionary<CharacterPartType, Dictionary<string, GameObject>>();
            currentParts = new Dictionary<CharacterPartType, GameObject>();

            foreach (var partData in allPartData)
            {
                if (!partPrefabs.ContainsKey(partData.partType))
                {
                    partPrefabs[partData.partType] = new Dictionary<string, GameObject>();
                }
                partPrefabs[partData.partType][partData.partID] = partData.partPrefab;
            }
        }

        public void ChangePart(CharacterPartType partType, string partID)
        {
            if (!partPrefabs.ContainsKey(partType) || !partPrefabs[partType].ContainsKey(partID))
                return;

            if (currentParts.ContainsKey(partType))
            {
                Destroy(currentParts[partType]);
            }

            GameObject newPart = Instantiate(partPrefabs[partType][partID], transform);
            currentParts[partType] = newPart;
            OnPartChanged?.Invoke(partType, partID);
        }

        public CharacterPresetData SaveCurrentPreset()
        {
            CharacterPresetData newPreset = ScriptableObject.CreateInstance<CharacterPresetData>();
            newPreset.partPresets = currentParts.Select(p => new PartPreset
            {
                partType = p.Key,
                partID = p.Value.GetComponent<CharacterPart>().PartID
            }).ToList();

            return newPreset;
        }
    }
}

