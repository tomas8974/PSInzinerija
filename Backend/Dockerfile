FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
COPY . ./src
WORKDIR /src
RUN dotnet build -o /app
RUN dotnet publish -o /publish

COPY ./Backend/GameRules /publish/GameRules
COPY appsettings*.json /publish
 
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY --from=build  /publish /app
WORKDIR /app
RUN mkdir -p /data
RUN chmod 777 /data
EXPOSE 5000
CMD ["./Backend"]