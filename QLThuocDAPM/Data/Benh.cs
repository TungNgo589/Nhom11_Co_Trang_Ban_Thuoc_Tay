using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class Benh
{
    public int MaBenh { get; set; }

    public string TenBenh { get; set; } = null!;

    public string MoTa { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
