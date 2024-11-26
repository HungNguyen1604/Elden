using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    // Lớp này quản lý thanh hiển thị trạng thái (stamina) trên giao diện người dùng.
    public class UI_StatBar : MonoBehaviour
    {
        // Tham chiếu đến thành phần Slider, được sử dụng để hiển thị giá trị stamina trên UI.
        private Slider slider; // Biến lưu trữ thành phần Slider.
        private RectTransform rectTransform; // Biến lưu trữ thành phần RectTransform.

        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStats = true; // Biến kiểm soát việc thay đổi chiều dài thanh theo giá trị.
        [SerializeField] protected float widthScaleMultiplier = 1; // Hệ số nhân chiều rộng cho thanh.

        protected virtual void Awake()
        {
            // Gán thành phần Slider của UI vào biến 'slider'.
            slider = GetComponent<Slider>();
            // Gán thành phần RectTransform của UI vào biến 'rectTransform'.
            rectTransform = GetComponent<RectTransform>();
        }

        // Phương thức này được gọi để cập nhật giá trị của thanh stamina khi có sự thay đổi.
        // 'newValue' là giá trị stamina hiện tại của nhân vật và sẽ được hiển thị trên thanh.
        public virtual void SetStat(int newValue)
        {
            // Đặt giá trị hiện tại của thanh slider theo 'newValue'.
            slider.value = newValue;
        }

        // Phương thức này đặt giá trị tối đa cho thanh stamina khi bắt đầu trò chơi hoặc khi có thay đổi.
        // 'maxValue' là giá trị tối đa của stamina.
        public virtual void SetMaxStat(int maxValue)
        {
            // Đặt giá trị tối đa của slider.
            slider.maxValue = maxValue;
            // Đồng thời đặt giá trị hiện tại của slider bằng với giá trị tối đa.
            slider.value = maxValue;

            // Nếu biến scaleBarLengthWithStats là true, thay đổi chiều dài của thanh dựa trên giá trị tối đa.
            if (scaleBarLengthWithStats)
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y); // Thay đổi kích thước thanh.
                PlayerUIManager.instance.playerUIHudManager.RefreshHUD(); // Cập nhật lại giao diện người dùng.
            }
        }
    }
}
