# Automação — Busca CEP e Rastreamento (Correios)

Projeto de testes de interface com **SpecFlow**, **NUnit** e **Selenium WebDriver** (Chrome).

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado (`dotnet --version` deve mostrar 8.x)
- **Google Chrome** instalado (versão compatível com o pacote `Selenium.WebDriver.ChromeDriver` do projeto)

## Como executar os testes

Na pasta do projeto (`Correios.Cep.Automation`):

```bash
dotnet restore
dotnet test
```

Ou, a partir da raiz do repositório:

```bash
dotnet test Correios.Cep.Automation/Correios.Cep.Automation.csproj
```

### Executar só o cenário da feature

O cenário está em `Features/BuscaCep.feature`. Para filtrar pelo nome do teste gerado:

```bash
dotnet test --filter "FullyQualifiedName~ValidarCEPInexistenteEPesquisarCEPValido"
```

### Visual Studio / Rider

Abra a solução ou a pasta do projeto, abra o **Test Explorer** e execute o teste desejado.

## Comportamento importante (captcha)

O fluxo inclui **pausas de 10 segundos** após preencher o CEP e após preencher o código de rastreamento, para você resolver o **captcha manualmente** antes do clique em **Buscar** / **Consultar**. Mantenha a janela do Chrome visível durante a execução.

## Problemas comuns

| Sintoma | O que fazer |
|--------|-------------|
| `ERR_CONNECTION_RESET` ou falha ao abrir o site | Verificar internet, VPN, firewall ou proxy; tentar de novo mais tarde. |
| ChromeDriver incompatível | Atualizar o Chrome ou ajustar a versão do pacote `Selenium.WebDriver.ChromeDriver` no `.csproj`. |

## Estrutura principal

| Pasta / arquivo | Função |
|-----------------|--------|
| `Features/BuscaCep.feature` | Cenário em linguagem Gherkin (português) |
| `Steps/BuscaCepSteps.cs` | Liga os passos da feature ao código |
| `Pages/CorreiosBuscaCepPage.cs` | Selenium: URLs, preenchimento e asserções |
| `Support/WebDriverHooks.cs` | Abre e fecha o Chrome por cenário |

O arquivo `Features/BuscaCep.feature.cs` é **gerado automaticamente** pelo SpecFlow ao compilar; edite apenas o `.feature` ou os `Steps`/`Pages`.
