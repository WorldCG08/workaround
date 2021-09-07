Currently not working... Cant start

Useful commands:

To do that, first we need to publish our application. In the project directory we run :

    dotnet publish -r win-x64 -c Release

    sc create TestService BinPath=C:\full\path\to\publish\dir\WindowsServiceExample.exe

    sc start TestService
    sc stop TestService
    sc delete TestService