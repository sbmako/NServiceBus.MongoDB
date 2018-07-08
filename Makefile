#
# Makefile for composing containers for the onbaord WiFi Portal
#

BASEDIR = $(CURDIR)
SOLUTION = $(BASEDIR)/NServiceBus.MongoDB.sln
TESTS = $(CURDIR)/src/NServiceBus.MongoDB.Tests

BUILD_OPTS = --no-restore
TEST_OPTS = --no-restore
RESTORE_OPTS =

default: restore build test

all: default

build:
	$(printTarget)
	dotnet build $(BUILD_OPTS) $(SOLUTION)

test: build
	$(printTarget)
	dotnet test $(TEST_OPTS) $(TESTS)

restore:
	$(printTarget)
	dotnet restore $(RESTORE_OPTS) $(SOLUTION)

clean:
	$(printTarget)
	dotnet clean $(SOLUTION)
	rm -rf src/*/bin src/*/obj

# Helper function to pretty print targets as they execute
TARGET_COLOR := \033[0;32m
RED := \033[0;31m
NO_COLOR := \033[m
CURRENT_TARGET = $(@)

define printTarget
	@printf "%b" "\n$(TARGET_COLOR)$(CURRENT_TARGET):$(NO_COLOR)\n";
endef
