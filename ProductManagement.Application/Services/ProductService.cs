using ProductManagement.Application.Dtos;
using ProductManagement.Application.Exceptions;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Application.Services
{
    public class ProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                QuantityPerUnit = p.QuantityPerUnit,
                ReorderLevel = p.ReorderLevel,
                SupplierId = p.SupplierId,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                UnitsOnOrder = p.UnitsOnOrder
            });
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
               throw new NotFoundException($"Product with ID {id} not found.");
            }
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                QuantityPerUnit = product.QuantityPerUnit,
                ReorderLevel = product.ReorderLevel,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder
            };
        }

        public async Task AddProductAsync(ProductDto productDto)
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (string.IsNullOrEmpty(productDto.Name))
            {
                validationErrors.Add("Name", new[] { "Product name is required." });
            }

            if (productDto.UnitPrice <= 0)
            {
                validationErrors.Add("UnitPrice", new[] { "Unit price must be greater than 0." });
            }

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(validationErrors);
            }

            var product = new Product
            {
                Name = productDto.Name,
                QuantityPerUnit = productDto.QuantityPerUnit,
                ReorderLevel = productDto.ReorderLevel,
                SupplierId = productDto.SupplierId,
                UnitPrice = productDto.UnitPrice,
                UnitsInStock = productDto.UnitsInStock,
                UnitsOnOrder = productDto.UnitsOnOrder
            };

            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productDto.Id);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {productDto.Id} not found.");
            }
            product.Name = productDto.Name;
            product.QuantityPerUnit = productDto.QuantityPerUnit;
            product.ReorderLevel = productDto.ReorderLevel;
            product.SupplierId = productDto.SupplierId;
            product.UnitPrice = productDto.UnitPrice;
            product.UnitsInStock = productDto.UnitsInStock;
            product.UnitsOnOrder = productDto.UnitsOnOrder;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
