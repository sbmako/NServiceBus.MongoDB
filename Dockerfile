# Sample Docker file that demonstrates building this project in Linux using .Net Core
FROM microsoft/dotnet:2.1-sdk

#Note: Have to install legacy libssl1.0.0 for GitVersion :(
RUN apt-get update && \
    apt-get -y install software-properties-common --no-install-recommends && \
    add-apt-repository 'deb http://deb.debian.org/debian jessie main' && \
    apt-get update && \
    apt-get -y install libssl1.0.0 --no-install-recommends

ADD . /src
WORKDIR /src

RUN rm -rf /src/tools && \
    ./build.sh CleanAll && \
    ./build.sh && \
    ./build.sh pack
