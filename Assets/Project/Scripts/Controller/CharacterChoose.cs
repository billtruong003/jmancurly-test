using System;
using System.Collections;
using System.Collections.Generic;
using Example.ExpertMovement;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Import the EventSystems namespace

public class CharacterChoose : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterNameShadow;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private ExpertGameplayManager expertGameplayManager;
    [SerializeField] private Character[] characters;
    [SerializeField] private int selectedCharacter = 0;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f; // Tốc độ xoay
    private bool isDragging = false;
    private float startMouseX;

    [Serializable]
    public class Character
    {
        public string name;
        public GameObject showcase;
        public NetworkObject networkObject;
        public Color color;

        public void SetStatus(bool status)
        {
            showcase.SetActive(status);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        nextButton.onClick.AddListener(Next);
        previousButton.onClick.AddListener(Previous);
        if (expertGameplayManager == null)
        {
            Debug.Log("ExpertGameplayManager is not assigned to CharacterChoose script. Please assign it in the inspector.");
            return;
        }
        else
        {
            SetCharacter(selectedCharacter);
        }
        FusionBootstrap.OnNetworkStarted += OnNetworkStartedHandler;
    }

    private void OnDestroy()
    {
        FusionBootstrap.OnNetworkStarted -= OnNetworkStartedHandler;
    }
    private void OnNetworkStartedHandler()
    {
        // Gọi hàm để tắt object chọn nhân vật khi mạng đã khởi động**
        DisableCharacterChooseObject();
    }

    private void DisableCharacterChooseObject()
    {
        gameObject.SetActive(false);
        Debug.Log("Character Choose object disabled because network started.");
    }

    public void Next()
    {
        characters[selectedCharacter].SetStatus(false);
        selectedCharacter++;

        if (selectedCharacter >= characters.Length)
        {
            selectedCharacter = 0;
        }
        characters[selectedCharacter].SetStatus(true);
        SetCharacter(selectedCharacter);
    }

    public void Previous()
    {
        characters[selectedCharacter].SetStatus(false);
        selectedCharacter--;

        if (selectedCharacter < 0)
        {
            selectedCharacter = characters.Length - 1;
        }
        characters[selectedCharacter].SetStatus(true);
        SetCharacter(selectedCharacter);
    }

    public void SetCharacter(int index)
    {
        selectedCharacter = index;
        expertGameplayManager.PlayerPrefabs[0] = characters[selectedCharacter].networkObject;
        characterName.text = characters[selectedCharacter].name;
        characterNameShadow.text = characters[selectedCharacter].name;
        characterName.color = characters[selectedCharacter].color;
    }

    void Update()
    {
        // Kiểm tra nếu chuột trái được nhấn xuống
        if (Input.GetMouseButtonDown(0))
        {
            // Kiểm tra xem chuột có đang ở trên UI không (tùy chọn, nếu bạn muốn xoay chỉ khi click vào UI)
            //if (EventSystem.current.IsPointerOverGameObject())
            //{
            isDragging = true;
            startMouseX = Input.mousePosition.x;
            //}
        }
        // Kiểm tra nếu chuột trái được thả ra
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Nếu đang kéo chuột
        if (isDragging)
        {
            float mouseDeltaX = Input.mousePosition.x - startMouseX;
            // Xoay showcase của nhân vật hiện tại theo chiều ngang (trục Y)
            characters[selectedCharacter].showcase.transform.Rotate(Vector3.up, -mouseDeltaX * rotationSpeed * Time.deltaTime);
            startMouseX = Input.mousePosition.x; // Cập nhật vị trí chuột bắt đầu cho lần kéo tiếp theo
        }
    }
}