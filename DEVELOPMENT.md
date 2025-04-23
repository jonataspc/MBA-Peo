# Notas de apoio para desenvolvedores
TODO:

Adicionar tratamento de erro global.
Replace PCF, MyBlog, etc


## EF Core: Geração das migrations 
```
Add-Migration [NAME] -verbose -Context [NAMEOFDBCONTEXT] -Project Peo.Identity.Infra.Data -Startup Peo.Web.Api
```