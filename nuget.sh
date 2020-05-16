#!/usr/bin/env bash
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
NUGET_API_KEY=$(lpass show --notes 6647695209481230060)

rm -rf bin
rm -rf obj

dotnet pack -c Release

NUPKG_FILE=$(find bin/Release -name '*.nupkg')

dotnet nuget push $NUPKG_FILE -k $NUGET_API_KEY -s $NUGET_SOURCE