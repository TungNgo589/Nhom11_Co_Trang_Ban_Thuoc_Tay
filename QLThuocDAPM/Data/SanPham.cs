using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSp { get; set; }

    public int MaBenh { get; set; }

    public int MaNhaCungCap { get; set; }

    public int MaGiamGia { get; set; }

    public int? SoBinhLuan { get; set; }

    public string ThanhPhan { get; set; }

    public string Congdung { get; set; }

    public string Cachdung { get; set; }

    public string Doituongsudung { get; set; }

    public string Tacdungphu { get; set; }

    public double GiaTien { get; set; }

    public string DonVi { get; set; }

    public DateOnly Ngaysanxuat { get; set; }

    public string Noisanxuat { get; set; }

    public DateOnly? HansuDung { get; set; }

    public string ChitietSp { get; set; }

    public int MaDm { get; set; }

    public int? SoLuong { get; set; }

    public int? SoLuongMua { get; set; }

    public string HinhAnh1 { get; set; }

    public string HinhAnh2 { get; set; }

    public string HinhAnh3 { get; set; }

    public string HinhAnh4 { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<HinhAnh> HinhAnhs { get; set; } = new List<HinhAnh>();

    public virtual Benh MaBenhNavigation { get; set; }

    public virtual DanhMuc MaDmNavigation { get; set; }

    public virtual GiamGium MaGiamGiaNavigation { get; set; }

    public virtual NhaCungCap MaNhaCungCapNavigation { get; set; }

    public double GiaSauGiam
    {
        get
        {
            // Kiểm tra xem có giảm giá không
            if (MaGiamGiaNavigation != null && MaGiamGiaNavigation.ThoiGianBatDau <= DateTime.Now && MaGiamGiaNavigation.ThoiGianKetThuc >= DateTime.Now)
            {
                double discountAmount = GiaTien * MaGiamGiaNavigation.GiaTri / 100; // Giả sử GiaTri là tỷ lệ giảm giá
                return GiaTien - discountAmount;
            }
            return GiaTien; // Trả về giá gốc nếu không có giảm giá
        }
    }

}
