using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class Benh
{
    public int MaBenh { get; set; }

    public string TenBenh { get; set; }

    public string MoTa1 { get; set; }

    public string MoTa2 { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
