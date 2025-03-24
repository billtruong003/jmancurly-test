using System.Collections;
using Example.SimpleStage;
using Fusion;
using TMPro;
using UnityEngine;

public class CharacterName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterNameShadowText;

    private NetworkObject _networkedCharacter; // Reference to NetworkedCharacter

    private void Awake()
    {
        // Get the NetworkedCharacter component (assuming it's on a parent GameObject)
        _networkedCharacter = GetComponentInParent<NetworkObject>();

        if (_networkedCharacter == null)
        {
            Debug.LogError("CharacterName script requires a NetworkedCharacter component on a parent GameObject!");
            return; // Exit if NetworkedCharacter is not found
        }
        StartCoroutine(UpdateCharacterName());
    }

    private IEnumerator UpdateCharacterName()
    {
        while (true)
        {
            yield return new WaitUntil(() => _networkedCharacter.HasInputAuthority); // Wait until the NetworkedCharacter is spawned
            PlayerRef playerRef = _networkedCharacter.InputAuthority;
            characterNameText.text = "Player " + playerRef.PlayerId.ToString(); // Set the character name text
            characterNameShadowText.text = "Player " + playerRef.PlayerId.ToString(); // Set the character name shadow text
        }
    }
}