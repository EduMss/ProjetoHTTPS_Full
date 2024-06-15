using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Adiciona a configura��o do appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

//Obtendo as informa��es do json
var configuration = builder.Configuration;
// Obter o caminho e a senha do certificado do appsettings.json
var certPath = configuration["Certificado_Dir"]; // Exemplo de como acessar uma configura��o aninhada
var certPassword = configuration["Certificado_Senha"];
//Verificado se o "certPath" e o "certPassword" n�o s�o nulos
if (certPath != null && certPassword != null)
{
    // Configurando Kestrel para usar HTTPS
    builder.WebHost.ConfigureKestrel(options =>
    {
        var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
            ClientCertificateMode = ClientCertificateMode.AllowCertificate,
            // passando as informa��es obtidas do certificado
            ServerCertificate = new X509Certificate2(certPath, certPassword)
        };

        options.ConfigureEndpointDefaults(listenOptions =>
        {
            listenOptions.UseHttps(httpsConnectionAdapterOptions);
        });
    });
    // Fim da configura��o do HTTPS
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (certPath != null && certPassword != null)
{
    app.UseHttpsRedirection();
}
    
app.UseAuthorization();
app.MapControllers();

app.Run();
