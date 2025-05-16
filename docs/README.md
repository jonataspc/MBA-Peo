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
├── Peo.GestaoConteudo/          # BC de Gestão de Conteúdo
├── Peo.GestaoAlunos/            # BC de Gestão de Alunos
├── Peo.Faturamento/             # BC de Faturamento
├── Peo.Identity/                # BC de Identidade
└── Peo.Core/                    # Core Compartilhado
```

## Gráfico de dependências
![Gráfico de dependências](./docs/Dependencias.png)

[Link para o gráfico - via MermaidChart](https://www.mermaidchart.com/play?utm_source=mermaid_live_editor&utm_medium=toggle#pako:eNqdlMFugzAMhl8F5bBbeYAdJlWgTdx6mNTDmFAG6RaJxgiCpmrau8-UtKSJU2CciB3_v_0R8sNKqAR7ZIcavssv3uroNc1VhM9OQJFAK4pMHVpepFzzt5xhNB6i8TkaD9GcvUebzdO1wNqFKUdsLz5sFVy65VNBVgmlpT4VKRy5VKbuEo3H6IJyb4CrRHgIqyh6oLohfLZNU8uSawler1bKc0IGcxYvotMcElBa9BXc8rjNzVJxpDw2jhxNiOzHDOHQCzr7tBzrZcyGpXklu3L9t3WvoKMQjpmFAI1MAJ-RWnO8iO4CniFwxvSf2ALOz1z3LT_iuXSOnZWYRWaLeMRsoTXA_M5oQx-X7bia1lLbDE_hZ3sW7oodPzW8JgeedsXjLruNe2YZJJdbBpJQkTUe0f9dpsQA9H_mewTvGPee80uJS5s6piFP77eceOHXxDJpmOEKv720uQ0YFfv9Azh3fd0)

## Principais Funcionalidades
- Gestão de Cursos e Aulas
- Matrícula de Alunos
- Processamento de Pagamentos
- Acompanhamento do Progresso de Aprendizado
- Geração de Certificados
- Autenticação e Autorização de Usuários
