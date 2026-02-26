var builder = WebApplication.CreateBuilder(args);

// Registra Controllers (para a API responder a rotas do tipo /api/...)
builder.Services.AddControllers();

// Swagger é opcional, mas ajuda muito a testar a API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: libera o front (Vue) acessar a API
// Sem isso, o navegador bloqueia as requisições (segurança do browser).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue",
        policy => policy
            // libera o endereço do Vite (Vue rodando)
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

// Em modo desenvolvimento, habilita o Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usa a política CORS que criamos
app.UseCors("AllowVue");

// Mapeia os controllers (sem isso as rotas não funcionam)
app.MapControllers();

app.Run();
