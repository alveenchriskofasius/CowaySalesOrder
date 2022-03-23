using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    public abstract class BaseMasterVO : BaseVO
    {
        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private string _no = "";
        public string No
        {
            get { return _no; }
            set
            {
                SetProperty(ref _no, value);
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
    }
}