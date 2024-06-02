using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest.RepositoryTest
{
    
    public class VendorRepositoryTest:BaseSetup
    {
        [Test]
        public async Task FailAdd()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _vendorRepo.Add(null));
            Assert.That(exception.Message, Is.EqualTo("Vendor cannot be null (Parameter 'item')"));



        }
        [Test]
        public async Task DuplicateFailAdd()
        {
            Vendor category = new Vendor()
            {
                VendorId = 1,
                VendorName = "sample",
                Address="ass",
                Phone="2132132213"
            };
           
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _vendorRepo.Add(category));

        }
        [Test]
        public async Task Delete()
        {
            Vendor vendor = new Vendor()
            {
                VendorId = 2,
                VendorName = "sample",
                Address="abc",
                Phone="1121232132"
            };
            await _vendorRepo.Add(vendor);
            var reult = await _vendorRepo.Delete(2);
            Assert.That(reult.VendorId, Is.EqualTo(2));



        }
        [Test]
        public async Task FailDelete()
        {
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _vendorRepo.Delete(2));

        }
        [Test]
        public async Task UpdateVendor()
        {

            Vendor vendor = new Vendor()
            {
                VendorId= 1,
                VendorName="dample",
                Address="abc",
                Phone="123213213"
            };
            var result = await _vendorRepo.Update(vendor);
            Assert.That(result.VendorId, Is.EqualTo(1));

        }
        [Test]
        public async Task UpdateCategoryFail()
        {
            Vendor vendor = new Vendor()
            {
                VendorId = 3,
                VendorName = "sample",
                Address = "abc",
                Phone="312121"
            };

            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _vendorRepo.Update(vendor));




        }


    }

}
