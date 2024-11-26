using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp PlayerEquipmentManager quản lý trang bị cho nhân vật người chơi
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        // Biến để lưu trữ tham chiếu đến PlayerManager
        PlayerManager player;

        // Các slot để chứa mô hình vũ khí ở tay phải và tay trái
        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot;

        [SerializeField] WeaponManager rightWeaponManager; // Quản lý thông tin và sát thương cho vũ khí tay phải
        [SerializeField] WeaponManager leftWeaponManager; // Quản lý thông tin và sát thương cho vũ khí tay trái

        // Các mô hình vũ khí hiện tại ở tay phải và tay trái
        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

        // Phương thức được gọi khi đối tượng được khởi tạo
        protected override void Awake()
        {
            base.Awake();
            // Lấy tham chiếu đến PlayerManager
            player = GetComponent<PlayerManager>();
            // Khởi tạo các slot vũ khí
            InitializeWeaponSlots();
        }

        // Phương thức được gọi khi đối tượng bắt đầu hoạt động
        protected override void Start()
        {
            base.Start();
            // Tải vũ khí cho cả hai tay
            LoadWeaponOnBothHands();
        }

        // Khởi tạo các slot vũ khí cho tay phải và tay trái
        private void InitializeWeaponSlots()
        {
            // Lấy tất cả các slot vũ khí con từ các đối tượng con của đối tượng này
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            // Duyệt qua từng slot vũ khí đã lấy được
            foreach (var weaponSlot in weaponSlots)
            {
                // Kiểm tra xem slot vũ khí thuộc tay phải hay tay trái
                if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    // Gán slot vũ khí cho tay phải
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    // Gán slot vũ khí cho tay trái
                    leftHandSlot = weaponSlot;
                }
            }
        }


        // Tải vũ khí cho cả hai tay
        public void LoadWeaponOnBothHands()
        {
            LoadRightWeapon(); // Tải vũ khí cho tay phải
            LoadLeftWeapon();  // Tải vũ khí cho tay trái
        }

        public void SwitchRightWeapon()
        {
            // Nếu người chơi không phải chủ sở hữu (trong trường hợp multiplayer), không thực hiện tiếp
            if (!player.IsOwner)
                return;

            // Gọi animation đổi vũ khí tay phải
            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);
            // Khởi tạo vũ khí được chọn là null ban đầu
            WeaponItem selectionWeapon = null;
            // Tăng chỉ số index của vũ khí tay phải để chuyển sang vũ khí kế tiếp
            player.playerInventoryManager.rightHandWeaponIndex += 1;


            // Nếu chỉ số vượt quá giới hạn (0 - 2), quay lại vị trí ban đầu
            if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
            {
                // Reset chỉ số về 0 nếu vượt quá
                player.playerInventoryManager.rightHandWeaponIndex = 0;
                // Khởi tạo biến đếm số lượng vũ khí
                float weaponCount = 0;
                // Biến lưu vũ khí đầu tiên tìm thấy
                WeaponItem firstWeapon = null;
                // Vị trí của vũ khí đầu tiên
                int firtWeaponPosition = 0;

                // Lặp qua tất cả các vũ khí trong các slot tay phải
                for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    // Nếu vũ khí hiện tại không phải là vũ khí tay không (unarmed weapon)
                    if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        // Tăng đếm số lượng vũ khí có sẵn (không phải vũ khí tay không)
                        weaponCount += 1;

                        // Gán vũ khí đầu tiên tìm thấy vào biến firstWeapon, nếu chưa có vũ khí nào được gán trước đó
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                            // Lưu lại vị trí của vũ khí đầu tiên tìm thấy
                            firtWeaponPosition = i;
                        }
                    }
                }


                // Nếu chỉ có một hoặc không có vũ khí nào, trang bị vũ khí tay không (unarmed weapon)
                if (weaponCount <= 1)
                {
                    // Đặt chỉ số của vũ khí tay phải về -1, thể hiện rằng không có vũ khí nào được trang bị
                    player.playerInventoryManager.rightHandWeaponIndex = -1;

                    // Gán vũ khí tay không (unarmed weapon) vào biến selectionWeapon
                    selectionWeapon = WorldItemDatabase.instance.unarmedWeapon;

                    // Cập nhật thông tin vũ khí hiện tại của người chơi lên mạng (dành cho chế độ multiplayer)
                    // Đặt ID của vũ khí tay phải bằng ID của vũ khí tay không
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectionWeapon.itemID;
                }
                else
                {
                    // Nếu có nhiều hơn một vũ khí hợp lệ trong các slot vũ khí
                    // Gán chỉ số của vũ khí tay phải bằng vị trí của vũ khí đầu tiên tìm thấy
                    player.playerInventoryManager.rightHandWeaponIndex = firtWeaponPosition;

                    // Cập nhật ID của vũ khí tay phải bằng ID của vũ khí đầu tiên
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
                }
                return;
            }

            // Duyệt qua tất cả các vũ khí trong slot tay phải
            foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
            {
                // Nếu vũ khí hiện tại không phải tay không, chọn vũ khí đó
                if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    // Gán vũ khí được chọn
                    selectionWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];

                    // Cập nhật vũ khí tay phải hiện tại qua mạng
                    player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                    return;
                }

            }
            // Nếu không chọn được vũ khí nào (selectionWeapon == null) và chỉ số vũ khí tay phải hiện tại nhỏ hơn 2
            if (selectionWeapon == null && player.playerInventoryManager.rightHandWeaponIndex < 2)
            {
                // Gọi hàm SwitchRightWeapon để thực hiện lại việc đổi vũ khí
                SwitchRightWeapon();
            }
        }

        // Tải vũ khí cho tay phải
        public void LoadRightWeapon()
        {
            // Kiểm tra xem có vũ khí nào được trang bị ở tay phải không
            if (player.playerInventoryManager.currentRightHandWeapon != null)
            {
                // Dỡ bỏ mô hình vũ khí hiện tại khỏi tay phải, nếu có
                rightHandSlot.UnloadWeapon();
                // Tạo mô hình vũ khí mới từ kho đồ (inventory) dựa trên vũ khí hiện tại của tay phải
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                // Tải mô hình vũ khí mới vào slot tay phải (gắn mô hình vào vị trí slot)
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                // Lấy thành phần WeaponManager từ mô hình vũ khí mới tạo
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                // Thiết lập sát thương và các chỉ số vũ khí cho vũ khí tay phải
                rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            }
        }


        public void SwitchLeftWeapon()
        {
            // Nếu người chơi không phải là chủ sở hữu (trong trường hợp multiplayer), không thực hiện tiếp
            if (!player.IsOwner)
                return;

            // Gọi animation đổi vũ khí tay trái
            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

            // Khởi tạo vũ khí được chọn là null ban đầu
            WeaponItem selectionWeapon = null;

            // Tăng chỉ số index của vũ khí tay trái để chuyển sang vũ khí kế tiếp
            player.playerInventoryManager.leftHandWeaponIndex += 1;

            // Nếu chỉ số vượt quá giới hạn (0 - 2), quay lại vị trí ban đầu
            if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 2)
            {
                // Reset chỉ số về 0 nếu vượt quá
                player.playerInventoryManager.leftHandWeaponIndex = 0;

                // Khởi tạo biến đếm số lượng vũ khí
                float weaponCount = 0;

                // Biến lưu vũ khí đầu tiên tìm thấy
                WeaponItem firstWeapon = null;

                // Vị trí của vũ khí đầu tiên
                int firstWeaponPosition = 0;

                // Lặp qua tất cả các vũ khí trong các slot tay trái
                for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
                {
                    // Nếu vũ khí hiện tại không phải là vũ khí tay không (unarmed weapon)
                    if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        // Tăng đếm số lượng vũ khí có sẵn (không phải vũ khí tay không)
                        weaponCount += 1;

                        // Gán vũ khí đầu tiên tìm thấy vào biến firstWeapon, nếu chưa có vũ khí nào được gán trước đó
                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                            // Lưu lại vị trí của vũ khí đầu tiên tìm thấy
                            firstWeaponPosition = i;
                        }
                    }
                }

                // Nếu chỉ có một hoặc không có vũ khí nào, trang bị vũ khí tay không (unarmed weapon)
                if (weaponCount <= 1)
                {
                    // Đặt chỉ số của vũ khí tay trái về -1, thể hiện rằng không có vũ khí nào được trang bị
                    player.playerInventoryManager.leftHandWeaponIndex = -1;

                    // Gán vũ khí tay không (unarmed weapon) vào biến selectionWeapon
                    selectionWeapon = WorldItemDatabase.instance.unarmedWeapon;

                    // Cập nhật thông tin vũ khí hiện tại của người chơi lên mạng (dành cho chế độ multiplayer)
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = selectionWeapon.itemID;
                }
                else
                {
                    // Nếu có nhiều hơn một vũ khí hợp lệ trong các slot vũ khí
                    // Gán chỉ số của vũ khí tay trái bằng vị trí của vũ khí đầu tiên tìm thấy
                    player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;

                    // Cập nhật ID của vũ khí tay trái bằng ID của vũ khí đầu tiên
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
                }
                return;
            }

            // Duyệt qua tất cả các vũ khí trong slot tay trái
            foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInLeftHandSlots)
            {
                // Nếu vũ khí hiện tại không phải tay không, chọn vũ khí đó
                if (player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    // Gán vũ khí được chọn
                    selectionWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];

                    // Cập nhật vũ khí tay trái hiện tại qua mạng
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
                    return;
                }
            }

            // Nếu không chọn được vũ khí nào (selectionWeapon == null) và chỉ số vũ khí tay trái hiện tại nhỏ hơn 2
            if (selectionWeapon == null && player.playerInventoryManager.leftHandWeaponIndex < 2)
            {
                // Gọi hàm SwitchLeftWeapon để thực hiện lại việc đổi vũ khí
                SwitchLeftWeapon();
            }
        }

        // Tải vũ khí cho tay trái
        public void LoadLeftWeapon()
        {
            // Kiểm tra xem có vũ khí nào được trang bị ở tay trái không
            if (player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                // Dỡ bỏ mô hình vũ khí hiện tại khỏi tay trái, nếu có
                leftHandSlot.UnloadWeapon();
                // Tạo mô hình vũ khí mới từ kho đồ (inventory) dựa trên vũ khí hiện tại của tay trái
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                // Tải mô hình vũ khí mới vào slot tay trái (gắn mô hình vào vị trí slot)
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                // Lấy thành phần WeaponManager từ mô hình vũ khí mới tạo
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                // Thiết lập sát thương và các chỉ số vũ khí cho vũ khí tay trái
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
            }
        }

        // Mở collider gây sát thương cho vũ khí hiện tại
        public void OpenDamageCollider()
        {
            // Kiểm tra nếu nhân vật đang sử dụng tay phải
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                // Nếu đúng, kích hoạt collider gây sát thương cho vũ khí ở tay phải
                rightWeaponManager.meleeDamageCollider.EnableDamageCollider();

                // Phát âm thanh "whoosh" từ mảng âm thanh của vũ khí ở tay phải,
                // tạo cảm giác tốc độ và lực cho đòn đánh
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.whooshes));
            }
            // Kiểm tra nếu nhân vật đang sử dụng tay trái
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                // Nếu đúng, kích hoạt collider gây sát thương cho vũ khí ở tay trái
                leftWeaponManager.meleeDamageCollider.EnableDamageCollider();

                // Phát âm thanh "whoosh" từ mảng âm thanh của vũ khí ở tay trái,
                // tạo hiệu ứng âm thanh cho đòn đánh từ tay trái
                player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.whooshes));
            }

            // (Ghi chú dành cho bạn phát triển thêm) Thực hiện phát hoạt ảnh tấn công khi mở collider
            // Điều này sẽ giúp kích hoạt hình ảnh tấn công đồng thời với âm thanh và sát thương
            // play animation (FPX hoặc đòn đánh tùy bạn cần thiết)
        }


        // Đóng collider gây sát thương cho vũ khí hiện tại
        public void CloseDamageCollider()
        {
            // Kiểm tra nếu nhân vật đang sử dụng tay phải
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                // Nếu đúng, vô hiệu hóa collider gây sát thương cho vũ khí ở tay phải
                rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
            // Kiểm tra nếu nhân vật đang sử dụng tay trái
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                // Nếu đúng, vô hiệu hóa collider gây sát thương cho vũ khí ở tay trái
                leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
            // Gọi hoạt ảnh tấn công (play animation) khi đóng collider
            // play fpx
        }

    }
}

