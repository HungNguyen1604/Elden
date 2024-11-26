using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace SG
{
    // Lớp này quản lý các thông tin liên quan đến mạng của nhân vật người chơi.
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player; // Khai báo biến để lưu trữ tham chiếu tới PlayerManager.

        // Biến này lưu tên nhân vật. 
        // Sử dụng NetworkVariable để đảm bảo tên nhân vật có thể được đồng bộ hóa giữa các client.
        // FixedString64Bytes đảm bảo rằng tên nhân vật không vượt quá 64 ký tự.
        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Equipment")]
        public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Biến mạng cho vũ khí tay phải
        // currentRightHandWeaponID được khởi tạo với giá trị 0, có thể đọc bởi mọi người (Everyone)
        // và chỉ có thể ghi bởi chủ sở hữu (Owner) của biến này.
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // Biến mạng cho vũ khí tay trái
        // currentLeftHandWeaponID cũng được khởi tạo với giá trị 0, có thể đọc bởi mọi người (Everyone)
        // và chỉ có thể ghi bởi chủ sở hữu (Owner) của biến này.
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        // Khai báo biến mạng isUsingRightHand để theo dõi trạng thái sử dụng tay phải
        // Mặc định là false, có thể được đọc bởi mọi người và chỉ người sở hữu (Owner) mới có quyền ghi
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Khai báo biến mạng isUsingLeftHand để theo dõi trạng thái sử dụng tay trái
        // Mặc định là false, có thể được đọc bởi mọi người và chỉ người sở hữu (Owner) mới có quyền ghi
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>(); // Lấy tham chiếu đến PlayerManager trên đối tượng hiện tại.
        }


        // Phương thức để thiết lập hành động tay của nhân vật
        public void SetCharacterActionHand(bool rightHandAction) // Tham số 'rightHandAction' xác định xem hành động sẽ sử dụng tay phải hay tay trái
        {
            if (rightHandAction)
            {
                // Nếu hành động sử dụng tay phải:
                isUsingLeftHand.Value = false;  // Đặt tay trái không sử dụng
                isUsingRightHand.Value = true;   // Đặt tay phải đang sử dụng
            }
            else
            {
                // Nếu hành động không sử dụng tay phải (sử dụng tay trái):
                isUsingLeftHand.Value = true;    // Đặt tay trái đang sử dụng
                isUsingRightHand.Value = false;   // Đặt tay phải không sử dụng
            }
        }

        // Phương thức này thiết lập giá trị máu tối đa mới dựa trên chỉ số sinh lực.
        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            // Tính toán giá trị máu tối đa mới dựa trên chỉ số sinh lực mới và cập nhật giá trị maxHealth.
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            // Cập nhật giá trị hiển thị máu tối đa mới trên giao diện người dùng.
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            // Đặt giá trị sức khỏe hiện tại bằng giá trị máu tối đa mới, đảm bảo nhân vật luôn có sức khỏe đầy đủ.
            currentHealth.Value = maxHealth.Value;
        }

        // Phương thức này thiết lập giá trị stamina tối đa mới dựa trên chỉ số sức bền.
        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            // Tính toán giá trị stamina tối đa mới dựa trên chỉ số sức bền mới và cập nhật giá trị maxStamina.
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
            // Cập nhật giá trị hiển thị stamina tối đa mới trên giao diện người dùng.
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
            // Đặt giá trị stamina hiện tại bằng giá trị stamina tối đa mới
            currentStamina.Value = maxStamina.Value;
        }


        // Hàm này được gọi khi ID vũ khí tay phải thay đổi
        public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
        {
            // Tạo vũ khí mới từ cơ sở dữ liệu vũ khí dựa trên ID mới
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));

            // Cập nhật vũ khí tay phải của người chơi trong hệ thống quản lý kho đồ
            player.playerInventoryManager.currentRightHandWeapon = newWeapon;

            // Tải vũ khí mới vào tay phải của nhân vật
            player.playerEquipmentManager.LoadRightWeapon();

            // Nếu nhân vật thuộc về người chơi (để kiểm tra client chủ sở hữu)
            if (player.IsOwner)
            {
                // Cập nhật biểu tượng ô nhanh cho vũ khí tay phải trên UI
                PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon(newID);
            }
        }

        // Hàm này được gọi khi ID vũ khí tay trái thay đổi
        public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
        {
            // Tạo vũ khí mới từ cơ sở dữ liệu vũ khí dựa trên ID mới
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));

            // Cập nhật vũ khí tay trái của người chơi trong hệ thống quản lý kho đồ
            player.playerInventoryManager.currentLeftHandWeapon = newWeapon;

            // Tải vũ khí mới vào tay trái của nhân vật
            player.playerEquipmentManager.LoadLeftWeapon();

            // Nếu nhân vật thuộc về người chơi (để kiểm tra client chủ sở hữu)
            if (player.IsOwner)
            {
                // Cập nhật biểu tượng ô nhanh cho vũ khí tay trái trên UI
                PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon(newID);
            }
        }


        // Phương thức này được gọi khi ID của vũ khí đang sử dụng thay đổi
        public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
        {
            // Tạo một vũ khí mới từ ID mới
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));

            // Cập nhật vũ khí đang được sử dụng trong quản lý chiến đấu của người chơi
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;
        }





        // Hàm RPC để thông báo cho server về hành động vũ khí mà client thực hiện
        // clientID: ID của client đang thực hiện hành động
        // actionID: ID của hành động vũ khí (ví dụ: tấn công, phòng thủ,...)
        // weaponID: ID của vũ khí đang sử dụng
        [ServerRpc]
        public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
        {
            // Chỉ thực hiện nếu đây là server
            if (IsServer)
            {
                // Thông báo lại cho các client khác thông qua Client RPC
                NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
            }
        }

        // Hàm Client RPC để đồng bộ hành động vũ khí với các client khác
        // clientID: ID của client đã thực hiện hành động
        // actionID: ID của hành động vũ khí
        // weaponID: ID của vũ khí đang sử dụng
        [ClientRpc]
        private void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
        {
            // Kiểm tra nếu client hiện tại không phải là client đã thực hiện hành động ban đầu
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                // Thực hiện hành động dựa trên ID hành động và ID vũ khí
                PerformWeaponBasedAction(actionID, weaponID);
            }
        }

        // Hàm thực hiện hành động dựa trên ID hành động và vũ khí
        // actionID: ID của hành động vũ khí (ví dụ: tấn công, phòng thủ,...)
        // weaponID: ID của vũ khí đang sử dụng
        private void PerformWeaponBasedAction(int actionID, int weaponID)
        {
            // Lấy đối tượng WeaponItemAction dựa trên weaponID từ WorldActionManager
            WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(weaponID);

            // Kiểm tra nếu hành động vũ khí không null
            if (weaponAction != null)
            {
                // Thực hiện hành động với người chơi và vũ khí đã được lấy từ WorldItemDatabase
                weaponAction.AttemptToPerformAction(player, WorldItemDatabase.instance.GetWeaponByID(weaponID));
            }
            else
            {
                // In ra lỗi nếu hành động không tồn tại
                Debug.LogError("Hành động không tồn tại, không thể thực hiện");
            }
        }

    }
}



