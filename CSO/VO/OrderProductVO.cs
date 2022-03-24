using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    internal class OrderProductVO : BaseMasterVO
    {
        #region Property setters/getters

        private int _productID = 0;
        public int ProductID
        {
            get { return _productID; }
            set
            {
                SetProperty(ref _productID, value);
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
        private int _orderID = 0;
        public int OrderID
        {
            get { return _orderID; }
            set
            {
                SetProperty(ref _orderID, value);
            }
        }

        private string _productName = "";
        public string ProductName
        {
            get { return _productName; }
            set
            {
                SetProperty(ref _productName, value);
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

        private decimal _amount = 0;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                SetProperty(ref _amount, value);
            }
        }

        private decimal _discountAmount = 0;
        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                SetProperty(ref _discountAmount, value);
            }
        }

        private decimal _totalAmount = 0;
        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                SetProperty(ref _totalAmount, value);
            }
        }

        private bool _isEdited = false;
        public bool IsEdited
        {
            get { return _isEdited; }
            set
            {
                SetProperty(ref _isEdited, value);
            }
        }

        public OrderProductVO()
        {
        }
        public OrderProductVO(OrderProductVO orderProduct)
        {
            SetValue(orderProduct);
        }
        #endregion
        public OrderProductVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }
        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}
