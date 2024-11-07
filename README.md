# nowAround (Back-End)

nowAround is a web application designed to bring together people and establishments on an interactive, map-based platform. The app provides users with a comprehensive view of nearby venues, including restaurants, cafes, museums, cinemas, nightclubs, and more. With planned features like event planning and social functionalities, nowAround aims to be a go-to app for people looking to explore and connect with local establishments in real-time.

This repository contains the back-end code for nowAround, developed using **.NET Core 8** and deployed on **Azure**. It utilizes **Mapbox** for interactive mapping and **Auth0** for secure authentication.

## Features

- Interactive map displaying a variety of establishments based on user location.
- Establishment profile pages with customizable information, categories (e.g., Restaurant, Cafe, Cinema) tags (e.g., free Wi-Fi, pet-friendly), and price categories.
- Future features:
  - Establishments can create events, which will appear on the map in real time.
  - Social features for users to plan outings in groups, invite friends to events, and share recommendations.

## Tech Stack

- **.NET Core 8**
- **Mapbox** for map-based visualization of establishments.
- **Auth0** for secure authentication and authorization.
- **MS SQL Server** for the database.
- **Azure** for deployment, with fully configured CI/CD pipelines.

## Getting Started

### Prerequisites

To run the back-end locally, ensure you have:

- .NET Core 8 SDK
- MS SQL Server
- Auth0 account (for authentication)
- Mapbox account (for map integration)

### Setting Up the Project

1. **Clone the Repository**
   ```bash
   git clone https://github.com/SamuelGunda/nowAround-backend.git
   cd nowAround-backend
   
2. **Install Dependencies**

   Ensure that all .NET packages are restored.

   ```bash
   dotnet restore
   
3. **Configure Settings**

   Update `appsettings.json` and `appsettings.Development.json` with the necessary configurations:

   - **Mapbox**: Add your Mapbox Access token.
   - **Auth0**: Add your Auth0 `Audience`, `ClientId`, and other relevant credentials.
   - **SQL Server**: Add your database connection string under the `ConnectionStrings` section.

    Example JSON:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Your-SQL-Server-Connection-String"
    },
    "Auth0": {
      "Audience": "Your-Auth0-Audience",
      "ClientId": "Your-Auth0-ClientId",
      "ClientSecret": "Your-Auth0-ClientSecret",
      "Domain": "Your-Auth0-Domain",
      "ManagementScopes": "Your-Auth0-ManagementScopes",
      "Roles": {
        "Establishment": "Your-Auth0-EstablishmentId",
        "User": "Your-Auth0-UserId",
        "Admin": "Your-Auth0-AdminId"
      },
      "M2MSecretKey": "Random-Secret-Known-Only-By-You"
    },
    "Mapbox": {
      "AccessToken": "Your-Mapbox-AccessToken"
    }
    ```
    
4. **Apply Database Migrations**

   Run the following command to apply migrations and set up the database schema:

   ```bash
   dotnet ef database update

5. **Configure the Auth0**

   In your Auth0 dashboard setup Post-User-Registration and Post-Login Triggers with

   1. **Add Roles to Token**: Add this script into Post-Login.

      Adds accounts roles into token.
      
      Include your Auth0 Api url.
      
      ####Click bellow to expand JS Script 
      <details>
        <summary></b>JS Script</summary>
        
          exports.onExecutePostLogin = async (event, api) => {
            const namespace = 'https://{your-auth0-api-url}';
            if (event.authorization) {
              api.idToken.setCustomClaim(`${namespace}/roles`, event.authorization.roles);
              api.accessToken.setCustomClaim(`${namespace}/roles`, event.authorization.roles);
            }
          }
      
      </details>
     
   2. **Back-end Connection**: Add this script into Post-User-Register.

      After user registers on auth0 send call to back-end in order to save his ID.
      Adds true check to app_metada on success.

      Store the M2MSecretKey, Domain, ClientId and ClientSecret within this functions secrets.
      Add Axios dependency.
  
      Include your deployed back-end url.
  
      Click bellow to expand JS Script
      <details>
        <summary>JS Script</summary>
        
          exports.onExecutePostUserRegistration = async (event, api) => {
            const axios = require('axios');
          
            const token = event.secrets.M2M_SECRET_KEY;
          
            const userId = event.user.user_id;
            if (event.user.app_metadata.role !== "Establishment" && 
            event.user.app_metadata.role !== "Admin")
            {
              const backendUrl = "https://{your-back-end-url}/api/User?auth0Id=" + userId;
          
              let attempts = 0;
              const maxAttempts = 3;
          
              const registerUser = async () => {
                try {
          
                  const response = await axios.post(backendUrl, {}, {
                      headers: {
                          'Auth0-Server-Token': token
                      }
                  });
                  
                  const ManagementClient = require('auth0').ManagementClient;
          
                  var management = new ManagementClient({
                    domain: event.secrets.domain,
                    clientId: event.secrets.clientId,
                    clientSecret: event.secrets.clientSecret,
                  });
          
                  await management.updateAppMetadata({id: event.user.user_id }, { registeredInApi: true});
                } catch (error) {
                  attempts++;
                  if (attempts < maxAttempts) {
                    await registerUser();
                  } else {
                    console.error('Failed to create user after multiple attempts', error);
                  }    
                }
              }
              await registerUser();
            }
          };
      
      </details>
      
   3. **Check Back-end Existance**: Add this script into Post-Login.
      
      Checks if user exist within our database by checking his app_metadata,
      if he does not exist and is not establishment or admin use previous script.

      Store the M2MSecretKey within this function secrets.
      Add Axios dependency.
  
      Include your deployed back-end url.
  
      Click bellow to expand JS Script
      <details>
        <summary>JS Script</summary>
        
          exports.onExecutePostLogin = async (event, api) => {
            const axios = require('axios');
          
            const serverAuthToken = event.secrets.M2M_SECRET_KEY;
          
            const userId = event.user.user_id;
            if (event.user.app_metadata.registeredInApi !== true && 
            event.user.app_metadata.role !== "Establishment" && 
            event.user.app_metadata.role !== "Admin")
            {
              const backendUrl = "https://{your-back-end-url}/api/User?auth0Id=" + userId;
          
              let attempts = 0;
              const maxAttempts = 3;
          
              const registerUser = async () => {
                try {
          
                  const response = await axios.post(backendUrl, {}, {
                      headers: {
                          'Auth0-Server-Token': serverAuthToken
                      }
                  });
          
                  api.user.setAppMetadata("registeredInApi", true)
                } catch (error) {
                  attempts++;
                  if (attempts < maxAttempts) {
                    await registerUser();
                  } else {
                    console.error('Failed to create user after multiple attempts', error);
                  }    
                }
              }
              await registerUser();
            }
          };
      
      </details>

6. **Run the Application**

   Start the application by running:

   ```bash
   dotnet run
   ```

### Deployment

The back-end is deployed on Azure with configured CI/CD pipelines.
You are free to choose deployment of your own, although be aware that this repo is meant for Azure Deployment.

### Documentation

API documentation is generated and available via **Swagger**. Once the application is running, you can view the full API documentation there.

### Usage

1. **Map-based Exploration**: Users can explore a variety of nearby establishments on the map, powered by Mapbox.
2. **Authentication**: Secure login and user management through Auth0.
3. **Establishment Profiles**: Establishments can customize their profiles with various tags and information like family-friendly, pet-friendly, free Wi-Fi, etc.

Future versions will include event planning and social functionalities, enabling establishments to post events and users to plan group activities.
