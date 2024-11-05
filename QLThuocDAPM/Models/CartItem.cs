using QLThuocDAPM.Data;

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
        public List<GiamGium> GiamGias { get; set; }
        public List<KhuyenMai> KhuyenMais { get; set; }
        public List<SanPham> SanPhams { get; set; }

        public CartItem()
        {
            GiamGias = new List<GiamGium>();
            KhuyenMais = new List<KhuyenMai>();
            SanPhams = new List<SanPham>();
        }

        // Thêm thuộc tính này để lưu thông tin về sản phẩm
        public SanPham SanPham { get; set; } // Sản phẩm tương ứng với CartItem

        public double GiaSauGiam
        {
            get
            {
                if (SanPham != null)
                {
                    double discountAmount = SanPham.GiaTien * (SanPham.MaGiamGiaNavigation?.GiaTri ?? 0) / 100;
                    return SanPham.GiaTien - discountAmount;
                }
                return DonGia; // Nếu không có sản phẩm, trả về đơn giá
            }
        }
    }
}
