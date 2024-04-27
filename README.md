# 🏥 Healthcare API

### ❕API and Desktop Client for a Healthcare System data manager implemented in .NET 8.0


## ➡️ Local Setup 
- Clone this repository to your machine
- In MS Visual Studio:
    - Navigate to the solution's folder and open the project
    - Compile and run the application
- Via CLI environment:
    - Navigate to the solution's folder
    - Execute the command `dotnet run`


## ➡️ API
- The API backend uses DTOs to transfer data from the user's request to the database
- The password received via DTO is hashed with salt using the `Argon2id` algorithm before being stored in the database
- If the user tries to access an endpoint using an invalid or expired JWT token, a `401 Unauthorized` will be returned
- The API endpoint can be consumed by connecting to the concatenation of the base URL with the specific API path shown in the following sections for each endpoint

The following accordions contain the model schemas and the API endpoints:

<style>
    summary { font-size: 22px; }
    summary::-webkit-details-marker { font-size: 22px; }
</style>

<details>
<summary>Schemas</summary>

`Login DTO`

    {
        username*	string
        password*	string
    }
    
`Staff Member DTO`

    {
        id	integer($int32)
        username*	string
        password*	string
        status	string
        name*	string
        email*	string
        phone	string
        address	string
        gender	string
        specialization	string
        age	integer($int32)
    }

`Patient DTO`

    {
        id	integer($int32)
        name*	string
        email*	string
        phone	string
        address	string
        gender	string
        age	integer($int32)
        birthDate*	string($date-time)
        bloodGroup	string
        allergies	string
        additionalInfo	string
    }

`Appointment`

    {
        id	integer($int32)
        patientId	integer($int32)
        patientName	string
        doctorId	integer($int32)
        doctorName	string
        title	string
        bookingDate	string($date-time)
        appointmentDate	string($date-time)
        isCompleted	boolean
        details	string
        results	string
    }

</details>


<details>
<summary>Auth Endpoints</summary>

  - `POST` ➔ `/auth/login/` ─ Log in using the username and password in the request body, return the JWT token as plain string on success.
    - Request Body Schema: `Login DTO`
    - Success Response: `200 OK`

  - `GET` ➔ `/auth/verify/` ─ Verify whether the Bearer token passed via header is valid and not expired.
    - Success Response: `200 OK`
    - Failure Response: `401 UNAUTHORIZED`

</details>

<details>
<summary>Appointments, Doctors, Staff Members and Patients Endpoints</summary>

The Appointments, Doctors, Staff Members and Patients share endpoints with similar features. To access each specific endpoint, simply connect to the URL path of that specific entity using the desired method. The URL paths for each entity are as follows:

    Appointments - /appointments/
    Doctors - /doctors/
    Staff Member - /staff/
    Patients - /patients/


- `GET` ➔ `/{entity}/` ─ Get all objects of the entity's type from the database as a list.
    - Success Response: `200 OK`
    - No Objects Found Response: `404 NOT FOUND`

- `GET` ➔ `/{entity}/{id}` ─ Get a specific object of the entity's type from the database using its id. 
    - Response Body Schema: `Entity Schema`
    - Success Response: `200 OK`
    - ID Not Found response: `404 NOT FOUND`

- `POST` ➔ `/{entity}/` ─ Create a new object of the entity's type and add it to the database.
    - Request Body Schema: `Entity DTO`
    - Sample Success Response: `201 CREATED`

- `PATCH` ➔ `/{entity}/{id}` ─ Search for a specific object of the entity's type using its id and update it in the database.
    - Request Body Schema: `Entity DTO`
    - Sample Success Response: `204 NO CONTENT`
    - ID Not Found response: `404 NOT FOUND`

- `DELETE` ➔ `/{entity}/{id}` ─ Search for a specific object of the entity's type using its id and delete it from the database.
    - Sample Success Response: `204 NO CONTENT`
    - ID Not Found response: `404 NOT FOUND`


</details>



## ➡️ Desktop Client
- The desktop app connects to the REST API and authenticates the user using JWT
- The app displays options to consume the API using the specific endpoint related to the selected option

<img src="https://raw.githubusercontent.com/xbandrade/healthcare-api/main/Images/new_doctor.png">

## ⚙️ Unit tests
Unit tests for the API were created and stored under the `API/Tests` folder. Tests were also performed using `Insomnia` with a mock database.