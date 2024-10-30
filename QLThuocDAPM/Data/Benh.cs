using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class Benh
{
    public int MaBenh { get; set; }

    public string TenBenh { get; set; }

    public string MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
