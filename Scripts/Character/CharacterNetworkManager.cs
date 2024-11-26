using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        // Tham chiếu đến CharacterManager để lấy đối tượng Animator
        CharacterManager character;

        [Header("Position")]
        // NetworkVariable để lưu trữ vị trí của nhân vật, có thể được đọc bởi tất cả mọi người và chỉ có thể ghi bởi chủ sở hữu.
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // NetworkVariable để lưu trữ góc quay của nhân vật, cũng có thể được đọc bởi tất cả mọi người và chỉ có thể ghi bởi chủ sở hữu.
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public Vector3 networkPositionVelocity;// Vận tốc để làm mượt chuyển động
        public float networkPositionSmoothTime = 0.1f; // Thời gian làm mượt vị trí
        public float networkRotationSmoothTime = 0.1f; // Thời gian làm mượt góc quay

        [Header("Animator")]
        // Biến mạng xác định trạng thái di chuyển của nhân vật AI (di chuyển hoặc không di chuyển).
        // Mọi người trong mạng có thể đọc giá trị này, nhưng chỉ chủ sở hữu mới có thể thay đổi nó.
        public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // Giá trị di chuyển ngang, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Giá trị di chuyển dọc, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Tổng hợp lượng di chuyển, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Target")]
        // Lưu trữ ID của đối tượng mạng (NetworkObject) mà nhân vật đang nhắm đến, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        [Header("Flags")]
        // Quản lý trạng thái lock-on trong mạng, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Trạng thái chạy nhanh, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Trạng thái nhảy, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Trạng thái đang sạc đòn tấn công, chỉ chủ sở hữu có quyền ghi, mọi người có thể đọc.
        public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Resources")] // Lưu trữ và đồng bộ thông tin về tài nguyên(máu, stamina).
        // Máu hiện tại của nhân vật, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Máu tối đa của nhân vật, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // Stamina hiện tại, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Stamina tối đa, chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        [Header("Stats")]
        // Chỉ số "vitality" (sinh lực), chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        // Chỉ số "endurance" (sức bền), chủ sở hữu được ghi, mọi người có thể đọc
        public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            // Lấy component CharacterManager trên đối tượng hiện tại
            character = GetComponent<CharacterManager>();
        }

        // Phương thức kiểm tra lượng sinh lực của nhân vật
        public void CheckHP(int oldValue, int newValue)
        {
            // Kiểm tra nếu máu hiện tại của nhân vật <= 0, tức là nhân vật đã chết
            if (currentHealth.Value <= 0)
            {
                // Bắt đầu coroutine xử lý sự kiện chết của nhân vật
                StartCoroutine(character.ProcessDeathEvent());
            }

            // Kiểm tra nếu nhân vật này là chủ sở hữu (đảm bảo chỉ chủ sở hữu mới có thể thay đổi máu)
            if (character.IsOwner)
            {
                // Nếu máu hiện tại vượt quá mức máu tối đa cho phép
                if (currentHealth.Value > maxHealth.Value)
                {
                    // Đặt lại giá trị máu hiện tại bằng với mức máu tối đa
                    currentHealth.Value = maxHealth.Value;
                }
            }
        }


        // Phương thức này được gọi khi ID mục tiêu tấn công (lock-on target) thay đổi.
        public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
        {
            // Nếu đối tượng không phải là người sở hữu, cập nhật mục tiêu tấn công cho nhân vật.
            if (!IsOwner)
            {
                // Lấy đối tượng từ SpawnedObjects dựa trên newID và gán cho currentTarget
                character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
            }
        }

        // Phương thức này được gọi khi trạng thái lock-on của nhân vật thay đổi.
        public void OnIsLockedOnChange(bool old, bool isLockedOn)
        {
            // Nếu không còn ở trạng thái lock-on, đặt lại mục tiêu tấn công cho nhân vật.
            if (!isLockedOn)
            {
                // Đặt lại currentTarget thành null khi không còn lock-on
                character.characterCombatManager.currentTarget = null;
            }
        }


        // Phương thức được gọi khi trạng thái sạc đòn tấn công thay đổi.
        // Cập nhật animator để phản ánh trạng thái mới của việc sạc đòn tấn công.
        public void OnIsChargingAttackChanged(bool oldStatus, bool newStatus)
        {
            // Thiết lập giá trị của biến "isChargingAttack" trong animator theo trạng thái mới.
            character.animator.SetBool("isChargingAttack", isChargingAttack.Value);
        }


        // Phương thức được gọi khi giá trị của isMoving thay đổi
        public void OnIsMovingChanged(bool oldStatus, bool newStatus)
        {
            // Cập nhật biến "isMoving" trong Animator của nhân vật
            // Điều này giúp đồng bộ trạng thái di chuyển của nhân vật với trạng thái mạng
            character.animator.SetBool("isMoving", isMoving.Value);
        }


        [ServerRpc] // Đây là một phương thức gọi từ máy khách tới máy chủ để thông báo về hành động hoạt ảnh mà máy khách đã thực hiện.
        public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // Kiểm tra xem có phải là máy chủ không
            if (IsServer)
            {
                // Nếu là máy chủ, gọi hàm ClientRpc để phát động hoạt ảnh cho tất cả máy khách
                PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc] // Phương thức này được gọi từ máy chủ đến tất cả các máy khách ngoại trừ máy khách gọi phương thức ban đầu.
        public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // Kiểm tra xem clientID có phải là ID của máy khách hiện tại không
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                // Nếu không, thực hiện hoạt ảnh từ máy chủ
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }
        // Phương thức này xử lý việc thực hiện hoạt ảnh được chỉ định từ máy chủ.
        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            // Cập nhật trạng thái root motion
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;

            // Chuyển đổi sang hoạt ảnh mục tiêu với thời gian chuyển đổi 0.2 giây
            character.animator.CrossFade(animationID, 0.2f);
        }

        //Attack Animation

        [ServerRpc] // Đây là một phương thức gọi từ máy khách tới máy chủ để thông báo về hành động hoạt ảnh mà máy khách đã thực hiện.
        public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // Kiểm tra xem có phải là máy chủ không
            if (IsServer)
            {
                // Nếu là máy chủ, gọi hàm ClientRpc để phát động hoạt ảnh cho tất cả máy khách
                PlayAttackActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc] // Phương thức này được gọi từ máy chủ đến tất cả các máy khách ngoại trừ máy khách gọi phương thức ban đầu.
        public void PlayAttackActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // Kiểm tra xem clientID có phải là ID của máy khách hiện tại không
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                // Nếu không, thực hiện hoạt ảnh từ máy chủ
                PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
            }
        }
        // Phương thức này xử lý việc thực hiện hoạt ảnh được chỉ định từ máy chủ.
        private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            // Cập nhật trạng thái root motion
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;

            // Chuyển đổi sang hoạt ảnh mục tiêu với thời gian chuyển đổi 0.2 giây
            character.animator.CrossFade(animationID, 0.2f);
        }




        // Xử lý thông báo gây sát thương đến máy chủ khi nhân vật tấn công
        [ServerRpc(RequireOwnership = false)]
        public void NotifyTheServerOfCharacterDamageServerRpc(
            ulong damagedCharacterID, // ID của nhân vật bị thiệt hại
            ulong characterCausingDamageID, // ID của nhân vật gây thiệt hại
            float physicalDamage, // Sát thương vật lý
            float magicDamage, // Sát thương phép thuật
            float fireDamage, // Sát thương lửa
            float holyDamage, // Sát thương thánh
            float poiseDamage, // Sát thương poise (độ vững vàng của nhân vật)
            float angleHitFrom, // Góc mà nhân vật bị tấn công
            float contactPointX, // Tọa độ X của điểm va chạm
            float contactPointY, // Tọa độ Y của điểm va chạm
            float contactPointZ) // Tọa độ Z của điểm va chạm
        {
            // Kiểm tra xem đây có phải là máy chủ hay không
            if (IsServer)
            {
                // Thông báo đến tất cả các client về thiệt hại đã xảy ra
                NotifyTheServerOfCharacterDamageClientRpc(
                    damagedCharacterID,
                    characterCausingDamageID,
                    physicalDamage,
                    magicDamage,
                    fireDamage,
                    holyDamage,
                    poiseDamage,
                    angleHitFrom,
                    contactPointX,
                    contactPointY,
                    contactPointZ);
            }
        }

        // Thông báo cho tất cả các client về thiệt hại của nhân vật
        [ClientRpc]
        public void NotifyTheServerOfCharacterDamageClientRpc(
            ulong damagedCharacterID, // ID của nhân vật bị thiệt hại
            ulong characterCausingDamageID, // ID của nhân vật gây thiệt hại
            float physicalDamage, // Sát thương vật lý
            float magicDamage, // Sát thương phép thuật
            float fireDamage, // Sát thương lửa
            float holyDamage, // Sát thương thánh
            float poiseDamage, // Sát thương poise
            float angleHitFrom, // Góc tấn công
            float contactPointX, // Tọa độ X của điểm va chạm
            float contactPointY, // Tọa độ Y của điểm va chạm
            float contactPointZ) // Tọa độ Z của điểm va chạm
        {
            // Xử lý thiệt hại cho nhân vật bị tấn công từ máy chủ
            ProcessCharacterDamageFromServer(
                damagedCharacterID,
                characterCausingDamageID,
                physicalDamage,
                magicDamage,
                fireDamage,
                holyDamage,
                poiseDamage,
                angleHitFrom,
                contactPointX,
                contactPointY,
                contactPointZ);
        }

        // Phương thức xử lý thiệt hại cho nhân vật bị tấn công
        public void ProcessCharacterDamageFromServer(
            ulong damagedCharacterID, // ID của nhân vật bị thiệt hại
            ulong characterCausingDamageID, // ID của nhân vật gây thiệt hại
            float physicalDamage, // Sát thương vật lý
            float magicDamage, // Sát thương phép thuật
            float fireDamage, // Sát thương lửa
            float holyDamage, // Sát thương thánh
            float poiseDamage, // Sát thương poise
            float angleHitFrom, // Góc tấn công
            float contactPointX, // Tọa độ X của điểm va chạm
            float contactPointY, // Tọa độ Y của điểm va chạm
            float contactPointZ) // Tọa độ Z của điểm va chạm
        {
            // Lấy tham chiếu đến nhân vật bị thiệt hại từ mạng
            CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
            // Lấy tham chiếu đến nhân vật gây thiệt hại từ mạng
            CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

            // Tạo hiệu ứng thiệt hại từ hiệu ứng toàn cục
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

            // Gán các giá trị sát thương cho hiệu ứng thiệt hại
            damageEffect.physicalDamage = physicalDamage; // Sát thương vật lý
            damageEffect.magicDamage = magicDamage; // Sát thương phép thuật
            damageEffect.fireDamage = fireDamage; // Sát thương lửa
            damageEffect.holyDamage = holyDamage; // Sát thương thánh
            damageEffect.poiseDamage = poiseDamage; // Sát thương poise
            damageEffect.angleHitFrom = angleHitFrom; // Gán góc tấn công
                                                      // Gán tọa độ va chạm
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
            damageEffect.characterCausingDamage = characterCausingDamage; // Gán nhân vật gây thiệt hại cho hiệu ứng

            // Xử lý hiệu ứng thiệt hại trên nhân vật
            damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }
    }
}

