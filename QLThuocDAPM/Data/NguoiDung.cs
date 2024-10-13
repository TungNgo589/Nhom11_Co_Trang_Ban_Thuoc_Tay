using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string Username { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Matkhau { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual PhanQuyen Role { get; set; } = null!;
}
