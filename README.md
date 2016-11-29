![Lockbox](http://spetz.github.io/img/lockbox_logo.png)

####**Dead simple, centralized and secured storage for your credentials.**

**What is Lockbox?**
----------------

**Lockbox** is a dead simple, **cross-platform** framework (API & Client), built to **save your credentials** (e.g. app settings) in a **centralized and secured storage**.

What does it mean in practice? Imagine the following scenario - you have your application configuration file containing vulnerable data. 
Now, how do you deploy it to the production environment? Keep credentials in private repository? Manually update the settings on your server? Encrypt and decrypt the file and re-upload to the hosting enviroment?

What if you could have a centralized service that your applications would ask for the secured settings? This is where Lockbox come in handy. Whether you have a simple website or a set of microservices - you can keep your settings in a secured and centralized storage.

In other words - it's like [Vault](https://www.vaultproject.io), but much more simpler.

**General concepts**
----------------

## Domain

- **API key** - unique token that identifies the user.
- **Box** - workspace that contains one or more users and collection of encrypted entries.
- **Box user** - existing user in the system that was granted access to the particular box.
- **Entry**- key/value object containing encrypted value that can be of any type (text, number, JSON etc.).
- **User** - unique user in the system (available roles: user, admin) that can manage boxes and entries. 

## Security

**Lockbox doesn't store encryption keys!** It only contains the encrypted value and its salt. Encryption key may be even unique (if you wish to do so) for each entry.
It means that even if the Lockbox database was compromised, the attacker will not be able to decrypt the values.

Encryption key has to be provided via the custom HTTP Header *X-Encryption-Key* when creating a new entry and fetching the value of existing one.
For the encryption purposes the [Triple DES](http://www.cryptographyworld.com/des.htm) algorithm is being used.

On the other hand, a secret key is being stored by Lockbox API and is used for the API key authentication mechanism based on [JWT](https://jwt.io) (JSON Web Tokens). 

## Typical workflow

1. Initialize the Lockbox for the first time.
2. Create a new user account.
3. Create a new box.
4. Add an entry to the box using custom encryption key.
5. Integrate your application by passing the custom encryption key while fetching the entry.


**Resources**
----------------
- [API documentation](http://docs.lockbox.apiary.io)
- [Lockbox Sandbox API](https://sandbox-api.getlockbox.com/)
- [Postman local requests](https://www.getpostman.com/collections/4f6336f107cc8a6a6721)
- [Postman sandbox requests](https://www.getpostman.com/collections/e8ec27a2bb4fe7ab66fb)


**Quick start**
----------------

## Docker way

Run docker container which requires external MongoDB instance (e.g. running on localhost):
```
docker pull lockbox/lockbox.server
docker run -p 5000:5000 lockbox.server 
```

Or use the docker compose:

```
git clone https://github.com/Lockbox-stack/Lockbox.Server
docker-compose up
```

Open the web browser at [http://localhost:5000](http://localhost:5000) - Lockbox API should be up and running!

## Classic way

In order to run the Lockbox you need to have installed:
- [.NET Core](https://dotnet.github.io)
- [MongoDB](https://www.mongodb.com/download-center)


Create a new .NET Core application and execute the following command:

```
Install-Package Lockbox.Api -Pre
```

Change your **Program.cs** code to look like this:

```
var host = new WebHostBuilder()
    .UseKestrel()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseStartup<LockboxStartup>()
    .Build();

host.Run();
```

Start the application and open the web browser at [http://localhost:5000](http://localhost:5000).


**Examples**
----------------

```
git clone https://github.com/Lockbox-stack/Lockbox
```

- **Lockbox.Examples.Api** - Lockbox server with minimal configuration.
- **Lockbox.Examples.WebApp** - Web application that uses Lockbox to fetch the vulnerable appsettings.

In order to play with Lockbox examples, please read the [API docs](http://docs.lockbox.apiary.io) and use the provided [Postman](https://www.getpostman.com) local requests [collection](https://www.getpostman.com/collections/4f6336f107cc8a6a6721) that will guide you through the most important concepts.


**Integrations**
----------------


```
Install-Package Lockbox.Client -Pre
```

Create a new .NET Core Web application and within the **Startup.cs** add the following code:

```
public IConfiguration Configuration { get; set; }

public Startup(IHostingEnvironment env)
{
    var builder = new ConfigurationBuilder()
        .AddLockbox(encryptionKey: "encryption_key",  
            apiUrl: "api_url",                         
            apiKey: "user_api_key",                    
            boxName: "box_name",
            entryKey: "entry_name_for_app_settings");
    Configuration = builder.Build();
}
```

When everything is set correctly, you will see that your app configuration gets updated based on the encrypted value stored in Lockbox.


**Settings**
----------------

Although you can provide all of the settings directly in the code, you may also use the system environment variables which might be especially useful while running the Lockbox via Docker container.

**Lockbox.Api**

```
LOCKBOX_SECRET_KEY
LOCKBOX_MONGO_CONNECTION_STRING
LOCKBOX_MONGO_DATABASE
```

**Lockbox.Client**
```
LOCKBOX_ENCRYPTION_KEY
LOCKBOX_API_URL
LOCKBOX_API_KEY
LOCKBOX_BOX_NAME
LOCKBOX_ENTRY_KEY
```