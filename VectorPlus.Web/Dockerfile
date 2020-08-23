FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app
COPY . ./

# install System.Drawing native dependencies
RUN apk update && apk upgrade
RUN apk add libgdiplus --repository=http://dl-cdn.alpinelinux.org/alpine/edge/testing/
RUN apk add libc6-compat
RUN dotnet publish VectorPlus.Web -c Release -o out
# RUN cp VectorPlus.Web/vectorplus.db out/vectorplus.db

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build-env /app/out .
RUN apk update && apk upgrade
RUN apk add libgdiplus --repository=http://dl-cdn.alpinelinux.org/alpine/edge/testing/
RUN apk add libc6-compat
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000
ENTRYPOINT ["dotnet", "VectorPlus.Web.dll"]
