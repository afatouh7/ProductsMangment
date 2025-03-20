using ProductManagement.Application.Dtos;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using ProductManagement.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application.Services
{
    public class SupplierService
    {
        private readonly SupplierRepository _repository;

        public SupplierService(ISupplierRepository repository)
        {
            _repository = (SupplierRepository?)repository;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<Supplier?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task AddAsync(SuplierDto supplier)
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (string.IsNullOrEmpty(supplier.Name))
            {
                validationErrors.Add("Name", new[] { "suplier name is required." });
            }

            var suplier = new Supplier
            {
                Name = supplier.Name
            };
            await _repository.AddAsync(suplier);
        }
       

        public async Task UpdateAsync(SuplierDto supplier)
        {
            var supp = await _repository.GetByIdAsync(supplier.Id);
            if (supp == null) throw new Exception("Product not found");
            supp.Name= supplier.Name;
            await _repository.UpdateAsync(supp);
        } 

        
        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}
