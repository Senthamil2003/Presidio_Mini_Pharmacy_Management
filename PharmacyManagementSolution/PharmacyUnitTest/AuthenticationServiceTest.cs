using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest
{
    public class AuthenticationServiceTest
    {
        private IAuthService _authService;
        private IReposiroty<int, Customer> _customerRepo;
        private IReposiroty<string, UserCredential> _credentialRepo;
        private PharmacyContext context;
        private  ITokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PharmacyContext>()
                      .UseSqlite("DataSource=:memory:")
                      .Options;

            context = new PharmacyContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureDeleted(); // Ensure database is deleted
            context.Database.EnsureCreated(); // Ensure the schema is created
            _customerRepo =new CustomerRepository(context);
            _credentialRepo = new UserCredentialRepository(context);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("Yes, making the Email the primary key in the UserCredential class is a valid approach, especially since emails are unique to each user and often used as the primary identifier in authentication systems.");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
             _tokenService = new TokenService(mockConfig.Object);
            _authService = new AuthService(_credentialRepo, _customerRepo, _tokenService);

            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbc@123",
                Name = "Tonny",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
             _authService.Register(registerDTO);


        }
        [Test]
        public async Task Register()
        {
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Abc@123",
                Name = "Sam",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
           var result=await _authService.Register(registerDTO);
            Assert.That(result.CustomerId, Is.EqualTo(2));
            Assert.IsNotNull(result);
            Assert.Pass();


        }
        [Test]
        public async Task RegisterFail()
        {
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbc@123",
                Name = "Sam",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _authService.Register(registerDTO));
            Assert.Pass();

        }
        [Test]
        public async Task MailValidationException()
        {
            // Arrange
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbc123", // Invalid email
                Name = "Sam",
                Address = "ABC 123 kpc",
                Password = "123",
                Phone = "123456 7890",
                Role = "User"
            };

          
            var context = new ValidationContext(registerDTO);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(registerDTO, context, validationResults, true);

            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage.Contains("Invalid email address.")));
        }
        [Test]
        public async Task RegisterPasswordValidation()
        {
           
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbc@123", 
                Name = "Sam",
                Address = "ABC 123 kpc",
                Password = "123", 
                Phone = "123456 7890",
                Role = "User"
            };

          
            var context = new ValidationContext(registerDTO, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(registerDTO, context, validationResults, true);

            
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage.Contains("Password must be between 6 and 100 characters.")));
        }
        [Test]
        public async Task LoginTest()
        {
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbc@133",
                Name = "Tonny",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
            await _authService.Register(registerDTO);
            LoginDTO loginDTO = new LoginDTO()
            {
                Email = "Bbc@133",
                Password = "1234567"
            };
          var result= await _authService.Login(loginDTO);
            Assert.That(result.Code, Is.EqualTo(200));
          Assert.IsNotNull(result);

        }
        [Test]
        public async Task LoginPasswordFail()
        {
            LoginDTO loginDTO = new LoginDTO()
            {
                Email = "Bbc@123",
                Password = "123456"
            };
            var exception = Assert.ThrowsAsync<UnAuthorizedUserException>(async () => await _authService.Login(loginDTO));
            Assert.That(exception.Message, Is.EqualTo("User Name or Password not correct"));
            Assert.Pass();
        }

        [Test]
        public async Task LoginEmailFail()
        {
            LoginDTO loginDTO = new LoginDTO()
            {
                Email = "Bbc@12",
                Password = "123456"
            };
            var exception = Assert.ThrowsAsync<NoUserCredentialFoundException>(async () => await _authService.Login(loginDTO));
            Assert.That(exception.Message, Is.EqualTo("No user found with given id Bbc@12"));
            Assert.Pass();
        }






    }
}
