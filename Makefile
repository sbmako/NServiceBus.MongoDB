BASEDIR = $(CURDIR)
SOLUTION = $(BASEDIR)/NServiceBus.MongoDB.sln
PROJECT = $(BASEDIR)/src/NServiceBus.MongoDB
TESTS = $(BASEDIR)/src/NServiceBus.MongoDB.Tests
CONFIGURATION ?= Debug
BUILD_OPTS = --no-restore -c $(CONFIGURATION)
TEST_OPTS = --no-restore -c $(CONFIGURATION)
RESTORE_OPTS =
PACK_OPTS = --no-restore -c $(CONFIGURATION) /p:Version=$(GitVersion_SemVer)

CLEAN_DIRS = \
	src/*/bin \
	src/*/obj 

default: restore build test

all: restore build test pack

build: restore
	$(printTarget)
	@dotnet build $(BUILD_OPTS) $(SOLUTION)
    
test: build
	$(printTarget)
	@dotnet test $(TEST_OPTS) $(TESTS)

pack: build
	$(printTarget)
	@dotnet pack $(PACK_OPTS) $(PROJECT)

restore:
	$(printTarget)
	@dotnet restore $(RESTORE_OPTS) $(SOLUTION)

clean:
	$(printTarget)
	@dotnet clean
	rm -rf $(CLEAN_DIRS) 

# Helper function to pretty print targets as they execute
TARGET_COLOR := \033[0;32m
RED := \033[0;31m
NO_COLOR := \033[m
CURRENT_TARGET = $(@)

define printTarget
	@printf "%b" "\n$(TARGET_COLOR)$(CURRENT_TARGET):$(NO_COLOR)\n";
endef
