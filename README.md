# 🌦️ Weather App
![Version](https://img.shields.io/badge/version-1.0.0-blueviolet)

Aplicativo full-stack que fornece informações climáticas em tempo real, com **FrontEnd em Angular** e **BackEnd em ASP.NET Core**.

---

## 🧠 Visão Geral

Este projeto é dividido em duas partes principais:

1. **Backend**: API REST em .NET, responsável por fornecer os dados de clima.
2. **Frontend**: Aplicação Angular que consome a API e exibe informações ao usuário.

---

## 🏗 Estrutura do Projeto

```text
Weather-App/
├─ BackEnd/
│  ├─ Controllers/       # Endpoints da API
│  ├─ Models/            # Modelos de dados
│  ├─ Services/          # Lógica de negócio
│  ├─ WeatherApi.csproj  # Projeto .NET
│  ├─ Program.cs         # Configuração e inicialização
│  └─ appsettings.json   # Configurações da API
│
├─ FrontEnd/weather-app/
│  ├─ public/            # Arquivos públicos (index.html, icons etc.)
│  ├─ src/               # Código fonte React
│  │  ├─ components/     # Componentes reutilizáveis
│  │  ├─ features/          # Páginas da aplicação
│  │  └─ services/       # Comunicação com a API
│  ├─ package.json       # Dependências e scripts do frontend
│  └─ tsconfig.json      # Configurações TypeScript
└─ README.md             # Este arquivo

------------------------------------------------------------
⚡ Funcionalidades
Buscar clima atual por cidade

Exibir temperatura, umidade e condições meteorológicas

Interface moderna e responsiva

Backend modularizado com controllers, services e models

Exemplo de endpoint: /api/weather?city=São Paulo
------------------------------------------------------------

🔧 Como Rodar o Projeto
Backend (.NET)
bash
Copiar código
cd BackEnd
dotnet restore
dotnet run

------------------------------------------------------------

⚠️ Certifique-se de configurar o appsettings.json com sua chave de API de clima, se necessário.

------------------------------------------------------------

Exemplo de chamada à API com cURL:

Copiar código
curl http://localhost:<PORT>/api/weather?city=São%20Paulo
Exemplo de resposta:

json
Copiar código
{
  "city": "São Paulo",
  "temperature": 25,
  "humidity": 80,
  "condition": "Ensolarado"
}
Frontend (React)
Copiar código
cd FrontEnd/weather-app
ng serve
O app abrirá em http://localhost:4200 e se conectará à API.
------------------------------------------------------------
C#
47.8%
 
TypeScript
28.4%
 
SCSS
13.9%
 
HTML
9.9%
