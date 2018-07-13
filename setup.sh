
#!/bin/bash
#set -x

###
    This is a Build Pre-Requisites Setup Script.
###

function setup_mac_osx () {
    echo 'Executing Build Pre-Requisites Setup Scrip for Mac/OSX'

    echo 'Installing GitVersion (http://gitversion.readthedocs.io)'
    home_brew_install 'gitversion' || return $?
}

function home_brew_install () {
    echo 'Checking if HomeBrew is Installed'
    local home_brew_command=$(which brew)
    
    if ([ -z home_brew_command ])
    then
        echo 'Error: Homebrew needs to be installed and in the PATH to run this setup script.  See: https://brew.sh/'
        return 1
    fi

    echo "Using Homebrew to install '$1'"
    brew install $1 || return $?
}

function main () {
    echo 'Executing Build Pre-Requisites Setup Script'
    local operating_system=$(uname)

    case $operating_system in
    'Darwin')
        setup_mac_osx
        return
        ;;
    *)
        echo "Setting up the Operating System '$operating_system' is not supported!"
        return 1
        ;;
    esac
}

main
