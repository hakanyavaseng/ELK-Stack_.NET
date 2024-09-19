# Use the .NET runtime image as the base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ELK.WebAPI/ELK.WebAPI.csproj", "ELK.WebAPI/"]
RUN dotnet restore "ELK.WebAPI/ELK.WebAPI.csproj"
COPY . .
WORKDIR "/src/ELK.WebAPI"
RUN dotnet build "ELK.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ELK.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create a new stage to generate the certificate
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS cert
WORKDIR /certs
RUN dotnet tool install --global dotnet-dev-certs
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https --trust

# Copy the generated certificates to the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=cert /root/.dotnet/https /app/certs
ENTRYPOINT ["dotnet", "ELK.WebAPI.dll"]
