using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using PharmacyManagementApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest
{
    public class FeedBackServiceTest:BaseSetup
    {

        [Test]
        public async Task AddFeedback()
        {
            FeedbackRequestDTO feedback = new FeedbackRequestDTO()
            {
                MedicineId = 2,
                CustomerId = 1,
                Feedback = "Good",
                Rating = 4

            };
           var result=  await _feedbackService.AddFeedback(feedback);
            Assert.That(result.Code, Is.EqualTo(200));

        }
        [Test]
        public async Task AddFeedbackDuplicateFail()
        {
            FeedbackRequestDTO feedback = new FeedbackRequestDTO()
            {
                MedicineId = 1,
                CustomerId = 1,
                Feedback = "Good",
                Rating = 4

            };
            var exception = Assert.ThrowsAsync<DuplicateValueException>(async () => await _feedbackService.AddFeedback(feedback));
           
            Assert.Pass();

        }
        [Test]
        public async Task ViewMyFeedback()
        {

           var result=await _feedbackService.ViewMyFeedBack(1);
           Assert.That(result.Count, Is.EqualTo(1));
            Assert.Pass();

        }
        [Test]
        public async Task ViewMyFeedbackNocustomerFail()
        {

            var exception = Assert.ThrowsAsync<NoCustomerFoundException>(async () => await _feedbackService.ViewMyFeedBack(5));
         
            Assert.Pass();
            Assert.Pass();

        }
        [Test]
        public async Task ViewMyFeedbackNoFeedbackFail()
        {
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbb@123",
                Name = "Tonny",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
            await _authService.Register(registerDTO);

            var exception = Assert.ThrowsAsync<NoFeedbackFoundException>(async () => await _feedbackService.ViewMyFeedBack(2));

            Assert.Pass();
            Assert.Pass();

        }
        [Test]
        public async Task MedicineFeedback()
        {
            MedicineFeedbackDTO result =await _feedbackService.MedicineFeedback(1);

            Assert.That(result.FeedbackRating, Is.EqualTo(4));

            Assert.Pass();
            Assert.Pass();

        }
        [Test]
        public async Task ViewFeedbackFail()
        {

            var exception = Assert.ThrowsAsync<NoFeedbackFoundException>(async () => await _feedbackService.MedicineFeedback(2));
            Assert.Pass();
            Assert.Pass();

        }

    }
}
