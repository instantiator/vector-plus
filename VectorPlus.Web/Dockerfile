FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish VectorPlus.Web -c Release -o out
RUN cp VectorPlus.Web/vectorplus.db out/vectorplus.db
#RUN export PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet tool install --global dotnet-ef
#RUN dotnet ef database update --project VectorPlus.Web

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000
ENTRYPOINT ["dotnet", "VectorPlus.Web.dll"]
