# Frontend Angular — TechStore Cloud

Aplicação de página única para administrar o catálogo de produtos consumindo a API REST do projeto.

## Funcionalidades

- Cadastro e validação de produtos.
- Listagem em tabela e busca por nome ou SKU.
- Edição e exclusão com confirmação.
- Mensagens de sucesso e erro retornadas pela API.
- Layout responsivo para computador e celular.

## Arquitetura

```text
src/app/
├── core/
│   ├── models/          # Contratos de dados
│   └── services/        # Comunicação HTTP com a API
├── pages/
│   └── products/        # Tela e regras de apresentação
├── shared/
│   └── components/      # Componentes reutilizáveis
├── app.config.ts
└── app.routes.ts
```

Os endereços da API ficam em `src/environments`. O ambiente local usa `http://localhost:5093/api/v1`. Antes do deploy manual, substitua o valor de produção; no GitHub Actions isso é feito pela variável `API_URL`.

## Executar

```powershell
npm install
npm start
```

A aplicação abre em `http://localhost:4200` e requer a API em execução.

## Gerar versão de produção

```powershell
npm run build
```

Os arquivos estáticos são criados em `dist/techstore-cloud-frontend/browser`, diretório publicado no contêiner `$web` do Azure Storage pelo pipeline.
