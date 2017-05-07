#!/bin/bash
PROJECTS=(Lockbox.Api Lockbox.Client Lockbox.Examples.Api Lockbox.Examples.WebApp Lockbox.Tests)
for PROJECT in ${PROJECTS[*]}
do
  dotnet restore $PROJECT --no-cache
  dotnet build $PROJECT
done