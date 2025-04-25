# Plataforma de Educação Online - Documentação

## Visão Geral
Esta documentação fornece um guia abrangente para a Plataforma de Educação Online, uma solução baseada em DDD para gerenciamento de conteúdo educacional.

## Índice
1. [Visão Geral da Arquitetura](./architecture.md)
2. [Bounded contexts (BCs)](./bounded-contexts.md)
4. [Estratégia de Testes](./testing.md)

## Stack Tecnológica
- .NET Core 9.0
- ASP.NET Core WebAPI
- Entity Framework Core
- SQL Server / SQLite
- Autenticação JWT
- Padrão CQRS
- Arquitetura DDD

## Estrutura do Projeto
```
src/
├── Peo.Web.Api/                 # Camada de API
├── Peo.ContentManagement/       # BC de Gestão de Conteúdo
├── Peo.StudentManagement/       # BC de Gestão de Alunos
├── Peo.Billing/                 # BC de Faturamento
├── Peo.Identity/                # BC de Identidade
└── Peo.Core/                    # Core Compartilhado
```

## Principais Funcionalidades
- Gestão de Cursos e Aulas
- Matrícula de Alunos
- Processamento de Pagamentos
- Acompanhamento do Progresso de Aprendizado
- Geração de Certificados
- Autenticação e Autorização de Usuários
