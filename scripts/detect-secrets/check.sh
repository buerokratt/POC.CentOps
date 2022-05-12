#!/bin/bash
set -euo pipefail

PARENT_PATH=$(
    cd "$(dirname "${BASH_SOURCE[0]}")"
    pwd -P
)

source $PARENT_PATH/activate.sh

check_python
ensure_env

if which detect-secrets-hook >/dev/null 2>&1; then
    VERSION=$(detect-secrets-hook --version)

    echo "Found detect-secrets-hook with version $VERSION"
    git diff --staged --name-only -z | xargs -0 detect-secrets-hook --baseline .secrets.baseline --word-list secrets-allow-list.txt
else
    echo "detect-secrets-hook not found. Run setup script and try again"
fi
