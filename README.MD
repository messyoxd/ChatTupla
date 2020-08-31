# ChatTupla
This program uses tuple space to make a chat
# Used in this project
[Myra](https://github.com/rds1983/Myra) for GUI API

[dotSpace](https://github.com/pSpaces/dotSpace) for tuple Space

[dotSpace-objectSpace](https://github.com/tmt96/dotSpace-objectSpace) for a dotnet core implementation of dotSpace

[MonoGame](https://github.com/MonoGame/MonoGame) for GUI
#
## SOs Testados

Ubuntu 20
#
## Instalação para o Ubuntu 20
#
1. Dotnet 3.1
```
wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get update

sudo apt-get install apt-transport-https

sudo apt-get update

sudo apt-get install dotnet-sdk-3.1
```
#
2. Dotnet restore na raiz do projeto
```
dotnet restore
```

## Executar projeto
#
1. Iniciar o servidor
```
dotnet run -p ChatTupla.TupleSpaceServer/ChatTupla.TupleSpaceServer.csproj [ip: Default -> 127.0.0.1] [port: Default -> 8989]
```
2. Iniciar a GUI
```
dotnet run -p ChatTupla.GUI/ChatTupla.GUI.csproj
```