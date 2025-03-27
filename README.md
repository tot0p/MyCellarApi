# MyCellarApi

This is a simple API to manage a wine cellar. It is a simple project of course "Architecture logiciel"

## Installation of secret files

Create a file named `appsettings.Secrets.json` in `MyCellarApi` folder and add the following content:

```json
{
    "Authentication": {
        "Google": {
            "ClientId": "YourClientId",
            "ClientSecret": "YourClientSecret"
        }
    }
}
```

To get the `ClientId` and `ClientSecret`, you need to use google Auth Platform [here](https://console.cloud.google.com/auth/overview).

## Author

- [Tot0p](https://github.com/tot0p)
- [Mkarten](https://github.com/mkarten)
- [Axou89](https://github.com/axou89)
