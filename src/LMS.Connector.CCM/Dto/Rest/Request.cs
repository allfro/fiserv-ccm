namespace LMS.Connector.CCM.Dto.Rest
{
    /// <summary>
    /// Generic class used in making POST requests with a header and a body payload to the CCM service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Request<T> where T : class
    {
        public T Body { get; set; }
    }
}
