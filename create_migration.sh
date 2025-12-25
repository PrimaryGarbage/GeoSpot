#!/bin/bash
if [ $# -eq 0 ]; then
	echo "Please provide migration name as the first argument, or use -d flag to display pending migration changes"
fi

for arg in "$@"; do
	if [[ "$arg" == "-d" || "$arg" == "--dry-run" ]]; then
		dotnet ef migrations has-pending-model-changes -p ./GeoSpot.Persistence/GeoSpot.Persistence.csproj -s ./GeoSpot.Api/GeoSpot.Api.csproj;
	else
		dotnet ef migrations add "$1" -p ./GeoSpot.Persistence/GeoSpot.Persistence.csproj -s ./GeoSpot.Api/GeoSpot.Api.csproj;
	fi
done



