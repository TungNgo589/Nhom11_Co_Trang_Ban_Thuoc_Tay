using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class GiamGium
{
    public int MaGiamGia { get; set; }

    public int GiaTri { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
