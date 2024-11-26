using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SG
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("You Died Pop Up")]
        // Tham chiếu tới GameObject của cửa sổ Pop-up khi người chơi chết ("You Died")
        [SerializeField] GameObject youDiedPopUpGameObject;
        // Tham chiếu tới TextMeshProUGUI để hiển thị văn bản nền (background) cho cửa sổ pop-up
        [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
        // Tham chiếu tới TextMeshProUGUI để hiển thị văn bản chính (nội dung thông báo) cho cửa sổ pop-up
        [SerializeField] TextMeshProUGUI youDiedPopUpText;
        // Tham chiếu tới CanvasGroup để điều khiển tính năng mờ dần (fade in/fade out) của cửa sổ pop-up
        [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

        // Phương thức này được gọi khi người chơi chết, kích hoạt cửa sổ pop-up "You Died" với các hiệu ứng.
        public void SendYouDiedPopUp()
        {
            // Hiển thị GameObject của cửa sổ pop-up
            youDiedPopUpGameObject.SetActive(true);
            // Đặt khoảng cách giữa các ký tự của văn bản nền về 0
            youDiedPopUpBackgroundText.characterSpacing = 0;

            // Bắt đầu hiệu ứng giãn nở văn bản nền (kéo dài khoảng cách giữa các ký tự theo thời gian)
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackgroundText, 8, 19));
            // Bắt đầu hiệu ứng mờ dần để làm xuất hiện pop-up
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
            // Đợi một khoảng thời gian và sau đó làm mờ dần pop-up để ẩn đi
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
        }

        // Coroutine này thực hiện việc kéo giãn (stretch) các ký tự trong văn bản nền của pop-up trong khoảng thời gian nhất định.
        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            // Kiểm tra nếu thời gian kéo giãn lớn hơn 0
            if (duration > 0f)
            {
                // Đặt khoảng cách giữa các ký tự của văn bản về 0 trước khi bắt đầu quá trình kéo giãn
                text.characterSpacing = 0;
                float timer = 0;  // Khởi tạo bộ đếm thời gian để theo dõi thời gian kéo giãn

                // Đợi 1 khung hình trước khi bắt đầu quá trình kéo giãn
                yield return null;

                // Vòng lặp trong khoảng thời gian được chỉ định (duration)
                while (timer < duration)
                {
                    // Cập nhật bộ đếm thời gian
                    timer = timer + Time.deltaTime;

                    // Sử dụng phương thức Lerp để từ từ tăng khoảng cách giữa các ký tự (characterSpacing)
                    // Mathf.Lerp lấy ba tham số: giá trị ban đầu, giá trị cuối cùng, và hệ số thay đổi theo thời gian.
                    // Ở đây, giá trị characterSpacing sẽ dần dần tăng từ 0 tới "stretchAmount" trong khoảng thời gian "duration".
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));

                    // Chờ đến khung hình tiếp theo trước khi tiếp tục thay đổi giá trị
                    yield return null;
                }
            }
        }


        // Coroutine này thực hiện hiệu ứng mờ dần (fade in) cho pop-up khi người chơi chết.
        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
        {
            if (duration > 0)
            {
                // Đặt độ mờ ban đầu của pop-up về 0 (tức là hoàn toàn không hiện)
                canvas.alpha = 0;
                float timer = 0; // Khởi tạo bộ đếm thời gian

                yield return null; // Đợi một khung hình trước khi bắt đầu

                // Vòng lặp để tăng dần độ mờ cho pop-up
                while (timer < duration)
                {
                    timer = timer + Time.deltaTime; // Cập nhật bộ đếm thời gian

                    // Tăng dần độ mờ của pop-up, làm nó xuất hiện dần dần
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    yield return null; // Đợi khung hình tiếp theo
                }
            }
            // Đảm bảo rằng sau khi kết thúc quá trình, pop-up hoàn toàn hiện rõ
            canvas.alpha = 1;
            yield return null; // Đợi một khung hình trước khi kết thúc
        }


        // Coroutine này thực hiện việc chờ đợi một khoảng thời gian (delay) và sau đó làm mờ dần pop-up để ẩn nó đi (fade out).
        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
        {
            if (duration > 0)
            {
                // Chờ đợi trong khoảng thời gian "delay" trước khi bắt đầu hiệu ứng mờ dần
                while (delay > 0)
                {
                    delay -= Time.deltaTime; // Giảm dần delay theo thời gian trôi qua
                    yield return null; // Đợi khung hình tiếp theo
                }

                // Đặt độ mờ ban đầu về 1 (pop-up đang hiển thị đầy đủ)
                canvas.alpha = 1;
                float timer = 0; // Khởi tạo bộ đếm thời gian

                yield return null; // Đợi một khung hình trước khi bắt đầu

                // Vòng lặp để giảm dần độ mờ cho pop-up
                while (timer < duration)
                {
                    timer = timer + Time.deltaTime; // Cập nhật bộ đếm thời gian

                    // Giảm dần độ mờ của pop-up để làm nó biến mất dần dần
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null; // Đợi khung hình tiếp theo
                }
            }
            // Đảm bảo rằng sau khi kết thúc quá trình, pop-up hoàn toàn biến mất
            canvas.alpha = 0;
            yield return null; // Đợi một khung hình trước khi kết thúc
        }

    }
}

