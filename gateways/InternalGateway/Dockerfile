FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim
EXPOSE 80
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "InternalGateway.dll"]