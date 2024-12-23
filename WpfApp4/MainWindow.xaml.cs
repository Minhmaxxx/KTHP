using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp4.Models;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        QlbanHangContext db = new QlbanHangContext();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HienThiDuLieu();
            var comboboxQuery = from lsp in db.LoaiSanPhams
                                select lsp;
            maloai.ItemsSource = comboboxQuery.ToList();
            maloai.DisplayMemberPath = "TenLoai";
            maloai.SelectedValuePath = "MaLoai";
            maloai.SelectedIndex = 0;
                                
        }
        

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            if (isCheckThem())
            {
                SanPham sp = new SanPham();
                sp.MaSp = masanpham.Text;
                sp.TenSp = tensanpham.Text;
                sp.MaLoai = maloai.SelectedValue.ToString();
                sp.DonGia = int.Parse(dongia.Text);
                sp.SoLuong = int.Parse(soluong.Text);

                db.SanPhams.Add(sp);
                db.SaveChanges();
                HienThiDuLieu(); 
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (isCheckSua())
            {
                var suaQuery = from sp in db.SanPhams
                               where sp.MaSp == masanpham.Text
                               select sp;
                SanPham spSua = suaQuery.FirstOrDefault();
                if (spSua != null)
                {
                    spSua.TenSp = tensanpham.Text;
                    spSua.MaLoai = maloai.SelectedValue.ToString();
                    spSua.DonGia = int.Parse(dongia.Text);
                    spSua.SoLuong = int.Parse(soluong.Text);
                }
                else
                {
                    MessageBox.Show("Khong tim thay ma san pham can sua!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                db.SaveChanges();
                HienThiDuLieu(); 
            }


        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            var xoaQuery = from sp in db.SanPhams
                           where sp.MaSp == masanpham.Text
                           select sp;
            SanPham spXoa = xoaQuery.FirstOrDefault();
            if (spXoa != null)
            {
                MessageBoxResult traloi = MessageBox.Show("Ban co chac chan muon xoa san pham nay?", "??", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (traloi == MessageBoxResult.Yes)
                {
                    db.SanPhams.Remove(spXoa);
                    db.SaveChanges();
                    HienThiDuLieu();
                }
            }
            else
            {
                MessageBox.Show("Khong tim thay ma san pham can xoa!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool isCheckThem()
        {
            if (masanpham.Text == "" || tensanpham.Text == "" || soluong.Text == "" || dongia.Text =="")
            {
                MessageBox.Show("Ban phai nhap toan bo truong du lieu", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private bool isCheckSua()
        {
            if(!int.TryParse(soluong.Text, out int SoLuong) || SoLuong <= 0)
            {
                MessageBox.Show("So luong phai la so nguyen va lon hon 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            var timQuery = from sp in db.SanPhams
                           join lsp in db.LoaiSanPhams
                           on sp.MaLoai equals lsp.MaLoai
                           group sp by lsp.TenLoai into nhom
                           select new
                           {
                               TenLoai = nhom.Key,
                               TongSoLuong = nhom.Sum(sp => sp.SoLuong),
                               TongSoTien = nhom.Sum(sp => sp.DonGia * sp.SoLuong)
                           };
            Window1 window1 = new Window1();
            window1.resultTim.ItemsSource = timQuery.ToList();
            window1.Show();

        }
        private void HienThiDuLieu()
        {
            var sanphamQuery = from sp in db.SanPhams
                               orderby sp.DonGia descending
                               select new
                               {
                                   sp.MaSp,
                                   sp.TenSp,
                                   sp.MaLoaiNavigation.TenLoai,
                                   sp.DonGia,
                                   sp.SoLuong,
                                   sp.MaLoai,
                                   ThanhTien = sp.DonGia * sp.SoLuong
                               };
            result.ItemsSource = sanphamQuery.ToList();
        }
        private void result_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(result.SelectedItem != null)
            {
                var itemChon = (dynamic)result.SelectedItem;

                masanpham.Text = itemChon.MaSp;
                tensanpham.Text = itemChon.TenSp;
                maloai.Text = itemChon.TenLoai;
                dongia.Text = itemChon.DonGia.ToString();
                soluong.Text = itemChon.SoLuong.ToString();
            }
        }
    }
}