using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSO.VO;
using Helper;

namespace CSO.Proxy
{
    internal class ProductProxy
    {
        public static List<ProductVO> Data(bool isDiscount = false, int typeID = 0, decimal discount = 0)
        {
            List<ProductVO> products = new List<ProductVO>();
            object[] parameters =
            {
                "@IsDiscount", isDiscount,
                "@TypeID",typeID,
                "@Discount",discount
            };
            DataSet result = DBHelper.ExecuteProcedure("uspProductGet", parameters);

            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                products.Add(new ProductVO(dataRow));
            }
            return products;
        }
    }
}
