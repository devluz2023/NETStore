dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate

dotnet ef database update