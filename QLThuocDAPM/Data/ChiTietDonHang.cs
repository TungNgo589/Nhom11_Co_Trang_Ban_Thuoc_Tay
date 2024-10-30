﻿using System;
using System.Collections.Generic;

namespace QLThuocDAPM.Data;

public partial class ChiTietDonHang
{
    public int Id { get; set; }

    public string MaDh { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public int TongTien { get; set; }

    public virtual DonHang MaDhNavigation { get; set; }

    public virtual SanPham MaSpNavigation { get; set; }
}
