FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY . /app/out
WORKDIR /app/out
ENTRYPOINT ["dotnet", "DevMikroblog.Api.dll"]