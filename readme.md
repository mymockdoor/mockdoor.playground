Prerequisites
=============
In order to run Mockdoor and these projects you need the following installed.

- dotnet 6.0 SDK
- A dotnet IDE/Editor such as Visual Studio, VS Code or Jet brains Rider
- For easy environment running/setup you will need
  - Microsoft tye installed, see [https://github.com/dotnet/tye](https://github.com/dotnet/tye)

Installation instructions
=========================

# Using Microsoft Tye

First clone the repository and its Mockdoor sub module, to do this:

Open command/terminal and use the following commands

```git clone --recurse-submodules https://github.com/mymockdoor/mockdoor.playground.git```

Then open the newly cloned folder

``` cd mockdoor.playground```

Ensure you have Microsoft Tye installed.

to install run this command (for full instructions see [https://github.com/dotnet/tye](https://github.com/dotnet/tye))

```  dotnet tool install -g Microsoft.Tye --version "0.11.0-alpha.22111.1" ```

To start all services using default configuration (not linking up mockdoor)

``` tye run tye.yaml```

to run with mockdoor pre configured

``` tye run tye-mockdoor-config.yaml ```

open your browser to [http://localhost:8000](http://localhost:8000) to see a list of the running microservices.

- You can open Mockdoor at [https://localhost:44304/dev/mockdoor/](https://localhost:44304/dev/mockdoor/) or using the dashboard
- You can open the demo store at [https://localhost:5006/](https://localhost:5006/) or using the dashboard
- you can open the live dashboard demo at [https://localhost:7094/](https://localhost:7094/) or using the dashboard

- Once the UI is open you can login in the top right, the logins are listed under the login area. For an admin it is admin:alice and for a customer it is customer:alice
- Note: this are known bugs in the store UI when navigating to pages before login in. This is to be fixed in the future

# Using the demo solution file

- Open the FullDemoSolution.sln file in your IDE of choice
- Set multiple start up projects with the following startup projects
  
  - mockdoor/MockDoor/Server/MockDoor.Server.csproj : this is Mockdoor
  - BFFUISample/src/Server/Blazor6.Server.csproj : this is the store UI
  - BFFUISample/src/StoreApi/StoreApi.csproj : this is the stores API
  - BFFUISample/src/StockApi/StockApi.csproj : this is the stock API
  - BFFUISample/src/OrderProcessor/OrderProcessorApi/OrderProcessorApi.csproj : this is the order processing api
  - IdentityServerSample/src/IdentityServer/IdentityServer.csproj : this is the main Identity Server (Authentication handler)
  - IdentityServerSample/src/UserStore/UserStore.csproj : this is the user store API
  - IdentityServerSample/src/UserPermissions/UserPermissions/UserPermissions.csproj : this is the permissions API
  - DataDash/DataDash/DataDash.csproj : this is the UI for the live dashboard
  - DataDash/TestWebApi/TestWebApi.csproj : this is the API for the live dashboard
- Start all projects

- You can open Mockdoor at [https://localhost:44304/dev/mockdoor/](https://localhost:44304/dev/mockdoor/) 
- You can open the demo store at [https://localhost:5006/](https://localhost:5006/) 
- you can open the live dashboard demo at [https://localhost:7094/](https://localhost:7094/) 

- Once the UI is open you can login in the top right, the logins are listed under the login area. For an admin it is admin:alice and for a customer it is customer:alice
- Note: this are known bugs in the store UI when navigating to pages before login in. This is to be fixed in the future

# The Playground Architecture
![Architecture Diagram](./Architecture%20diagram.jpg)
