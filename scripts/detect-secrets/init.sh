#!/bin/bash
set -euo pipefail

PARENT_PATH=$(
    cd "$(dirname "${BASH_SOURCE[0]}")"
    pwd -P
)

source $PARENT_PATH/activate.sh
check_python
ensure_env
install
