using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SG
{
    // Lớp quản lý UI của người chơi, chịu trách nhiệm quản lý các thành phần UI khác nhau trong trò chơi.
    public class PlayerUIManager : MonoBehaviour
    {
        // Singleton instance để dễ dàng truy cập từ các lớp khác
        public static PlayerUIManager instance;

        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient; // Biến kiểm soát việc khởi động trò chơi dưới dạng Client

        // Tham chiếu đến PlayerUIHudManager, dùng để quản lý các thành phần HUD của người chơi (như thanh stamina, máu, ...).
        [HideInInspector] public PlayerUIHudManager playerUIHudManager;
        // Tham chiếu đến PlayerUIPopUpManager, quản lý các cửa sổ pop-up (thông báo khi nhân vật chết).
        [HideInInspector] public PlayerUIPopUpManager PlayerUIPopUpManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this; // Gán instance nếu chưa có
            }
            else
            {
                Destroy(gameObject); // Hủy đối tượng nếu đã có instance khác
            }
            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();// Lấy thành phần PlayerUIHudManager từ các đối tượng con (child objects).
            PlayerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>(); // Lấy thành phần PlayerUIPopUpManager từ các đối tượng con.
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject); // Giữ đối tượng không bị hủy khi tải cảnh mới
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false; // Đặt lại biến để tránh lặp lại
                NetworkManager.Singleton.Shutdown(); // Tắt NetworkManager hiện tại nếu có
                NetworkManager.Singleton.StartClient(); // Bắt đầu chế độ Client để kết nối vào Host
            }
        }
    }
}

