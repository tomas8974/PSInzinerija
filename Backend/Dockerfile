FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
COPY . ./src
WORKDIR /src
RUN dotnet build -o /app
RUN dotnet publish -o /publish
RUN ls /publish
COPY appsettings*.json /publish
 
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY --from=build  /publish /app
WORKDIR /app
EXPOSE 5000
CMD ["./Backend"]