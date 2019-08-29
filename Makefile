BASEDIR = $(CURDIR)
SOLUTION = $(BASEDIR)/NServiceBus.MongoDB.sln
PROJECT = $(BASEDIR)/src/NServiceBus.MongoDB
TESTS = $(BASEDIR)/src/NServiceBus.MongoDB.Tests
CONFIGURATION ?= Debug
GitVersion_SemVer ?= $$(gitversion /showvariable SemVer || echo '1.0.0-local')
BUILD_OPTS = --no-restore -c $(CONFIGURATION)
TEST_OPTS = --no-restore -c $(CONFIGURATION)
RESTORE_OPTS =
PUBLISH_OPTS = -c $(CONFIGURATION)
PACK_OPTS = --no-restore -c $(CONFIGURATION) /p:Version=$(GitVersion_SemVer)

CLEAN_DIRS = \
	src/*/bin \
	src/*/obj 

default: restore build test

all: restore build test publish pack

build: restore
	$(printTarget)
	@dotnet build $(BUILD_OPTS) $(SOLUTION)
    
test: build
	$(printTarget)
	@dotnet test $(TEST_OPTS) $(TESTS)

publish:
	$(printTarget)
	@dotnet publish $(PUBLISH_OPTS)

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
