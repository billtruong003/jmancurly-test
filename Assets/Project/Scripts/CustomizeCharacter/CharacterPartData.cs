using UnityEngine;
using System.Collections.Generic;

namespace CharacterCustomization
{
    [CreateAssetMenu(fileName = "CharacterPartData", menuName = "Character/Customization/PartData")]
    public class CharacterPartData : ScriptableObject
    {
        [Header("Basic Info")]
        public string partID;
        public string partName;
        public CharacterPartType partType;
        public GenderType genderType;

        [Header("Visual")]
        public GameObject partPrefab;
        public Sprite partIcon;

        [Header("Compatibility")]
        public List<string> compatibleParts = new List<string>();
    }
}
