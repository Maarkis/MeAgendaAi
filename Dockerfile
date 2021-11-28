#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MeAgendaAi.Application/MeAgendaAi.Application.csproj", "src/MeAgendaAi.Application/"]
RUN dotnet restore "src/MeAgendaAi.Application/MeAgendaAi.Application.csproj"
COPY . .
WORKDIR "/src/src/MeAgendaAi.Application"
RUN dotnet build "MeAgendaAi.Application.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR /test
RUN dotnet test

FROM build AS publish
RUN dotnet publish "MeAgendaAi.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeAgendaAi.Application.dll"]