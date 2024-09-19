using ELK.WebAPI.Entities;
using ELK.WebAPI.Services;
using ELK.WebAPI.Services.Interfaces;
using ELK.WebAPI.Settings;
using ELK.WebAPI.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Elasticsearch test endpoints

app.MapGet("/products", async (IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var data = await elasticsearchService.GetDocumentsAsync<Product>(ElasticsearchIndexes.Products, cancellationToken);
    return Results.Ok(data);
});

app.MapGet("/products/{id}",
    async (string id, IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
    {
        var data = await elasticsearchService.GetDocumentAsync<Product>(id, ElasticsearchIndexes.Products,
            cancellationToken);

        return Results.Ok(data);
    });

app.MapGet("/products-details",
    async (IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
    {
        var datas = await elasticsearchService.SearchAsync<Product>(product =>
            product.Index(ElasticsearchIndexes.Products)
                .From(0)
                .Size(10), cancellationToken);

        return Results.Ok(datas);
    });

app.MapGet("/products-match/{query_keyword}", async (string query_keyword, IElasticsearchService elasticsearchService,
    CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.MatchQueryAsync<Product>(p => p.Name, query_keyword,
        ElasticsearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-fuzzy/{query_keyword}", async (string query_keyword, IElasticsearchService elasticsearchService,
    CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.FuzzyQueryAsync<Product>(p => p.Name, query_keyword,
        ElasticsearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-wildcard/{query_keyword}", async (string query_keyword,
    IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.WildcardQueryAsync<Product>(p => p.Name, query_keyword,
        ElasticsearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-exists",
    async (IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
    {
        var datas = await elasticsearchService.ExistsQueryAsync<Product>(p => p.Name, ElasticsearchIndexes.Products,
            cancellationToken);

        return Results.Ok(datas);
    });

app.MapGet("/products-bool", async (IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.BoolQueryAsync<Product>(p => p.Name, "Mouse", p => p.Name, "erasr",
        p => p.Name, "*se*", ElasticsearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-term/{query_keyword}", async (string query_keyword, IElasticsearchService elasticsearchService,
    CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.TermQueryAsync<Product>(p => p.Name, query_keyword,
        ElasticsearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-count", async (IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var count = await elasticsearchService.CountDocumentsAsync<Product>(ElasticsearchIndexes.Products,
        cancellationToken);

    return Results.Ok(count);
});

app.MapPost("/products",
    async (CreateProductVm createProductVM, IElasticsearchService elasticsearchService,
        CancellationToken cancellationToken) =>
    {
        Product product = new()
        {
            Name = createProductVM.Name,
            Price = createProductVM.Price,
            Quantity = createProductVM.Quantity
        };
        bool result =
            await elasticsearchService.CreateDocumentAsync(product, ElasticsearchIndexes.Products, cancellationToken);

        return Results.Ok(result);
    });

app.MapPut("/products", async (UpdateProductVm updateProductVM, IElasticsearchService elasticsearchService,
    CancellationToken cancellationToken) =>
{
    bool result = await elasticsearchService.UpdateDocumentAsync<Product>(updateProductVM.Id, updateProductVM,
        ElasticsearchIndexes.Products, cancellationToken);

    return Results.Ok(result);
});

app.MapDelete("/products/{id}",
    async (string id, IElasticsearchService elasticsearchService, CancellationToken cancellationToken) =>
    {
        bool result =
            await elasticsearchService.DeleteDocumentAsync<Product>(id, ElasticsearchIndexes.Products,
                cancellationToken);

        return Results.Ok(result);
    });

#endregion

app.UseHttpsRedirection();

app.Run();