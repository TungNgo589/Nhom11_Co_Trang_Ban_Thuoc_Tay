using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class NhaCungCap
{
    public int MaNhaCungCap { get; set; }

    public string TenNhaCungCap { get; set; }

    public string HinhAnhNhaCungCap { get; set; }

    public string MoTaNhaCungCap { get; set; }

    public string Sdt { get; set; }

    public string DiaChi { get; set; }

    public string Email { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
