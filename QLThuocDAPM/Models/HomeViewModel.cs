using QLThuocDAPM.Data;

using System.Collections.Generic;

namespace QLThuocDAPM.ViewModels
{
    public class HomeViewModel
    {
        public List<DanhMuc> DanhMucs { get; set; } // Danh sách danh mục
        public Dictionary<int, List<SanPham>> SanPhams { get; set; } // Sản phẩm theo danh mục
    }
}
