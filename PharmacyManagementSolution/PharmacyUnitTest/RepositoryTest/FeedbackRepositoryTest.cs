using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest.RepositoryTest
{
    public class FeedbackRepositoryTest:BaseSetup
    {
        [Test]
        public async Task FailAdd()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _feedbackRepo.Add(null));
            Assert.That(exception.Message, Is.EqualTo("Feedback cannot be null (Parameter 'item')"));


        }
        [Test]
        public async Task DuplicateFailAdd()
        {
            Feedback feedback = new Feedback()
            {
                FeedbackId=2,
                MedicineId=1,
                CustomerId=1,
                FeedbackMessage="good",
                Rating=3,
                
            };
            await _feedbackRepo.Add(feedback);
            Feedback feedback1 = new Feedback()
            {
                FeedbackId = 2,
                MedicineId = 1,
                CustomerId = 1,
                FeedbackMessage = "good",
                Rating = 4
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _feedbackRepo.Add(feedback1));

        }
        [Test]
        public async Task Delete()
        {
            
            
            var reult = await _feedbackRepo.Delete(1);
            Assert.That(reult.FeedbackId, Is.EqualTo(1));



        }
        [Test]
        public async Task FailDelete()
        {
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _categoryRepo.Delete(2));

        }
        [Test]
        public async Task UpdateFeedback()
        {
            Feedback feedback = new Feedback()
            {
                FeedbackId = 2,
                MedicineId = 1,
                CustomerId = 1,
                FeedbackMessage = "good",
                Rating = 3,

            };
            await _feedbackRepo.Add(feedback);
            Feedback feedback1 = new Feedback()
            {
                FeedbackId = 2,
                MedicineId = 1,
                CustomerId = 1,
                FeedbackMessage = "good",
                Rating = 4
            }; 
            var result = await _feedbackRepo.Update(feedback1);
            Assert.That(result.FeedbackId, Is.EqualTo(2));

        }
        [Test]
        public async Task UpdateCategoryFail()
        {
            Feedback feedback1 = new Feedback()
            {
                FeedbackId = 2,
                MedicineId = 1,
                CustomerId = 1,
                FeedbackMessage = "good",
                Rating = 4
            };

            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _feedbackRepo.Update(feedback1));

        }

    }
}
