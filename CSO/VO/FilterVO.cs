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
        private string _statusList = "";
        public string StatusList
        {
            get { return _statusList; }
            set
            {
                SetProperty(ref _statusList, value);
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
        private int _statusID = 0;
        public int StatusID
        {
            get { return _statusID; }
            set
            {
                SetProperty(ref _statusID, value);
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

        private int _customerID = 0;
        public int CustomerID
        {
            get { return _customerID; }
            set
            {
                SetProperty(ref _customerID, value);
            }
        }
        private int _customerTypeID = 0;
        public int CustomerTypeID
        {
            get { return _customerTypeID; }
            set
            {
                SetProperty(ref _customerTypeID, value);
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
        private DateTime _dateFrom = DateTime.Now;
        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set
            {
                SetProperty(ref _dateFrom, value);
            }
        }
        private DateTime _dateTo = DateTime.Now;
        public DateTime DateTo
        {
            get { return _dateTo; }
            set
            {
                SetProperty(ref _dateTo, value);
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
