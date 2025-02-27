# Guia Moteis API

Este projeto é uma API desenvolvida em **.NET** com **Entity Framework Core**, **JWT para autenticação** e **SQLite/SQL Server** como banco de dados.

## ✅ Requisitos
Antes de rodar o projeto, certifique-se de ter os seguintes itens instalados:

- [.NET SDK 7.0+](https://dotnet.microsoft.com/en-us/download)
- [SQLite](https://www.sqlite.org/download.html) ou SQL Server (se em produção)
- [Postman](https://www.postman.com/) (opcional, para testar os endpoints)
- [Git](https://git-scm.com/)

## ✅ Configuração do Ambiente

### ■ Clonar o repositório
```bash
git clone https://github.com/rmoraes-oliveira/GuiaMoteisApi.git
cd GuiaMoteisApi
```

### 2️⃣ Configurar variáveis de ambiente
Crie um arquivo **appsettings.Development.json** e configure a conexão com o banco:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=guia_moteis.db"
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SECRETA_AQUI",
    "Issuer": "GuiaMoteisAPI",
    "Audience": "GuiaMoteisClients"
  }
}
```

### 3️⃣ Instalar dependências
```bash
dotnet restore
```

### 4️⃣ Rodar as migrações do banco
```bash
dotnet ef database update
```
Se precisar resetar as migrações:
```bash
dotnet ef migrations remove
```
Depois, gere uma nova:
```bash
dotnet ef migrations add InitialCreate
```
E aplique novamente:
```bash
dotnet ef database update
```

## ▶️ Rodando o Projeto
```bash
dotnet run
```
A API estará disponível em **http://localhost:5000**.

## 🛠 Testando no Swagger
Acesse **http://localhost:5000/swagger** para visualizar e testar os endpoints da API.

Se o Swagger não carregar, certifique-se de que o middleware está habilitado no `Program.cs`:
```csharp
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Guia Moteis API v1");
});
```

## 🔑 Autenticação JWT
### Criar usuário
Faça um **POST** para `/api/auth/register` com um JSON como:
```json
{
  "email": "usuario@email.com",
  "password": "SenhaForte123!"
}
```

### Fazer login
POST para `/api/auth/login`:
```json
{
  "email": "usuario@email.com",
  "password": "SenhaForte123!"
}
```
Ele retornará um token JWT, que deve ser enviado no **Authorization Header** nas próximas requisições:
```
Authorization: Bearer SEU_TOKEN_AQUI
```

## 📊 Endpoint de Faturamento Mensal
Para obter o faturamento mensal, faça um **GET** para:
```http
GET /api/reservations/faturamento-mensal?year=2024&month=2
```
**Resposta:**
```json
{
  "year": 2024,
  "month": 2,
  "revenue": 50000.00
}
```
Esse endpoint utiliza **cache** para otimizar consultas repetidas e só recalcula o valor após 10 minutos.

## 📊 Endpoint de consulta de reservas por período
Para obter o faturamento mensal, faça um **GET** para:
```http
GET /api/reservations?startDate=2024-02-01&endDate=2024-02-10
```
**Resposta:**
```json
{
	"totalRecords": 1000,
	"pageNumber": 1,
	"pageSize": 10,
	"totalPages": 100,
	"data": [
		{
			"id": 1,
			"checkIn": "2025-03-09T14:50:03.284834",
			"checkout": "2025-03-10T14:50:03.284834",
			"total": 299.0,
			"customerId": 46,
			"customer": {
				"id": 46,
				"name": "Davi Lucca Melo",
				"phone": "(02) 76001-5075",
				"email": "Caio.Batista23@yahoo.com"
			}
        }
    ]
}
```

## 🏗 Estrutura do Projeto
```
GuiaMoteisApi/
│── Controllers/        # Lógica dos endpoints
│── Models/            # Definição dos modelos de dados
│── Services/          # Serviços auxiliares
│── Data/              # Configuração do banco e seed de dados
│── Program.cs         # Configuração principal da aplicação
│── appsettings.json   # Configuração da aplicação
│── README.md          # Instruções do projeto
```

## 📌 Melhorias Futuras
- 📌 Implementar cache para otimizar consultas.
- 📌 Melhorar logs e monitoramento com OpenTelemetry.
- 📌 Criar testes automatizados para os endpoints.

## 📄 Licença
Este projeto está licenciado sob a **MIT License**.

