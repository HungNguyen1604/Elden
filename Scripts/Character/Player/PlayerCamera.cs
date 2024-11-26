using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SG
{
    // Lớp quản lý camera của người chơi, sử dụng Singleton để đảm bảo chỉ có một instance duy nhất
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance; // Singleton instance để dễ dàng truy cập từ các lớp khác

        public PlayerManager player; // Tham chiếu đến PlayerManager để biết nhân vật nào cần theo dõi
        public Camera cameraObject; // Camera chính của người chơi
        [SerializeField] Transform cameraPivotTransform; // Transform dùng để xoay camera lên/xuống

        [Header("Camera Setting")]
        private float cameraSmoothSpeed = 1f; // Tốc độ mượt mà của camera khi theo dõi nhân vật
        [SerializeField] float leftAndRightRotationSpeed = 220f; // Tốc độ xoay camera trái/phải
        [SerializeField] float upAndDownRotationSpeed = 220f; // Tốc độ xoay camera lên/xuống
        [SerializeField] float minimumPivot = -30f; // Góc xoay thấp nhất (điểm nhìn thấp nhất)
        [SerializeField] float maximumPivot = 60f; // Góc xoay cao nhất (điểm nhìn cao nhất)

        [SerializeField] float cameraColisionRadius = 0.2f; // Bán kính kiểm tra va chạm của camera
        [SerializeField] LayerMask collideWithPlayers; // Các layer mà camera có thể va chạm

        [Header("Camera Values")]
        private Vector3 cameraVelocity; // Vector dùng cho SmoothDamp trong việc theo dõi vị trí
        private Vector3 cameraObjectPosition; // Vị trí hiện tại của camera
        [SerializeField] float leftaAndRightLookAngle; // Góc nhìn trái/phải hiện tại của camera
        [SerializeField] float upAndDownLookAngle; // Góc nhìn lên/xuống hiện tại của camera

        private float cameraZPosition; // Vị trí Z hiện tại của camera
        private float targetCameraZPosition; // Vị trí Z mục tiêu của camera


        [Header("Lock On")]
        [SerializeField] float lockOnRadius = 20; // Bán kính để tìm kiếm mục tiêu để lock-on
        [SerializeField] float miniumViewableAngle = -50; // Góc nhìn tối thiểu cho việc lock-on (độ)
        [SerializeField] float maxiumViewableAngle = 50; // Góc nhìn tối đa cho việc lock-on (độ)
        [SerializeField] float lockOnTargetFollowSpeed = 0.2f; // Tốc độ theo dõi mục tiêu lock-on

        [SerializeField] float setCameraHeightSpeed = 1; // Tốc độ điều chỉnh chiều cao camera khi khóa mục tiêu
        [SerializeField] float unlockedCameraHeight = 1.65f; // Chiều cao của camera khi không khóa mục tiêu
        [SerializeField] float lockedCameraHeight = 2f; // Chiều cao của camera khi khóa mục tiêu


        private Coroutine cameraLockOnHeightCoroutine; // Coroutine để điều chỉnh chiều cao của camera khi khóa mục tiêu
        private List<CharacterManager> availableTargets = new List<CharacterManager>(); // Danh sách lưu trữ các mục tiêu khả dụng để khóa vào trong hệ thống Lock-On.
        public CharacterManager nearestLockOnTarget; // Mục tiêu lock-on gần nhất hiện tại
        public CharacterManager leftLockOnTarget; // Biến để lưu trữ mục tiêu khóa bên trái, là một đối tượng CharacterManager
        public CharacterManager rightLockOnTarget; // Biến để lưu trữ mục tiêu khóa bên phải, là một đối tượng CharacterManager


        private void Awake()
        {
            if (instance == null)
            {
                instance = this; // Gán instance nếu chưa có
            }
            else
            {
                Destroy(gameObject); // Hủy đối tượng hiện tại nếu đã có instance khác
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject); // Giữ camera không bị hủy khi chuyển sang cảnh khác
            cameraZPosition = cameraObject.transform.localPosition.z; // Lấy vị trí Z ban đầu của camera
        }

        // Phương thức gọi tất cả các hành động của camera
        public void AllCameraActions()
        {
            if (player != null)
            {
                FollowTarget(); // Theo dõi vị trí của nhân vật
                Rotations(); // Xoay camera dựa trên đầu vào của người chơi
                Collisions(); // Xử lí va chạm nhân vật của camera
            }
        }

        // Phương thức theo dõi vị trí của nhân vật một cách mượt mà
        private void FollowTarget()
        {
            // Tính toán vị trí mục tiêu của camera dựa trên vị trí của nhân vật
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition; // Cập nhật vị trí của camera
        }

        // Phương thức xử lý xoay camera dựa trên đầu vào từ người chơi
        private void Rotations()
        {


            // Kiểm tra xem nhân vật có đang ở trạng thái lock-on hay không
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                // Tính toán hướng xoay để nhìn vào mục tiêu lock-on
                Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
                rotationDirection.Normalize(); // Chuẩn hóa vector để có độ dài bằng 1
                rotationDirection.y = 0; // Đặt thành 0 để chỉ xoay quanh trục Y

                // Tạo một Quaternion mục tiêu từ hướng đã tính toán
                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                // Xoay camera về hướng mục tiêu lock-on một cách mượt mà
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

                // Cập nhật hướng xoay cho camera pivot (trục xoay của camera)
                rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
                rotationDirection.Normalize(); // Chuẩn hóa vector

                // Tạo Quaternion mục tiêu cho camera pivot
                targetRotation = Quaternion.LookRotation(rotationDirection);
                // Xoay camera pivot về hướng mục tiêu lock-on một cách mượt mà
                cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

                // Lưu lại góc xoay hiện tại của camera để sử dụng sau này
                leftaAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = transform.eulerAngles.x;
            }
            else
            {
                // Nếu không ở trạng thái lock-on, cập nhật góc nhìn dựa trên đầu vào của người chơi
                // Cập nhật góc nhìn trái/phải dựa trên đầu vào horizontal từ PlayerInputManager
                leftaAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

                // Cập nhật góc nhìn lên/xuống dựa trên đầu vào vertical từ PlayerInputManager
                upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;

                // Giới hạn góc nhìn lên/xuống trong khoảng từ minimumPivot đến maximumPivot
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

                Vector3 cameraRotation = Vector3.zero; // Khởi tạo vector xoay
                Quaternion targetRotation; // Quaternion mục tiêu để xoay

                // Xoay theo trục Y (trái/phải)
                cameraRotation.y = leftaAndRightLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation); // Tạo Quaternion từ góc xoay
                transform.rotation = targetRotation; // Cập nhật góc xoay cho camera

                cameraRotation = Vector3.zero; // Reset vector xoay

                // Xoay theo trục X (lên/xuống)
                cameraRotation.x = upAndDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation); // Tạo Quaternion từ góc xoay
                cameraPivotTransform.localRotation = targetRotation; // Cập nhật góc xoay cho pivot của camera
            }
        }

        // Phương thức xử lí va chạm cho camera (fix lỗi nhân vật đứng sau tường sẽ bị che khuất)
        private void Collisions()
        {
            targetCameraZPosition = cameraZPosition;// Đặt lại vị trí Z mục tiêu
            RaycastHit hit;// Thông tin về va chạm
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;// Hướng từ pivot đến camera
            direction.Normalize();// Chuẩn hóa hướng

            // Kiểm tra va chạm bằng SphereCast từ pivot đến hướng camera
            if (Physics.SphereCast(cameraPivotTransform.position,
                cameraColisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithPlayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);// Khoảng cách từ pivot đến điểm va chạm
                targetCameraZPosition = -(distanceFromHitObject - cameraColisionRadius);// Điều chỉnh vị trí Z mục tiêu 
            }
            // Giữ khoảng cách Z tối thiểu để camera không bị vượt quá giới hạn
            if (Mathf.Abs(targetCameraZPosition) < cameraColisionRadius)
            {
                targetCameraZPosition = -cameraColisionRadius;
            }

            // Làm mượt chuyển động của camera bằng cách sử dụng Lerp
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition; // Cập nhật vị trí Z của camera
        }

        // Phương thức được sử dụng để tìm kiếm và xác định các mục tiêu mà nhân vật có thể khóa (lock-on) trong một khoảng cách nhất định
        public void LocatingLockOnTargets()
        {
            // Khai báo biến để lưu khoảng cách ngắn nhất từ nhân vật đến mục tiêu lock-on.
            float shortestDistance = Mathf.Infinity; // Khởi tạo khoảng cách ngắn nhất là vô hạn
            float shortestDistanceOfRightTarget = Mathf.Infinity; // Khoảng cách ngắn nhất cho mục tiêu bên phải
            float shortestDistanceOfLeftTarget = -Mathf.Infinity; // Khoảng cách ngắn nhất cho mục tiêu bên trái

            // Lấy tất cả các collider trong bán kính lockOnRadius từ vị trí của nhân vật
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

            // Lặp qua tất cả các collider đã tìm thấy
            for (int i = 0; i < colliders.Length; i++)
            {
                // Lấy Component CharacterManager từ collider để xác định xem đây có phải là một mục tiêu lock-on không
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                // Nếu không tìm thấy CharacterManager, bỏ qua collider này
                if (lockOnTarget != null)
                {
                    // Tính toán hướng từ nhân vật đến mục tiêu lock-on
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;

                    // Tính khoảng cách từ nhân vật đến vị trí của mục tiêu lock-on
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);

                    // Tính góc nhìn giữa hướng nhân vật và hướng camera
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                    // Kiểm tra nếu mục tiêu lock-on đã chết, bỏ qua mục tiêu này
                    if (lockOnTarget.isDead.Value)
                        continue;

                    // Kiểm tra nếu mục tiêu lock-on thuộc về cùng root với nhân vật (đồng đội), bỏ qua mục tiêu này
                    if (lockOnTarget.transform.root == player.transform.root)
                        continue;

                    // Kiểm tra nếu góc nhìn của mục tiêu nằm trong khoảng cho phép.
                    if (viewableAngle > miniumViewableAngle && viewableAngle < maxiumViewableAngle)
                    {
                        RaycastHit hit;

                        // Kiểm tra xem có vật cản giữa nhân vật và mục tiêu lock-on hay không.
                        if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position,
                            lockOnTarget.characterCombatManager.lockOnTransform.position,
                            out hit,
                            WorldUtilityManager.instance.GetEnviroLayers()))
                        {
                            // Nếu có vật cản (ví dụ: tường), bỏ qua mục tiêu này và không thêm vào danh sách khóa.
                            continue;
                        }
                        else
                        {
                            // Nếu không có vật cản, thêm mục tiêu này vào danh sách các mục tiêu khả dụng.
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }
            // Duyệt qua danh sách các mục tiêu khả dụng để tìm mục tiêu gần nhất
            for (int k = 0; k < availableTargets.Count; k++)
            {
                // Kiểm tra xem mục tiêu có hợp lệ không (không bị null)
                if (availableTargets[k] != null)
                {
                    // Tính khoảng cách giữa người chơi và mục tiêu hiện tại
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);

                    // Nếu khoảng cách từ mục tiêu hiện tại nhỏ hơn khoảng cách ngắn nhất đã được xác định
                    if (distanceFromTarget < shortestDistance)
                    {
                        // Cập nhật khoảng cách ngắn nhất và lưu mục tiêu gần nhất
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[k];
                    }

                    // Kiểm tra nếu người chơi đang khóa mục tiêu
                    if (player.playerNetworkManager.isLockedOn.Value)
                    {
                        // Tính vị trí tương đối của mục tiêu so với người chơi
                        Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);
                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        // Nếu mục tiêu hiện tại không phải là mục tiêu đang chiến đấu
                        if (availableTargets[k] != player.playerCombatManager.currentTarget)
                            continue;

                        // Nếu mục tiêu nằm bên trái và khoảng cách lớn hơn khoảng cách ngắn nhất bên trái
                        if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget; // Cập nhật khoảng cách ngắn nhất bên trái
                            leftLockOnTarget = availableTargets[k]; // Lưu mục tiêu khóa bên trái
                        }
                        // Nếu mục tiêu nằm bên phải và khoảng cách nhỏ hơn khoảng cách ngắn nhất bên phải
                        else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget; // Cập nhật khoảng cách ngắn nhất bên phải
                            rightLockOnTarget = availableTargets[k]; // Lưu mục tiêu khóa bên phải
                        }
                    }
                }
                else
                {
                    // Nếu mục tiêu không hợp lệ (null), xóa danh sách mục tiêu và hủy trạng thái lock-on
                    ClearLockOnTargets(); // Xóa mục tiêu khóa
                    player.playerNetworkManager.isLockedOn.Value = false; // Hủy trạng thái lock-on
                }
            }
        }


        // Phương thức để thiết lập chiều cao camera khi khóa mục tiêu
        public void SetLockCameraHeight()
        {
            // Nếu có coroutine điều chỉnh chiều cao camera đang chạy, dừng nó
            if (cameraLockOnHeightCoroutine != null)
            {
                StopCoroutine(cameraLockOnHeightCoroutine);
            }

            // Khởi động một coroutine mới để thiết lập chiều cao camera
            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }


        // Phương thức này sẽ xóa toàn bộ các mục tiêu lock-on hiện tại
        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;// Đặt mục tiêu lock-on gần nhất về null
            leftLockOnTarget = null; // Đặt lại mục tiêu khóa bên trái
            rightLockOnTarget = null; // Đặt lại mục tiêu khóa bên phải
            availableTargets.Clear();// Xóa danh sách các mục tiêu khả dụng để lock-on
        }

        // Coroutine để đợi cho đến khi nhân vật không thực hiện hành động trước khi tìm mục tiêu mới
        public IEnumerator WaitThenFindNewTarget()
        {
            // Vòng lặp kiểm tra trạng thái thực hiện hành động
            while (player.isPerformingAction)
            {
                // Nếu nhân vật đang thực hiện hành động, chờ đến khi nó không còn thực hiện hành động
                yield return null; // Đợi cho đến khung hình tiếp theo
            }

            // Khi nhân vật không còn thực hiện hành động, xóa các mục tiêu đã khóa
            ClearLockOnTargets();

            // Tìm kiếm mục tiêu mới
            LocatingLockOnTargets();

            // Kiểm tra xem có mục tiêu gần nhất hay không
            if (nearestLockOnTarget != null)
            {
                // Nếu có, đặt mục tiêu mới cho nhân vật
                player.playerCombatManager.SetTarget(nearestLockOnTarget);

                // Đặt trạng thái khóa mục tiêu thành true
                player.playerNetworkManager.isLockedOn.Value = true;
            }

            // Đợi khung hình tiếp theo trước khi kết thúc coroutine
            yield return null;
        }

        //Phương thức này điều chỉnh chiều cao của camera giữa hai trạng thái:
        //1-Khóa mục tiêu một cách mượt mà trong một khoảng thời gian xác định.
        //2-không khóa mục tiêu một cách mượt mà trong một khoảng thời gian xác định.
        private IEnumerator SetCameraHeight()
        {
            // Thời gian cần thiết để hoàn thành việc điều chỉnh chiều cao camera
            float duration = 1;
            float timer = 0;

            // Biến tốc độ cho SmoothDamp
            Vector3 velocity = Vector3.zero;

            // Tạo vector cho chiều cao camera khóa và không khóa
            Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
            Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

            // Vòng lặp để điều chỉnh chiều cao camera trong khoảng thời gian xác định
            while (timer < duration)
            {
                timer += Time.deltaTime; // Cập nhật bộ đếm thời gian
                if (player != null) // Kiểm tra xem biến player có hợp lệ không
                {
                    if (player.playerCombatManager.currentTarget != null) // Nếu có mục tiêu hiện tại
                    {
                        // Điều chỉnh vị trí camera sang chiều cao khóa
                        cameraPivotTransform.transform.localPosition =
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                        // Điều chỉnh góc quay của camera về hướng nhất định (0 độ ở đây)
                        cameraPivotTransform.transform.localRotation =
                            Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), setCameraHeightSpeed);
                    }
                    else // Nếu không có mục tiêu hiện tại
                    {
                        // Điều chỉnh vị trí camera sang chiều cao không khóa
                        cameraPivotTransform.transform.localPosition =
                           Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    }
                }
                yield return null; // Chờ cho đến frame tiếp theo
            }

            // Đảm bảo rằng camera đạt được vị trí cuối cùng sau khi vòng lặp kết thúc
            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null) // Nếu có mục tiêu
                {
                    // Đặt vị trí camera về chiều cao khóa
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0); // Đặt góc quay về 0
                }
                else // Nếu không có mục tiêu
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight; // Đặt vị trí camera về chiều cao không khóa
                }
            }
            yield return null; // Chờ cho đến frame tiếp theo
        }

    }
}

