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

- You can open Mockdoor at [http://localhost:44304/dev/mockdoor/](http://localhost:44304/dev/mockdoor/) or using the dashboard
- You can open the demo store at [https://localhost:5006/(https://localhost:5006/) or using the dashboard
- you can open the live dashboard demo at [https://localhost:7094/](https://localhost:7094/) or using the dashboard

# Using the demo solution file
