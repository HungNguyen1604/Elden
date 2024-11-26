using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    // Quản lý hoạt ảnh của nhân vật
    public class CharacterAnimatorManager : MonoBehaviour
    {
        // Tham chiếu đến CharacterManager để lấy đối tượng Animator
        CharacterManager character;

        // Biến lưu các hash giá trị của các tham số điều khiển animator
        private int horizontal;
        private int vertical;

        [Header("Flags")]
        public bool applyRootMotion = false;// Cờ quyết định xem root motion có được áp dụng hay không. 
        // Nếu true, vị trí của nhân vật sẽ được điều khiển bởi thông tin từ hoạt ảnh. 
        // Nếu false, vị trí sẽ do code điều khiển.

        [Header("Damage Animations")]
        // Lưu lại hoạt ảnh sát thương cuối cùng mà nhân vật đã thực hiện.
        public string lastDamageAnimationPlayed;

        // Các chuỗi lưu tên hoạt ảnh sát thương phía trước.
        [SerializeField] string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
        [SerializeField] string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";

        // Các chuỗi lưu tên hoạt ảnh sát thương phía sau.
        [SerializeField] string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
        [SerializeField] string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";

        // Các chuỗi lưu tên hoạt ảnh sát thương phía bên trái.
        [SerializeField] string hit_Left_Medium_01 = "Hit_Left_Medium_01";
        [SerializeField] string hit_Left_Medium_02 = "Hit_Left_Medium_02";

        // Các chuỗi lưu tên hoạt ảnh sát thương phía bên phải.
        [SerializeField] string hit_Right_Medium_01 = "Hit_Right_Medium_01";
        [SerializeField] string hit_Right_Medium_02 = "Hit_Right_Medium_02";


        // Danh sách các hoạt ảnh sát thương phía trước, lưu trữ nhiều hoạt ảnh để lựa chọn ngẫu nhiên.
        public List<string> forward_Medium_Damage = new List<string>();
        // Danh sách các hoạt ảnh sát thương phía sau.
        public List<string> backward_Medium_Damage = new List<string>();
        // Danh sách các hoạt ảnh sát thương phía bên trái.
        public List<string> left_Medium_Damage = new List<string>();
        // Danh sách các hoạt ảnh sát thương phía bên phải.
        public List<string> right_Medium_Damage = new List<string>();


        protected virtual void Awake()
        {
            // Lấy component CharacterManager trên đối tượng hiện tại
            character = GetComponent<CharacterManager>();

            // Chuyển đổi tên các tham số thành hash để tối ưu hiệu suất
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        // Phương thức khởi tạo (Start) được gọi khi script bắt đầu hoạt động.
        protected virtual void Start()
        {
            // Thêm các hoạt ảnh sát thương phía trước vào danh sách forward_Medium_Damage.
            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);

            // Thêm các hoạt ảnh sát thương phía sau vào danh sách backward_Medium_Damage.
            backward_Medium_Damage.Add(hit_Backward_Medium_01);
            backward_Medium_Damage.Add(hit_Backward_Medium_02);

            // Thêm các hoạt ảnh sát thương phía bên trái vào danh sách left_Medium_Damage.
            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);

            // Thêm các hoạt ảnh sát thương phía bên phải vào danh sách right_Medium_Damage.
            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);
        }


        // Phương thức này trả về một hoạt ảnh ngẫu nhiên từ danh sách truyền vào, loại bỏ hoạt ảnh đã chơi lần cuối.
        public string GetRandomAnimationFromList(List<string> animationList)
        {
            // Tạo một danh sách tạm finalist để lưu trữ các hoạt ảnh hợp lệ.
            List<string> finalist = new List<string>();

            // Sao chép tất cả các mục từ animationList vào finalist.
            foreach (var item in animationList)
            {
                finalist.Add(item);
            }

            // Loại bỏ hoạt ảnh đã chơi lần cuối khỏi danh sách finalist để tránh trùng lặp.
            finalist.Remove(lastDamageAnimationPlayed);

            // Xóa các phần tử null khỏi danh sách finalist (nếu có).
            for (int i = finalist.Count - 1; i > -1; i--) // Đúng ra phải là "i > -1" để duyệt từ cuối danh sách.
            {
                if (finalist[i] == null)
                {
                    finalist.RemoveAt(i);
                }
            }

            // Chọn một giá trị ngẫu nhiên từ finalist và trả về hoạt ảnh tương ứng.
            int randomValue = Random.Range(0, finalist.Count);

            return finalist[randomValue]; // Trả về hoạt ảnh ngẫu nhiên từ finalist.
        }

        // Hàm cập nhật các tham số hoạt ảnh theo hướng di chuyển
        public void UpdateAnimationMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            // Khởi tạo các giá trị snapped (gắn giá trị) cho chuyển động ngang và dọc
            float snappedHorizontal;
            float snappedVertical;

            // Gắn giá trị cho chuyển động ngang (Horizontal)
            if (horizontalMovement > 0 && horizontalMovement <= 0.5f)
            {
                snappedHorizontal = 0.5f; // Nếu chuyển động ngang từ 0.0 đến 0.5, gán giá trị 0.5
            }
            else if (horizontalMovement > 0.5f && horizontalMovement <= 1)
            {
                snappedHorizontal = 1; // Nếu chuyển động ngang từ 0.5 đến 1, gán giá trị 1
            }
            else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
            {
                snappedHorizontal = -0.5f; // Nếu chuyển động ngang từ -0.5 đến 0.0, gán giá trị -0.5
            }
            else if (horizontalMovement < -0.5f && horizontalMovement >= -1)
            {
                snappedHorizontal = -1; // Nếu chuyển động ngang từ -1 đến -0.5, gán giá trị -1
            }
            else
            {
                snappedHorizontal = 0; // Nếu không có chuyển động ngang thì gắn giá trị 0
            }

            // Gắn giá trị cho chuyển động dọc (Vertical)
            if (verticalMovement > 0 && verticalMovement <= 0.5f)
            {
                snappedVertical = 0.5f; // Nếu chuyển động dọc từ 0.0 đến 0.5, gán giá trị 0.5
            }
            else if (verticalMovement > 0.5f && verticalMovement <= 1)
            {
                snappedVertical = 1; // Nếu chuyển động dọc từ 0.5 đến 1, gán giá trị 1
            }
            else if (verticalMovement < 0 && verticalMovement >= -0.5f)
            {
                snappedVertical = -0.5f; // Nếu chuyển động dọc từ -0.5 đến 0.0, gán giá trị -0.5
            }
            else if (verticalMovement < -0.5f && verticalMovement >= -1)
            {
                snappedVertical = -1; // Nếu chuyển động dọc từ -1 đến -0.5, gán giá trị -1
            }
            else
            {
                snappedVertical = 0; // Nếu không có chuyển động dọc thì gắn giá trị 0
            }

            // Kiểm tra xem nhân vật có đang chạy nhanh hay không
            if (isSprinting)
            {
                snappedVertical = 2; // Nếu đang chạy nhanh, gán giá trị chuyển động dọc là 2
            }

            // Cập nhật các tham số của Animator cho hoạt ảnh dựa trên các giá trị snapped
            character.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime); // Cập nhật tham số ngang
            character.animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime); // Cập nhật tham số dọc
        }

        // Phát hoạt ảnh và kiểm soát trạng thái của nhân vật bằng PlayTargetActionAnimation
        public virtual void PlayTargetActionAnimation(
            string targetAnimation, // Tên của hoạt ảnh mà bạn muốn phát (ví dụ: Roll_Forward_01, Attack_01, etc.)
            bool isPerformingAction, // Cờ để xác định xem nhân vật có đang thực hiện hành động nào không (dùng để chặn các hành động khác khi đang thực hiện hoạt ảnh này)
            bool applyRootMotion = true, // Tùy chọn có sử dụng root motion hay không (root motion giúp hoạt ảnh di chuyển nhân vật một cách tự động)
            bool canRotate = false, // Cờ xác định xem nhân vật có thể xoay trong khi thực hiện hoạt ảnh này hay không, mặc định là false
            bool canMove = false) // Cờ xác định xem nhân vật có thể di chuyển trong khi thực hiện hoạt ảnh này hay không, mặc định là false    )
        {
            //Debug.Log("Đang phát hoạt ảnh: " + targetAnimation);

            // Thiết lập giá trị root motion cho nhân vật.
            // Nếu applyRootMotion là true, Unity sẽ sử dụng root motion từ hoạt ảnh để di chuyển nhân vật.
            this.applyRootMotion = applyRootMotion;

            // Phát hoạt ảnh được chỉ định bằng cách crossfade. CrossFade giúp chuyển đổi mượt mà từ hoạt ảnh hiện tại sang hoạt ảnh mới.
            // targetAnimation: Tên hoạt ảnh cần phát.
            // 0.2f: Thời gian để thực hiện quá trình crossfade (chuyển đổi giữa hai hoạt ảnh).
            character.animator.CrossFade(targetAnimation, 0.2f);

            // Đặt cờ isPerformingAction để quản lý trạng thái hành động của nhân vật.
            // Nếu isPerformingAction là true, nhân vật sẽ bị chặn thực hiện các hành động khác cho đến khi hoạt ảnh kết thúc.
            character.isPerformingAction = isPerformingAction;

            // Cập nhật cờ canRotate để xác định xem nhân vật có thể xoay hay không
            character.characterLocomotionManager.canRotate = canRotate;

            // Cập nhật cờ canMove để xác định xem nhân vật có thể di chuyển hay không
            character.characterLocomotionManager.canMove = canMove;

            // Gọi hàm RPC để thông báo cho máy chủ về hành động hoạt ảnh (đồng bộ hóa hành động)
            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void PlayTargetAttackActionAnimation(
            AttackType attackType, // attackType: Loại tấn công (LightAttack01, HeavyAttack, v.v.)
            string targetAnimation, // Tên của hoạt ảnh mà bạn muốn phát (ví dụ: Roll_Forward_01, Attack_01, etc.)
            bool isPerformingAction, // Cờ để xác định xem nhân vật có đang thực hiện hành động nào không (dùng để chặn các hành động khác khi đang thực hiện hoạt ảnh này)
            bool applyRootMotion = true, // Tùy chọn có sử dụng root motion hay không (root motion giúp hoạt ảnh di chuyển nhân vật một cách tự động)
            bool canRotate = false, // Cờ xác định xem nhân vật có thể xoay trong khi thực hiện hoạt ảnh này hay không, mặc định là false
            bool canMove = false) // Cờ xác định xem nhân vật có thể di chuyển trong khi thực hiện hoạt ảnh này hay không, mặc định là false    )
        {

            // Thiết lập loại tấn công hiện tại cho nhân vật dựa trên biến attackType
            character.characterCombatManager.currentAttackType = attackType;

            // Lưu tên của animation tấn công cuối cùng đã thực hiện vào biến lastAttackAnimationPerformed.
            // Điều này giúp hệ thống biết được hành động tấn công trước đó để phục vụ cho logic combo.
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;

            // Thiết lập giá trị root motion cho nhân vật.
            // Nếu applyRootMotion là true, Unity sẽ sử dụng root motion từ hoạt ảnh để di chuyển nhân vật.
            this.applyRootMotion = applyRootMotion;

            // Phát hoạt ảnh được chỉ định bằng cách crossfade. CrossFade giúp chuyển đổi mượt mà từ hoạt ảnh hiện tại sang hoạt ảnh mới.
            // targetAnimation: Tên hoạt ảnh cần phát.
            // 0.2f: Thời gian để thực hiện quá trình crossfade (chuyển đổi giữa hai hoạt ảnh).
            character.animator.CrossFade(targetAnimation, 0.2f);

            // Đặt cờ isPerformingAction để quản lý trạng thái hành động của nhân vật.
            // Nếu isPerformingAction là true, nhân vật sẽ bị chặn thực hiện các hành động khác cho đến khi hoạt ảnh kết thúc.
            character.isPerformingAction = isPerformingAction;

            // Cập nhật cờ canRotate để xác định xem nhân vật có thể xoay hay không
            character.characterLocomotionManager.canRotate = canRotate;
            // Cập nhật cờ canMove để xác định xem nhân vật có thể di chuyển hay không
            character.characterLocomotionManager.canMove = canMove;

            // Gọi hàm RPC để thông báo cho máy chủ về hành động hoạt ảnh (đồng bộ hóa hành động)
            character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        // Phương thức ảo để kích hoạt khả năng thực hiện combo.
        // Có thể được ghi đè bởi các lớp kế thừa nếu cần thêm logic.
        public virtual void EnableCanDoComBo()
        {
        }

        // Phương thức ảo để vô hiệu hóa khả năng thực hiện combo.
        // Có thể được ghi đè bởi các lớp kế thừa để tắt combo khi cần.
        public virtual void DisableCanDoComBo()
        {
        }

    }
}

