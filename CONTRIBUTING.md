# Contributing

## Developer Setup For This Repository

### Feature Toggles

| Feature Toggle    | Purpose                       |  appSettings.json | yaml environment |
|:------------------|:------------------------------| ------------------|--------|
| InMemoryDataStore | Enables an In-memory data store which can be used for testing purposes only | "FeatureToggles" : { "InMemoryStore": true } | FEATURETOGGLES__INMEMORYSTORE=true

### Detect-Secrets Pre-commit Hook

This repo uses the [Yelp detect-secrets](https://github.com/Yelp/detect-secrets) tool to scan commits and ensure they don't accidentally contain secrets which should never be persisted in source control.

The scripts have been slightly tinkered with (very slightly) but the originals can be found [here](https://github.com/wbreza/pre-commit-hooks/tree/main/detect-secrets).

_Please install the git pre-commit hook to ensure you don't commit secrets to the repo._

#### Installation

You will need Python installed.
Windows Users can run 'python' on the command line to trigger the Microsoft Store or go there directly.

The installation scripts are included in the ```scripts/detect-secrets``` directory.

From the root of the repository execute this bash script:

```bash
./scripts/.venv/init.sh
```

> Windows users (use a Git Bash Shell to execute this.)

Detect-secrets already has a baseline for this repository so it should only check for new secrets.  For false positives note the allow-listing mechanism:

```c
// pragma: allowlist secret
```

>Also covered in the detect-secrets [README.md](https://github.com/Yelp/detect-secrets)

### *Help! Something went wrong with the pre-commit hook!*

If dependencies are missing or something unexpected happens, you can remove the git pre-commit hook and go back to what you had before.
  Look in the ```.git/hooks/``` directory for a file called ```pre-commit```.  Either remove or rename the file.

If you've resolved a dependency issue or fixed something that caused the installation to fail, remove the ```scripts/.venv``` directory and you should be good to run the ```init.sh``` script again.
