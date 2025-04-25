# Testes

Este documento descreve a estratégia de testes da plataforma, incluindo tipos de testes, cobertura e ferramentas.

## Tipos de Testes

### 1. Testes Unitários
- **Objetivo**: Testar unidades individuais de código
- **Escopo**: Classes, métodos e funções
- **Ferramentas**: xUnit, Moq
- **Cobertura Mínima**: 80%

### 2. Testes de Integração
- **Objetivo**: Testar a integração entre componentes
- **Escopo**: Casos de uso completos
- **Ferramentas**: xUnit, TestServer
- **Cobertura**: Todos os casos de uso críticos

## Cobertura de Código
- Cobertura mínima de 80%

## Ferramentas e Tecnologias
- xUnit
- Moq
- FluentAssertions
- TestServer
- dotCover 
- GitHub Actions (CI)