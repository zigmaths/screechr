**Screechr Coding Challenge**

**Instructions for running the application:**
1. Clone the screechr project from the following GitHub repo: https://github.com/zigmaths/screechr
2. From the command line, navigate to ...\repos\screechr\screechr and execute the following command: dotnet run
3. Collection of Postman requests are provided to facilitate testing of the Web API. Please see screechr.postman_collection JSON file in the GitHub repo, and import into Postman (optional).

**IMPORTANT**: Click "Disable SSL Verification" in Postman before sending any requests.

Port number: 7119 (as configured in the launchSettings.json file in the screechr Properties directory).

**Accessing the Web API:**
  - Authentication: https://localhost:7119/api/authentication/authenticate
  - User Profiles: https://localhost:7119/api/userprofiles
  - Screeches: https://localhost:7119/api/screeches

**Authentication:**
  - Authentication endpoint accepts user name and password, and returns token (i.e. Bearer token authentication using JwtBearer middleware)
  - Send Post request to authentication endpoint and get back a token
  - Token must then be provided with each request from the client as a bearer token

**Application loads the following default User Profiles:**
  - User 1: username: iamyourfather, password: Password1
  - User 2: username: chewy, password: Password2
  - User 3: username: merc44, password: Password3
  - User 4: username: redbull33, password: Password4

**API Basics:**
  - No authentication required
    - Get all screeches -> Get
  - Authentication required
    - Get all user profiles -> Get
    - Get a user profile by Id -> Get
    - Create user profile -> Post
    - Partial profile update -> Patch
    - Full profile update -> Put
    - Get screech by Id -> Get
    - Create new screech -> Post
    - Update screech content -> Put
    
**Dependencies:**
  - Automapper.Extensions.Microsoft.DependencyInjection
  - FakeItEasy

**Notes:**
  - I was unable to add support for returning a paged list of screeches due to time constraint.
  - API documentation can be accessed via the following URL: https://localhost:7119/swagger/index.html
  
  ![image](https://user-images.githubusercontent.com/106723612/172982562-afd0a313-a426-424a-9803-30853c915ffc.png)
