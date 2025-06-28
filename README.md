# 🧠 Product Catalogue Azure Functions (C#)

This repository contains an Azure Functions project for managing a **product catalogue**, supporting API endpoints to fetch **categories** and **attributes** from a backend data stores:

- **Azure Database for PostgreSQL Flexible Server** for relational data with managed identity authentication

---

## 📦 Project Structure

CategoryMapper.FunctionApp/
│
├── Data/ # EF DbContext
├── Models/ # Models necesary
├── Services/ # Service layer abstraction
├── Functions/ # Azure Function endpoints (HTTP-triggered)
├── Program.cs # Host registration

CategoryMapper.Tests/
├── Unit tests

## 🔧 Function Endpoints

### `GET /api/categories`

Returns a paginated list of categories with optional parent filtering and product inclusion.

**Query Parameters:**

- `parentId`: (GUID) Filter by parent category
- `page`: (int) Pagination (default = 1)
- `size`: (int) Page size (default = 50)
- `includeProducts`: (bool) Include products in response

### `GET /api/attributes`

Returns a paginated list of attributes, filtered by category, link types, keyword, or "not applicable" attributes.

**Query Parameters:**

- `categoryIds`: Comma-separated list of GUIDs
- `linkTypes`: Comma-separated values (`direct`, `inherited`, `global`)
- `keyword`: Search keyword
- `notApplicable`: (bool) Return attributes not linked to the given categories
- `page`, `size`: Pagination controls

I have also included the sql sample and the deployment.bicep that I used to test.
