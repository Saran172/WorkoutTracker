using Shared.Entities;

namespace WorkoutTracker.API.LoginServices
{
    public interface IloginService
    {
        public Task<ResponseDto> WebLogin(WebLoginDto LoginDto);
        public Task<ResponseDto> WebRegister(WebRegisterDto LoginDto);
    }
}
