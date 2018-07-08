#
# Makefile for composing containers for the onbaord WiFi Portal
#

BASEDIR = $(CURDIR)
SOLUTION = $(BASEDIR)/NServiceBus.MongoDB.sln
PROJECT = $(BASEDIR)/src/NServiceBus.MongoDB
TESTS = $(BASEDIR)/src/NServiceBus.MongoDB.Tests

BUILD_OPTS = --no-restore
TEST_OPTS = --no-restore
RESTORE_OPTS =
PUBLISH_OPTS = -c Release
PACK_OPTS = -c Release

CLEAN_DIRS = \
	src/*/bin \
	src/*/obj 

default: restore build test

all: restore build test publish pack

build:
	$(printTarget)
	@dotnet build $(BUILD_OPTS) $(SOLUTION)

test: build
	$(printTarget)
	@dotnet test $(TEST_OPTS) $(TESTS)

publish:
	$(printTarget)
	@dotnet publish $(PUBLISH_OPTS)

pack:
	$(printTarget)
	@dotnet pack $(PACK_OPTS) $(PROJECT)

restore:
	$(printTarget)
	@dotnet restore $(RESTORE_OPTS) $(SOLUTION)

clean:
	$(printTarget)
	rm -rf $(CLEAN_DIRS) 

# Helper function to pretty print targets as they execute
TARGET_COLOR := \033[0;32m
RED := \033[0;31m
NO_COLOR := \033[m
CURRENT_TARGET = $(@)

define printTarget
	@printf "%b" "\n$(TARGET_COLOR)$(CURRENT_TARGET):$(NO_COLOR)\n";
endef
