using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Quản lý danh sách người chơi đang hoạt động trong phiên game.
    // Đảm bảo rằng không có người chơi nào được thêm hai lần và danh sách luôn được cập nhật sạch sẽ.
    public class WorldGameSessionManager : MonoBehaviour
    {
        // Biến tĩnh để lưu trữ phiên bản duy nhất của WorldGameSessionManager (Singleton Pattern)
        public static WorldGameSessionManager instance;

        [Header("Active Players In Session")]
        // Danh sách chứa các người chơi đang hoạt động trong phiên game
        public List<PlayerManager> players = new List<PlayerManager>();

        private void Awake()
        {
            // Kiểm tra nếu instance chưa được khởi tạo
            if (instance == null)
            {
                instance = this; // Gán instance là đối tượng hiện tại
            }
            else
            {
                // Nếu đã có instance, hủy đối tượng trùng lặp
                Destroy(gameObject);
            }
        }

        // Phương thức thêm người chơi vào danh sách người chơi đang hoạt động
        public void AddPlayerToActivePlayerList(PlayerManager player)
        {
            // Kiểm tra xem người chơi đã có trong danh sách chưa
            if (!players.Contains(player))
            {
                players.Add(player); // Thêm người chơi vào danh sách
            }

            // Xóa các mục null khỏi danh sách người chơi
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i); // Xóa người chơi null
                }
            }
        }

        // Phương thức xóa người chơi khỏi danh sách người chơi đang hoạt động
        public void RemovePlayerFromActivePlayerList(PlayerManager player)
        {
            // Kiểm tra xem người chơi có trong danh sách không
            if (players.Contains(player))
            {
                players.Remove(player); // Xóa người chơi khỏi danh sách
            }

            // Xóa các mục null khỏi danh sách người chơi
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i); // Xóa người chơi null
                }
            }
        }
    }
}

