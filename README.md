# BhopMapAutoDownloader - BMD
[![Build](https://github.com/Jonesoez/BhopMapAutoDownloader/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/Jonesoez/BhopMapAutoDownloader/actions/workflows/build.yml)

Downloads recently uploaded BHOP maps from `gamebanana.com` for the game `CS:S`.


**Features:**
- Checks for new uploaded maps every `n` seconds via API call
- Downloads and extracts compressed files
- Puts downloaded maps into the specified folder
- Compresses downloaded maps with bzip2  and puts them into the specified folder
- Infos about the maps are saved in a SQLite database
- Every action is logged (daily) and can be found in the `Logs` folder


The app runs on Windows for now, Linux support is in the works. Discord integration for map requests will also be added soon™.

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
