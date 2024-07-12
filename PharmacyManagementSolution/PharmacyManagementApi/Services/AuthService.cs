using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging; 
using System;
using System.Security.Claims;

namespace PharmacyManagementApi.Services
{
    /// <summary>
    /// Service responsible for authentication and user management.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IRepository<int, Customer> _customerRepo;
        private readonly ITokenService _tokenService;
        private readonly IRepository<string, UserCredential> _credentialRepo;
        private readonly ILogger<AuthService> _logger; // Logger instance

        /// <summary>
        /// Constructor for AuthService.
        /// </summary>
        /// <param name="credentialRepo">Repository for user credentials.</param>
        /// <param name="customerRepo">Repository for customers.</param>
        /// <param name="tokenService">Service for generating tokens.</param>
        /// <param name="logger">Logger instance.</param>
        public AuthService(IRepository<string, UserCredential> credentialRepo, IRepository<int, Customer> customerRepo, ITokenService tokenService, ILogger<AuthService> logger)
        {
            _credentialRepo = credentialRepo;
            _customerRepo = customerRepo;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Logs in a user based on login credentials.
        /// </summary>
        /// <param name="loginDTO">Login credentials.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation that returns a <see cref="SuccessLoginDTO"/>.</returns>
        public async Task<SuccessLoginDTO> Login(LoginDTO loginDTO)
        {
            try
            {
                UserCredential userCredential = await _credentialRepo.Get(loginDTO.Email);
                HMACSHA512 hash = new HMACSHA512(userCredential.HasedPassword);
                var password = hash.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                if (await CheckPassword(userCredential.Password, password))
                {
                    if (userCredential.AccountStatus == "Enable")
                    {
                        Customer customer = await _customerRepo.Get(userCredential.UserId);

                        SuccessLoginDTO success = new SuccessLoginDTO()
                        {
                            Code = 200,
                            Role = customer.Role,
                            AccessToken = await _tokenService.GenerateToken(customer)
                        };
                        return success;
                    }
                    //throw new UserNotVerifiedException("User is not verified");

                }
                throw new UnAuthorizedUserException("User Name or Password not correct");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in."); // Log the error
                throw;
            }
        }

        [ExcludeFromCodeCoverage] // Excluded from code coverage as it's a utility method
        private async Task<UserCredential> CreateCredential(string password, string email)
        {
            try
            {
                HMACSHA512 hash = new HMACSHA512();

                UserCredential user = new UserCredential()
                {
                    Email = email,
                    HasedPassword = hash.Key,
                    Password = hash.ComputeHash(Encoding.UTF8.GetBytes(password))
                };
                return user;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user credential."); // Log the error
                throw;
            }
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
                UserCredential credential = await CreateCredential(customerDTO.Password, customerDTO.Email);
                credential.UserId = customer.CustomerId;
                await _credentialRepo.Add(credential);

                SuccessRegisterDTO success = new SuccessRegisterDTO()
                {
                    Code = 200,
                    Message = "User Registered Successsfully",
                    CustomerId = customer.CustomerId

                };

                return success;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user."); // Log the error
                throw;
            }

        }
        public (bool isValid, string? role) ValidateUserTokenAndGetRole(string token)
        {
            try
            {
                var (isValid, claimsPrincipal) = _tokenService.ValidateToken(token);
                if (!isValid || claimsPrincipal == null)
                {
                    return (false, null);
                }

                var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
                string? role = roleClaim?.Value;

                return (true, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating user token and extracting role.");
                throw;
            }
        }


    }
}
