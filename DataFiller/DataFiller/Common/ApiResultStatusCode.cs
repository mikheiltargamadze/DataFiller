using System.ComponentModel.DataAnnotations;

namespace Common
{
    public enum ApiResultStatusCode
    {
        [Display(Name = "The operation was successful.")]
        Success = 200,

        [Display(Name = "An error has occurred on the server")]
        ServerError = 500,

        [Display(Name = "The parameters sent are not valid")]
        BadRequest = 400,

        [Display(Name = "not found")]
        NotFound = 404,

        [Display(Name = "The list is empty")]
        ListEmpty = 204,

        [Display(Name = "A processing error occurred")]
        LogicError = 409,

        [Display(Name = "Authentication error")]
        UnAuthorized = 403,
        
        [Display(Name = "Sales restrictions")]
        SaleLimitation = 210
    }
}