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
    internal class PromotionProxy
    {
        public static List<PromotionVO> Data()
        {
            List<PromotionVO> promotions = new List<PromotionVO>();

            DataSet result = DBHelper.ExecuteProcedure("uspPromotionGet");

            foreach (DataRow dataRow in result.Tables[0].Rows)
            {
                promotions.Add(new PromotionVO(dataRow));
            }
            return promotions;
        }
    }
}
