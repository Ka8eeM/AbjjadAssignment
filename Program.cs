using AbjjadAssignment;
using AbjjadAssignment.endpoints;
using AbjjadAssignment.middlewares;
using AbjjadAssignment.services;

var builder = WebApplication.CreateBuilder(args);


{
    builder.Services.AddPresentation().AddServices();
    builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
}

var app = builder.Build();

app.UseGlobalExceptionMiddleware();

app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseImageValidation();
app.UseSwagger();
app.UseSwaggerUI();
app.MapEndPoints();
app.Run();
