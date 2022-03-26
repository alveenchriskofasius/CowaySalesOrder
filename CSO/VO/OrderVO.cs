﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    public class OrderVO : BaseHeaderVO
    {
        #region Property setters/getters

        private DateTime _deliveryDate = DateTime.Now;
        public DateTime DeliveryDate
        {
            get { return _deliveryDate; }
            set
            {
                SetProperty(ref _deliveryDate, value);
            }
        }
        private int _productTypeID = 0;
        public int ProductTypeID
        {
            get { return _productTypeID; }
            set
            {
                SetProperty(ref _productTypeID, value);
            }
        }

        private int _personInChargeID = 0;
        public int PersonInChargeID
        {
            get { return _personInChargeID; }
            set
            {
                SetProperty(ref _personInChargeID, value);
            }
        }

        private int _assignID = 0;
        public int AssignID
        {
            get { return _assignID; }
            set
            {
                SetProperty(ref _assignID, value);
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
        private byte[] _urlImage = new byte[0];

        public byte[] URLImage
        {
            get { return _urlImage; }
            set { SetProperty(ref _urlImage, value); }
        }

        private int _customerID = 0;
        public int CustomerID
        {
            get { return _customerID; }
            set
            {
                SetProperty(ref _customerID, value);
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

        private int _paymentTypeID = 0;
        public int PaymentTypeID
        {
            get { return _paymentTypeID; }
            set
            {
                SetProperty(ref _paymentTypeID, value);
            }
        }
        private string _paymentTypeName = "";
        public string PaymentTypeName
        {
            get { return _paymentTypeName; }
            set
            {
                SetProperty(ref _paymentTypeName, value);
            }
        }

        private int _promotionID = 0;
        public int PromotionID
        {
            get { return _promotionID; }
            set
            {
                SetProperty(ref _promotionID, value);
            }
        }

        private int _orderProductID = 0;
        public int OrderProductID
        {
            get { return _orderProductID; }
            set
            {
                SetProperty(ref _orderProductID, value);
            }
        }

        private string _installAddress = "";
        public string InstallAddress
        {
            get { return _installAddress; }
            set
            {
                SetProperty(ref _installAddress, value);
            }
        }

        private int _quantity = 0;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                SetProperty(ref _quantity, value);
            }
        }
        private int _servicePackageID = 0;
        public int ServicePackageID
        {
            get { return _servicePackageID; }
            set
            {
                SetProperty(ref _servicePackageID, value);
            }
        }

        private string _servicePackageName = "";
        public string ServicePackageName
        {
            get { return _servicePackageName; }
            set
            {
                SetProperty(ref _servicePackageName, value);
            }
        }

        private decimal _grandTotal = 0;
        public decimal GrandTotal
        {
            get { return _grandTotal; }
            set
            {
                SetProperty(ref _grandTotal, value);
            }
        }

        private bool _isCustomerAddress = false;
        public bool IsCustomerAddress
        {
            get { return _isCustomerAddress; }
            set
            {
                SetProperty(ref _isCustomerAddress, value);
            }
        }

        private bool _isDiscounted = false;
        public bool IsDiscounted
        {
            get { return _isDiscounted; }
            set
            {
                SetProperty(ref _isDiscounted, value);
            }
        }

        private bool _isUploaded = false;
        public bool IsUploaded
        {
            get { return _isUploaded; }
            set
            {
                SetProperty(ref _isUploaded, value);
            }
        }

        private decimal _discount = 0;
        public decimal Discount
        {
            get { return _discount; }
            set
            {
                SetProperty(ref _discount, value);
            }
        }
        private bool _isServiceChange = false;
        public bool IsServiceChange
        {
            get { return _isServiceChange; }
            set
            {
                SetProperty(ref _isServiceChange, value);
            }
        }
        private ObservableCollection<OrderProductVO> _products = new ObservableCollection<OrderProductVO>();
        public ObservableCollection<OrderProductVO> Products
        {
            get
            {
                return _products;
            }
            set
            {
                SetProperty(ref _products, value);
            }
        }
        #endregion
        public OrderVO()
        {
            Status = "Open";
        }
        public OrderVO(OrderVO order)
        {
            SetValue(order);
        }

        public OrderVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }
        private OrderVO _temp;
        public void Validate()
        {
            // save to temp
            _temp = new OrderVO(this);

            // dummy values to validate all data
            PersonInChargeID = CustomerID = ServicePackageID = PaymentTypeID = -1;
            InstallAddress = "InstallAddress";
        }

        public void Restore()
        {
            // restore to original values
            SetValue(_temp);
        }
        public override string GetError(string columnName)
        {
            string result = string.Empty;
            switch (columnName)
            {
                case "CustomerID":
                    if (CustomerID == 0)
                    {
                        result = "Pelanggan wajib di isi";
                    }
                    break;
                case "PaymentTypeID":
                    if (PaymentTypeID == 0)
                    {
                        result = "Tipe pembayaran wajib di isi";
                    }
                    break;
                case "ServicePackageID":
                    if (ServicePackageID == 0)
                    {
                        result = "Paket wajib di isi";
                    }
                    break;
                case "PersonInChargeID":
                    if (PersonInChargeID == 0)
                    {
                        result = "PIC wajib di isi";
                    }
                    break;
                case "InstallAddress":
                    if (string.IsNullOrEmpty(InstallAddress))
                    {
                        result = "Alamat Pemasangan wajib di isi";
                    }
                    break;
            }
            return result;
            throw new NotImplementedException();
        }
    }
}
