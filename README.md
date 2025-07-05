# PTM

## Running Migrations

To run database migrations and apply them, follow these steps:

1.  Navigate to the `PTM.Infrastructure` directory in your terminal:
    ```bash
    cd PTM.Infrastructure
    ```
2.  Create a new migration file (replace `[MigrationName]` with a descriptive name):
    ```bash
    dotnet ef migrations add [MigrationName]
    ```
3.  Run the following command to apply any pending migrations to your database:
    ```bash
    dotnet ef database update
    ```

## Default Users

There are two default users configured for the application:

*   **Admin User**
    *   Username: `admin`
    *   Password: `123`

*   **Regular User**
    *   Username: `user`
    *   Password: `123`

## API Documentation

Once the application is running, you can access the API documentation by navigating to the following URL in your browser:

```
/scalar/v1
```