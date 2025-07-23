# Developer Evaluation Project

## Descrição do Projeto
Este projeto consiste na implementação de uma API completa de vendas baseada na arquitetura DDD, com controle sobre produtos, usuários, carrinhos, vendas, e outros recursos. A API oferece funcionalidades CRUD para todos os recursos, além de validar regras de negócio específicas e possibilitar o disparo de eventos (simulados) de criação, modificação e cancelamento de vendas e itens.

## Tecnologias Utilizadas
- **Backend:** .NET (C#)
- **Frontend:** Angular (não detalhado aqui, mas suportado pelo projeto)
- **Banco de Dados:** [especifique o banco, ex. SQL Server, MongoDB, etc.]
- **Hospedagem/Deployment:** [por exemplo, IIS, Docker, etc.]
- **Ferramentas:** Git, GitHub, etc.

## Como Executar o Projeto
1. **Clone o repositório:**
   ```bash
   git clone https://github.com/devluz2023/NETStore.git


2. Configurar o ambiente:
Restarure as dependências do backend com o seu IDE (.NET)
Configure a conexão com o banco de dados (segundo seu setup)

3. Executar o projeto:
Inicie o backend na porta 80
Configure o frontend Angular (se aplicável), ajustando as URLs conforme necessário

4. Acessar a API:
API hospedada em http://localhost:80/api/
Endpoints Principais
Produtos
Método	Endpoint	Descrição
POST	/api/products	Criar novo produto
PUT	/api/products/{id}	Atualizar produto existente
GET	/api/products	Listar produtos (com paginação e ordenação)
GET	/api/products/{id}	Obter produto por ID
GET	/api/products/categories	Listar categorias
GET	/api/products/category/{category}	Listar produtos por categoria
DELETE	/api/products/{id}	Remover produto
Usuários
Método	Endpoint	Descrição
POST	/api/users	Criar usuário
PUT	/api/users/{id}	Atualizar usuário
GET	/api/users	Listar usuários
GET	/api/users/{id}	Obter usuário por ID
DELETE	/api/users/{id}	Remover usuário
Vendas
Método	Endpoint	Descrição
POST	/api/sales	Registrar nova venda
PUT	/api/sales/{id}	Atualizar venda
GET	/api/sales	Listar vendas (com paginação)
GET	/api/sales/{id}	Detalhar venda
PUT	/api/sales/{id}/cancel	Cancelar venda
PUT	/api/sales/{id}/items/{itemId}/cancel	Cancelar item da venda
DELETE	/api/sales/{id}	Remover venda
Carrinhos
Método	Endpoint	Descrição
GET	/api/carts	Listar carrinhos
POST	/api/carts	Criar carrinho
GET	/api/carts/{id}	Detalhar carrinho
PUT	/api/carts/{id}	Atualizar carrinho
DELETE	/api/carts/{id}	Remover carrinho


## Organização do Repositório
O repositório possui a seguinte estrutura:

/Backstore - código fonte do backend
/web-store- aplicação Angular

README.md - este arquivo