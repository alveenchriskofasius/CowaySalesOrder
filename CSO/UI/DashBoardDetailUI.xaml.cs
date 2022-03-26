using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSO.VO;
using CSO.Proxy;
namespace CSO.UI
{
    /// <summary>
    /// Interaction logic for DashBoardDetailUI.xaml
    /// </summary>
    public partial class DashBoardDetailUI : BaseUI
    {
        OrderDetailVO _orderDetail;
        public DashBoardDetailUI()
        {
            InitializeComponent();
        }
        public void FillDetail(OrderVO order)
        {

            _orderDetail.SetValue(OrderProxy.DataDetail(order.ID));
            GridProduct.ItemsSource = OrderProxy.DataProduct(order.ID);
            GetImagesFromDatabase(_orderDetail.URLImage);
        }
        void GetImagesFromDatabase(byte[] pictureIdByte)
        {
            try
            {

                if (pictureIdByte.Length > 0)
                {
                    using (var stream = new MemoryStream(pictureIdByte))
                    {
                        _orderDetail.Picture = BitmapFrame.Create(stream,
                                                BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    ImageUpload.Source = _orderDetail.Picture;
                }
            }
            catch (Exception e)
            {
                Main.ShowMessage("Gagal tarik gambar", e.Message, Message.Error);
            }
        }
        private void BaseUI_Loaded(object sender, RoutedEventArgs e)
        {
            _orderDetail = new OrderDetailVO();
            DataContext = _orderDetail;
        }
    }
}
