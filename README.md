# Serverless Student Portfolio Hub (SSPH)

> A cloud-native portfolio website fully hosted on Azure, showcasing projects and skills using a modern serverless architecture with real-time analytics.

[![Azure Static Web Apps](https://img.shields.io/badge/Azure-Static%20Web%20Apps-blue)](https://azure.microsoft.com/en-us/services/app-service/static/)
[![Azure Functions](https://img.shields.io/badge/Azure-Functions-blue)](https://azure.microsoft.com/en-us/services/functions/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 📋 Project Overview

**CCDE Project**: Cloud Computing & DevOps Engineering Portfolio

This project demonstrates hands-on experience with Azure cloud services, serverless architecture, and DevOps best practices. Built as a comprehensive 20-hour portfolio project, it showcases:

- ✅ **5+ Azure Services** integrated seamlessly
- ✅ **Real-time visitor analytics** powered by serverless backend
- ✅ **Automated CI/CD deployment** via GitHub Actions
- ✅ **Infrastructure-as-Code** for reproducible deployments
- ✅ **Production-ready monitoring** with Application Insights

## 🏗️ Architecture

```
┌────────────┐
│GitHub Repo │
└─────┬──────┘
      │
      ▼
[GitHub Actions]
      │
      ▼
┌─────────────────────┐
│Azure Static Web Apps│
│+ Azure Functions    │
└─────┬─────┬─────────┘
      │     │
      ▼     ▼
[Cosmos DB] [App Insights]
```

### Technology Stack

| Component | Technology | Purpose |
|-----------|-----------|----------|
| **Frontend** | HTML/CSS/JavaScript | Static portfolio site |
| **Hosting** | Azure Static Web Apps | CDN, HTTPS, global distribution |
| **Backend API** | Azure Functions | Serverless API endpoints |
| **Database** | Cosmos DB (Serverless) | Real-time analytics data |
| **CI/CD** | GitHub Actions | Automated deployment pipeline |
| **Monitoring** | Application Insights | Logging, telemetry, error tracking |

## 🚀 Features

- **Dynamic Visitor Counter**: Real-time page view tracking
- **Portfolio Sections**: About, Skills, Projects, Contact
- **Serverless Architecture**: Cost-effective, auto-scaling backend
- **Automated Deployments**: Push to deploy with GitHub Actions
- **Monitoring & Analytics**: Built-in observability with App Insights
- **Secure**: CORS policies, secret management, partition key scaling

## 📊 API Endpoints

### Get Visitor Count

```http
GET /api/visitor-count?page=home
```

**Response:**
```json
{
  "count": 1248,
  "page": "home",
  "timestamp": "2025-11-19T10:00:00Z"
}
```

## 🗄️ Database Schema

**Cosmos DB Document:**
```json
{
  "id": "home-page-counter",
  "pageId": "home",
  "viewCount": 1247,
  "lastUpdated": "2025-11-18T14:30:00Z"
}
```

- **Partition Key**: `/pageId` for horizontal scaling
- **Operations**: Atomic increments per page view

## 🛠️ Setup & Deployment

### Prerequisites

- Azure account with active subscription
- GitHub account
- Node.js 18+ (for local development)
- Azure CLI (optional, for manual deployments)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/Dumdart/CCDEProject-SSPH.git
   cd CCDEProject-SSPH
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure local settings**
   - Copy `local.settings.json.example` to `local.settings.json`
   - Add your Cosmos DB connection string

4. **Run Azure Functions locally**
   ```bash
   npm start
   ```

### Deployment

**Automated via GitHub Actions:**
- Push to `main` branch triggers automatic deployment
- Secrets configured in GitHub repository settings
- Preview environments for pull requests

**Manual deployment:**
```bash
az staticwebapp create \
  --name ssph-portfolio \
  --resource-group rg-ccde-project \
  --location westeurope
```

## 🔒 Security Considerations

- ✅ CORS policies configured for API endpoints
- ✅ Secrets stored in GitHub Actions, never in code
- ✅ Cosmos DB partition keys for secure scaling
- ✅ HTTPS enforced across all endpoints
- ✅ Connection strings and keys managed via Azure Key Vault

## 📈 Monitoring & Observability

**Application Insights provides:**
- Request/response telemetry
- Error tracking and stack traces
- Performance metrics (latency, throughput)
- Custom logging for debugging

**Access dashboard:**
```bash
az monitor app-insights component show \
  --app ssph-insights \
  --resource-group rg-ccde-project
```

## 🧪 Testing

- **Local Development**: Azure Functions emulator + Cosmos DB emulator
- **CI/CD Pipeline**: Automated tests on push
- **Integration Testing**: API functionality verified post-deployment
- **Monitoring Validation**: App Insights telemetry checked

## 📦 Project Structure

```
CCDEProject-SSPH/
├── .github/
│   └── workflows/
│       └── azure-static-web-apps.yml    # CI/CD pipeline
├── api/
│   ├── visitor-count/
│   │   └── index.js                     # Azure Function
│   ├── host.json
│   └── package.json
├── src/
│   ├── index.html                       # Main portfolio page
│   ├── css/
│   │   └── styles.css
│   └── js/
│       └── main.js                      # Frontend logic
├── docs/
│   ├── architecture-diagram.png
│   └── concept.pdf
├── .gitignore
└── README.md
```

## 🎯 Learning Outcomes

✅ **Azure Services**: Practical experience with Static Web Apps, Functions, Cosmos DB, App Insights

✅ **Serverless Patterns**: Event-driven architecture, pay-per-use cost optimization

✅ **DevOps Practices**: CI/CD automation, IaC, monitoring, logging

✅ **Cloud Security**: Secret management, CORS, least privilege access

✅ **Production Deployment**: Real-world application hosting and scaling

## 🚧 Challenges & Solutions

| Challenge | Solution |
|-----------|----------|
| CORS errors | Configured allowed origins in Function app settings |
| Cold start latency | Accepted trade-off for serverless; premium plan as alternative |
| Database costs | Utilized serverless tier + optimized Request Units |
| Secret management | GitHub Secrets for CI/CD, never hardcoded |
| Local development | Azure emulators for offline testing |

## 🔮 Future Enhancements

- [ ] Multi-page analytics tracking (Projects, Skills, Contact)
- [ ] Admin dashboard for analytics visualization
- [ ] Managed Identity for enhanced security
- [ ] Custom domain with Azure DNS
- [ ] Dark mode theme toggle
- [ ] Contact form integration via Logic Apps
- [ ] Blog section with CMS integration

## 📚 Documentation

- **Concept Document**: `/docs/concept.pdf`
- **Architecture Diagram**: `/docs/architecture-diagram.png`
- **Presentation Slides**: `/docs/presentation.pdf`
- **Azure Setup Guide**: `/docs/azure-setup.md`

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details

## 👤 Author

**CCDE Student Project**
- GitHub: [@Dumdart](https://github.com/Dumdart)
- Project Timeline: 20 hours (Nov 2025)

## 🙏 Acknowledgments

- Azure Documentation and tutorials
- CCDE program instructors and mentors
- Open-source community for tooling and best practices

---

**Built with ☁️ on Azure**