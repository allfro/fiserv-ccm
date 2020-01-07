using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Helpers
{
    public static class XmlSerializationExtensions
    {
        /// <summary>
        /// Serializes an object into XML and then converts it to a string.
        /// Adds custom namespaces to the serialized XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToXmlString<T>(this T obj)
        {
            var xmlString = string.Empty;

            if (obj == null)
            {
                return xmlString;
            }

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("msg", "http://www.opensolutions.com/CCM/CcmMessage.xsd");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            var xmlSerializer = new XmlSerializer(typeof(T));
            var sw = new StringWriter();

            using (var xmlWriter = XmlWriter.Create(sw))
            {
                xmlSerializer.Serialize(xmlWriter, obj, namespaces);
                xmlString = sw.ToString();
            }

            return xmlString;
        }

        /// <summary>
        /// Serializes a string into a byte[].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] SerializeToByteArray(this string str)
        {
            var bf = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, str);
                var byteArray = ms.ToArray();

                return byteArray;
            }
        }

        /// <summary>
        /// Serializes an object into a byte[].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToByteArray<T>(this T obj)
        {
            byte[] byteArray = null;

            if (obj == null)
            {
                return byteArray;
            }

            var bf = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                byteArray = ms.ToArray();

                return byteArray;
            }
        }

        /// <summary>
        /// Deserializes an XML string into a known object type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString">XML string</param>
        /// <returns></returns>
        public static T Deserialize<T>(this string xmlString) where T : class
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var sr = new StringReader(xmlString))
            {
                var obj = (T)xmlSerializer.Deserialize(sr);

                return obj;
            }
        }

        /// <summary>
        /// Deserializes an XML node into a known object type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNode">XML node</param>
        /// <returns></returns>
        public static T Deserialize<T>(this XmlNode xmlNode) where T : class
        {
            var ms = new MemoryStream();

            var sw = new StreamWriter(ms);
            sw.Write(xmlNode.OuterXml);
            sw.Flush();

            ms.Position = 0;

            var xmlSerializer = new XmlSerializer(typeof(T));
            T result = (xmlSerializer.Deserialize(ms) as T);

            return result;
        }

        /// <summary>
        /// Deserializes a byte array into a known object type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }

            using (var ms = new MemoryStream(byteArray))
            {
                var bf = new BinaryFormatter();

                ms.Write(byteArray, 0, byteArray.Length);
                ms.Seek(0, SeekOrigin.Begin);

                var obj = (T)bf.Deserialize(ms);

                return obj;
            }
        }
    }
}
