# Aggregail Admin UI (MongoDB)

An administration UI for event stores built using [Aggregail.MongoDB](../Aggregail.MongoDB).

![Preview](Documentation/preview.gif)

## Getting started

 1. Download the latest [Release](https://github.com/Nillerr/EventSourcing.Demo/releases).
 2. Unzip
 3. Start the application:
    ```bash
    dotnet Aggregail.MongoDB.Admin.dll \
      --ConnectionString=<Your MongoDB Connection String> \
      --Database=<Your Database> \
      --Collection=<Your Streams Collection>
    ```
     - `--Database` defaults to `aggregail`
     - `--Collection` defaults to `streams`
     - The arguments can also be specified using [environment variables](#environment-variables)
 4. Log in at `http://localhost:5000` with the user `admin` and password `changeit`.
 5. By default the application will launch on the urls: `http://localhost:5000` and 
    `https://localhost:5001`. This can be changed by specifying the `--urls` command line argument:
    ```bash
    dotnet Aggregail.MongoDB.Admin.dll \
      --urls=http://localhost:3014
    ``` 
    
The application will create a new collection called `_aggregail_users` in the database specified 
when launching the application, containing a single user `admin` with the password `changeit`. The 
users collection, database and event MongoDB server can be changed using configurations:

```sh
--Users:ConnectionString="..." \
--Users:Database="..." \
--Users:Collection="..."
``` 

Furthermore, specifying `--QuietStartup=true` will disable printing the launch configuration to 
`stdout`. The password will automatically be removed from the connection string before printing 
the launch configuration.
 
### Environment Variables

The application can also be configured using these environment variables:

```sh
AGGREGAIL_ConnectionString
AGGREGAIL_Database
AGGREGAIL_Collection (Defaults to `streams`)

AGGREGAIL_Users_ConnectionString (Defaults to value of ConnectionString)
AGGREGAIL_Users_Database (Defaults to value of Database)
AGGREGAIL_Users_Collection (Defaults to `_aggregail_users`)

ASPNETCORE_Urls
```

## Troubleshooting and FAQ

The application is by no means perfect, and several corners have been cut in an effort to 
get a minimal viable product ready. Here are some of the known issues you might experience.


### This application looks very familiar...

The layout, the URLs, even the HTTP API and the polling model is a blatant clone of the 
Admin UI for Event Store. This application was written from scratch though, and shares 
no code or resources with Event Store.
    

### Some buttons are always disabled

There are several features that haven't been implemented yet, some of which may yet be 
dropped. The buttons are there only to confuse you.


#### I can't change a user password

Changing user password is one of the features that haven't been implemented yet. The 
suggested workaround is to delete and and create the user again with the same username and 
a new password.


### All users have been deleted

Restarting the application, while there are no users in the `_aggregail_users`  
(or the one you configured) MongoDB collection, will re-create the default `admin` user, 
with the password `changeit`.


### I have been locked out

 1. Connect to your MongoDB server
 2. Open the database specified when launching the application
 3. Delete the `_aggregail_users` collection (or the one you configured)
 4. See [All users have been deleted](#all-users-have-been-deleted)


### Missing UI / Theme / Stylesheet

If a mismatch occurs with the `localStorage` keys `theme` and `theme-style` vs the provided 
themes in the application, there's not implemented any nice fallback, and as such, the UI will 
look very pale. Delete those `localStorage` keys to restore the default theme and style, or 
clear all site data.


### My theme is not synchronized with my user

The selected theme is stored in `localStorage`, and will not be synchronized with your user 
login.


### Frequent polling, really?

Since this application is essentially a clone of the Admin UI included in Event Store, so 
is the polling model - it's simple, it works.

Using the polling model meant we ended up developing most of a REST API for accessing an 
[Aggregail.MongoDB](../Aggregail.MongoDB) event store, although it was never the plan to support 
such a thing.

There are plans to implement real-time updates using SignalR, by using the 
[db.collection.watch()](https://docs.mongodb.com/manual/reference/method/db.collection.watch/) 
in MongoDB.
