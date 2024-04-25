# 🏥 Healthcare API

## ❕API and Desktop Client for a Healthcare System data manager implemented in .NET 8.0


### ➡️ Local Setup 
- Clone this repository to your machine
- In MS Visual Studio:
    - Navigate to the solution's folder and open the project
    - Compile and run the application
- Via CLI environment:
    - Navigate to the solution's folder
    - Execute the command `dotnet run`


## ➡️ API
- The API backend uses DTOs to transfer data from the user's request to the database
- The password received via DTO is hashed with salt using the Argon2id algorithm before being stored in the database
- If the user tries to access an endpoint using an invalid or expired JWT token, a `401 Unauthorized` will be returned


## ➡️ Desktop Client
- The desktop app connects to the REST API and authenticates the user using JWT
- The app displays options to consume the API using the specific endpoint related to the selected option

<img src="https://raw.githubusercontent.com/xbandrade/healthcare-api/main/Images/new_doctor.png">


## ⚙️ Unit tests
Unit tests for the API were created and stored under the `API\Tests` folder. Tests were also performed using `Insomnia` with a mock database.