using UnityEngine;

namespace CharacterCustomization
{
    public class CharacterPart : MonoBehaviour
    {
        [SerializeField] private string partID;
        [SerializeField] private CharacterPartType partType;
        [SerializeField] private GenderType genderType;

        public string PartID => partID;
        public CharacterPartType PartType => partType;
        public GenderType GenderType => genderType;
    }
}