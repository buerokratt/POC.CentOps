# Introduction

Central Operations (CentOps)


## Developer Setup

### Detect-Secrets Pre-commit Hook

This repo uses the [Yelp detect-secrets](https://github.com/Yelp/detect-secrets) tool to scan commits and ensure they don't accidentally contain secrets which are private.

Please install the git pre-commit hook to ensure you don't commit secrets to the repo.

#### Installation

##### Dependencies

Python

The installation scripts are included in the ```scripts/detected-secrets``` directory.



##### Something went wrong!

If dependencies are missing or something unexpected happens, you can remove the git pre-commit hook and go back to what you had before.

Look in the ```.git/hooks/``` directory for a file called pre-commit.  


