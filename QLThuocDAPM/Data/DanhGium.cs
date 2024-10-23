using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class DanhGium
{
    public int MaDanhGia { get; set; }

    public int MaSanPham { get; set; }

    public int MaNguoiDung { get; set; }

    public decimal? SoSao { get; set; }

    public decimal? SoSaoTrungBinh { get; set; }

    public DateTime NgayBinhLuan { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
