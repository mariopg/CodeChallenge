# Code Challenge

This project is a Single Page Application built with:
- ASP.NET Core
- Entity Framework Core
- Vue + Vite

It loads data from a CSV file on backend startup and exposes search endpoints
consumed by the Vue frontend.

## Requirements

To run this project you need:

- .NET SDK **8.0.4xx**  
  (Tested with 8.0.406, any 8.0.4xx version is valid)
- Node.js version 20.19+, 22.12+
- npm **9+**

Verify your installation:

```bash
dotnet --version
node --version
npm --version

## Running the Project in Visual Studio

To start both the backend and frontend automatically in Visual Studio:

1. Open the solution in Visual Studio.
2. Right-click on the **solution** in Solution Explorer and select **Properties**.
3. Go to **Common Properties → Startup Project**.
4. Select **Multiple startup projects**.
5. For each project (e.g., `codechallenge.client` and `codechallenge.Server`), set **Action** to **Start**.
6. Click **OK**.
7. Press **F5** or click **Start** to run both projects at the same time.