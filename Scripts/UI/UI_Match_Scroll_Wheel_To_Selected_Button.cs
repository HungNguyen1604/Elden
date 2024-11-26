using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SG
{
    public class UI_Match_Scroll_Wheel_To_Selected_Button : MonoBehaviour
    {
        [SerializeField] GameObject currentSelected; // Đối tượng nút hiện tại đang được chọn
        [SerializeField] GameObject previouslySelected; // Đối tượng nút trước đó đã được chọn
        [SerializeField] RectTransform currentSelectedTranform; // RectTransform của nút hiện tại

        [SerializeField] RectTransform contentPanel; // Bảng chứa các nút
        [SerializeField] ScrollRect scrollRect; // Đối tượng ScrollRect để quản lý cuộn

        private void Update()
        {
            currentSelected = EventSystem.current.currentSelectedGameObject; // Lấy đối tượng nút hiện tại từ hệ thống sự kiện

            if (currentSelected != null)
            {
                if (currentSelected != previouslySelected) // Kiểm tra nếu nút hiện tại khác nút trước đó
                {
                    previouslySelected = currentSelected; // Cập nhật nút trước đó
                    currentSelectedTranform = currentSelected.GetComponent<RectTransform>(); // Lấy RectTransform của nút hiện tại
                    SnapTo(currentSelectedTranform); // Cuộn đến nút hiện tại
                }
            }
        }

        private void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases(); // Cập nhật Canvas để đảm bảo rằng tất cả các đối tượng UI đã được vẽ đúng

            // Tính toán vị trí mới dựa trên vị trí của contentPanel và target
            Vector2 newPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

            newPosition.x = 0; // Đảm bảo không thay đổi vị trí theo trục x

            contentPanel.anchoredPosition = newPosition; // Cập nhật vị trí của contentPanel
        }
    }
}

