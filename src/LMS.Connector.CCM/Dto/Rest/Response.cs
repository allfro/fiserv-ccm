namespace LMS.Connector.CCM.Dto.Rest
{
    /// <summary>
    /// Generic class for holding the payload body of a CCM service response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T> : ResponseBase
    {
        public T Body { get; set; }
    }
}
