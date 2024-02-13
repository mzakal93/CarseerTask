# build stage 
#we copy csproj file first because it's barely changed so it will make build process faster
#to copy all dependencies in project and install it in our image folder
#then we copy other files 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY CarseerTask.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o output

# final stage
#copy from build stage result -- build is the name of the stage alias
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
copy --from=build /src/output .
ENTRYPOINT ["dotnet","CarseerTask.dll"]