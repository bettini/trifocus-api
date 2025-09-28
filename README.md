# Trifocus API

A .NET API with Azure AD authentication and role-based authorization.

## Prerequisites

- .NET 9.0 SDK or later
- An Azure account with Azure AD access
- Visual Studio Code or Visual Studio 2022

## Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/bettini/trifocus-api.git
   cd trifocus-api
   ```

2. Create your local settings:
   ```bash
   cp trifocus/appsettings.template.json trifocus/appsettings.json
   cp trifocus/appsettings.template.json trifocus/appsettings.Development.json
   ```

3. Configure Azure AD:
   - Go to Azure Portal > Azure Active Directory
   - Register a new application or use an existing one
   - Note down the following values:
     - Application (client) ID
     - Directory (tenant) ID
     - Your domain (e.g., yourdomain.onmicrosoft.com)

4. Set up user secrets (for development):
   ```bash
   cd trifocus
   dotnet user-secrets init
   dotnet user-secrets set "AzureAd:TenantId" "your-tenant-id"
   dotnet user-secrets set "AzureAd:ClientId" "your-client-id"
   dotnet user-secrets set "AzureAd:Domain" "your-domain.onmicrosoft.com"
   ```

5. Update your appsettings.json and appsettings.Development.json:
   - Replace placeholder values with your Azure AD configuration
   - Keep sensitive information in user secrets, not in appsettings files

6. Configure Azure AD Authentication:
   - In Azure Portal > Your App Registration:
     - Add redirect URI: https://localhost:7006/swagger/oauth2-redirect.html
     - Under "Expose an API":
       - Set Application ID URI: api://your-client-id
       - Add scope: api.access
     - Under "Authentication":
       - Enable implicit grant flow for Access tokens
       - Allow public client flows

## Running the API

1. Build and run the project:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

2. Access Swagger UI:
   - Open https://localhost:7006/swagger in your browser
   - Click "Authorize" to authenticate
   - Use the provided endpoints

## Authentication

The API uses Azure AD authentication with the following scopes:
- openid: OpenID Connect authentication
- profile: Access to basic user profile
- api://your-client-id/api.access: Access to the API

## Authorization

The API implements role-based access control:
- Athletes can access weather forecasts
- Admins can access admin-only data

## Security Notes

- Never commit appsettings.json or appsettings.Development.json to source control
- Use user secrets for development
- Use Azure Key Vault or environment variables for production
- Keep your Azure AD credentials secure

## Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## License

[Your chosen license]