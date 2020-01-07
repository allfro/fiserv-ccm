using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.CCMSoapWebService;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Models;

namespace LMS.Connector.CCM.Repositories
{
    /// <summary>
    /// Provides a repository for interacting with the CCM SOAP service.
    /// </summary>
    public interface ISoapRepository : IServiceRepository
    {
        CredentialsHeader CredentialsHeader { get; set; }

        CcmWebServiceSoap SoapClient { get; set; }

        ProcessMessageNodeRequest ProcessMessageNodeRequest { get; set; }

        ProcessMessageNodeResponse ProcessMessageNodeResponse { get; set; }

        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Factory method for ProcessMessageNodeRequest object.
        /// </summary>
        /// <param name="credentialsHeader"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        ProcessMessageNodeRequest GetProcessMessageNodeRequest(CredentialsHeader credentialsHeader, string request);

        /// <summary>
        /// Calls CCM SOAP Web Service to process a Message and returns a MessageResponse.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        MessageResponse ProcessMessage(string message);

        /// <summary>
        /// Calls CCM SOAP Web Service to add an account.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="app"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        MessageResponse AddAccount(AddAccount account, Application app, Applicant applicant);

        /// <summary>
        /// Calls CCM SOAP Web Service to add an account party relationship.
        /// </summary>
        /// <param name="accountPartyRelationship"></param>
        /// <param name="app"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        MessageResponse AddAccountPartyRelationship(AddAccountPartyRelationship accountPartyRelationship, Application app, Applicant applicant);

        /// <summary>
        /// Calls CCM SOAP Web Service to add a card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="app"></param>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        MessageResponse AddCard(AddCard card, Application app, LmsPerson lmsPerson);

        /// <summary>
        /// Calls CCM SOAP Web Service to add an organization.
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="app"></param>
        /// <param name="applicant"></param>
        /// <param name="address"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        MessageResponse AddOrganization(AddOrganization organization, Application app, Applicant applicant, Address address, Phone phone);

        /// <summary>
        /// Calls CCM SOAP Web Service to add a person.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="app"></param>
        /// <param name="lmsPerson"></param>
        /// <param name="address"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        MessageResponse AddPerson(AddPerson person, Application app, LmsPerson lmsPerson, Address address, Phone phone);

        /// <summary>
        /// Calls CCM SOAP Web Service to update an account.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="app"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        MessageResponse UpdateAccount(UpdateAccount account, Application app, Applicant applicant);

        /// <summary>
        /// Calls CCM SOAP Web Service to update person.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        MessageResponse UpdatePerson(UpdatePerson person, Application app);
    }
}
