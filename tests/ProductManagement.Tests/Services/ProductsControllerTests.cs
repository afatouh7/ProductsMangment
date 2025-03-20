using Moq;
using ProductManagement.Api.Controllers;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ProductManagement.Application.Dtos;
using AutoMapper;
using ProductManagement.Application.Services;

namespace ProductManagement.Api.Tests.Services
{
    public class ProductsControllerTests
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private readonly Mock<IMapper> _mockMapper;

        private readonly Mock<ProductService> _mockProductService;

        public ProductsControllerTests()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _mockMapper = new Mock<IMapper>();
            _mockProductService = new Mock<ProductService>();
            _controller = new ProductsController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            
            var mockProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", QuantityPerUnit = "box", ReorderLevel = 10, SupplierId = 1, UnitPrice = 100, UnitsInStock = 50, UnitsOnOrder = 5 },
                new Product { Id = 2, Name = "Product 2", QuantityPerUnit = "can", ReorderLevel = 5, SupplierId = 2, UnitPrice = 200, UnitsInStock = 20, UnitsOnOrder = 10 }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(2, products.Count);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var mockProduct = new Product { Id = 1, Name = "Product 1", QuantityPerUnit = "box", ReorderLevel = 10, SupplierId = 1, UnitPrice = 100, UnitsInStock = 50, UnitsOnOrder = 5 };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockProduct);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, product.Id);
        }

        [Fact]
        
        public async Task CreateProduct_ReturnsCreatedAtAction_WithProduct()
        {
            // Arrange
            var newProductDto = new ProductDto
            {
                Id = 0,
                Name = "New Product",
                QuantityPerUnit = "box",
                ReorderLevel = 10,
                SupplierId = 1,
                UnitPrice = 100,
                UnitsInStock = 50,
                UnitsOnOrder = 5
            };

            var newProduct = new Product
            {
                Id = 0,
                Name = "New Product",
                QuantityPerUnit = "box",
                ReorderLevel = 10,
                SupplierId = 1,
                UnitPrice = 100,
                UnitsInStock = 50,
                UnitsOnOrder = 5
            };

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _mockMapper.Setup(mapper => mapper.Map<Product>(newProductDto)).Returns(newProduct);

            // Act
            var result = await _controller.AddProduct(newProductDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var productDto = Assert.IsType<ProductDto>(createdAtActionResult.Value);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenProductExists()
        {
            // Arrange
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Product 1",
                QuantityPerUnit = "box",
                ReorderLevel = 10,
                SupplierId = 1,
                UnitPrice = 100,
                UnitsInStock = 50,
                UnitsOnOrder = 5
            };

            var productDto = new ProductDto
            {
                Id = 1,
                Name = "Product 1",
                QuantityPerUnit = "box",
                ReorderLevel = 10,
                SupplierId = 1,
                UnitPrice = 100,
                UnitsInStock = 50,
                UnitsOnOrder = 5
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
            _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProduct(1, productDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductExists()
        {
            
            var existingProduct = new Product { Id = 1, Name = "Product 1", QuantityPerUnit = "box", ReorderLevel = 10, SupplierId = 1, UnitPrice = 100, UnitsInStock = 50, UnitsOnOrder = 5 };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
            _mockRepo.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.DeleteProduct(1);
            Assert.IsType<NoContentResult>(result);
        }
    }
}

