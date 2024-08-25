using InstitutoTriboDeDavi.API.ViewModels;

namespace InstitutoTriboDeDavi.API.Utilities
{
    public static class Responses
    {
        public static ResultViewModel ApplicationErrorMessage() 
        {
            return new ResultViewModel
            {
                Message = "Ocorreu algum erro interno na aplicação, por favor tente novamente.",
                Success = false,
                Data = null
            };
        }

        public static ResultViewModel DomainErrorMessage(string message)
        {
            return new ResultViewModel
            {
                Message = message,
                Success = false,
                Data = null
            };
        }

        public static ResultViewModel DomainErrorMessage(string message, IReadOnlyCollection<string> errors)
        {
            return new ResultViewModel
            {
                Message = message,
                Success = false,
                Data = errors
            };
        }

        public static ResultViewModel UnathorizedErrorMessage() 
        {
            return new ResultViewModel
            {
                Message = "A comn]binação de login e senha está incorreta!",
                Success = false,
                Data = null
            };
        }
    }
}
