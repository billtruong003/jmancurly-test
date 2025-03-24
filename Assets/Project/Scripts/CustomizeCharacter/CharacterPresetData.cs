using UnityEngine;
using System.Collections.Generic;

namespace CharacterCustomization
{
    [CreateAssetMenu(fileName = "CharacterPresetData", menuName = "Character/Customization/PresetData")]
    public class CharacterPresetData : ScriptableObject
    {
        public string presetID;
        public string presetName;
        public GenderType genderType;
        public Color skinColor;
        public List<PartPreset> partPresets = new List<PartPreset>();
    }
}