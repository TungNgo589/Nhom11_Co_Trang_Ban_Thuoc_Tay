﻿using System.Net.Mail;
using System.Net;

namespace QLThuocDAPM.Common
{
    public class Common1
    {
        private static string email;
        private static string password;

        // Constructor để khởi tạo cấu hình từ IConfiguration
        public Common1(IConfiguration configuration)
        {
            email = configuration["EmailSettings:Email"];
            password = configuration["EmailSettings:PasswordEmail"];
        }

        public static bool SendMail(string name, string subject, string content, string toMail)
        {
            bool result = false;
            try
            {
                // Tạo đối tượng MailMessage
                MailMessage message = new MailMessage();

                // Cấu hình SMTP server sử dụng Gmail
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // host name
                    Port = 587, // port number
                    EnableSsl = true, // Nếu SMTP yêu cầu SSL
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = email,
                        Password = password
                    }
                };

                // Cấu hình thông tin người gửi và người nhận
                MailAddress fromAddress = new MailAddress(email, name);
                message.From = fromAddress;
                message.To.Add(toMail); // Địa chỉ email của người nhận
                message.Subject = subject; // Tiêu đề email
                message.IsBodyHtml = true; // Nội dung email ở dạng HTML
                message.Body = content; // Nội dung email

                // Gửi email
                smtp.Send(message);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                result = false;
            }
            return result;
        }

        // Phương thức định dạng số (ví dụ: định dạng tiền tệ)
        public static string FormatNumber(object value, int SoSauDauPhay = 2)
        {
            bool isNumber = IsNumeric(value);
            decimal GT = 0;
            if (isNumber)
            {
                GT = Convert.ToDecimal(value);
            }
            string str = "";
            string thapPhan = "";
            for (int i = 0; i < SoSauDauPhay; i++)
            {
                thapPhan += "#";
            }
            if (thapPhan.Length > 0) thapPhan = "." + thapPhan;
            string snumformat = string.Format("0:#,##0{0}", thapPhan);
            str = String.Format("{" + snumformat + "}", GT);

            return str;
        }

        private static bool IsNumeric(object value)
        {
            return value is sbyte
                       || value is byte
                       || value is short
                       || value is ushort
                       || value is int
                       || value is uint
                       || value is long
                       || value is ulong
                       || value is float
                       || value is double
                       || value is decimal;
        }

        // Phương thức trả về HTML biểu diễn đánh giá sao
        public static string HtmlRate(int rate)
        {
            var str = "";
            if (rate == 1)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 2)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 3)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 4)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star-o' aria-hidden='true'></i></li>";
            }
            if (rate == 5)
            {
                str = @"<li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>
                        <li><i class='fa fa-star' aria-hidden='true'></i></li>";
            }
            return str;
        }
    }
}