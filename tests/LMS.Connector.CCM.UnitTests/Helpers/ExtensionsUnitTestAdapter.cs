using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Akcelerant.Core.Data.DTO;
using LMS.Connector.CCM.Helpers;

namespace LMS.Connector.CCM.UnitTests.Helpers
{
    public class ExtensionsUnitTestAdapter
    {
        public string SerializeToXmlString<T>(T obj) where T : class
        {
            var xmlString = obj.SerializeToXmlString();

            return xmlString;
        }

        public byte[] SerializeToByteArray(string str)
        {
            var byteArray = str.SerializeToByteArray();

            return byteArray;
        }

        public T Deserialize<T>(byte[] byteArray) where T : class
        {
            var obj = byteArray.Deserialize<T>();

            return obj;
        }

        public T Deserialize<T>(string xmlString) where T : class
        {
            var obj = xmlString.Deserialize<T>();

            return obj;
        }
    }
}
