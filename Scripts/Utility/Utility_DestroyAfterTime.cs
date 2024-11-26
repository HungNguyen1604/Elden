using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp này được sử dụng để tự động hủy (xóa) các đối tượng sau một khoảng thời gian nhất định
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeUntilDestroyed = 5;
        // Biến `timeUntilDestroyed` được gán giá trị mặc định là 5 giây.
        // Thời gian sau khi đối tượng (VFX) sẽ bị hủy tự động sau khi được tạo.
        // Biến này có thể được chỉnh sửa từ Unity Inspector mà không cần thay đổi mã nguồn.

        private void Awake()
        {
            // Hủy đối tượng (gameObject) sau khi thời gian `timeUntilDestroyed` đã trôi qua.
            // Hàm `Destroy` sẽ tự động xóa đối tượng này khỏi game sau khi hết thời gian.
            Destroy(gameObject, timeUntilDestroyed);
        }
    }
}

