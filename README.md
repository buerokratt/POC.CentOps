# Introduction

Central Operations (CentOps)


## Developer Setup

### Detect-Secrets Pre-commit Hook

This repo uses the [Yelp detect-secrets](https://github.com/Yelp/detect-secrets) tool to scan commits and ensure they don't accidentally contain secrets which are private.

Please install the git pre-commit hook to ensure you don't commit secrets to the repo.
#### Installation
The installation scripts are included.

Dependencies are python3 and python3-venv:

```
sudo apt-get update
sudo apt-get install python3.8
sudo apt-get install python3-venv
```