FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Use SDK image for final stage (has EF tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /src

# Copy the ENTIRE source code so EF can find the project
COPY --from=build /src ./

# Install EF tools
RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Install sqlcmd
RUN apt-get update && apt-get install -y curl gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools18 && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

ENV PATH="$PATH:/opt/mssql-tools18/bin"

COPY entrypoint.sh /src/
RUN chmod +x /src/entrypoint.sh

EXPOSE 5000
ENTRYPOINT ["/src/entrypoint.sh"]
