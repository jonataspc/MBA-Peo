# Feedback - Avaliação Geral

## Organização do Projeto
- **Pontos positivos:**
  - Projeto muito bem estruturado em múltiplos contextos (`Billing`, `ContentManagement`, `StudentManagement`).
  - Separação clara entre Application, Domain, Infra e Integrations para cada contexto.
  - Solução `.sln` presente na raiz, documentação adicional no diretório `docs`, e README funcional.
  - Arquivos de configuração e scripts de teste bem organizados.

- **Pontos negativos:**
  - **Uso integral do inglês** em todos os domínios, entidades, variáveis e documentação interna. Isso **contraria o requisito do desafio**, que exige o projeto em português para alinhamento com o domínio. **(resposta: 1)**
  - Centralização da resolução de dependências em um único ponto compartilhado (provavelmente no projeto `Core`), fazendo com que todos os contextos compartilhem infraestrutura de injeção de dependência. Isso **compromete a independência dos módulos** e **viola o princípio de isolamento entre bounded contexts**. **(resposta: 2)**

## Modelagem de Domínio
- **Pontos positivos:**
  - Entidades como `Payment`, `Course`, `Lesson`, `Student`, `Enrollment` estão bem modeladas, com encapsulamento e uso de Value Objects.
  - Aplicação de boas práticas de modelagem com regras de negócio nas entidades e domínio orientado a comportamento.

- **Pontos negativos:**
  - Apesar da boa modelagem, o domínio de cada contexto está acoplado à infraestrutura dos outros, perdendo a autonomia. **(resposta: 2)**

## Casos de Uso e Regras de Negócio
- **Pontos positivos:**
  - Todos os fluxos funcionais do escopo estão implementados.
  - Casos de uso segregados em comandos, handlers e endpoints por aplicação de CQRS.

- **Pontos negativos:**
  - O fluxo de orquestração entre os domínios é acoplado: o contexto de aluno consome diretamente o serviço do contexto de pagamento. **(resposta: 3)**
  - Faltam mecanismos para validar regras de forma assíncrona e independente entre contextos. **(resposta: 3)**

## Integração entre Contextos
- **Pontos negativos:**
  - **Não há comunicação por eventos** entre contextos. **(resposta: 3)**
  - Os contextos se comunicam via injeção direta de serviços e repositórios entre eles, caracterizando **acoplamento técnico forte**. **(resposta: 3)**
  - A centralização da injeção de dependência reforça o acoplamento e **compromete totalmente a modularidade esperada** em uma arquitetura DDD distribuída. **(resposta: 2)**

## Estratégias Técnicas Suportando DDD
- **Pontos positivos:**
  - Uso de CQRS e separação clara entre camadas.
  - Aplicação de abstrações, serviços e handlers com boas práticas.

- **Pontos negativos:**
  - A arquitetura falha ao não isolar os contextos na prática. **(resposta: 3)**
  - Eventos de domínio não foram utilizados. **(resposta: 3)**
  - O acoplamento via dependência compartilhada é um erro estrutural grave neste tipo de projeto. **(resposta: 3)**

## Autenticação e Identidade
- **Pontos positivos:**
  - Autenticação JWT implementada.
  - Separação clara de perfis (Aluno/Admin).

- **Pontos negativos:**
  - O vínculo entre identidade e domínio não está documentado. **(resposta: 4)**

## Execução e Testes
- **Pontos positivos:**
  - Projeto configurado com SQLite e scripts para seed e testes.
  - Cobertura razoável com script para teste e medição de cobertura.

- **Pontos negativos:**
  - Cobertura não é uniforme entre todos os contextos. **(resposta: 5)**
  - Não há testes cobrindo falhas ou integrações entre domínios. **(resposta: 6)**

## Documentação
- **Pontos positivos:**
  - Boa documentação técnica nos arquivos `docs`, incluindo arquitetura, testes e definição dos contextos.

- **Pontos negativos:**
  - Toda documentação está em inglês, o que **desalinha com o contexto em português exigido pelo projeto**. **(resposta: 1)**

## Conclusão

Este projeto demonstra **excelente capacidade técnica e implementação funcional completa** dos requisitos. Entretanto, **três falhas críticas** comprometem seriamente sua avaliação como projeto DDD:

1. **Uso do idioma inglês** em todo o código e documentação, **em desacordo com o requisito oficial**. **(resposta: 1)**
2. **Acoplamento direto entre contextos**: domínios que deveriam ser independentes trocam chamadas diretamente, sem eventos ou isolamento real. **(resposta: 3)**
3. **Centralização da injeção de dependência**, onde todos os contextos compartilham infraestrutura de resolução, **descaracterizando a independência dos módulos**. **(resposta: 2)**

Para conformidade total com os princípios ensinados, é necessário:
- Isolar a injeção de dependência em cada contexto. **(resposta: 2)**
- Refatorar os fluxos de comunicação para uso de eventos. **(resposta: 3)**


--- 

# Respostas:
## 1. 
Ajustes realizados. 

## 2. 
A orquestração das dependências era feita no projeto Peo.IoC. Lá eram configuradas as dependências dos contextos, repositórios, serviços e handlers.
 
A camada de apresentação (Peo.Web.Api) referenciava o projeto Peo.IoC, e este por sua vez possuia referência aos demais projetos dos BCs.

Ou seja, os BCs são independentes e não referenciam um ao outro, nem compartilham setup de infra.

Porém, assim como conversamos no Discord, fiz a remoção da camada de IoC e criei as classes de resolução de dependencia diretamente nos projetos de cada BC.

## 3.
Refatorado para implementar uso de eventos e remover acoplamento entre BCs (exemplo: `PagamentoService.ProcessarPagamentoMatriculaAsync()`) 

## 4.
A documentação consta em https://github.com/jonataspc/MBA-Peo/blob/master/docs/bounded-contexts.md#contexto-de-identidade

## 5.
Todos os BCs possuem cobertura de teste acima de 80%:
```
Peo.GestaoConteudo.Domain: 87%
Peo.GestaoConteudo.Application: 84%
Peo.GestaoConteudo.Infra.Data: 96%

Peo.Faturamento.Domain: 85%
Peo.Faturamento.Application: 91%
Peo.Faturamento.Infra.Data: 97%
Peo.Faturamento.Integrations.Paypal: 100%

Peo.GestaoAlunos.Domain: 85%
Peo.GestaoAlunos.Application: 95%
Peo.GestaoAlunos.Infra.Data: 100%
```

(pode ser conferido no relatório gerado via Github Actions e disponivel conforme instruções: https://github.com/jonataspc/MBA-Peo/tree/master?tab=readme-ov-file#code-coverage-e-ci)

## 6.
Existem, ex: `ProcessarPagamentoMatriculaAsync_DeveLidarComFalhaPagamento` e `ProcessarPagamentoMatriculaAsync_DeveProcessarPagamentoComSucesso`, eles validam matrícula do aluno e pagamento (BC Faturamento).