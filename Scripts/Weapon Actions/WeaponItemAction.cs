using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo một ScriptableObject cho các hành động vũ khí, có thể tạo qua menu Unity
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]

    public class WeaponItemAction : ScriptableObject
    {
        // ID của hành động, giúp định danh hành động này
        public int actionID;

        // Hàm thực hiện hành động, có thể được override trong các class con
        public virtual void AttemptToPerformAction(PlayerManager playerPerformAction, WeaponItem weaponPerformAction)
        {
            // Kiểm tra nếu người chơi hiện tại là chủ sở hữu
            if (playerPerformAction.IsOwner)
            {
                // Cập nhật ID của vũ khí đang sử dụng cho NetworkManager của người chơi
                playerPerformAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformAction.itemID;
            }
        }
    }
}
