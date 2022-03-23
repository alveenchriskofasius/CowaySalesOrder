using System;

namespace CSO.VO
{

    public class FilterVO : BaseVO
    {
        #region Property setters/getters
        private string _no = "";
        public string No
        {
            get { return _no; }
            set
            {
                SetProperty(ref _no, value);
            }
        }
        private string _fullName = "";
        public string FullName
        {
            get { return _fullName; }
            set
            {
                SetProperty(ref _fullName, value);
            }
        }

        private bool _active = true;
        public bool Active
        {
            get { return _active; }
            set
            {
                SetProperty(ref _active, value);
            }
        }
        private int _provinceID = 0;
        public int ProvinceID
        {
            get { return _provinceID; }
            set
            {
                SetProperty(ref _provinceID, value);
            }
        }
        private int _cityID = 0;
        public int CityID
        {
            get { return _cityID; }
            set
            {
                SetProperty(ref _cityID, value);
            }
        }

        private int _areaID = 0;
        public int AreaID
        {
            get { return _areaID; }
            set
            {
                SetProperty(ref _areaID, value);
            }
        }

        private int _tradertypeID = 0;
        public int TraderTypeID
        {
            get { return _tradertypeID; }
            set
            {
                SetProperty(ref _tradertypeID, value);
            }
        }
        private DateTime _date = DateTime.Now;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        #endregion

        public FilterVO()
        {
        }

        public FilterVO(FilterVO filter)
        {
            SetValue(filter);
        }

        #region BaseVO Members
        public override string GetError(string columnName)
        {
            string result = null;
            return result;
        }

        #endregion

    }

}
