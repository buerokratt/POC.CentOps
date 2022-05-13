#!/bin/bash
set -euo pipefail

PARENT_PATH=$(
    cd "$(dirname "${BASH_SOURCE[0]}")"
    pwd -P
)
PYTHON_REF=null
VENV_PATH=${VIRTUAL_ENV:-}

check_python() {
    echo "Checking python version..."

    if which python3 >/dev/null 2>&1; then
      PYTHON_REF="python3"
    else
      if which python >/dev/null 2>&1; then
          PYTHON_REF="python"
      else
          echo "Python is not installed.  Install Python and try again"
          exit 1
      fi
    fi

    PYTHON_VERSION=$($PYTHON_REF --version 2>&1 | awk '{print $2}')
    echo "Found Python with version $PYTHON_VERSION"
}

activate() {
    VENV_PATH="$PARENT_PATH/.venv"

    if [[ ! -d "$VENV_PATH" ]]; then
        echo "Creating environment @ '$VENV_PATH'"
        $PYTHON_REF -m venv $VENV_PATH
    fi

    echo "Activating environment @ '$VENV_PATH'"
    if [ "$(uname)" == "Darwin" ]; then
        . $VENV_PATH/bin/activate
    elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
        . $VENV_PATH/bin/activate
    elif [ "$(expr substr $(uname -s) 1 10)" == "MINGW32_NT" ]; then
        . $VENV_PATH/scripts/activate
    elif [ "$(expr substr $(uname -s) 1 10)" == "MINGW64_NT" ]; then
        . $VENV_PATH/scripts/activate
    fi
}

install() {
    echo 'Installing pre-commit framework...'
    pip install wheel
    pip install pre-commit detect-secrets==1.2.0 pyahocorasick

    echo 'Installing pre-commit hooks from configuration...'
    pre-commit install
    pre-commit run --all-files
}

ensure_env() {
    if [[ -z $VENV_PATH ]]; then
        activate
        VENV_PATH=${VIRTUAL_ENV:-}
    fi
}
