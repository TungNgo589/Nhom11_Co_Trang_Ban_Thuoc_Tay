using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class DanhMuc
{
    public int MaDm { get; set; }

    public string TenDm { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
