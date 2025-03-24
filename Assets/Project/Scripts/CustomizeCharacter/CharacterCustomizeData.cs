using UnityEngine;
using System.Collections.Generic;

namespace CharacterCustomization
{
    public enum CharacterPartType
    {
        Head,
        Torso,
        Hips,
        Arms,
        Legs,
        Hair,
        FacialHair,
        Helmet,
        Shoulder,
        Back,
        HipsAttachment
    }

    public enum GenderType
    {
        Male,
        Female
    }

    [System.Serializable]
    public class PartPreset
    {
        public CharacterPartType partType;
        public string partID;
    }
}