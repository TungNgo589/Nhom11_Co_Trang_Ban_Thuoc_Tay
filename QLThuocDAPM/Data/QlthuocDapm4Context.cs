using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLThuocDAPM.Data;

public partial class QlthuocDapm4Context : DbContext
{
    public QlthuocDapm4Context()
    {
    }

    public QlthuocDapm4Context(DbContextOptions<QlthuocDapm4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Benh> Benhs { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GiamGium> GiamGia { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HinhAnh> HinhAnhs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhanQuyen> PhanQuyens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-Q5EI430;Initial Catalog=QLThuocDAPM4;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Benh>(entity =>
        {
            entity.HasKey(e => e.MaBenh).HasName("PK__Benh__E84E00CFED5DE2AB");

            entity.ToTable("Benh");

            entity.Property(e => e.MaBenh).HasColumnName("maBenh");
            entity.Property(e => e.MoTa1)
                .IsRequired()
                .HasMaxLength(900)
                .HasColumnName("moTa1");
            entity.Property(e => e.MoTa2)
                .IsRequired()
                .HasMaxLength(900)
                .HasColumnName("moTa2");
            entity.Property(e => e.TenBenh)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("tenBenh");
        });

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BinhLuan__87CB66A09F8D09A0");

            entity.ToTable("BinhLuan");

            entity.Property(e => e.MaSp).HasColumnName("maSP");
            entity.Property(e => e.NgayBinhLuan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).IsRequired();

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BinhLuan__MaNguo__5FB337D6");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BinhLuan__maSP__5EBF139D");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietD__3213E83F431979D2");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaDh)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("maDH");
            entity.Property(e => e.MaSp).HasColumnName("maSP");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TongTien).HasColumnName("tongTien");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__maDH__6B24EA82");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDon__maSP__6A30C649");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietG__3213E83F87C9AB21");

            entity.ToTable("ChiTietGioHang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaGh).HasColumnName("maGH");
            entity.Property(e => e.MaSp).HasColumnName("maSP");
            entity.Property(e => e.SoLuongSp).HasColumnName("soLuongSP");
            entity.Property(e => e.TongTien).HasColumnName("tongTien");

            entity.HasOne(d => d.MaGhNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaGh)
                .HasConstraintName("FK__ChiTietGio__maGH__71D1E811");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK__ChiTietGio__maSP__70DDC3D8");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DanhGia__AA9515BF0428DDAF");

            entity.Property(e => e.NgayBinhLuan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoSao).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaNguoi__6477ECF3");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__MaSanPh__6383C8BA");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DanhMuc__7A3EF408769399D8");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.MaDm).HasColumnName("maDM");
            entity.Property(e => e.TenDm)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("tenDM");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonHang__7A3EF40F17E84809");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDh)
                .HasMaxLength(255)
                .HasColumnName("maDH");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Diachi)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("diachi");
            entity.Property(e => e.HoTen)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.MaKhuyenMai)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.Sdt)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.TongTien).HasColumnName("tongTien");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("trangThai");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("username");

            entity.HasOne(d => d.MaKhuyenMaiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKhuyenMai)
                .HasConstraintName("FK__DonHang__MaKhuye__5535A963");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__DonHang__maNguoi__5629CD9C");
        });

        modelBuilder.Entity<GiamGium>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK__GiamGia__EF9458E45C1471F6");

            entity.Property(e => e.ThoiGianBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGh).HasName("PK__GioHang__7A3E2D6B67617B39");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaGh).HasColumnName("maGH");
            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__GioHang__maNguoi__6E01572D");
        });

        modelBuilder.Entity<HinhAnh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HinhAnh__3213E83FE7D976E0");

            entity.ToTable("HinhAnh");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MaSp).HasColumnName("maSP");
            entity.Property(e => e.UrlHinh)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnName("urlHinh");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.HinhAnhs)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK__HinhAnh__maSP__6754599E");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKhuyenMai).HasName("PK__KhuyenMa__6F56B3BD648DE4BA");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.MaKhuyenMai)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.DieuKienApDung).HasDefaultValue(0);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(10);
            entity.Property(e => e.ThoiGianBatDau)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__446439EAF0B84A83");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.MaNguoiDung).HasColumnName("maNguoiDung");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.HoTen)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("hoTen");
            entity.Property(e => e.Matkhau)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("matkhau");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.Sdt)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("sdt");
            entity.Property(e => e.TrangThai)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("trangThai");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguoiDung__roleI__52593CB8");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__NhaCungC__53DA92051C9C9F2E");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HinhAnhNhaCungCap)
                .HasMaxLength(700)
                .HasColumnName("hinhAnhNhaCungCap");
            entity.Property(e => e.MoTaNhaCungCap)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhaCungCap)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<PhanQuyen>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__PhanQuye__CD98460AD3C659DF");

            entity.ToTable("PhanQuyen");

            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__7A227A7AEC019D72");

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSp).HasColumnName("maSP");
            entity.Property(e => e.Cachdung)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("cachdung");
            entity.Property(e => e.ChitietSp)
                .HasMaxLength(1000)
                .HasColumnName("chitietSP");
            entity.Property(e => e.Congdung)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("congdung");
            entity.Property(e => e.Doituongsudung)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("doituongsudung");
            entity.Property(e => e.DonVi)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("donVi");
            entity.Property(e => e.GiaSauGiam).HasColumnName("giaSauGiam");
            entity.Property(e => e.GiaTien).HasColumnName("giaTien");
            entity.Property(e => e.HansuDung).HasColumnName("hansuDung");
            entity.Property(e => e.HinhAnh1)
                .HasMaxLength(700)
                .HasColumnName("hinhAnh1");
            entity.Property(e => e.HinhAnh2)
                .HasMaxLength(700)
                .HasColumnName("hinhAnh2");
            entity.Property(e => e.HinhAnh3)
                .HasMaxLength(700)
                .HasColumnName("hinhAnh3");
            entity.Property(e => e.HinhAnh4)
                .HasMaxLength(700)
                .HasColumnName("hinhAnh4");
            entity.Property(e => e.MaBenh).HasColumnName("maBenh");
            entity.Property(e => e.MaDm).HasColumnName("maDM");
            entity.Property(e => e.Ngaysanxuat).HasColumnName("ngaysanxuat");
            entity.Property(e => e.Noisanxuat)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("noisanxuat");
            entity.Property(e => e.SoBinhLuan)
                .HasDefaultValue(0)
                .HasColumnName("soBinhLuan");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");
            entity.Property(e => e.SoLuongMua).HasColumnName("soLuongMua");
            entity.Property(e => e.Tacdungphu)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("tacdungphu");
            entity.Property(e => e.TenSp)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("tenSP");
            entity.Property(e => e.ThanhPhan)
                .IsRequired()
                .HasMaxLength(700)
                .HasColumnName("thanhPhan");

            entity.HasOne(d => d.MaBenhNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaBenh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__maBenh__4CA06362");

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__maDM__4D94879B");

            entity.HasOne(d => d.MaGiamGiaNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaGiamGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaGiamG__4BAC3F29");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNhaCungCap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SanPham__MaNhaCu__4AB81AF0");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B258442372EC20");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaDh)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("maDH");
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__maDH__5AEE82B9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
