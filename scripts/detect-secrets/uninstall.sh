#!/bin/bash
set -euo pipefail

PARENT_PATH=$(
    cd "$(dirname "${BASH_SOURCE[0]}")"
    pwd -P
)

source $PARENT_PATH/activate.sh

VENV_PATH=${VIRTUAL_ENV:-}

uninstall() {
    echo "Uninstalling pre-commit framework..."
    if which pre-commit >/dev/null 2>&1; then
        pre-commit uninstall
    fi
}

check_python
ensure_env
uninstall
