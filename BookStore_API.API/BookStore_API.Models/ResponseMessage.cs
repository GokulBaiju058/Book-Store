
namespace BookStore_API.Models
{
    /// <summary>
    /// Represents a response message with a specific data type.
    /// Inherits from GenericResponseMessage.
    /// </summary>
    public class ResponseMessage<TDataType> : GenericResponseMessage
    {
        public TDataType? Data { get; set; }

        public void SetErrorMessage(string DetailMessage, string Message = "Oops, something went wrong!")
        {
            this.Data = default(TDataType);
            base.Success = false;
            base.Message = Message;
            base.DetailMessage = DetailMessage;
        }
    }
}
