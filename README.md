# 🌦️ Previsão do Tempo – Weather API

API desenvolvida em **.NET** para consulta de dados meteorológicos em tempo real, consumindo uma **API externa de clima** e expondo os dados de forma padronizada para uso em aplicações frontend.

> 🚀 Projeto estruturado com boas práticas, DTOs, Services, Controllers e testes de integração.

---

## 📌 Visão Geral

Este projeto faz parte da aplicação **Previsão do Tempo**, que será composta por:

- 🔹 **Backend**: API REST em .NET
- 🔹 **Frontend**: (em desenvolvimento)
- 🔹 **Testes de Integração** para validação da API

---

## 🧱 Estrutura do Projeto

```text
PrevisaoTempo
│
├── backend
│   └── WeatherApi
│       ├── Controllers
│       ├── DTOs
│       ├── Services
│       ├── Properties
│       ├── appsettings.json
│       ├── Program.cs
│       └── WeatherApi.csproj
│
├── WeatherApi.Tests
│   ├── Controllers
│   ├── Integration
│   └── WeatherApi.Tests.csproj
│
├── .gitignore
├── PrevisaoTempo.sln
└── README.md
