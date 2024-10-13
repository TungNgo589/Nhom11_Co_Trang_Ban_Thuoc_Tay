using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class DonHang
{
    public string MaDh { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? Sdt { get; set; }

    public string Diachi { get; set; } = null!;

    public int MaKhuyenMai { get; set; }

    public double TongTien { get; set; }

    public int SoLuong { get; set; }

    public string? GhiChu { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? MaNguoiDung { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual KhuyenMai MaKhuyenMaiNavigation { get; set; } = null!;

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
