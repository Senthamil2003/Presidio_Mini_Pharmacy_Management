using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using System.Security.Cryptography;
using System.Text;

namespace PharmacyManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IReposiroty<int , Customer> _customerRepo;
        private readonly ITokenService _tokenService;
        private readonly IReposiroty<string,UserCredential> _credentialRepo;

        public AuthService(IReposiroty<string,UserCredential> credentialRepo, IReposiroty<int ,Customer> customerRepo,ITokenService tokenService) {
            _credentialRepo = credentialRepo;
            _customerRepo = customerRepo;
            _tokenService= tokenService;

        }
        private async Task<bool> CheckPassword(byte[] userPassword, byte[] GivenPassword)
        {
            for (int i = 0; i < userPassword.Length; i++)
            {
                if (userPassword[i] != GivenPassword[i])
                {
                    return false;

                }
            }
            return true;
        }
        public async Task<SuccessLoginDTO> Login(LoginDTO loginDTO)
        {
            try
            {
                UserCredential userCredential = await _credentialRepo.Get(loginDTO.Email);
                HMACSHA512 hash = new HMACSHA512(userCredential.HasedPassword);
                var password = hash.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                if (await CheckPassword(userCredential.Password,password))
                {
                    if (userCredential.AccountStatus == "Disable")
                    {
                        Customer customer =await _customerRepo.Get(userCredential.UserId);

                        SuccessLoginDTO success = new SuccessLoginDTO()
                        {
                            Code = 200,
                            Message ="Welcome back "+ customer.Name,
                            AccessToken = await _tokenService.GenerateToken(customer)
                        };
                        return success;
                    }
                    throw new UserNotVerifiedException("User is not verified");

                }
                throw new UnAuthorizedUserException("User Name or Password not correct");
            }
            catch
            {
                throw;
            }

        }

        private async Task<UserCredential> CreateCredential(string password,string email)
        {
            try
            {
                HMACSHA512 hash = new HMACSHA512();

                UserCredential user = new UserCredential()
                {
                    Email = email,
                    HasedPassword=hash.Key,
                    Password = hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                };
                return user;
            }
            catch 
            {
                throw;
            }
        }
        public async Task<SuccessRegisterDTO> Register(RegisterDTO customerDTO)
        {
            try
            {
                Customer customer = new Customer()
                {
                    Name = customerDTO.Name,
                    Phone = customerDTO.Phone,
                    Role = customerDTO.Role,
                    Address = customerDTO.Address,
                    Email = customerDTO.Email,          
                    
                };

                await _customerRepo.Add(customer);
                UserCredential credential= await CreateCredential(customerDTO.Password,customerDTO.Email);
                credential.UserId=customer.CustomerId;
                await _credentialRepo.Add(credential);

                SuccessRegisterDTO success = new SuccessRegisterDTO()
                {
                    Code = 200,
                    Message = "User Registered Successsfully",
                    CustomerId = customer.CustomerId
                    
                };

                return success;

            }
            catch
            {
                throw;
            }

        }
    }
}
