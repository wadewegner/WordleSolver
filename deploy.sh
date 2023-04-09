#!/bin/sh

# Run tests and check if they pass
dotnet test
if [ $? -ne 0 ]; then
    echo "Tests failed. Deployment aborted."
    exit 1
fi

# If tests pass, continue with deployment
cp WordleSolver.csproj.heroku WordleSolver.csproj

git add -A
git commit -m "updated for heroku"
git push heroku main
heroku open

cp WordleSolver.csproj.local WordleSolver.csproj

git add -A
git commit -m "updated for local"