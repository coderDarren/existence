# existence
The goal is to exist.

## Using Git CLI (Command Line Interface)

Download git: https://git-scm.com/downloads
Download cmder: https://cmder.net (recommended)

Open cmder

1. Navigate to the project location `cd to/your/project`
2. Clone the project `git clone git@github.com:coderDarren/existence.git`
3. Navigate to existence `cd existence`
(Now you are in the git project)
4. Get the git status `git status`
5. Develop on your own "version" of the project `git branch myBranch`
6. Switch from master to your branch `git checkout myBranch`
(Work on the game)
(Now its time to publish your work)
7. Commit your changes `git add .` // `git commit -m "your change notes"`
8. Create a copy of your branch `git branch myBranchCopy`
9. Go back to master `git checkout master`
10. Pull the most recent changes from your team `git pull`
(Now you need to merge your changes with the up-to-date changes. This can result in a merge conflict which is why we create branch copies)
11. Go back to your copy branch `git checkout myBranchCopy`
12. Merge master `git merge master`
(If it failed, ask for help)
(If it succeeded..)
13. Go back to master `git checkout master`
14. Merge the confirmed success `git merge myBranchCopy`
15. Publish your changes `git push`

## Using the [Game Server](game-server)

The game server runs on node version [10.x](https://nodejs.org/dist/latest-v10.x). Install this verison of node, then navigate to the game-server directory and run `npm i` to install all of the node modules. Verify your verison of node using `node --version` To start the server run `node index.js`. Now when you play the game on the Unity side, you will have socket access to the server.
