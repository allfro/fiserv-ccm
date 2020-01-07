using LMS.Connector.CCM.Models;

namespace LMS.Connector.CCM.Repositories
{
    /// <summary>
    /// Provides a repository for interacting with the CCM service proxy.
    /// </summary>
    public interface IServiceRepository
    {
        string UserToken { get; set; }

        ILmsRepository LmsRepository { get; set; }

        Credentials Credentials { get; set; }
    }
}
