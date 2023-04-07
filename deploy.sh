
cp WordleSolver.csproj.heroku WordleSolver.csproj

git add -A
git commit -m "updated"
git push heroku main
heroku open

cp WordleSolver.csproj.local WordleSolver.csproj