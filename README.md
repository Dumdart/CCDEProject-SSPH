# Serverless Student Portfolio Hub (SSPH)

> A cloud-native portfolio application fully hosted on Azure, demonstrating modern serverless architecture with real-time analytics, automated CI/CD, and enterprise authentication.

[![Azure Static Web Apps](https://img.shields.io/badge/Azure-Static%20Web%20Apps-blue)](https://azure.microsoft.com/en-us/services/app-service/static/)
[![Azure Functions](https://img.shields.io/badge/Azure-Functions-blue)](https://azure.microsoft.com/en-us/services/functions/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Live Application:** [https://thankful-ocean-0bb855e03.4.azurestaticapps.net/](https://thankful-ocean-0bb855e03.4.azurestaticapps.net/)

## 📋 Project Overview

**CCDE Project**: Cloud Computing & DevOps Engineering Portfolio

This project demonstrates comprehensive hands-on experience with Azure cloud services, serverless architecture, and DevOps best practices. Developed over 29 hours as a portfolio project, it showcases:

- ✅ **5+ Azure Services** integrated seamlessly (Static Web Apps, Functions, Cosmos DB, Application Insights, Entra ID)
- ✅ **Real-time visitor analytics** powered by serverless backend with Entity Framework Core
- ✅ **Automated CI/CD deployment** via GitHub Actions with separate pipelines for API and frontend
- ✅ **Enterprise authentication** using Microsoft Entra ID with domain-restricted access
- ✅ **Production-ready monitoring** with Application Insights and comprehensive logging
- ✅ **Secure architecture** with proper secret management and CORS configuration

## 🏗️ Architecture

```
┌────────────┐
│GitHub Repo │
└─────┬──────┘
      │
      ▼
[GitHub Actions CI/CD]
      │
      ├─────────────────┬────────────────┐
      ▼                 ▼                ▼
┌──────────┐  ┌─────────────┐  ┌──────────────┐
│Static Web│  │Azure        │  │Cosmos DB     │
│Apps      │──│Functions    │──│(EF Core)     │
│(VueJS)   │  │(.NET 8)     │  │              │
└──────────┘  └─────────────┘  └──────────────┘
      │              │                 │
      └──────────────┴─────────────────┘
                     │
                     ▼
          ┌──────────────────────┐
          │Application Insights  │
          │+ Entra ID Auth       │
          └──────────────────────┘
```

### Technology Stack

| Component | Technology | Purpose |
|-----------|-----------|----------|
| **Frontend** | VueJS | Modern reactive portfolio interface |
| **Hosting** | Azure Static Web Apps | Global CDN, HTTPS, automatic preview environments |
| **Backend API** | Azure Functions (.NET 8) | Serverless API with HTTP triggers |
| **Database** | Cosmos DB (NoSQL) | Real-time analytics storage with EF Core integration |
| **ORM** | Entity Framework Core | Database abstraction and migrations |
| **Authentication** | Microsoft Entra ID | Domain-restricted secure access |
| **CI/CD** | GitHub Actions | Separate pipelines for API and frontend deployment |
| **Monitoring** | Application Insights | Telemetry, logging, and error tracking |
| **Secret Management** | GitHub Secrets + Azure App Settings | Secure credential storage |

## 🚀 Features

### Core Functionality
- **Dynamic Visitor Counter**: Real-time page view tracking with atomic database operations
- **VueJS Frontend**: Modern, reactive single-page application
- **Portfolio Sections**: About, Skills, Projects, Contact with analytics integration
- **Entity Framework Integration**: Type-safe database operations with automatic migrations

### Cloud & DevOps
- **Serverless Architecture**: Cost-effective, auto-scaling backend
- **Dual CI/CD Pipelines**: Separate workflows for API and frontend with automated deployment
- **Azure Credentials Authentication**: Service principal-based deployment for Flex Consumption plan
- **Environment Variables**: Secure secret injection via GitHub Actions
- **CORS Configuration**: Proper cross-origin resource sharing for secure API access

### Security & Access
- **Microsoft Entra ID Integration**: Domain-restricted authentication (@htl-neufelden.at)
- **Function Key Authorization**: Protected API endpoints
- **Firewall Configuration**: Cosmos DB access restricted to Azure services
- **Secret Management**: No hardcoded credentials, all secrets in GitHub/Azure

### Observability
- **Application Insights**: Comprehensive logging and telemetry
- **Verbose Logging**: Detailed database operation tracking
- **Error Tracking**: Full stack traces for debugging
- **Performance Metrics**: Request latency and throughput monitoring

## 📊 API Endpoints

### Get Visitor Count

```http
GET /api/visitor-count?pageId=home
Authorization: Function Key (x-functions-key)
```

**Response:**
```json
{
  "ViewCount": 42,
  "PageId": "home",
  "LastUpdated": "2026-01-18T14:30:00Z"
}
```

## 🗄️ Database Schema

**Cosmos DB Document (EF Core Model):**
```csharp
public class CounterDoc
{
    [JsonProperty("id")]
    public string Id { get; set; } = "home-page-counter";
    
    public string PageId { get; set; } = "home";
    public int ViewCount { get; set; } = 0;
    public DateTime LastUpdated { get; set; }
}
```

**Database Context Configuration:**
```csharp
modelBuilder.Entity<CounterDoc>()
    .ToContainer("PageViews")
    .HasPartitionKey(x => x.PageId)
    .HasNoDiscriminator();
```

- **Container**: `PageViews`
- **Partition Key**: `/PageId` for horizontal scaling
- **Operations**: Atomic increments with EF Core tracking

## 🛠️ Setup & Deployment

### Prerequisites

- Azure account with active subscription (Student tier compatible)
- GitHub account with Actions enabled
- Node.js 18+ (for VueJS development)
- .NET 8 SDK (for Azure Functions)
- Azure CLI (optional, for manual operations)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/Dumdart/CCDEProject-SSPH.git
   cd CCDEProject-SSPH
   ```

2. **Configure API settings**
   Create `api/local.settings.json`:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
       "CosmosDbConnectionString": "YOUR_CONNECTION_STRING",
       "DatabaseId": "PortfolioDB",
       "ContainerId": "PageViews"
     }
   }
   ```

3. **Install dependencies**
   ```bash
   # API
   cd api
   dotnet restore
   
   # Frontend
   cd ../client
   npm install
   ```

4. **Run locally**
   ```bash
   # API
   cd api
   dotnet run
   
   # Frontend
   cd client
   npm run dev
   ```

### Deployment

#### Automated Deployment (Recommended)

The project uses two GitHub Actions workflows:

1. **API Deployment** (`.github/workflows/api-cd.yml`):
   - Triggers on push to `main` branch
   - Builds .NET project
   - Deploys to Azure Functions using Azure credentials
   - Injects secrets from GitHub Secrets

2. **Frontend Deployment** (`.github/workflows/static-web-app-cd.yml`):
   - Triggers on push to `main` branch
   - Builds VueJS application
   - Deploys to Azure Static Web Apps
   - Creates preview environments for PRs

**Required GitHub Secrets:**
- `AZURE_CREDENTIALS`: Service principal JSON for API deployment
- `AZURE_STATIC_WEB_APPS_API_TOKEN`: Static Web Apps deployment token
- `COSMOSDB_CONNECTION_STRING`: Cosmos DB connection string
- `DATABASE_ID`: Cosmos DB database name
- `CONTAINER_ID`: Cosmos DB container name
- `API_FUNCTION_KEY`: Azure Functions authorization key

#### Manual Deployment

```bash
# Deploy Azure Functions
az functionapp deployment source config-zip \
  --resource-group rg-ccde-project \
  --name ccdeproject-api \
  --src api.zip

# Deploy Static Web App
az staticwebapp create \
  --name ssph-portfolio \
  --resource-group rg-ccde-project \
  --location westeurope
```

## 🔒 Security Implementation

### Authentication
- ✅ Microsoft Entra ID integration for frontend
- ✅ Domain-restricted access (@htl-neufelden.at)
- ✅ Function key authorization for API endpoints
- ✅ Role-based access control (configured roles: "leser")

### Network Security
- ✅ CORS policies configured for specific origins
- ✅ Cosmos DB firewall allowing only Azure datacenters
- ✅ HTTPS enforced across all endpoints
- ✅ Static Web Apps managed certificates

### Secret Management
- ✅ GitHub Secrets for CI/CD credentials
- ✅ Azure Application Settings for runtime secrets
- ✅ Environment variable injection in pipelines
- ✅ No hardcoded credentials in source code
- ✅ Connection string rotation capability

## 📈 Monitoring & Observability

**Application Insights Integration:**
- Request/response telemetry with full context
- Error tracking with stack traces and custom properties
- Database operation logging (queries, updates, execution time)
- Performance metrics (latency, RU consumption)
- Custom logging at Info, Warning, and Error levels

**Sample Log Output:**
```
[Information] PageView called with URL: .../api/visitor-count?pageId=home
[Information] Querying for id=home-page-counter, pageId=home
[Information] Found doc: exists
[Information] Updated existing doc
[Information] Executed ReplaceItem (96.35 ms, 1.24 RU)
```

**Access Monitoring:**
```bash
az monitor app-insights component show \
  --app ssph-insights \
  --resource-group rg-ccde-project
```

## 🧪 Testing Approach

- **Local Development**: Azure Functions Core Tools + Cosmos DB emulator
- **CI Pipeline**: Automated build verification on every push
- **Integration Testing**: Post-deployment API functionality validation
- **Monitoring Validation**: Application Insights telemetry verification
- **Manual Testing**: End-to-end testing of authentication and analytics flow

## 📦 Project Structure

```
CCDEProject-SSPH/
├── .github/
│   └── workflows/
│       ├── api-cd.yml                    # Azure Functions CI/CD
│       ├── static-web-app-cd.yml         # Static Web Apps CI/CD
│       └── ci.yml                        # Build verification
├── api/
│   ├── PageView.cs                       # Azure Function HTTP trigger
│   ├── DatabaseContext.cs                # EF Core DbContext
│   ├── CounterDoc.cs                     # Database model
│   ├── Program.cs                        # Dependency injection setup
│   ├── host.json                         # Function runtime config
│   ├── local.settings.json.example       # Local development template
│   └── PortfolioApi.csproj               # .NET project file
├── client/
│   ├── src/
│   │   ├── App.vue                       # Main Vue component
│   │   ├── components/                   # Vue components
│   │   └── main.js                       # Entry point
│   ├── public/                           # Static assets
│   ├── package.json                      # NPM dependencies
│   └── vite.config.js                    # Build configuration
├── docs/
│   └── Portfolio-App-Mitschrift.docx     # Detailed project log
├── .gitignore
└── README.md
```

## 🎯 Learning Outcomes

### Azure Services Mastery
✅ **Azure Static Web Apps**: CDN configuration, custom domains, preview environments

✅ **Azure Functions**: Flex Consumption plan, HTTP triggers, dependency injection

✅ **Cosmos DB**: NoSQL design, partition keys, firewall rules, RU optimization

✅ **Application Insights**: Telemetry, custom logging, performance monitoring

✅ **Microsoft Entra ID**: App registration, authentication flows, role management

### Cloud Development Patterns
✅ **Serverless Architecture**: Event-driven design, stateless functions, pay-per-use optimization

✅ **Entity Framework Core**: Cosmos DB provider, DbContext configuration, data modeling

✅ **RESTful APIs**: HTTP triggers, JSON serialization, error handling

✅ **CORS Configuration**: Cross-origin policies, secure API access

### DevOps Practices
✅ **CI/CD Automation**: GitHub Actions workflows, multi-stage pipelines

✅ **Secret Management**: GitHub Secrets, Azure Key Vault, environment variables

✅ **Infrastructure as Code**: Azure resource configuration via code

✅ **Deployment Strategies**: Service principal authentication, automated rollouts

### Production Engineering
✅ **Observability**: Structured logging, distributed tracing, error tracking

✅ **Security**: Authentication, authorization, firewall configuration, secret rotation

✅ **Debugging**: Live troubleshooting with Application Insights, log analysis

## 🚧 Challenges Overcome

| Challenge | Root Cause | Solution | Time Impact |
|-----------|-----------|----------|-------------|
| API deployment failure | Flex Consumption plan incompatible with publish profiles | Switched to Azure credential-based deployment with service principal | 2+ hours |
| Database connection errors | Cosmos DB firewall blocking Azure Functions outbound IPs | Enabled "Accept connections from Azure datacenters" | 1 hour |
| Entity tracking conflicts | EF Core tracking multiple instances of same entity | Removed explicit `Update()` call, relied on change tracking | 1.5 hours |
| Partition key mismatch | Incorrect partition key property in DbContext | Configured `HasPartitionKey(x => x.PageId)` properly | 30 min |
| JSON serialization issues | Pascal case API vs camel case frontend | Configured `JsonSerializerOptions` with camel case | 30 min |
| CORS errors | Static Web App origin not in allowed list | Added frontend URL to Function App CORS settings | 20 min |

## 🔮 Future Enhancements

### Analytics Expansion
- [ ] Multi-page analytics tracking (Projects, Skills, Contact)
- [ ] Admin dashboard for analytics visualization with charts
- [ ] Geographic visitor distribution mapping
- [ ] Session tracking and user journey analysis

### Feature Additions
- [ ] Contact form integration via Logic Apps
- [ ] Blog section with content management
- [ ] Project showcase with dynamic loading
- [ ] Skills proficiency visualization
- [ ] Dark mode theme toggle

### Infrastructure Improvements
- [ ] Custom domain with Azure DNS
- [ ] Managed Identity for passwordless authentication
- [ ] Azure Key Vault integration for enhanced secret management
- [ ] Azure Front Door for enhanced CDN and WAF
- [ ] Automated backup strategy for Cosmos DB

### Development Enhancements
- [ ] Unit tests for Azure Functions
- [ ] Integration tests for API endpoints
- [ ] E2E tests for frontend workflows
- [ ] Performance benchmarking suite
- [ ] Load testing with Azure Load Testing

## 📚 Documentation & Resources

- **Project Log**: `/docs/Portfolio-App-Mitschrift.docx` - Detailed 29-hour development journal
- **Architecture Diagram**: Available in documentation
- **Live Application**: [https://thankful-ocean-0bb855e03.4.azurestaticapps.net/](https://thankful-ocean-0bb855e03.4.azurestaticapps.net/)

### Key Technical References
- [Azure Static Web Apps Documentation](https://docs.microsoft.com/azure/static-web-apps/)
- [Azure Functions .NET Isolated Worker](https://docs.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide)
- [Entity Framework Core Cosmos DB Provider](https://docs.microsoft.com/ef/core/providers/cosmos/)
- [GitHub Actions for Azure](https://docs.microsoft.com/azure/developer/github/github-actions)
- [Microsoft Entra ID Authentication](https://docs.microsoft.com/azure/active-directory/develop/)

## ⏱️ Project Timeline

| Date | Duration | Activity |
|------|----------|----------|
| 25.11.2025 | 50 min | Project concept & task management |
| 2.12.2025 | 75 min | Azure services setup |
| 9.12.2025 | 70 min | Initial API programming |
| 16.12.2025 | 110 min | Function development & testing |
| 13.1.2026 | 145 min | CI/CD pipeline implementation |
| 14.1.2026 | 297 min | Static Web App CD & Function deployment |
| 17.1.2026 | 380 min | CORS, API integration, logging |
| 18.1.2026 | 280 min | Database connection, EF Core, authentication |
| **Total** | **~29 hours** | **Full project completion** |

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details

## 👤 Author

**Paul Thumfart** - CCDE Student Project
- GitHub: [@Dumdart](https://github.com/Dumdart)
- Repository: [CCDEProject-SSPH](https://github.com/Dumdart/CCDEProject-SSPH)
- Institution: HTL Neufelden
- Project Duration: November 2025 - January 2026

## 🙏 Acknowledgments

- Azure for Students program for cloud resources
- CCDE program instructors for guidance
- Microsoft documentation and community resources
- Entity Framework Core team for Cosmos DB provider
- GitHub Actions community for workflow examples

## 🎓 Educational Value

This project serves as a comprehensive demonstration of:
- **Cloud-native application architecture** with real-world complexity
- **Modern DevOps practices** including CI/CD automation and secret management
- **Production-grade engineering** with monitoring, security, and scalability considerations
- **Problem-solving skills** through documented troubleshooting and iterative refinement

**Access Request**: For testing access with Entra ID authentication, contact the project author.

---

**Built with ☁️ on Azure | Secured with 🔐 Entra ID | Deployed with 🚀 GitHub Actions**