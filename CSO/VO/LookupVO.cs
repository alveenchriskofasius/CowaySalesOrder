using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace CSO.VO
{
    public class LookupVO : BaseVO
    {
        #region Property getters and setters

        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }
        #endregion
        public LookupVO()
        {
        }

        public LookupVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }

        #region BaseVO getError
        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
