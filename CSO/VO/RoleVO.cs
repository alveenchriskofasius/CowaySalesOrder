using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSO.VO
{
    public class RoleVO : BaseVO
    {
        #region Property setters/getters
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

        private string _userName = "";
        public string Username
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
            }
        }
        private bool _selected = false;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value);
            }
        }
        #endregion

        public RoleVO(RoleVO role)
        {
            SetValue(role);
        }

        public RoleVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }

        public RoleVO()
        {
        }

        #region Base Member
        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
