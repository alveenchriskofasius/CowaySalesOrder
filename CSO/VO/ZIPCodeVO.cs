using System;
using System.Data;

namespace CSO.VO
{
    public class ZIPCodeVO : BaseVO
    {
        private string _province;
        public string Province
        {
            get { return _province; }
            set { SetProperty(ref _province, value); }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        private string _district;
        public string District
        {
            get { return _district; }
            set { SetProperty(ref _district, value); }
        }

        private string _urban;
        public string Urban
        {
            get { return _urban; }
            set { SetProperty(ref _urban, value); }
        }

        private string _zip;
        public string ZIP
        {
            get { return _zip; }
            set { SetProperty(ref _zip, value); }
        }

        public ZIPCodeVO(ZIPCodeVO zip)
        {
            SetValue(zip);

        }

        public ZIPCodeVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }

        public ZIPCodeVO()
        {
        }

        public override string GetError(string columnName)
        {
            throw new NotImplementedException();
        }
    }
}
