FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# FIX: Added .csproj to the copy command
COPY ["somibot.csproj", "./"]
RUN dotnet restore "./somibot.csproj"

# Copy the rest of the source code and build it
COPY . .
RUN dotnet build "somibot.csproj" -c Release -o /app/build

# 2. Publish the compiled binaries
FROM build AS publish
RUN dotnet publish "somibot.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Build the final runtime image using .NET 10 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "somibot.dll"]