# PostidentCheck

Postident check, contrary to it's name, will check not only post ident numbers but also address of possible parcel recipient.

## What kind of data can this application validate:

This app validates post ident number, zip code, city name, country code, street name and number of any address globally against German DHL validation api.\
Service validates this data separately and as a whole unit.\
Packstations and post offices are supported - post ident number is **mandatory** in those cases.

## Are there any caveats?
DHL api will report some addresses from countries outside EU (China for example) as valid even if they are not - it is not checking address as a whole.

## What will be returned:
Data will be validated offline and online, you will get a status (valid / not checked / invalid) and every message from the online service in German language, so you will know what kind of errors (if any) were detected. This data will be saved into the database and logged.

## Command line options:
| option | alternative | description | possible params | example |
| :----:  | :-----:     | :---------- | :--- | :---
| --all  | -a          | Default option. <br/>all data from database will be check
| --carrier | -c | Only data with specified carrier(s) set will checked | DHL, DHL_Sperr, DPD, DPD_PL, HES, Schenker, Schenker_PL, Pick_up, Self_delivery or code numbers (matches DB enumeration, fe. DHL is 6) | PostidentCheck.exe --carrier DHL Schenker
| --id | -i | Check data by given id | | PostidentCheck.exe --id 123456 888777555 |
|--help | -h | Displays help text for application | | 

## Configuration:
Those settings are commonly adjustable, rest should mostly be left alone - the do not need to be changed basically ever.

### Appsettings.json:
* DilosDEV - main connection string
* DHL
    * BaseAddress - main url to soap service of dhl
    * Secret - KeePass guid for http client authorization data
    * XmlSecret - KeePass guid for 'in xml' authorization data
    * MaxQueriesPerSecond - maximum number of queries per one second
    * MaxValidationsInQuery - maximum validation data packs in one query
* KeePass
    * BaseAddress - KeePass server address
    * Username
    * Password

If you need to bypass KeePass service (malfunction or otherwise), add those settings to appsettings or appsettings.'ASPNETCORE_ENVIROMENT' (according to your run configuration. If no ASPNETCORE_ENVIROMENT is set, it defaults to 'Production')

* KeePassOfflineStore
    * Login - http client login
    * Password - http client password
    * XmlLogin - Login 'in xml' authorization data
    * XmlPassword - Password 'in xml' authorization data

### DefaultNamingMap (Common folder)

Contains all synonyms for DHL default namings.\
Default is:
```"DefaultNamingMap": {
    "Map": {
      "Packstation": [ "Paketstation" ]
    }
  }
```
This will replace 'Paketstation' occurrence recived from DB to 'Packstation'\
You can add additional synonyms for this and other words you like, just like to any other json 
```dictionary<string, List<string>>``` representation.

### DefaultShipmentValues (Common folder)
Here you can configure all of DHL required service types, values etc.
Default implementation works with whole world, but in case of some kind of service changes you can reconfigure it here.\
\
It also contains list of countries currently in EU, so in case of another Brexit or some new members - you can add / remove them here - This app generates request according to country location, as DHL requires.
