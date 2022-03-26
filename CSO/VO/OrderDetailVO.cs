using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CSO.VO
{
    public class OrderDetailVO : BaseHeaderVO
    {
        #region Property setters/getters
        private string _areaName = "";
        public string AreaName
        {
            get { return _areaName; }
            set
            {
                SetProperty(ref _areaName, value);
            }
        }
        private string _cityName = "";
        public string CityName
        {
            get { return _cityName; }
            set
            {
                SetProperty(ref _cityName, value);
            }
        }

        private string _districtName = "";
        public string DistrictName
        {
            get { return _districtName; }
            set
            {
                SetProperty(ref _districtName, value);
            }
        }

        private string _urbanName = "";
        public string UrbanName
        {
            get { return _urbanName; }
            set
            {
                SetProperty(ref _urbanName, value);
            }
        }
        private string _customerName = "";
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                SetProperty(ref _customerName, value);
            }
        }
        private string _personInChargeName = "";
        public string PersonInChargeName
        {
            get { return _personInChargeName; }
            set
            {
                SetProperty(ref _personInChargeName, value);
            }
        }
        private string _ctName = "";
        public string CTName
        {
            get { return _ctName; }
            set
            {
                SetProperty(ref _ctName, value);
            }
        }
        private string _phone = "";
        public string Phone
        {
            get { return _phone; }
            set
            {
                SetProperty(ref _phone, value);
            }
        }
        public ImageSource Picture { get; set; }
        private byte[] _urlImage = new byte[0];

        public byte[] URLImage
        {
            get { return _urlImage; }
            set { SetProperty(ref _urlImage, value); }
        }
        private string _address = "";
        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
            }
        }
        public OrderDetailVO()
        {
            Status = "Open";
        }
        public OrderDetailVO(OrderDetailVO orderDetail)
        {
            SetValue(orderDetail);
        }

        public OrderDetailVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }
        #endregion
        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}
