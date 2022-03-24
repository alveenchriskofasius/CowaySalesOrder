using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    internal class ProductVO : BaseMasterVO
    {
        #region Property setters/getters
        private decimal _price = 0;
        public decimal Price
        {
            get { return _price; }
            set
            {
                SetProperty(ref _price, value);
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

        private int _productTypeID = 0;
        public int ProductTypeID
        {
            get { return _productTypeID; }
            set
            {
                SetProperty(ref _productTypeID, value);
            }
        }
        #endregion
        public ProductVO()
        {
        }
        public ProductVO(ProductVO product)
        {
            SetValue(product);
        }

        public ProductVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }
        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}
