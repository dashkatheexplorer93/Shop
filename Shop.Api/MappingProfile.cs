using AutoMapper;
using Shop.Contract.Requests;
using Shop.Contract.Responses;
using Shop.Data.Entities;

namespace Shop.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<CreateCategoryDto, Category>();

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.CategoryId));
        CreateMap<CreateProductDto, Product>();

        // Customer mappings
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.CustomerId))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // OrderItem mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        CreateMap<CreateOrderItemDto, OrderItem>();
    }
}
