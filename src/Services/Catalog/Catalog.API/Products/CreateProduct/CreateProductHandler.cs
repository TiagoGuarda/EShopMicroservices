using BuildingBlocks.CQRS;
using Catalog.API.Models;

namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    List<string> Categories,
    string Description,
    string ImageFile,
    decimal Price
) : ICommand<CreateProductResult>;

public record CreateProductResult(
    Guid Id
);

internal class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Business logic to create a product

        //create Product entity from command object
        //save to database
        //return CreateProductResult result

        var product = new Product
        {
            Name = command.Name,
            Categories = command.Categories,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        //save to database

        return new CreateProductResult(Guid.NewGuid()); // Simulating a new product ID
    }
}