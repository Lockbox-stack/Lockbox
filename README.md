![Lockbox](http://spetz.github.io/img/lockbox_logo.png)

# Dead simple, centralized and secured storage for your credentials.

# [getlockbox.com](https://getlockbox.com)

|Branch             |Build status                                                  
|-------------------|-----------------------------------------------------
|master             |[![master branch build status](https://api.travis-ci.org/Lockbox-stack/Lockbox.svg?branch=master)](https://travis-ci.org/Lockbox-stack/Lockbox)
|develop            |[![develop branch build status](https://api.travis-ci.org/Lockbox-stack/Lockbox.svg?branch=develop)](https://travis-ci.org/Lockbox-stack/Lockbox/branches)

**What is Lockbox?**
----------------

Having nightmares about **storing** and **deplyoing** the **vulnerable configurations** of your applications?

**Lockbox** is a dead simple, **cross-platform** library (API & Client), built to **save your credentials** (e.g. app settings) in a **centralized and secured storage**.

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
For the encryption purposes the [AES](https://pl.wikipedia.org/wiki/Advanced_Encryption_Standard) algorithm is being used.

On the other hand, a secret key is being stored by Lockbox API and is used for the API key authentication mechanism based on [JWT](https://jwt.io) (JSON Web Tokens). 

## Typical workflow

1. Initialize the Lockbox for the first time.
2. Create a new user account.
3. Create a new box.
4. Add an entry to the box using custom encryption key.
5. Integrate your application by passing the custom encryption key while fetching the entry.


**Resources**
----------------
- [Wiki](https://github.com/Lockbox-stack/Lockbox/wiki)
- [API documentation](http://docs.lockbox.apiary.io)
- [Lockbox Sandbox API](https://sandbox-api.getlockbox.com/)
- [Postman local requests](https://www.getpostman.com/collections/4f6336f107cc8a6a6721)
- [Postman sandbox requests](https://www.getpostman.com/collections/e8ec27a2bb4fe7ab66fb)
- [CURL requests examples](https://github.com/Lockbox-stack/Lockbox/wiki/CURL-requests-examples)

**Quick start**
----------------

## Docker way

Run docker container which requires an external [MongoDB](https://www.mongodb.com) instance (e.g. running on localhost):
```
docker pull lockbox/lockbox.server
docker run -p 5000:5000 lockbox.server 
```

Or use the _docker compose_ to build [Lockbox.Server](https://github.com/Lockbox-stack/Lockbox.Server):

```
git clone https://github.com/Lockbox-stack/Lockbox.Server
docker-compose up
```

Open the web browser at [http://localhost:5000](http://localhost:5000) - Lockbox API should be up and running!

## Classic way

In order to run the Lockbox you need to have installed:
- [.NET Core](https://dotnet.github.io)
- [MongoDB](https://www.mongodb.com/download-center)


Create a new [.NET Core](https://www.microsoft.com/net/core) application and execute the following command via NuGet package manager:

```
dotnet add package Lockbox.Api
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
dotnet add package Lockbox.Client
```

Create a new [.NET Core](https://www.microsoft.com/net/core) Web application and add the following code to the **Startup.cs**:

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

You can provide all of the settings directly in the code via _appsettings.json_ file for the Lockbox.API:

```
{
	"feature": {
		"entrySizeBytesLimit": 524288,
		"entriesPerBoxLimit": 20,
		"usersPerBoxLimit": 20,
		"boxesPerUserLimit": 3,
		"requireAdminToCreateUser": false
	},
	"lockbox": {
		"secretKey": "your_secret_key"
	},
	"mongoDb": {
		"connectionString": "mongodb://localhost:27017",
		"database": "Lockbox"
	}
}
```

Although, you may also use the system environment variables which might be especially useful while running the Lockbox via [Docker container](https://hub.docker.com/r/lockbox/lockbox.server/).

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

**Contribute**
----------------

Want to contribute? Please check the **[contribution guidelines](https://github.com/lockbox-stack/lockbox/blob/master/CONTRIBUTING.md)**. 