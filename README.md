<p align="center">
  <img src="Kerja_Projek/Assets/logopkk.png" alt="Logo BengkelYann" width="180">
</p>

<h1 align="center">BengkelYann</h1>

<p align="center">
  Aplikasi desktop manajemen bengkel berbasis C# Windows Forms dan SQL Server.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Framework 4.7.2">
  <img src="https://img.shields.io/badge/Windows%20Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white" alt="Windows Forms">
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server">
</p>

---

## Tentang Proyek

**BengkelYann** adalah aplikasi desktop untuk mengelola operasional bengkel kendaraan, mulai dari pendataan customer dan kendaraan, pembuatan Work Order (WO), permintaan sparepart, pengerjaan oleh mekanik, pembayaran, hingga pencetakan struk dan laporan PDF.

Aplikasi ini memiliki pembagian hak akses berdasarkan peran pengguna:

| Peran | Tanggung Jawab |
|---|---|
| **Admin** | Mengelola master data, pengguna, laporan, dan audit log |
| **Frontdesk** | Mendata customer/kendaraan, membuat Work Order, memonitor dan menyerahkan kendaraan |
| **Gudang** | Mengelola stok sparepart, barang masuk, dan permintaan sparepart |
| **Mekanik** | Melihat job yang ditugaskan, mengisi detail servis, dan mengajukan permintaan sparepart |
| **Kasir** | Memproses pembayaran, mencetak struk, dan mengekspor riwayat transaksi ke PDF |

## Fitur Utama

- Login multi-role dengan sesi pengguna.
- Dashboard ringkasan operasional bengkel.
- Master data customer, kendaraan, supplier, sparepart, jasa, dan pengguna.
- Pembuatan Work Order dengan nomor otomatis berformat `WO-YYYYMMDD-NNN`.
- Monitoring status Work Order: `Baru`, `Dikerjakan`, `Menunggu sparepart`, `Selesai`, `Diserahkan`, dan `Sudah dibayar`.
- Alur permintaan, persetujuan, dan penolakan sparepart.
- Detail servis dan perhitungan biaya jasa serta sparepart.
- Pembayaran, cetak struk, dan pencatatan transaksi.
- Riwayat transaksi dengan filter periode dan ekspor laporan PDF.
- Audit log untuk mencatat aktivitas pengguna (login, logout, tambah, edit, hapus, pembayaran, serah terima, permintaan sparepart, ekspor PDF, dan lainnya).

## Bahasa Pemrograman

- **C#** — seluruh logika aplikasi dan antarmuka.
- **SQL (T-SQL)** — query dan manipulasi data pada SQL Server.

## Teknologi dan Library

| Komponen | Versi | Keterangan |
|---|---|---|
| C# | - | Bahasa pemrograman utama |
| .NET Framework | 4.7.2 | Target framework aplikasi |
| Windows Forms | - | Framework antarmuka desktop |
| SQL Server Express | - | Basis data relasional |
| iTextSharp | 5.5.13.6 | Pembuatan dan ekspor laporan PDF |
| BouncyCastle.Cryptography | 2.6.2 | Dependensi kriptografi untuk iTextSharp |
| System.Data.SqlClient | - | Akses database melalui ADO.NET |
| System.Windows.Forms.DataVisualization | - | Grafik dan visualisasi data pada dashboard |
| System.Drawing | - | Pengolahan logo, gambar, dan struk |
| System.Xml.Linq | - | Pemrosesan data XML |

## Struktur Proyek

```text
Kerja_Projek/
├── Kerja_Projek.sln
├── packages.config
├── README.md
└── Kerja_Projek/
    ├── Admin/                  # Master data, dashboard, laporan, audit log
    ├── Fondasi/                # Form login dan menu utama per role
    ├── Frontdesk/              # Customer, kendaraan, dan Work Order
    ├── Gudang/                 # Stok, barang masuk, permintaan sparepart
    ├── Kasir/                  # Invoice, pembayaran, riwayat transaksi
    ├── KumpulanFormDialog/     # Form dialog tambah/edit/pembayaran
    ├── Mekanik/                # Job mekanik dan detail servis
    ├── Properties/             # Resource, pengaturan, dan logo
    ├── Assets/                 # Aset gambar/logo
    ├── koneksi.cs              # Koneksi database dan helper audit log
    ├── usersession.cs          # Sesi pengguna yang sedang login
    └── hashpassword.cs         # Helper hash password
```

## Kebutuhan Sistem

- Windows 10/11
- Visual Studio 2022 (dengan workload **.NET desktop development**)
- .NET Framework 4.7.2 Developer Pack
- SQL Server Express
- NuGet Package Restore untuk `iTextSharp` dan `BouncyCastle.Cryptography`

## Cara Menjalankan

1. Clone repositori:

   ```bash
   git clone https://github.com/Sayayyan/KP_SofianBahtiar.git
   ```

2. Buka `Kerja_Projek.sln` menggunakan Visual Studio.
3. Lakukan **Restore NuGet Packages**.
4. Sesuaikan connection string pada `Kerja_Projek/koneksi.cs` dengan instance SQL Server lokal:

   ```csharp
   string lokasi = "Data source = NAMA_SERVER\\SQLEXPRESS; database = KerjaProjek; integrated security = true";
   ```

5. Pastikan database `KerjaProjek` sudah tersedia pada instance SQL Server tersebut.
6. Build solution, lalu jalankan aplikasi dengan tombol **Start**.

Alternatif build melalui command line:

```bash
MSBuild.exe Kerja_Projek.sln /t:Rebuild /p:Configuration=Debug
```

## Catatan

- Seluruh aksi penting pengguna dicatat ke tabel `audit_log`.
- Logo aplikasi tersedia di `Kerja_Projek/Assets/logopkk.png`.
- Hak akses menu menyesuaikan role pengguna yang sedang login.

## Lisensi

Proyek ini menggunakan lisensi [MIT](LICENSE).

---

<p align="center">
  Dibuat oleh <strong>Sofian Bahtiar</strong>
</p>
