FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ["Nebula.Job/Nebula.Job.csproj", "Nebula.Job/"]
RUN dotnet restore "Nebula.Job/Nebula.Job.csproj"

# copy and publish
COPY Nebula.Job/ Nebula.Job/
WORKDIR /src/Nebula.Job
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "Nebula.Job.dll"]
