FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY . .
RUN dotnet publish "TimeCalculator/TimeCalculator.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 10000
ENV ASPNETCORE_HTTP_PORTS=10000
ENTRYPOINT ["dotnet", "TimeCalculator.dll"]
