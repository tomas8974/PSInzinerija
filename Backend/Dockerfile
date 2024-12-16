FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
COPY . ./src
WORKDIR /src
RUN dotnet build -o /app
RUN dotnet publish -o /publish

COPY ./Backend/GameRules /publish/GameRules
COPY ./Backend/WordListSources /publish/WordListSources
COPY appsettings*.json /publish
 
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY --from=build  /publish /app
WORKDIR /app
RUN mkdir -p /data
RUN chmod 777 /data
ENV ConnectionStrings__DefaultConnection="Data Source=/data/SQLLiteDatabase.db"
EXPOSE 5000
CMD ["./Backend"]