using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace CSO.VO
{
    public enum Level { Any = 0, Province, City, Area };
    public class AreaVO : BaseVO
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

        private Level _level;
        public Level Level
        {
            get { return _level; }
            set
            {
                SetProperty(ref _level, value);
            }
        }

        private int _parentID = 0;
        public int ParentID
        {
            get { return _parentID; }
            set
            {
                SetProperty(ref _parentID, value);
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

        private string _parentName = "";
        public string ParentName
        {
            get { return _parentName; }
            set
            {
                SetProperty(ref _parentName, value);
            }
        }
        #endregion

        public AreaVO(AreaVO area)
        {
            SetValue(area);

        }

        public AreaVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }

        public AreaVO()
        {
        }

        #region BaseVO Members
        public override string GetError(string columnName)
        {
            string result = null;
            switch (columnName)
            {
                case "Name":
                    if (string.IsNullOrEmpty(Name))
                    {
                        result = "Nama wajib diisi";
                    }

                    break;

            }

            return result;
        }
        #endregion



    }
}
