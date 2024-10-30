namespace QLThuocDAPM.Models
{
    public class CartItem
    {
        public int MaHh { get; set; } // Mã hàng hóa
        public string TenHH { get; set; } // Tên hàng hóa
        public double DonGia { get; set; } // Đơn giá
        public string Hinh { get; set; } // Đường dẫn hình ảnh
        public int SoLuong { get; set; } // Số lượng
        public double TongTien { get; set; }
        public int TongTien2 { get; set; }

    }
}
