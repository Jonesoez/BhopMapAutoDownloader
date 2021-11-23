# BhopMapAutoDownloader - BMD
[![Build](https://github.com/Jonesoez/BhopMapAutoDownloader/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/Jonesoez/BhopMapAutoDownloader/actions/workflows/build.yml)

Downloads BHOP maps for `CS:S` which were uploaded to `gamebanana.com`. 

The basic stuff works but this is still unfinished and not optimized enough to be used right now.

## Setup:
You can run the app once to create a `settings.json` file or create one yourself (must be in the same directory as the executable). I'd recommended to set `CheckInterval` to 5 or 10 minutes as **seconds** if you want to run it for a longer period. For the sake of debugging I'm using 5 seconds (not recommended).

##### settings.json
```json
{
	"DownloadPath": "tmp_maps",
    "KeepDownloadFiles": false,
    "ExtractPath": "extracted_maps",
    "FastDlPath": "fastdl",
    "CheckInterval": 300
}
```

Contributions are appreciated and very welcome.
