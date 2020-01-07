using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.FrameworkSecurity;
using Akcelerant.Lending.Data.DTO.Applications;
using Akcelerant.Lending.Lookups;
using Akcelerant.Lending.Lookups.Constants;
using LMS.Connector.CCM.Behaviors;
using LMS.Connector.CCM.Behaviors.Soap;
using LMS.Connector.CCM.CCMSoapWebService;
using LMS.Connector.CCM.Dto;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Proxies;
using LMS.Connector.CCM.Repositories;
using LMS.Core.HostValues.Utility.Translator.Xml;

namespace LMS.Connector.CCM.UnitTests.Behaviors.Soap
{
    [TestFixture]
    public class UpdatePersonBehaviorTests
    {
    }
}
