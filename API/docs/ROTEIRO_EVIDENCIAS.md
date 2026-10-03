# Roteiro de evidências para o PDF

Use o nome exigido no Moodle: `seunome_sistemas_em_nuvem_com_azure_pd.pdf`.

## Prints obrigatórios sugeridos

1. Repositório GitHub mostrando README, código, `infra/main.bicep` e workflows.
2. GitHub Actions verde na etapa de build e testes.
3. Grupo de recursos no portal com VM, Storage, Azure SQL, Key Vault, Log Analytics, Monitor/Application Insights.
4. Deployments do grupo de recursos mostrando o Bicep concluído.
5. Storage > Static website com o endpoint e o frontend Angular aberto, exibindo o formulário e a tabela na mesma tela.
6. Swagger com os cinco endpoints: GET lista, GET por ID, POST, PUT e DELETE.
7. Execução do POST com resposta 201 e do GET com o produto salvo.
8. Azure SQL > Query editor exibindo `SELECT * FROM Products`.
9. VM > Identity mostrando a identidade gerenciada ativa; Key Vault > Access policies mostrando somente Get/List.
10. Log Analytics com uma consulta retornando logs da API, por exemplo `Syslog | where ProcessName contains "docker" | take 20`.
11. Azure Monitor mostrando métricas da VM e uma regra de alerta.
12. Defender for Cloud mostrando recomendações e ao menos uma correção aplicada ou justificativa do que ficou pendente.
13. Azure Policy mostrando atribuições de localização permitida, TLS e tags, se criadas no portal.
14. Cost Management > Budget e análise de custos, comprovando cuidado com o plano gratuito.
15. Diagrama `docs/architecture/diagrama-arquitetura.svg` e a tabela de decisões do documento de arquitetura.

## Estrutura recomendada do PDF

1. Capa e objetivo.
2. Arquitetura e fluxo da solução.
3. Código e CRUD.
4. Provisionamento e CI/CD.
5. Segurança, LGPD e governança.
6. Monitoramento e custos.
7. Testes e evidências.
8. Análise Well-Architected e plano de evolução.
9. Conclusão e links do GitHub/frontend.

Não afirme que um recurso está configurado sem anexar o print correspondente. O código prepara a solução, mas os prints do portal só podem ser obtidos depois de você provisionar na sua assinatura.
