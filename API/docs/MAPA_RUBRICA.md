# Mapa da rubrica

| Item avaliado | Evidência no projeto |
|---|---|
| Comparar VM, App Service, Functions, AKS e Container Apps | `docs/ARQUITETURA.md`, seção Comparação de computação |
| Blob, File, Table, Queue, tiers e redundância | `docs/ARQUITETURA.md`, seção Armazenamento |
| Edições/decisão Azure SQL | Azure SQL serverless em `infra/main.bicep` e tabela de decisões |
| Modelagem, partição, RUs e consistência Cosmos DB | Comparação SQL versus Cosmos em `docs/ARQUITETURA.md` |
| Estratégia de migração/6 R's | Plano de evolução e justificativa de plataforma no documento |
| Azure Monitor, Log Analytics e Application Insights | Recursos no Bicep, agente/DCR e roteiro de prints |
| Key Vault e identidades gerenciadas | Vault, secret e SystemAssigned Identity no Bicep |
| Azure Policy e Landing Zone/Blueprints | Seção Segurança e roteiro de evidências; atribuições devem ser feitas no portal |
| Defender for Cloud | Roteiro de evidências e plano de remediação |
| CI/CD | `../../.github/workflows/ci.yml` e `deploy.yml`; o pipeline compila API e Angular e publica VM + Blob Static Website |
| IaC | `infra/main.bicep` e parâmetros sem segredos |
| Azure Well-Architected | Tabela por cinco pilares no documento de arquitetura |
| Plano de evolução | Quatro fases priorizadas no documento de arquitetura |

> Atenção: a rubrica menciona pipeline para App Service/Functions, mas o enunciado da Parte 2 exige API em VM. Este projeto atende ao componente obrigatório (VM) e documenta App Service/Functions como alternativas e evolução. Explique essa decisão explicitamente na apresentação.
