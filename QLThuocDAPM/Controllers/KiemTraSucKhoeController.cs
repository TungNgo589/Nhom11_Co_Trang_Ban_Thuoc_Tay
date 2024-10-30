using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class KiemTraSucKhoeController : Controller
{
    // Hiển thị trang kiểm tra sức khỏe
    public IActionResult Index()
    {
        return View();
    }

    // Hiển thị câu hỏi khảo sát dựa trên loại bệnh
    public IActionResult KhaoSat(string loaiBenh)
    {
        // Tạo danh sách câu hỏi tùy theo loại bệnh
        var surveyQuestions = GetSurveyQuestions(loaiBenh);

        // Gửi danh sách câu hỏi và loại bệnh đến View
        ViewBag.LoaiBenh = loaiBenh;
        ViewBag.SurveyQuestions = surveyQuestions;

        return View();
    }

    // Nhận kết quả khảo sát
    [HttpPost]
    public IActionResult KetQuaKhaoSat(string loaiBenh, IFormCollection form)
    {
        bool nguyCoCao = false;
        int diem = 0; // Điểm tích lũy dựa trên câu trả lời

        // Duyệt qua tất cả các câu hỏi trong form
        foreach (var key in form.Keys)
        {
            if (key.StartsWith("answers[q")) // Chỉ xử lý các câu trả lời
            {
                // Lấy tất cả các giá trị cho câu hỏi này
                var answers = form[key];
                foreach (var answer in answers)
                {
                    if (answer == "Có")
                    {
                        diem += 2; // Câu trả lời "Có" tăng nguy cơ
                    }
                    else if (answer == "Thỉnh thoảng")
                    {
                        diem += 1; // Câu trả lời "Thỉnh thoảng" có nguy cơ nhẹ
                    }
                    // Không làm gì nếu trả lời là "Không"
                }
            }
        }

        // Xác định nguy cơ dựa trên tổng điểm
        if (diem >= 10) // Ví dụ: nếu tổng điểm >= 10, người dùng có nguy cơ cao
        {
            nguyCoCao = true;
        }

        // Thông báo kết quả
        string resultMessage = nguyCoCao ?
            $"Bạn có nguy cơ cao mắc bệnh {loaiBenh}. Vui lòng tham khảo ý kiến bác sĩ." :
            $"Bạn không có nguy cơ cao mắc bệnh {loaiBenh}. Tiếp tục duy trì lối sống lành mạnh!";

        // Lưu kết quả vào ViewBag
        ViewBag.ResultMessage = resultMessage;

        return View("KetQuaKhaoSat");
    }

    // Phương thức trả về câu hỏi dựa trên loại bệnh
    private List<SurveyQuestion> GetSurveyQuestions(string loaiBenh)
    {
        if (loaiBenh == "Tim mach") // Kiểm tra loại bệnh bằng chuỗi thay vì số
        {
            return new List<SurveyQuestion>
        {
            new SurveyQuestion { Id = 1, QuestionText = "Bạn có thường xuyên tập thể dục không?" },
            new SurveyQuestion { Id = 2, QuestionText = "Bạn có tiền sử gia đình mắc bệnh tim mạch không?" },
            new SurveyQuestion { Id = 3, QuestionText = "Bạn có thường xuyên cảm thấy đau thắt ngực hoặc khó thở không?" },
            new SurveyQuestion { Id = 4, QuestionText = "Bạn có bao giờ cảm thấy nhịp tim không đều hoặc đập nhanh hơn bình thường không?" },
            new SurveyQuestion { Id = 5, QuestionText = "Bạn có thường xuyên mệt mỏi ngay cả khi không hoạt động thể chất nhiều không?" },
            new SurveyQuestion { Id = 6, QuestionText = "Bạn có bị sưng phù chân, mắt cá chân hoặc bàn chân không?" },
            new SurveyQuestion { Id = 7, QuestionText = "Bạn có từng cảm thấy chóng mặt hoặc ngất xỉu mà không rõ nguyên nhân không?" },
            new SurveyQuestion { Id = 8, QuestionText = "Bạn có cảm giác khó chịu hoặc đau lan xuống cánh tay, cổ, hoặc hàm không?" },
        };

        }
        else if (loaiBenh == "Alzheimer")
        {
            return new List<SurveyQuestion>
        {
            new SurveyQuestion { Id = 1, QuestionText = "Bạn có gặp khó khăn trong việc nhớ lại các sự kiện hoặc thông tin mới học không?" },
            new SurveyQuestion { Id = 2, QuestionText = "Bạn có gia đình bị Alzheimer không?" },
            new SurveyQuestion { Id = 3, QuestionText = "Bạn có thường quên tên người thân hoặc các địa điểm quen thuộc không?" },
            new SurveyQuestion { Id = 4, QuestionText = "Bạn có bao giờ cảm thấy khó khăn khi lập kế hoạch hoặc hoàn thành các công việc hàng ngày không?" },
            new SurveyQuestion { Id = 5, QuestionText = "Bạn có gặp khó khăn trong việc định hướng thời gian và không gian không?" },
            new SurveyQuestion { Id = 6, QuestionText = "Bạn có thường xuyên mất đồ và không nhớ mình đã đặt chúng ở đâu không?" },
            new SurveyQuestion { Id = 7, QuestionText = "Bạn có cảm thấy khó khăn trong việc đưa ra quyết định hoặc giải quyết vấn đề không?" },
            new SurveyQuestion { Id = 8, QuestionText = "Bạn có gặp khó khăn trong việc giao tiếp, ví dụ như khó tìm từ đúng hoặc hiểu sai ý của người khác không?" },
        };
        }
        else if (loaiBenh == "Ha duong huyet")
        {
            return new List<SurveyQuestion>
        {
            new SurveyQuestion { Id = 1, QuestionText = "Bạn có thường xuyên cảm thấy đau đầu hoặc chóng mặt không?" },
            new SurveyQuestion { Id = 2, QuestionText = "Bạn có thấy mệt mỏi hoặc mất sức mà không rõ nguyên nhân không?" },
            new SurveyQuestion { Id = 3, QuestionText = "Bạn có gặp khó khăn trong việc tập trung hoặc nhớ thông tin không?" },
            new SurveyQuestion { Id = 4, QuestionText = "Bạn có thấy tim đập nhanh hoặc không đều không?" },
            new SurveyQuestion { Id = 5, QuestionText = "Bạn có bị sưng phù ở chân, mắt cá chân hoặc bàn chân không?" },
            new SurveyQuestion { Id = 6, QuestionText = "Bạn có thường xuyên bị cảm giác căng thẳng hoặc lo âu không?" },
            new SurveyQuestion { Id = 7, QuestionText = "Bạn có tiền sử gia đình bị huyết áp cao không?" },
            new SurveyQuestion { Id = 8, QuestionText = "Bạn có thấy khó thở hoặc tức ngực khi hoạt động thể chất không?" },
        };
        }
        else
        {
            return new List<SurveyQuestion>();
        }
    }
}

// Mô hình cho câu hỏi khảo sát
public class SurveyQuestion
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
}
