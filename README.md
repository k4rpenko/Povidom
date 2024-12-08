# Spacegram  
Spacegram is a social network pet project that combines:  
- **Security, optimization, and speed** of Telegram, providing data privacy, end-to-end message encryption, and instant feedback from the interface.  
- Ease of publishing and interacting, similar to Twitter and Instagram: users can easily create posts, edit them, comment, repost, and interact with the community.  

---

## Launching the project  

### Frontend: Angular  

1. Change to the `client` directory (or the corresponding directory with Angular code).  
2. Install the dependencies:
 
   ```bash
   npm install

### Backend: ASP.NET
1. Add the configuration files appsettings.json and appsettings.Development.json to the root directory of the backend project.
2. Example of the structure of appsettings.json:
 
   ```bash
   {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "Npgsql": {
        "ConnectionString": ""
      },
      "Redis": {
        "ConnectionString": ""
      },
      "Mailhog": {
        "Host": "",
        "Port": ,
        "SenderName": "",
        "SenderEmail": "",
        "SenderPassword": ""
      },
      "GoogleAuth": {
        "ClientID": "",
        "ClientSecret": ""
      },
      "MongoDB": {
        "ConnectionString": "",
        "MongoDbDatabase": "",
        "MongoDbDatabaseChat": ""
      }
   }
