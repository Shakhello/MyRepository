namespace PikAPI.Models
{
    public class Response
    {
        public object Data { get; set; }
        public ResponseStatus Status { get; set; }
        public string Msg { get; set; }

        public static Response Ok(object data)
        {
            return new Response()
            {
                Data = data,
                Status = ResponseStatus.OK
            };
        }

        public static Response Error(string msg)
        {
            return new Response()
            {
                Status = ResponseStatus.ERROR,
                Msg = msg
            };
        }
    }
}