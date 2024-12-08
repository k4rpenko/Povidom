# Spacegram  
Spacegram — це пет-проєкт соціальної мережі, яка поєднує:  
- **Безпеку, оптимізацію та швидкість** Telegram, забезпечуючи конфіденційність даних, наскрізне шифрування повідомлень, а також миттєвий відгук інтерфейсу.  
- **Зручність публікацій та взаємодій**, як у Twitter та Instagram: користувачі можуть легко створювати пости, редагувати їх, коментувати, робити репости та взаємодіяти зі спільнотою.  

---

## Запуск проекту  

### Frontend: Angular  

1. Перейдіть у директорію `client` (або відповідну директорію з Angular-кодом).  
2. Встановіть залежності:
 
   ```bash
   npm install

### Backend: ASP.NET
1. Додайте файли конфігурації appsettings.json та appsettings.Development.json у кореневу директорію бекенд-проекту.
2. Приклад структури appsettings.json:
 
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
