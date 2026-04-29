# syntax=docker/dockerfile:1.7

#
# Stage 1 — build the Vue admin frontend.
#
FROM node:22-alpine AS frontend
WORKDIR /src

COPY frontend/package.json frontend/package-lock.json* ./
RUN npm ci

COPY frontend/ ./
RUN npm run build

#
# Stage 2 — restore + publish the .NET backend.
#
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src

# Copy props files first so a small change to source doesn't bust the
# package-restore cache.
COPY backend/global.json ./backend/
COPY backend/Directory.Build.props backend/Directory.Packages.props ./backend/
COPY backend/Smolla.IdentityProvider.sln* ./backend/
COPY backend/src/Smolla.IdentityProvider.Domain/*.csproj ./backend/src/Smolla.IdentityProvider.Domain/
COPY backend/src/Smolla.IdentityProvider.Application/*.csproj ./backend/src/Smolla.IdentityProvider.Application/
COPY backend/src/Smolla.IdentityProvider.Infrastructure/*.csproj ./backend/src/Smolla.IdentityProvider.Infrastructure/
COPY backend/src/Smolla.IdentityProvider.Api/*.csproj ./backend/src/Smolla.IdentityProvider.Api/
COPY backend/src/Smolla.IdentityProvider.Host/*.csproj ./backend/src/Smolla.IdentityProvider.Host/

WORKDIR /src/backend
RUN dotnet restore src/Smolla.IdentityProvider.Host/Smolla.IdentityProvider.Host.csproj

WORKDIR /src
COPY backend/src/ ./backend/src/

WORKDIR /src/backend
RUN dotnet publish src/Smolla.IdentityProvider.Host/Smolla.IdentityProvider.Host.csproj \
    -c Release \
    -o /publish \
    --no-restore \
    /p:UseAppHost=false

#
# Stage 3 — runtime image.
#
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

WORKDIR /app
COPY --from=backend /publish ./
COPY --from=frontend /src/dist ./wwwroot/

EXPOSE 8080

USER app
ENTRYPOINT ["dotnet", "Smolla.IdentityProvider.Host.dll"]
