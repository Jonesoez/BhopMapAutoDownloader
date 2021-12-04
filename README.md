# BhopMapAutoDownloader - BMD
[![Build](https://github.com/Jonesoez/BhopMapAutoDownloader/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/Jonesoez/BhopMapAutoDownloader/actions/workflows/build.yml)

Downloads BHOP maps for `CS:S` which were uploaded to `gamebanana.com`. 

The basic stuff works but this is still unfinished and not optimized enough to be used right now.

## Setup:
Create and configure the `appsettings.json` file and put it in the same directory as the executable. I'd recommend to set `CheckInterval` to 5 or 10 minutes as **seconds** if you want to run it for a longer period. For the sake of debugging I'm using 5 seconds (not recommended).

##### appsettings.json
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Error",
        "System": "Warning"
      }
    }
  },
  "DownloadPath": "tmp_maps",
  "KeepDownloadFiles": false,
  "ExtractPath": "extracted_maps",
  "EnableFastDlCompression": false,
  "FastDlPath": "\\var\\lib\\pterodactyl\\volumes\\fastdl\\",
  "MapTypes": [
    "bhop",
    "kz",
    "autobhop"
  ],
  "CheckInterval": 300,
  "NumberOfMapsToCheck": 3
}
```

Contributions are appreciated and very welcome.
