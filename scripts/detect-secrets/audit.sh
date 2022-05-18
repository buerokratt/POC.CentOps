#!/bin/bash
set -euo pipefail

PARENT_PATH=$(
    cd "$(dirname "${BASH_SOURCE[0]}")"
    pwd -P
)

source $PARENT_PATH/activate.sh

check_python
ensure_env

if which detect-secrets >/dev/null 2>&1; then
    VERSION=$(detect-secrets --version)

    echo "Found detect-secrets with version $VERSION"
    detect-secrets audit .secrets.baseline

    echo "Secrets baseline has been updated with the results of your audit"
    echo "Please review all baseline updates before committing changes."
else
    echo "detect-secrets not found. Run setup script and try again"
fi
