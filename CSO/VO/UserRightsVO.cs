using System.Data;

namespace CSO.VO
{
    public class RightsVO : BaseVO
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private bool _isAssigned = false;
        public bool IsAssigned
        {
            get { return _isAssigned; }
            set { SetProperty(ref _isAssigned, value); }
        }
        public RightsVO()
        {
        }

        public RightsVO(DataRow dataRow)
        {
            SetValue(dataRow);
        }

        public override string GetError(string columnName)
        {
            string result = null;
            return result;
        }
    }
}
