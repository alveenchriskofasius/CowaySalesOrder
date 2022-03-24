using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    internal class PromotionVO : BaseMasterVO
    {
        public PromotionVO()
        {
        }
        public PromotionVO(PromotionVO promotion)
        {
            SetValue(promotion);
        }

        public PromotionVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }
        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}
