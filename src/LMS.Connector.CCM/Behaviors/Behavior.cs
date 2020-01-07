using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public abstract class Behavior
    {
        protected IRestRepository _restRepository;
        protected ISoapRepository _soapRepository;
        protected ILmsRepository _lmsRepository;

        public virtual IRestRepository RestRepository
        {
            get
            {
                return _restRepository;
            }
            set
            {
                _restRepository = value;
            }
        }

        public virtual ISoapRepository SoapRepository
        {
            get
            {
                return _soapRepository;
            }
            set
            {
                _soapRepository = value;
            }
        }

        public virtual ILmsRepository LmsRepository
        {
            get
            {
                return _lmsRepository;
            }
            set
            {
                _lmsRepository = value;
            }
        }
    }
}
