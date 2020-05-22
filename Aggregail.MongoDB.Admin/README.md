# Aggregail Admin UI (MongoDB)

An administration UI for event stores built using [Aggregail.MongoDB](../Aggregail.MongoDB).

![NuGet](Documentation/preview.gif)

## Installation

 1. Download the latest [Release](https://github.com/Nillerr/EventSourcing.Demo/releases).
 2. Unzip
 3. Start the application:
    ```bash
    dotnet Aggregail.MongoDB.Admin.dll \
      --ConnectionString=<Your MongoDB Connection String> \
      --Database=<Your Database> \
      --Collection=<Your Streams Collection>
    ```
 4. Log in with the user `admin` and password `changeit`.
 5. By default the application will launch on the urls: `http://localhost:5000` and 
    `https://localhost:5001`. This can be changed by specifying the `--urls` command line argument:
    ```bash
    dotnet Aggregail.MongoDB.Admin.dll \
      --urls=http://localhost:3014
    ``` 

## Troubleshooting

### All users have been deleted

Restarting the application, while there are no users in the `users` MongoDB collection, will 
re-create the default `admin` user, with the password `changeit`.

### I have been locked out

 1. Connect to your MongoDB server
 2. Open the database specified when launching the application
 3. Delete the `users` collection
 4. See [All users have been deleted](#All-users-have-been-deleted)
 
