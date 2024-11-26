using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
namespace SG
{
    // Lớp PlayerManager kế thừa từ CharacterManager
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;// Khai báo biến playerAnimationManager để quản lý animation của người chơi
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager; // Khai báo biến playerLocomotionManager để quản lý chuyển động của người chơi
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;// Khai báo biến playerNetworkManager để quản lý các hành động liên quan đến mạng của người chơi.
        [HideInInspector] public PlayerStatsManager playerStatsManager;// Khai báo biến playerStatsManager để quản lý các chỉ số (stats) của người chơi(sức bền , thanh máu ,..)
        [HideInInspector] public PlayerInventoryManager playerInventoryManager; // Khai báo biến để quản lý kho đồ của người chơi
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager; // Khai báo biến để quản lý kho đồ của người chơi
        [HideInInspector] public PlayerCombatManager playerCombatManager; // Khai báo biến để quản lý các hành động chiến đấu của người chơi. 
        protected override void Awake()
        {
            base.Awake(); // Gọi phương thức Awake của lớp cha (CharacterManager)

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update(); // Gọi phương thức Update của lớp cha (CharacterManager)

            //Dòng mã này giúp ngăn chặn việc máy khách không phải là chủ sở hữu can thiệp vào hoặc cập nhật vị trí và góc quay của nhân vật
            if (!IsOwner)
                return;

            // Gọi phương thức AllMovement của playerLocomotionManager để xử lý mọi hành động di chuyển
            playerLocomotionManager.AllMovement();

            // Gọi phương thức RegenerateStamina để tái tạo stamina cho người chơi
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            //Nếu không phải nhân vật của người chơi sở hữu
            if (!IsOwner)
                return; //phương thức sẽ dừng lại và không thực hiện các thao tác tiếp theo
                        //nhằm ngăn camera hoạt động cho nhân vật không phải của người chơi.
            base.LateUpdate();

            // Thực hiện các hành động của camera
            PlayerCamera.instance.AllCameraActions();

        }


        // Phương thức được gọi khi đối tượng bị được đăng kí trong mạng.
        // Dăng ký các callback và sự kiện liên quan đến người chơi.
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // Đăng ký phương thức OnClientConnectedCallback để xử lý sự kiện khi client kết nối với server.
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;


            // Kiểm tra xem đối tượng hiện tại có phải là của người chơi đang điều khiển không
            if (IsOwner)
            {
                // Nếu đúng, gán đối tượng người chơi hiện tại cho camera của người chơi
                PlayerCamera.instance.player = this;
                // Gán đối tượng người chơi hiện tại cho trình quản lý đầu vào của người chơi
                PlayerInputManager.instance.player = this;
                // Gán đối tượng người chơi hiện tại cho trình quản lý lưu trữ game
                WorldSaveGameManager.instance.player = this;

                // Đăng ký sự kiện OnValueChanged cho vitality, gọi phương thức SetNewMaxHealthValue khi giá trị thay đổi.
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                // Đăng ký sự kiện OnValueChanged cho endurance, gọi phương thức SetNewMaxStaminaValue khi giá trị thay đổi.
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                // Gán sự kiện để lắng nghe thay đổi giá trị currentHealth, gọi phương thức SetNewHeathValue để cập nhật UI.
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHeathValue;
                // Gán sự kiện để lắng nghe thay đổi giá trị currentStamina, gọi phương thức SetNewStaminaValue để cập nhật UI cho thanh stamina.
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                // Gán sự kiện lắng nghe cho currentStamina, gọi ResetStaminaRegenTimer khi giá trị thay đổi để reset timer hồi phục stamina.
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }
            // Đăng ký sự kiện OnValueChanged cho biến currentHealth, khi giá trị này thay đổi, gọi phương thức CheckHP để kiểm tra tình trạng máu của nhân vật
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

            // Đăng ký sự kiện OnValueChanged cho biến isLockedOn, khi giá trị này thay đổi, gọi phương thức OnIsLockedOnChange để xử lý logic khi trạng thái lock-on thay đổi
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChange;

            // Đăng ký sự kiện OnValueChanged cho biến currentRightHandWeaponID,khi ID vũ khí tay phải thay đổi, phương thức OnCurrentRightHandWeaponIDChange sẽ được gọi
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            // Đăng ký sự kiện OnValueChanged cho biến currentLeftHandWeaponID,khi ID vũ khí tay trái thay đổi, phương thức OnCurrentLeftHandWeaponIDChange sẽ được gọi
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            // Đăng ký sự kiện OnValueChanged cho biến currentWeaponBeingUsed,khi ID của vũ khí hiện đang được sử dụng thay đổi, phương thức OnCurrentWeaponBeingUsedIDChange sẽ được gọi.
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            // Đăng ký sự kiện thay đổi trạng thái isChargingAttack.
            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;


            // Nếu là người chơi nhưng không phải là server, tải dữ liệu game từ nhân vật hiện tại
            if (IsOwner && !IsServer)
            {
                LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }


        // Phương thức được gọi khi đối tượng bị hủy trong mạng.
        // Hủy đăng ký các callback và sự kiện liên quan đến người chơi.
        public override void OnNetworkDespawn()
        {
            // Gọi phương thức cơ sở để xử lý hủy đối tượng mạng.
            base.OnNetworkDespawn();

            // Hủy đăng ký callback khi có client kết nối.
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

            // Kiểm tra nếu đối tượng thuộc quyền sở hữu của người chơi.
            if (IsOwner)
            {
                // Hủy đăng ký sự kiện thay đổi giá trị sinh lực và sức chịu đựng.
                playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

                // Hủy đăng ký sự kiện thay đổi giá trị sức khỏe và stamina trên UI.
                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHeathValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
            }

            // Hủy đăng ký sự kiện kiểm tra HP khi giá trị sức khỏe thay đổi.
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
            // Hủy đăng ký sự kiện thay đổi trạng thái lock-on.
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChange;
            // Hủy đăng ký sự kiện thay đổi ID vũ khí tay phải và tay trái.
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            // Hủy đăng ký sự kiện thay đổi vũ khí đang sử dụng.
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            // Hủy đăng ký sự kiện thay đổi trạng thái sạc đòn tấn công.
            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;
        }

        // Phương thức này được gọi khi một client kết nối thành công với server, với clientID là ID của client vừa kết nối.
        private void OnClientConnectedCallback(ulong clientID)
        {
            // Thêm người chơi hiện tại vào danh sách người chơi đang hoạt động khi kết nối thành công với máy chủ
            WorldGameSessionManager.instance.AddPlayerToActivePlayerList(this);

            // Kiểm tra xem người chơi hiện tại có phải là chủ sở hữu đối tượng và không phải là máy chủ (client)
            if (!IsServer && IsOwner)
            {
                // Lặp qua tất cả người chơi trong danh sách người chơi của WorldGameSessionManager
                foreach (var player in WorldGameSessionManager.instance.players)
                {
                    // Nếu người chơi hiện tại không phải chính mình
                    if (player != this)
                    {
                        // Tải dữ liệu nhân vật của người chơi khác để đồng bộ hóa với client mới
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }


        // Coroutine dùng để xử lý sự kiện chết của nhân vật trong game.
        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            // Kiểm tra xem đối tượng hiện tại có phải là của người chơi đang điều khiển không
            if (IsOwner)
            {
                // Gọi phương thức để hiển thị pop-up thông báo "Bạn đã chết"
                PlayerUIManager.instance.PlayerUIPopUpManager.SendYouDiedPopUp();
            }

            // Gọi phương thức cơ sở (base) để thực hiện các hành động chết khác (nếu có)
            return base.ProcessDeathEvent(manuallySelectDeathAnimation);
        }


        // Phương thức này thực hiện logic hồi sinh nhân vật.
        public override void ReviveCharacter()
        {
            base.ReviveCharacter(); // Gọi phương thức ReviveCharacter của lớp cha để thực hiện các bước khởi tạo cần thiết.

            // Kiểm tra xem đối tượng hiện tại có phải là của người chơi điều khiển không.
            if (IsOwner)
            {
                // Đặt lại trạng thái chết của nhân vật thành false để đánh dấu nhân vật đã hồi sinh.
                isDead.Value = false;

                // Đặt lại sức khỏe và stamina hiện tại của người chơi về giá trị tối đa.
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

                // Chạy hoạt ảnh hồi sinh (hoặc hoạt ảnh mặc định) cho nhân vật.
                playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
            }
        }

        // Phương thức này lưu trữ thông tin của nhân vật vào đối tượng dữ liệu hiện tại.
        // Các thông tin bao gồm tên nhân vật và tọa độ vị trí x, y, z của nhân vật trong không gian game.
        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex; // Lưu chỉ số cảnh hiện tại.
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString(); // Lưu tên nhân vật vào dữ liệu.
            currentCharacterData.xPosition = transform.position.x; // Lưu vị trí x của nhân vật.
            currentCharacterData.yPosition = transform.position.y; // Lưu vị trí y của nhân vật.
            currentCharacterData.zPosition = transform.position.z; // Lưu vị trí z của nhân vật.

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value; // Lưu giá trị currentHealth từ playerNetworkManager vào currentCharacterData.
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value; // Lưu giá trị currentStamina từ playerNetworkManager vào currentCharacterData.

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;  // Gán giá trị "vitality" từ playerNetworkManager vào dữ liệu nhân vật hiện tại.
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;  // Gán giá trị "endurance" từ playerNetworkManager vào dữ liệu nhân vật hiện tại.
        }

        // Phương thức này khôi phục thông tin của nhân vật từ đối tượng dữ liệu hiện tại.
        // Nó sẽ cập nhật tên nhân vật và tọa độ vị trí của nhân vật dựa trên dữ liệu đã lưu.
        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName; // Khôi phục tên nhân vật từ dữ liệu.
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition); // Tạo một vector 3 từ tọa độ đã lưu.
            transform.position = myPosition; // Cập nhật vị trí của nhân vật trong không gian game.

            playerNetworkManager.vitality.Value = currentCharacterData.vitality;  // Cập nhật giá trị "vitality" trong playerNetworkManager từ dữ liệu nhân vật hiện tại.
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;  // Cập nhật giá trị "endurance" trong playerNetworkManager từ dữ liệu nhân vật hiện tại.

            // Tính toán và cập nhật maxHealth dựa trên chỉ số vitality.
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
            // Tính toán và cập nhật maxStamina dựa trên chỉ số endurance.
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth; // Cập nhật giá trị currentHealth từ dữ liệu đã lưu.
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina; // Cập nhật giá trị currentStamina từ dữ liệu đã lưu.
            // Gọi phương thức để cập nhật thanh stamina tối đa trên giao diện người dùng với giá trị "maxStamina" hiện tại.
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);

        }


        // Đảm bảo rằng các thông tin liên quan đến vũ khí của người chơi (tay phải và tay trái) được tải và đồng bộ hóa với máy chủ.
        public void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            // Tải trạng thái vũ khí của người chơi khác khi tham gia máy chủ
            // Gọi phương thức để cập nhật vũ khí tay phải của người chơi với ID hiện tại
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);

            // Gọi phương thức để cập nhật vũ khí tay trái của người chơi với ID hiện tại
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

            // Nếu người chơi hiện tại đang lock-on vào một mục tiêu
            if (playerNetworkManager.isLockedOn.Value)
            {
                // Gọi phương thức để cập nhật mục tiêu lock-on với ID hiện tại
                playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
            }
        }





    }
}

